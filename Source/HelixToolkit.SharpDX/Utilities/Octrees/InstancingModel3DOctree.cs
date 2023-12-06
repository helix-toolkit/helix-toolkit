using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Octree for instancing
/// </summary>
[Obsolete("Please use StaticInstancingOctree for better performance")]
public class InstancingModel3DOctree : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
{
    private IList<Matrix> InstanceMatrix;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instanceMatrix"></param>
    /// <param name="geometryBound"></param>
    /// <param name="parameter"></param>
    /// <param name="stackCache"></param>
    public InstancingModel3DOctree(IList<Matrix> instanceMatrix, BoundingBox geometryBound, OctreeBuildParameter? parameter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : base(ref geometryBound, null, parameter, stackCache)
    {
        InstanceMatrix = instanceMatrix;
        var counter = 0;
        var totalBound = geometryBound.Transform(instanceMatrix[0]);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, instanceMatrix[0])).ToArray());
        for (var i = 0; i < instanceMatrix.Count; ++i)
        {
            var b = geometryBound.Transform(instanceMatrix[i]);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, m)).ToArray());
            Objects?.Add(new KeyValuePair<int, BoundingBox>(counter, b));
            BoundingBox.Merge(ref totalBound, ref b, out totalBound);
            ++counter;
        }
        this.Bound = totalBound;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bound"></param>
    /// <param name="instanceMatrix"></param>
    /// <param name="objects"></param>
    /// <param name="parent"></param>
    /// <param name="parameter"></param>
    /// <param name="stackCache"></param>
    protected InstancingModel3DOctree(ref BoundingBox bound, IList<Matrix> instanceMatrix, List<KeyValuePair<int, BoundingBox>>? objects, IDynamicOctree? parent, OctreeBuildParameter? parameter, Stack<KeyValuePair<int, IDynamicOctree[]>>? stackCache = null)
        : base(ref bound, objects, parent, parameter, stackCache)
    {
        InstanceMatrix = instanceMatrix;
    }
    /// <summary>
    /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(HitTestContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sphere"></param>
    /// <param name="points"></param>
    /// <param name="isIntersect"></param>
    /// <returns></returns>
    public override bool FindNearestPointBySphereExcludeChild(HitTestContext? context, ref global::SharpDX.BoundingSphere sphere,
        ref List<HitTestResult> points, ref bool isIntersect)
    {
        return false;
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
        var bound = Bound.Transform(modelMatrix);// BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
        var rayWS = context.RayWS;
        if (rayWS.Intersects(ref bound))
        {
            isIntersect = true;
            if (this.Objects is not null)
            {
                for (var i = 0; i < this.Objects.Count; ++i)
                {
                    var b = Objects[i].Value.Transform(modelMatrix);// BoundingBox.FromPoints(t.Item2.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
                    if (b.Intersects(ref rayWS))
                    {
                        var result = new HitTestResult()
                        {
                            Tag = Objects[i].Key
                        };
                        hits.Add(result);
                        isHit = true;
                    }
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
    protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> objList, IDynamicOctree parent)
    {
        return new InstancingModel3DOctree(ref bound, this.InstanceMatrix, objList, parent, this.Parameter, this.stack);
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
}
