using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Octree for points
/// </summary>
[Obsolete("Please use StaticPointGeometryOctree for better performance")]
public class PointGeometryOctree : DynamicOctreeBase<int>
{
    private IList<Vector3>? Positions;
    private static readonly Vector3 BoundOffset = new(0.0001f);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="stackCache"></param>
    public PointGeometryOctree(IList<Vector3>? positions, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : this(positions, null, stackCache)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="positions"></param>
    /// <param name="parameter"></param>
    /// <param name="stackCache"></param>
    public PointGeometryOctree(IList<Vector3>? positions,
        OctreeBuildParameter? parameter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
           : base(null, parameter, stackCache)
    {
        Positions = positions;
        Bound = BoundingBoxExtensions.FromPoints(positions);
        Objects = Positions is null ? null : new List<int>(Positions.Count);
        if (Positions is not null)
        {
            for (var i = 0; i < Positions.Count; ++i)
            {
                Objects!.Add(i);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="positions"></param>
    /// <param name="list"></param>
    /// <param name="parent"></param>
    /// <param name="paramter"></param>
    /// <param name="stackCache"></param>
    protected PointGeometryOctree(BoundingBox bound, IList<Vector3>? positions, List<int>? list, IDynamicOctree? parent, OctreeBuildParameter? paramter,
        Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache)
        : base(ref bound, list, parent, paramter, stackCache)
    {
        Positions = positions;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    protected override bool IsContains(BoundingBox source, BoundingBox target, int obj)
    {
        return source.Contains(Positions![obj]) != ContainmentType.Disjoint;
    }
    /// <summary>
    /// Get the distance from ray to a point
    /// </summary>
    /// <param name="r"></param>
    /// <param name="p"></param>
    /// <returns></returns>
    public static double DistanceRayToPoint(ref Ray r, ref Vector3 p)
    {
        var v = r.Direction;
        var w = p - r.Position;

        var c1 = Vector3.Dot(w, v);
        var c2 = Vector3.Dot(v, v);
        var b = c1 / c2;

        var pb = r.Position + v * b;
        return (p - pb).Length();
    }
    /// <summary>
    /// Return nearest point it gets hit. And the distance from ray origin to the point it gets hit
    /// </summary>
    /// <param name="model"></param>
    /// <param name="geometry"></param>
    /// <param name="modelMatrix"></param>
    /// <param name="hits"></param>
    /// <param name="isIntersect"></param>
    /// <param name="context"></param>
    /// <param name="rayModel"></param>
    /// <param name="hitThickness"></param>
    /// <returns></returns>
    public override bool HitTestCurrentNodeExcludeChild(HitTestContext? context, object? model, Geometry3D? geometry, Matrix modelMatrix,
        ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
    {
        isIntersect = false;
        if (!this.treeBuilt || context == null)
        {
            return false;
        }
        var isHit = false;

        var bound = Bound;

        if (rayModel.Intersects(ref bound))
        {
            isIntersect = true;
            if (Objects is null || Objects.Count == 0)
            {
                return false;
            }
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var svpm = context.RenderMatrices?.ScreenViewProjectionMatrix ?? Matrix.Identity;
            var smvpm = modelMatrix * svpm;
            var clickPoint3 = context.HitPointSP.ToVector3() * (context.RenderMatrices?.DpiScale ?? 1.0f);
            var rayWS = context.RayWS;
            var pos3 = rayWS.Position;
            Vector3.TransformCoordinate(ref clickPoint3, ref svpm, out var clickPoint);
            Vector3.TransformCoordinate(ref pos3, ref svpm, out pos3);

            var dist = hitThickness;
            for (var i = 0; i < Objects.Count; ++i)
            {
                var v0 = Positions![Objects[i]];
                var p0 = Vector3.TransformCoordinate(v0, smvpm);
                var pv = p0 - clickPoint;
                var d = pv.Length();
                if (d < dist) // If d is NaN, the condition is false.
                {
                    dist = d;
                    result.IsValid = true;
                    result.ModelHit = model;
                    var px = Vector3.TransformCoordinate(v0, modelMatrix);
                    result.PointHit = px;
                    result.Distance = (rayWS.Position - px).Length();
                    result.Tag = Objects[i];
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
    /// 
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="objList"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<int>? objList, IDynamicOctree? parent)
    {
        return new PointGeometryOctree(bound, Positions, objList, parent, Parameter, stack);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected override BoundingBox GetBoundingBoxFromItem(int item)
    {
        return new BoundingBox(Positions![item] - BoundOffset, Positions[item] + BoundOffset);
    }
    /// <summary>
    /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(HitTestContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
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
            var resultTemp = new HitTestResult();
            resultTemp.Distance = float.MaxValue;
            for (var i = 0; i < Objects.Count; ++i)
            {
                var p = Positions![Objects[i]];
                containment = sphere.Contains(ref p);
                if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                {
                    var d = (p - sphere.Center).Length();
                    if (resultTemp.Distance > d)
                    {
                        resultTemp.Distance = d;
                        resultTemp.IsValid = true;
                        resultTemp.PointHit = p;
                        resultTemp.Tag = Objects[i];
                        isHit = true;
                    }
                }
            }
            if (isHit)
            {
                isHit = false;
                if (result.Count > 0)
                {
                    if (result[0].Distance > resultTemp.Distance)
                    {
                        result[0] = resultTemp;
                        isHit = true;
                    }
                }
                else
                {
                    result.Add(resultTemp);
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
