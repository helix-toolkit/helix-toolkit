using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// Octree for batched geometry array
/// </summary>
public class StaticBatchedGeometryBoundsOctree : StaticOctree<KeyValuePair<int, BoundingBox>>
{
    protected readonly BatchedMeshGeometryConfig[] Geometries;
    protected readonly BoundingBox[] GeometryBound;
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticInstancingModelOctree"/> class.
    /// </summary>
    /// <param name="geometries">Batched geometries.</param>
    /// <param name="parameter">The parameter.</param>
    public StaticBatchedGeometryBoundsOctree(BatchedMeshGeometryConfig[] geometries, OctreeBuildParameter parameter)
        : base(parameter)
    {
        Geometries = geometries;
        GeometryBound = geometries.Select(x => x.Geometry.Bound.Transform(x.ModelTransform)).ToArray();
    }
    /// <summary>
    /// Gets the bounding box from item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected override BoundingBox GetBoundingBoxFromItem(ref KeyValuePair<int, BoundingBox> item)
    {
        return item.Value;
    }
    /// <summary>
    /// Gets the maximum bound.
    /// </summary>
    /// <returns></returns>
    protected override BoundingBox GetMaxBound()
    {
        var totalBound = GeometryBound[0];
        for (var i = 0; i < GeometryBound.Length; ++i)
        {
            BoundingBox.Merge(ref totalBound, ref GeometryBound[i], out totalBound);
        }
        return totalBound;
    }
    /// <summary>
    /// Gets the objects.
    /// </summary>
    /// <returns></returns>
    protected override KeyValuePair<int, BoundingBox>[] GetObjects()
    {
        var bounds = new KeyValuePair<int, BoundingBox>[GeometryBound.Length];
        for (var i = 0; i < GeometryBound.Length; ++i)
        {
            bounds[i] = new KeyValuePair<int, BoundingBox>(i, GeometryBound[i]);
        }
        return bounds;
    }
    /// <summary>
    /// Hits the test current node exclude child.
    /// </summary>
    /// <param name="octant">The octant.</param>
    /// <param name="context">The context.</param>
    /// <param name="model">The model.</param>
    /// <param name="geometry"></param>
    /// <param name="modelMatrix">The model matrix.</param>
    /// <param name="rayModel">The ray model.</param>
    /// <param name="returnMultiple"></param>
    /// <param name="hits">The hits.</param>
    /// <param name="isIntersect">if set to <c>true</c> [is intersect].</param>
    /// <param name="hitThickness">The hit thickness.</param>
    /// <returns></returns>
    protected override bool HitTestCurrentNodeExcludeChild(ref Octant octant, HitTestContext? context,
        object? model, Geometry3D? geometry, Matrix modelMatrix, ref Ray rayModel, bool returnMultiple,
        ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
    {
        if (context is null)
        {
            return false;
        }

        isIntersect = false;
        if (!octant.IsBuilt)
        {
            return false;
        }
        var isHit = false;
        var bound = octant.Bound.Transform(modelMatrix);
        var rayWS = context.RayWS;
        if (rayWS.Intersects(ref bound))
        {
            isIntersect = true;
            for (var i = octant.Start; i < octant.End; ++i)
            {
                var b = Objects![i].Value.Transform(modelMatrix);
                if (b.Intersects(ref rayWS))
                {
                    ref var geo = ref Geometries[Objects[i].Key];
                    if (geo.Geometry is MeshGeometry3D mesh)
                    {
                        var currCount = hits.Count;
                        var hasHit = mesh.HitTest(context, geo.ModelTransform * modelMatrix, ref hits, model);
                        if (hasHit)
                        {
                            var newCount = hits.Count;
                            for (var j = currCount; j < newCount; ++j)
                            {
                                hits.Add(new BatchedMeshHitTestResult(Objects[i].Key, ref geo, hits[j]));
                            }
                            hits.RemoveRange(currCount, newCount - currCount);
                        }
                        isHit |= hasHit;
                    }
                }
            }
        }
        return isHit;
    }
    /// <summary>
    /// Finds the nearest point by sphere exclude child.
    /// </summary>
    /// <param name="octant">The octant.</param>
    /// <param name="context">The context.</param>
    /// <param name="sphere">The sphere.</param>
    /// <param name="points">The points.</param>
    /// <param name="isIntersect">if set to <c>true</c> [is intersect].</param>
    /// <returns></returns>
    protected override bool FindNearestPointBySphereExcludeChild(ref Octant octant, HitTestContext? context, ref BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
    {
        return false;
    }
}
