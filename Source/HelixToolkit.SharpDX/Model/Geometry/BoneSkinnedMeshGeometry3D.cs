using SharpDX;

namespace HelixToolkit.SharpDX;

[Serializable]
public class BoneSkinnedMeshGeometry3D : MeshGeometry3D
{
    public BoneSkinnedMeshGeometry3D()
    {

    }

    public BoneSkinnedMeshGeometry3D(MeshGeometry3D mesh)
    {
        mesh.AssignTo(this);
    }

    private IList<BoneIds>? vertexBoneIds;
    /// <summary>
    /// Gets or sets the vertex bone ids and bone weights.
    /// </summary>
    /// <value>
    /// The vertex bone ids.
    /// </value>
    public IList<BoneIds>? VertexBoneIds
    {
        set
        {
            Set(ref vertexBoneIds, value);
        }
        get
        {
            return vertexBoneIds;
        }
    }

    /// <summary>
    /// Creates the node based bone matrices.
    /// </summary>
    /// <param name="bones">The bones.</param>
    /// <param name="rootInvTransform">The root inv transform.</param>
    /// <returns></returns>
    public static Matrix[]? CreateNodeBasedBoneMatrices(IList<Animations.Bone> bones, ref Matrix rootInvTransform)
    {
        Matrix[]? m = null;
        CreateNodeBasedBoneMatrices(bones, ref rootInvTransform, ref m);
        return m;
    }

    /// <summary>
    /// Creates the node based bone matrices.
    /// </summary>
    /// <param name="bones">The bones.</param>
    /// <param name="matrices"></param>
    /// <param name="rootInvTransform"></param>
    /// <returns></returns>
    public static void CreateNodeBasedBoneMatrices(IList<Animations.Bone> bones, ref Matrix rootInvTransform, ref Matrix[]? matrices)
    {
        var m = matrices ?? new Matrix[bones.Count];
        for (var i = 0; i < bones.Count; ++i)
        {
            if (bones[i].Node != null)
            {
                m[i] = bones[i].InvBindPose * bones[i].Node.TotalModelMatrixInternal * rootInvTransform;
            }
            else
            {
                m[i] = Matrix.Identity;
            }
        }
        matrices = m;
    }


    public static BoneSkinnedMeshGeometry3D? CreateSkeletonMesh(IList<Animations.Bone>? bones, float scale = 1f)
    {
        if (bones is null)
        {
            return null;
        }

        var builder = new MeshBuilder(true, false);
        builder.AddPyramid(new Vector3(0, scale / 2, 0).ToVector(), Vector3.UnitZ.ToVector(), Vector3.UnitY.ToVector(), scale, 0, true);
        var singleBone = builder.ToMesh().ToMeshGeometry3D();
        var boneIds = new List<BoneIds>();
        var positions = new Vector3Collection(bones.Count * singleBone.Positions?.Count ?? 0);
        var tris = new IntCollection(bones.Count * singleBone.Indices?.Count ?? 0);

        var offset = 0;

        if (singleBone.Indices is not null && singleBone.Positions is not null)
        {
            for (var i = 0; i < bones.Count; ++i)
            {
                if (bones[i].ParentIndex >= 0)
                {
                    var currPos = positions.Count;
                    tris.AddRange(singleBone.Indices.Select(x => x + offset));
                    var j = 0;
                    for (; j < singleBone.Positions.Count - 6; j += 3)
                    {
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j + 1], bones[bones[i].ParentIndex].BindPose));
                        positions.Add(bones[i].BindPose.TranslationVector);
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                        boneIds.Add(new BoneIds() { Bone1 = i, Weights = new Vector4(1, 0, 0, 0) });
                    }
                    for (; j < singleBone.Positions.Count; ++j)
                    {
                        positions.Add(Vector3.TransformCoordinate(singleBone.Positions[j], bones[bones[i].ParentIndex].BindPose));
                        boneIds.Add(new BoneIds() { Bone1 = bones[i].ParentIndex, Weights = new Vector4(1, 0, 0, 0) });
                    }
                    offset += singleBone.Positions.Count;
                }
            }
        }

        builder = new MeshBuilder(true, false);
        for (var i = 0; i < bones.Count; ++i)
        {
            var currPos = builder.Positions.Count;
            builder.AddSphere(Vector3.Zero.ToVector(), scale / 2, 12, 12);
            for (var j = currPos; j < builder.Positions.Count; ++j)
            {
                builder.Positions[j] = Vector3.TransformCoordinate(builder.Positions[j].ToDxVector(), bones[i].BindPose).ToVector();
                boneIds.Add(new BoneIds() { Bone1 = i, Weights = new Vector4(1, 0, 0, 0) });
            }
        }
        positions.AddRange(builder.Positions.Select(t => t.ToDxVector()));
        tris.AddRange(builder.TriangleIndices.Select(x => x + offset));
        var mesh = new BoneSkinnedMeshGeometry3D() { Positions = positions, Indices = tris, VertexBoneIds = boneIds };
        mesh.Normals = Converter.ToVector3Collection(mesh.ToWndMeshGeometry3D().CalculateNormals());
        return mesh;
    }

    private IEnumerable<Triangle> SkinnedTriangles(Vector3[] skinnedVertices)
    {
        if (Indices is not null)
        {
            for (var i = 0; i < Indices.Count; i += 3)
            {
                yield return new Triangle() { P0 = skinnedVertices[Indices[i]], P1 = skinnedVertices[Indices[i + 1]], P2 = skinnedVertices[Indices[i + 2]], };
            }
        }
    }

    public virtual bool HitTestWithSkinnedVertices(HitTestContext? context, Vector3[] skinnedVertices, Matrix modelMatrix,
        ref List<HitTestResult> hits, object? originalSource)
    {
        if (context is null)
        {
            return false;
        }

        if (skinnedVertices == null || skinnedVertices.Length == 0
            || Indices == null || Indices.Count == 0)
        {
            return false;
        }
        var isHit = false;
        var result = new HitTestResult
        {
            Distance = double.MaxValue
        };
        var modelInvert = modelMatrix.Inverted();
        if (modelInvert == Matrix.Zero)//Check if model matrix can be inverted.
        {
            return false;
        }
        var rayWS = context.RayWS;
        //transform ray into model coordinates
        var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInvert), Vector3.Normalize(Vector3.TransformNormal(rayWS.Direction, modelInvert)));

        var index = 0;
        var minDistance = float.MaxValue;
        foreach (var t in SkinnedTriangles(skinnedVertices))
        {
            // Used when geometry size is really small, causes hit test failure due to SharpDX.MathUtils.ZeroTolerance.
            var scaling = 1f;
            var rayScaled = rayModel;
            if (EnableSmallTriangleHitTestScaling)
            {
                if ((t.P0 - t.P1).LengthSquared() < SmallTriangleEdgeLengthSquare
                    || (t.P1 - t.P2).LengthSquared() < SmallTriangleEdgeLengthSquare
                    || (t.P2 - t.P0).LengthSquared() < SmallTriangleEdgeLengthSquare)
                {
                    scaling = SmallTriangleHitTestScaling;
                    rayScaled = new Ray(rayModel.Position * scaling, rayModel.Direction);
                }
            }
            var v0 = t.P0 * scaling;
            var v1 = t.P1 * scaling;
            var v2 = t.P2 * scaling;

            if (Collision.RayIntersectsTriangle(ref rayScaled, ref v0, ref v1, ref v2, out float d))
            {
                if (d >= 0 && d < minDistance) // If d is NaN, the condition is false.
                {
                    minDistance = d;
                    result.IsValid = true;
                    result.ModelHit = originalSource;
                    var pointWorld = Vector3.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                    result.PointHit = pointWorld;
                    result.Distance = (rayWS.Position - pointWorld).Length();
                    var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                    var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                    var p2 = Vector3.TransformCoordinate(v2, modelMatrix);
                    var n = Vector3.Cross(p1 - p0, p2 - p0);
                    n.Normalize();
                    // transform hit-info to world space now:
                    result.NormalAtHit = n;// Vector3.TransformNormal(n, m).ToVector3D();
                    result.TriangleIndices = new System.Tuple<int, int, int>(Indices[index], Indices[index + 1], Indices[index + 2]);
                    result.Tag = index / 3;
                    result.Geometry = this;
                    isHit = true;
                    if (ReturnMultipleHitsOnHitTest)
                    {
                        hits.Add(result);
                        result = new HitTestResult();
                    }
                }
            }
            index += 3;
        }
        if (isHit)
        {
            hits.Add(result);
        }
        return isHit;
    }
}
