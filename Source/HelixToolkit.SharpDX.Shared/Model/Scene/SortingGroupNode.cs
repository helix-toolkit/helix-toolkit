/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
    namespace Model.Scene
    {
        using SortStruct = KeyValuePair<float, SceneNode>;
        /// <summary>
        /// Used for geometry sorting
        /// </summary>
        public enum SortingMethod
        {
            /// <summary>
            /// Sort on the distance from camera to bounding bound center.
            /// </summary>
            BoundingBoxCenter,

            /// <summary>
            /// Sort on the minimum distance from camera to bounding bound corners.
            /// </summary>
            BoundingBoxCorners,

            /// <summary>
            /// Sort on the minimum distance from camera to bounding sphere surface.
            /// </summary>
            BoundingSphereSurface
        }

        public class SortingGroupNode : GroupNode
        {
            /// <summary>
            /// Gets or sets a value indicating whether [enable sorting].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable sorting]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableSorting { set; get; } = true;
            /// <summary>
            /// Gets or sets the sorting interval by milliseconds. Default is 500ms.
            /// </summary>
            /// <value>
            /// The sorting interval.
            /// </value>
            public int SortingInterval { set; get; } = 500;
            /// <summary>
            /// Gets or sets the last sort time.
            /// </summary>
            /// <value>
            /// The last sort time.
            /// </value>
            public long LastSortTime { private set; get; } = 0;
            /// <summary>
            /// Gets or sets a value indicating whether [sort transparent only].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [sort transparent only]; otherwise, <c>false</c>.
            /// </value>
            public bool SortTransparentOnly { set; get; } = true;
            /// <summary>
            /// Gets or sets the sorting method.
            /// </summary>
            /// <value>
            /// The sorting method.
            /// </value>
            public SortingMethod SortingMethod { set; get; } = SortingMethod.BoundingBoxCorners;

            private readonly List<SortStruct> sortingTransparentCache = new List<SortStruct>();
            private readonly List<SortStruct> sortingOpaqueCache = new List<SortStruct>();
            private readonly List<SceneNode> notSorted = new List<SceneNode>();

            protected override bool OnAttach(IRenderHost host)
            {
                LastSortTime = 0;
                return base.OnAttach(host);
            }

            public override void UpdateNotRender(RenderContext context)
            {
                base.UpdateNotRender(context);
                if (!EnableSorting || ItemsInternal.Count == 0)
                { return; }
                long currTime = Stopwatch.GetTimestamp() * 1000 / Stopwatch.Frequency;

                if (currTime - LastSortTime > SortingInterval)
                {
                    Sort(ItemsInternal, context);
                    LastSortTime = currTime;
                }
            }

            protected virtual void Sort(IList<SceneNode> nodes, RenderContext context)
            {
                sortingTransparentCache.Clear();
                sortingOpaqueCache.Clear();
                notSorted.Clear();

                Vector3 cameraPosition = context.Camera.Position;
                if (SortTransparentOnly)
                {
                    for (int i = 0; i < nodes.Count; ++i)
                    {
                        if (nodes[i].RenderCore.RenderType == RenderType.Transparent)
                        {
                            sortingTransparentCache.Add(new SortStruct(GetDistance(nodes[i], ref cameraPosition), nodes[i]));
                        }
                        else
                        {
                            notSorted.Add(nodes[i]);
                        }
                    }
                    sortingTransparentCache.Sort(delegate (SortStruct a, SortStruct b) { return a.Key > b.Key ? -1 : a.Key < b.Key ? 1 : 0; });
                }
                else
                {
                    for (int i = 0; i < nodes.Count; ++i)
                    {
                        if (nodes[i].RenderCore.RenderType == RenderType.Transparent)
                        {
                            sortingTransparentCache.Add(new SortStruct(GetDistance(nodes[i], ref cameraPosition), nodes[i]));
                        }
                        else if (nodes[i].RenderCore.RenderType == RenderType.Opaque)
                        {
                            sortingOpaqueCache.Add(new SortStruct(GetDistance(nodes[i], ref cameraPosition), nodes[i]));
                        }
                        else
                        {
                            notSorted.Add(nodes[i]);
                        }
                    }
                    if (sortingTransparentCache.Count > 50 && sortingOpaqueCache.Count > 50)
                    {
                        Parallel.Invoke(() =>
                        {
                            sortingTransparentCache.Sort(delegate (SortStruct a, SortStruct b) { return a.Key > b.Key ? -1 : a.Key < b.Key ? 1 : 0; });
                        }, () =>
                        {
                            sortingOpaqueCache.Sort(delegate (SortStruct a, SortStruct b) { return a.Key > b.Key ? 1 : a.Key < b.Key ? -1 : 0; });
                        });
                    }
                    else
                    {
                        sortingTransparentCache.Sort(delegate (SortStruct a, SortStruct b) { return a.Key > b.Key ? -1 : a.Key < b.Key ? 1 : 0; });
                        sortingOpaqueCache.Sort(delegate (SortStruct a, SortStruct b) { return a.Key > b.Key ? 1 : a.Key < b.Key ? -1 : 0; });
                    }
                }

                ItemsInternal.Clear();
                for (int i = 0; i < notSorted.Count; ++i)
                {
                    ItemsInternal.Add(notSorted[i]);
                }
                for (int i = 0; i < sortingOpaqueCache.Count; ++i)
                {
                    ItemsInternal.Add(sortingOpaqueCache[i].Value);
                }
                for (int i = 0; i < sortingTransparentCache.Count; ++i)
                {
                    ItemsInternal.Add(sortingTransparentCache[i].Value);
                }

                sortingTransparentCache.Clear();
                sortingOpaqueCache.Clear();
                notSorted.Clear();
                InvalidateSceneGraph();
            }

            protected float GetDistance(SceneNode node, ref Vector3 cameraPos)
            {
                switch (SortingMethod)
                {
                    case SortingMethod.BoundingBoxCenter:
                        var center = (node.BoundsWithTransform.Maximum + node.BoundsWithTransform.Minimum) / 2;
                        return (cameraPos - center).LengthSquared();
                    case SortingMethod.BoundingBoxCorners:
                        var bound = node.BoundsWithTransform;
                        //https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Collision.cs
                        float distance = 0f;
                        if (cameraPos.X < bound.Minimum.X)
                            distance += (bound.Minimum.X - cameraPos.X) * (bound.Minimum.X - cameraPos.X);
                        if (cameraPos.X > bound.Maximum.X)
                            distance += (cameraPos.X - bound.Maximum.X) * (cameraPos.X - bound.Maximum.X);

                        if (cameraPos.Y < bound.Minimum.Y)
                            distance += (bound.Minimum.Y - cameraPos.Y) * (bound.Minimum.Y - cameraPos.Y);
                        if (cameraPos.Y > bound.Maximum.Y)
                            distance += (cameraPos.Y - bound.Maximum.Y) * (cameraPos.Y - bound.Maximum.Y);

                        if (cameraPos.Z < bound.Minimum.Z)
                            distance += (bound.Minimum.Z - cameraPos.Z) * (bound.Minimum.Z - cameraPos.Z);
                        if (cameraPos.Z > bound.Maximum.Z)
                            distance += (cameraPos.Z - bound.Maximum.Z) * (cameraPos.Z - bound.Maximum.Z);
                        return distance;
                    case SortingMethod.BoundingSphereSurface:
                        float distS = (node.BoundsSphereWithTransform.Center - cameraPos).Length() - node.BoundsSphereWithTransform.Radius;
                        return Math.Max(distS, 0f);
                    default:
                        return 0;
                }
            }
        }
    }

}
