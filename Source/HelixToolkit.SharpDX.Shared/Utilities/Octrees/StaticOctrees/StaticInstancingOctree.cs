/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG
using HelixToolkit.Mathematics;
using System.Collections.Generic;
using Matrix = System.Numerics.Matrix4x4;
using System.Linq;
#if NETFX_CORE
namespace HelixToolkit.UWP.Utilities
#else
namespace HelixToolkit.Wpf.SharpDX.Utilities
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class StaticInstancingModelOctree : StaticOctree<KeyValuePair<int, BoundingBox>>
    {
        protected readonly IList<Matrix> InstanceMatrix;
        protected readonly BoundingBox GeometryBound;
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticInstancingModelOctree"/> class.
        /// </summary>
        /// <param name="instanceMatrix">The instance matrix.</param>
        /// <param name="geometryBound">The geometry bound.</param>
        /// <param name="parameter">The parameter.</param>
        public StaticInstancingModelOctree(IList<Matrix> instanceMatrix, BoundingBox geometryBound, OctreeBuildParameter parameter)
            : base(parameter)
        {
            InstanceMatrix = instanceMatrix;
            GeometryBound = geometryBound;
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
            var totalBound = GeometryBound.Transform(InstanceMatrix[0]);
            for (int i = 0; i < InstanceMatrix.Count; ++i)
            {
                var b = GeometryBound.Transform(InstanceMatrix[i]);
                BoundingBox.Merge(ref totalBound, ref b, out totalBound);
            }
            return totalBound;
        }
        /// <summary>
        /// Gets the objects.
        /// </summary>
        /// <returns></returns>
        protected override KeyValuePair<int, BoundingBox>[] GetObjects()
        {
            var bounds = new KeyValuePair<int, BoundingBox>[InstanceMatrix.Count];
            for (int i = 0; i < InstanceMatrix.Count; ++i)
            {
                var b = GeometryBound.Transform(InstanceMatrix[i]);
                bounds[i] = new KeyValuePair<int, BoundingBox>(i, b);
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
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="rayModel">The ray model.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="isIntersect">if set to <c>true</c> [is intersect].</param>
        /// <param name="hitThickness">The hit thickness.</param>
        /// <returns></returns>
        protected override bool HitTestCurrentNodeExcludeChild(ref Octant octant, RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt)
            {
                return false;
            }
            bool isHit = false;
            var bound = octant.Bound.Transform(modelMatrix);
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                for (int i = octant.Start; i < octant.End; ++i)
                {
                    var b = Objects[i].Value.Transform(modelMatrix);
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
        protected override bool FindNearestPointBySphereExcludeChild(ref Octant octant, RenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            return false;
        }
    }

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
            GeometryBound = geometries.Select(x=>x.Geometry.Bound.Transform(x.ModelTransform)).ToArray();
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
            for (int i = 0; i < GeometryBound.Length; ++i)
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
            for (int i = 0; i < GeometryBound.Length; ++i)
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
        /// <param name="rayWS">The ray ws.</param>
        /// <param name="rayModel">The ray model.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="isIntersect">if set to <c>true</c> [is intersect].</param>
        /// <param name="hitThickness">The hit thickness.</param>
        /// <returns></returns>
        protected override bool HitTestCurrentNodeExcludeChild(ref Octant octant, RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt)
            {
                return false;
            }
            bool isHit = false;
            var bound = octant.Bound.Transform(modelMatrix);
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                for (int i = octant.Start; i < octant.End; ++i)
                {
                    var b = Objects[i].Value.Transform(modelMatrix);
                    if (b.Intersects(ref rayWS))
                    {
                        var geo = Geometries[Objects[i].Key];
                        if(geo.Geometry is MeshGeometry3D mesh)
                        {
                            isHit |= mesh.HitTest(context, geo.ModelTransform * modelMatrix, ref rayWS, ref hits, model);
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
        protected override bool FindNearestPointBySphereExcludeChild(ref Octant octant, RenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            return false;
        }
    }
}
