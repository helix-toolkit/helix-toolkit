using HelixToolkit.Geometry;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct2D1;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX;

[Serializable]
[DataContract]
public class MeshGeometry3D : Geometry3D
{
    private static readonly PropertyChangedEventArgs textureCoordChangedArgs = new(nameof(TextureCoordinates));

    /// <summary>
    /// Used to scale up small triangle during hit test.
    /// </summary>
    public static float SmallTriangleHitTestScaling = 1e3f;
    /// <summary>
    /// Used to determine if the triangle is small.
    /// Small triangle is defined as any edge length square is smaller than
    /// <see cref="SmallTriangleEdgeLengthSquare"/>.
    /// </summary>
    public static float SmallTriangleEdgeLengthSquare = 1e-3f;
    /// <summary>
    /// Used to enable small triangle hit test. It uses <see cref="SmallTriangleEdgeLengthSquare"/>
    /// to determine if triangle is too small. If it is too small, scale up the triangle before
    /// hit test.
    /// </summary>
    public static bool EnableSmallTriangleHitTestScaling = true;
    /// <summary>
    /// Does not raise property changed event
    /// </summary>
    [DataMember]
    public Vector3Collection? Normals
    {
        get; set;
    }

    private Vector2Collection? textureCoordinates = null;
    /// <summary>
    /// Texture Coordinates
    /// </summary>
    [DataMember]
    public Vector2Collection? TextureCoordinates
    {
        get
        {
            return textureCoordinates;
        }
        set
        {
            if (Set(ref textureCoordinates, value, false))
            {
                RaisePropertyChanged(textureCoordChangedArgs);
            }
        }
    }
    /// <summary>
    /// Does not raise property changed event
    /// </summary>
    [DataMember]
    public Vector3Collection? Tangents
    {
        get; set;
    }

    /// <summary>
    /// Does not raise property changed event
    /// </summary>
    [DataMember]
    public Vector3Collection? BiTangents
    {
        get; set;
    }

    public IEnumerable<Triangle> Triangles
    {
        get
        {
            if (Indices is not null && Positions is not null)
            {
                for (var i = 0; i < Indices.Count; i += 3)
                {
                    yield return new Triangle() { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], P2 = Positions[Indices[i + 2]], };
                }
            }
        }
    }

    /// <summary>
    /// A proxy member for <see cref="Geometry3D.Indices"/>
    /// </summary>
    [IgnoreDataMember]
    public IntCollection? TriangleIndices
    {
        get
        {
            return Indices;
        }
        set
        {
            Indices = value;
        }
    }

    /// <summary>
    /// Merge meshes into one
    /// </summary>
    /// <param name="meshes"></param>
    /// <returns></returns>
    public static MeshGeometry3D Merge(params MeshGeometry3D[] meshes)
    {
        var positions = new Vector3Collection();
        var indices = new IntCollection();

        var normals = meshes.All(x => x.Normals != null) ? new Vector3Collection() : null;
        var colors = meshes.All(x => x.Colors != null) ? new Color4Collection() : null;
        var textureCoods = meshes.All(x => x.TextureCoordinates != null) ? new Vector2Collection() : null;
        var tangents = meshes.All(x => x.Tangents != null) ? new Vector3Collection() : null;
        var bitangents = meshes.All(x => x.BiTangents != null) ? new Vector3Collection() : null;

        var index = 0;
        foreach (var part in meshes)
        {
            if (part.Positions is null || part.Indices is null)
            {
                continue;
            }

            positions.AddRange(part.Positions);
            indices.AddRange(part.Indices.Select(x => x + index));
            index += part.Positions.Count;
        }

        if (normals != null)
        {
            normals = new Vector3Collection(meshes.SelectMany(x => x.Normals!));
        }

        if (colors != null)
        {
            colors = new Color4Collection(meshes.SelectMany(x => x.Colors!));
        }

        if (textureCoods != null)
        {
            textureCoods = new Vector2Collection(meshes.SelectMany(x => x.TextureCoordinates!));
        }

        if (tangents != null)
        {
            tangents = new Vector3Collection(meshes.SelectMany(x => x.Tangents!));
        }

        if (bitangents != null)
        {
            bitangents = new Vector3Collection(meshes.SelectMany(x => x.BiTangents!));
        }

        var mesh = new MeshGeometry3D()
        {
            Positions = positions,
            Indices = indices,
            Normals = normals,
            Colors = colors,
            TextureCoordinates = textureCoods,
            Tangents = tangents,
            BiTangents = bitangents
        };
        return mesh;
    }


    protected override IOctreeBasic? CreateOctree(OctreeBuildParameter parameter)
    {
        if (this.Positions is null || this.Indices is null)
        {
            return null;
        }

        return new StaticMeshGeometryOctree(this.Positions, this.Indices, parameter);
    }

    protected override void OnAssignTo(Geometry3D target)
    {
        base.OnAssignTo(target);
        if (target is MeshGeometry3D mesh)
        {
            mesh.Normals = this.Normals;
            mesh.TextureCoordinates = this.TextureCoordinates;
            mesh.Tangents = this.Tangents;
            mesh.BiTangents = this.BiTangents;
        }
    }

    public void UpdateNormals()
    {
        if (Positions is null || Indices is null) return;
        var normals = MeshGeometryHelper.CalculateNormals(Positions, Indices);
        Normals ??= new Vector3Collection();
        if (normals is Vector3Collection ns)
        {
            Normals = ns;
        }
        else
        {
            Normals.Clear();
            Normals.AddRange(normals);
        }
    }


    /// <summary>
    /// Callers should set this property to true before calling HitTest if the callers need multiple hits throughout the geometry.
    /// This is useful when the geometry is cut by a plane.
    /// </summary>
    public bool ReturnMultipleHitsOnHitTest { get; set; } = false;

    public virtual bool HitTest(HitTestContext? context, Matrix modelMatrix, ref List<HitTestResult> hits, object? originalSource)
    {
        if (context is null)
        {
            return false;
        }

        if (Positions == null || Positions.Count == 0
            || Indices == null || Indices.Count == 0)
        {
            return false;
        }
        var isHit = false;
        if (Octree != null)
        {
            isHit = Octree.HitTest(context, originalSource, this, modelMatrix, ReturnMultipleHitsOnHitTest, ref hits);
        }
        else
        {
            var result = new HitTestResult
            {
                Distance = double.MaxValue
            };
            var modelInvert = modelMatrix.Inverted();
            if (modelInvert == MatrixHelper.Zero)//Check if model matrix can be inverted.
            {
                return false;
            }
            //transform ray into model coordinates
            var rayModel = new Ray(Vector3Helper.TransformCoordinate(context.RayWS.Position, modelInvert), Vector3.Normalize(Vector3.TransformNormal(context.RayWS.Direction, modelInvert)));

            var b = this.Bound;

            //Do hit test in local space
            if (rayModel.Intersects(ref b))
            {
                var index = 0;
                var minDistance = float.MaxValue;

                foreach (var t in Triangles)
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
                        d /= scaling;
                        // For CrossSectionMeshGeometryModel3D another hit than the closest may be the valid one, since the closest one might be removed by a crossing plane
                        if (ReturnMultipleHitsOnHitTest)
                        {
                            minDistance = float.MaxValue;
                        }

                        if (d >= 0 && d < minDistance) // If d is NaN, the condition is false.
                        {
                            minDistance = d;
                            result.IsValid = true;
                            result.ModelHit = originalSource;
                            // transform hit-info to world space now:
                            var pointWorld = Vector3Helper.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                            result.PointHit = pointWorld;
                            result.Distance = (context.RayWS.Position - pointWorld).Length();
                            var p0 = Vector3Helper.TransformCoordinate(t.P0, modelMatrix);
                            var p1 = Vector3Helper.TransformCoordinate(t.P1, modelMatrix);
                            var p2 = Vector3Helper.TransformCoordinate(t.P2, modelMatrix);
                            var n = Vector3.Normalize(Vector3.Cross(p1 - p0, p2 - p0));
                            // transform hit-info to world space now:
                            result.NormalAtHit = n;// Vector3.TransformNormal(n, m).ToVector3D();
                            result.TriangleIndices = new System.Tuple<int, int, int>(Indices[index], Indices[index + 1], Indices[index + 2]);
                            result.Tag = index / 3;
                            result.IndiceStartLocation = index / 3;
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
            }
            if (isHit && result.IsValid && !ReturnMultipleHitsOnHitTest)
            {
                hits.Add(result);
            }
        }
        return isHit;
    }

    /// <summary>
    /// Call to manually update texture coordinate buffer.
    /// </summary>
    public void UpdateTextureCoordinates()
    {
        RaisePropertyChanged(nameof(TextureCoordinates));
    }

    protected override void OnClearAllGeometryData()
    {
        base.OnClearAllGeometryData();
        Normals?.Clear();
        Normals?.TrimExcess();
        TextureCoordinates?.Clear();
        TextureCoordinates?.TrimExcess();
        Tangents?.Clear();
        Tangents?.TrimExcess();
        BiTangents?.Clear();
        BiTangents?.TrimExcess();
    }
}
