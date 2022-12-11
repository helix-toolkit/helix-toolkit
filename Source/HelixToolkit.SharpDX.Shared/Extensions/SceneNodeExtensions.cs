/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public static class SceneNodeExtensions
    {
        /// <summary>
        /// Gets the type of the scene node from scene graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static IList<T> GetSceneNodeByType<T>(this SceneNode root) where T : SceneNode
        {
            var ret = new List<T>();
            foreach (var node in root.Traverse())
            {
                if (node is T m)
                {
                    ret.Add(m);
                }
            }
            return ret;
        }

        /// <summary>
        /// Updates all transform matrix from the root node to the child.
        /// </summary>
        /// <param name="root">The root.</param>
        public static void UpdateAllTransformMatrix(this SceneNode root)
        {
            foreach (var node in root.Traverse())
            {
                node.ComputeTransformMatrix();
            }
        }

        /// <summary>
        /// Try to get centroid of all meshes from current scene root. Centroid is calculated by averaging all vertices.
        /// <para>
        /// To make sure all transform matrics are updated.
        /// Call <see cref="UpdateAllTransformMatrix(SceneNode)"/> before calling <see cref="TryGetCentroid(SceneNode, out Vector3)"/>.
        /// </para>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public static bool TryGetCentroid(this SceneNode root, out Vector3 centroid)
        {
            Vector3? result = null;
            int count = 0;
            foreach (var node in root.Traverse())
            {
                if (node is GeometryNode geoNode)
                {
                    if (geoNode.Geometry != null
                        && geoNode.Geometry.Positions != null
                        && geoNode.Geometry.Positions.Count > 0)
                    {
                        var c = geoNode.Geometry.Positions.GetCentroid();
                        c = Vector3.Transform(c, geoNode.TotalModelMatrix).ToVector3();
                        ++count;
                        if (result.HasValue)
                        {
                            result += (c - result.Value) / count;
                        }
                        else
                        {
                            result = c;
                        }
                    }
                }
            }
            centroid = result.HasValue ? result.Value : Vector3.Zero;
            return result.HasValue;
        }

        /// <summary>
        /// Try to get total bound of all meshes from current scene root.
        /// To make sure all transform matrics are updated.
        /// Call <see cref="UpdateAllTransformMatrix(SceneNode)"/> before calling <see cref="TryGetBound(SceneNode, out BoundingBox)"/>.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static bool TryGetBound(this SceneNode root, out BoundingBox bound)
        {
            BoundingBox? result = null;
            foreach (var node in root.Traverse())
            {
                if (node is GeometryNode geoNode)
                {
                    if (geoNode.Geometry != null
                        && geoNode.Geometry.Positions != null
                        && geoNode.Geometry.Positions.Count > 0)
                    {
                        geoNode.Geometry.UpdateBounds();
                        var b = geoNode.Geometry.Bound;
                        b = b.Transform(geoNode.TotalModelMatrix);
                        if (result.HasValue)
                        {
                            result = BoundingBox.Merge(result.Value, b);
                        }
                        else
                        {
                            result = b;
                        }
                    }
                }
            }
            bound = result.HasValue ? result.Value : new BoundingBox();
            return result.HasValue;
        }
    }
}
