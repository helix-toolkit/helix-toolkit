using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// MeshGeometryOctree slices mesh geometry by triangles into octree. Objects are KeyValuePair of each triangle index and its bounding box.
/// </summary>
[Obsolete("Please use StaticMeshGeometryOctree for better performance")]
public class MeshGeometryOctree
    : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
{
    /// <summary>
    /// 
    /// </summary>
    public IList<Vector3>? Positions
    {
        private set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    public IList<int>? Indices
    {
        private set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="indices"></param>
    /// <param name="stackCache"></param>
    public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : this(positions, indices, null, stackCache)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="indices"></param>
    /// <param name="parameter"></param>
    /// <param name="stackCache"></param>
    public MeshGeometryOctree(IList<Vector3>? positions, IList<int>? indices,
        OctreeBuildParameter? parameter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : base(null, parameter, stackCache)
    {
        Positions = positions;
        Indices = indices;
        Bound = BoundingBoxExtensions.FromPoints(positions);
        Objects = indices is null ? null : new List<KeyValuePair<int, BoundingBox>>(indices.Count / 3);
        // Construct triangle index and its bounding box KeyValuePair
        if (indices is not null)
        {
            for (var i = 0; i < indices.Count / 3; ++i)
            {
                Objects!.Add(new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i)));
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="indices"></param>
    /// <param name="bound"></param>
    /// <param name="triIndex"></param>
    /// <param name="parent"></param>
    /// <param name="paramter"></param>
    /// <param name="stackCache"></param>
    protected MeshGeometryOctree(IList<Vector3>? positions, IList<int>? indices, ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> triIndex,
        IDynamicOctree parent, OctreeBuildParameter? paramter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache)
        : base(ref bound, triIndex, parent, paramter, stackCache)
    {
        Positions = positions;
        Indices = indices;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="list"></param>
    /// <param name="parent"></param>
    /// <param name="paramter"></param>
    /// <param name="stackCache"></param>
    protected MeshGeometryOctree(BoundingBox bound, List<KeyValuePair<int, BoundingBox>> list, IDynamicOctree parent, OctreeBuildParameter? paramter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache)
        : base(ref bound, list, parent, paramter, stackCache)
    {
    }

    private BoundingBox GetBoundingBox(int triangleIndex)
    {
        var actual = triangleIndex * 3;
        var v1 = Positions![Indices![actual++]];
        var v2 = Positions![Indices![actual++]];
        var v3 = Positions![Indices![actual]];
        var maxX = Math.Max(v1.X, Math.Max(v2.X, v3.X));
        var maxY = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y));
        var maxZ = Math.Max(v1.Z, Math.Max(v2.Z, v3.Z));

        var minX = Math.Min(v1.X, Math.Min(v2.X, v3.X));
        var minY = Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
        var minZ = Math.Min(v1.Z, Math.Min(v2.Z, v3.Z));

        return new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
    {
        return item.Value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="region"></param>
    /// <param name="objList"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox region, List<KeyValuePair<int, BoundingBox>> objList, IDynamicOctree parent)
    {
        return new MeshGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.stack);
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="geometry"></param>
    /// <param name="modelMatrix"></param>
    /// <param name="rayModel"></param>
    /// <param name="hits"></param>
    /// <param name="isIntersect"></param>
    /// <param name="hitThickness"></param>
    /// <returns></returns>
    public override bool HitTestCurrentNodeExcludeChild(HitTestContext? context, object? model, Geometry3D? geometry, Matrix modelMatrix,
        ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
    {
        if (context is null)
        {
            return false;
        }

        isIntersect = false;
        if (!this.treeBuilt)
        {
            return false;
        }
        var isHit = false;
        var bound = Bound;
        //Hit test in local space.
        if (rayModel.Intersects(ref bound))
        {
            isIntersect = true;
            if (Objects is null || Objects.Count == 0)
            {
                return false;
            }
            var result = new HitTestResult
            {
                Distance = double.MaxValue
            };
            var rayWS = context.RayWS;
            for (var i = 0; i < Objects.Count; ++i)
            {
                var idx = Objects[i].Key * 3;
                var t1 = Indices![idx];
                var t2 = Indices![idx + 1];
                var t3 = Indices![idx + 2];
                var v0 = Positions![t1];
                var v1 = Positions![t2];
                var v2 = Positions![t3];
                float d;

                if (Collision.RayIntersectsTriangle(ref rayModel, ref v0, ref v1, ref v2, out d))
                {
                    if (d >= 0 && d < result.Distance) // If d is NaN, the condition is false.
                    {
                        result.IsValid = true;
                        result.ModelHit = model;
                        // transform hit-info to world space now:
                        var pointWorld = Vector3Helper.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                        result.PointHit = pointWorld;
                        result.Distance = (rayWS.Position - pointWorld).Length();

                        var p0 = Vector3Helper.TransformCoordinate(v0, modelMatrix);
                        var p1 = Vector3Helper.TransformCoordinate(v1, modelMatrix);
                        var p2 = Vector3Helper.TransformCoordinate(v2, modelMatrix);
                        var n = Vector3.Normalize(Vector3.Cross(p1 - p0, p2 - p0));
                        // transform hit-info to world space now:
                        result.NormalAtHit = n;// Vector3.TransformNormal(n, m).ToVector3D();
                        result.TriangleIndices = new Tuple<int, int, int>(t1, t2, t3);
                        result.Tag = Objects[i].Key;
                        result.Geometry = geometry;
                        isHit = true;
                    }
                }
            }

            if (isHit)
            {
                isHit = false;
                if (hits.Count > 0)
                {
                    if (hits[0].Distance > result.Distance)
                    {
                        hits[0] = result;
                        isHit = true;
                    }
                }
                else
                {
                    hits.Add(result);
                    isHit = true;
                }
            }
        }

        return isHit;
    }
    /// <summary>
    /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(HitTestContext, ref BoundingSphere, ref List{HitTestResult}, ref bool)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sphere"></param>
    /// <param name="result"></param>
    /// <param name="isIntersect"></param>
    /// <returns></returns>
    public override bool FindNearestPointBySphereExcludeChild(HitTestContext? context, ref BoundingSphere sphere,
        ref List<HitTestResult> result, ref bool isIntersect)
    {
        var isHit = false;
        var containment = Bound.Contains(ref sphere);
        if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
        {
            isIntersect = true;
            if (Objects is null || Objects.Count == 0)
            {
                return false;
            }
            var tempResult = new HitTestResult();
            tempResult.Distance = float.MaxValue;
            for (var i = 0; i < Objects.Count; ++i)
            {
                containment = Objects[i].Value.Contains(sphere);
                if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                {
                    Vector3 cloestPoint;

                    var idx = Objects[i].Key * 3;
                    var t1 = Indices![idx];
                    var t2 = Indices![idx + 1];
                    var t3 = Indices![idx + 2];
                    var v0 = Positions![t1];
                    var v1 = Positions![t2];
                    var v2 = Positions![t3];
                    Collision.ClosestPointPointTriangle(ref sphere.Center, ref v0, ref v1, ref v2, out cloestPoint);
                    var d = (cloestPoint - sphere.Center).Length();
                    if (tempResult.Distance > d)
                    {
                        tempResult.Distance = d;
                        tempResult.IsValid = true;
                        tempResult.PointHit = cloestPoint;
                        tempResult.TriangleIndices = new Tuple<int, int, int>(t1, t2, t3);
                        tempResult.Tag = Objects[i].Key;
                        isHit = true;
                    }
                }
            }
            if (isHit)
            {
                isHit = false;
                if (result.Count > 0)
                {
                    if (result[0].Distance > tempResult.Distance)
                    {
                        result[0] = tempResult;
                        isHit = true;
                    }
                }
                else
                {
                    result.Add(tempResult);
                    isHit = true;
                }
            }
        }
        else
        {
            isIntersect = false;
        }
        return isHit;
    }
}
