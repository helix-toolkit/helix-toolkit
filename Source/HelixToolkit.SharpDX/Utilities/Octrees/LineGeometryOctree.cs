using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[Obsolete("Please use StaticLineGeometryOctree for better performance")]
public class LineGeometryOctree : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
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
    public LineGeometryOctree(IList<Vector3>? positions, IList<int>? indices, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
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
    public LineGeometryOctree(IList<Vector3>? positions, IList<int>? indices,
        OctreeBuildParameter? parameter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : base(null, parameter, stackCache)
    {
        Positions = positions;
        Indices = indices;
        Bound = BoundingBoxExtensions.FromPoints(positions);
        Objects = indices is null ? null : new List<KeyValuePair<int, BoundingBox>>(indices.Count / 2);
        // Construct triangle index and its bounding box KeyValuePair
        if (indices is not null)
        {
            for (var i = 0; i < indices.Count / 2; ++i)
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
    protected LineGeometryOctree(IList<Vector3>? positions, IList<int>? indices, ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> triIndex,
        IDynamicOctree? parent, OctreeBuildParameter? paramter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache)
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
    protected LineGeometryOctree(BoundingBox bound, List<KeyValuePair<int, BoundingBox>>? list, IDynamicOctree? parent, OctreeBuildParameter? paramter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache)
        : base(ref bound, list, parent, paramter, stackCache)
    {
    }

    private BoundingBox GetBoundingBox(int triangleIndex)
    {
        var actual = triangleIndex * 2;
        var v1 = Positions![Indices![actual++]];
        var v2 = Positions![Indices![actual]];
        var maxX = Math.Max(v1.X, v2.X);
        var maxY = Math.Max(v1.Y, v2.Y);
        var maxZ = Math.Max(v1.Z, v2.Z);

        var minX = Math.Min(v1.X, v2.X);
        var minY = Math.Min(v1.Y, v2.Y);
        var minZ = Math.Min(v1.Z, v2.Z);

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
        return new LineGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.stack);
    }
    /// <summary>
    /// <see cref="DynamicOctreeBase{T}.HitTestCurrentNodeExcludeChild(HitTestContext, object, Geometry3D, Matrix, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
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
        bound.Maximum += new Vector3(hitThickness);
        bound.Minimum -= new Vector3(hitThickness);
        var lastDist = double.MaxValue;
        //Hit test in local space.
        if (rayModel.Intersects(ref bound))
        {
            isIntersect = true;
            if (Objects is null || Objects.Count == 0)
            {
                return false;
            }
            var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
            result.Distance = double.MaxValue;
            var rayWS = context.RayWS;
            for (var i = 0; i < Objects.Count; ++i)
            {
                var idx = Objects[i].Key * 2;
                var idx1 = Indices![idx];
                var idx2 = Indices![idx + 1];
                var v0 = Positions![idx1];
                var v1 = Positions![idx2];

                var t0 = Vector3.TransformCoordinate(v0, modelMatrix);
                var t1 = Vector3.TransformCoordinate(v1, modelMatrix);
                Vector3 sp, tp;
                float sc, tc;
                var rayToLineDistance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out sp, out tp, out sc, out tc);
                var svpm = context.RenderMatrices?.ScreenViewProjectionMatrix ?? Matrix.Identity;
                Vector4 sp4;
                Vector4 tp4;
                Vector3.Transform(ref sp, ref svpm, out sp4);
                Vector3.Transform(ref tp, ref svpm, out tp4);
                var sp3 = sp4.ToVector3();
                var tp3 = tp4.ToVector3();
                var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                var dist = tv2.Length();
                if (dist < lastDist && dist <= hitThickness)
                {
                    lastDist = dist;
                    result.PointHit = sp;
                    result.NormalAtHit = sp - tp; // not normalized to get length
                    result.Distance = (rayWS.Position - sp).Length();
                    result.RayToLineDistance = rayToLineDistance;
                    result.ModelHit = model;
                    result.IsValid = true;
                    result.Tag = Objects[i].Key; // For compatibility
                    result.LineIndex = Objects[i].Key;
                    result.TriangleIndices = null; // Since triangles are shader-generated
                    result.RayHitPointScalar = sc;
                    result.LineHitPointScalar = tc;
                    result.Geometry = geometry;
                    isHit = true;
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
    /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(HitTestContext, ref global::SharpDX.BoundingSphere,
    /// ref List{HitTestResult}, ref bool)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sphere"></param>
    /// <param name="result"></param>
    /// <param name="isIntersect"></param>
    /// <returns></returns>
    public override bool FindNearestPointBySphereExcludeChild(HitTestContext? context, ref global::SharpDX.BoundingSphere sphere,
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
            var tempResult = new LineHitTestResult();
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
                    var v0 = Positions![t1];
                    var v1 = Positions![t2];
                    float t;
                    var distance = LineBuilder.GetPointToLineDistance2D(ref sphere.Center, ref v0, ref v1, out cloestPoint, out t);
                    if (tempResult.Distance > distance)
                    {
                        tempResult.Distance = distance;
                        tempResult.IsValid = true;
                        tempResult.PointHit = cloestPoint;
                        tempResult.TriangleIndices = null;
                        tempResult.RayHitPointScalar = t;
                        tempResult.Tag = tempResult.LineIndex = Objects[i].Key;
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
