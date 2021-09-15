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
        public static IList<T> GetSceneNodeByType<T>(SceneNode root) where T : SceneNode
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
        public static void UpdateAllTransformMatrix(SceneNode root)
        {
            foreach (var node in root.Traverse())
            {
                node.ComputeTransformMatrix();
            }
        }
    }
}
