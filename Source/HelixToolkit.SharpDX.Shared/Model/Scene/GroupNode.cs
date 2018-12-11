/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections.Generic;
using System.Diagnostics;

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
        /// <summary>
        ///
        /// </summary>
        public class GroupNode : GroupNodeBase, IHitable
        {
            private IOctreeManager octreeManager;
            public IOctreeManager OctreeManager
            {
                set
                {
                    var old = octreeManager;
                    if(Set(ref octreeManager, value))
                    {
                        old?.Clear();
                        if (octreeManager != null)
                        {
                            foreach(var item in ItemsInternal)
                            {
                                octreeManager.AddPendingItem(item);
                            }
                        }
                    }
                }
                get { return octreeManager; }
            }

            /// <summary>
            /// Gets the octree in OctreeManager.
            /// </summary>
            /// <value>
            /// The octree.
            /// </value>
            public IOctreeBasic Octree
            {
                get
                {
                    return OctreeManager != null ? OctreeManager.Octree : null;
                }
            }

            public GroupNode()
            {
                OnAddChildNode += NodeGroup_OnAddChildNode;
                OnRemoveChildNode += NodeGroup_OnRemoveChildNode;
                OnClear += NodeGroup_OnClear;
            }

            private void NodeGroup_OnClear(object sender, OnChildNodeChangedArgs e)
            {
                OctreeManager?.Clear();
                OctreeManager?.RequestRebuild();
            }

            private void NodeGroup_OnRemoveChildNode(object sender, OnChildNodeChangedArgs e)
            {
                OctreeManager?.RemoveItem(e);
            }

            private void NodeGroup_OnAddChildNode(object sender, OnChildNodeChangedArgs e)
            {
                OctreeManager?.AddPendingItem(e);
            }

            /// <summary>
            /// Updates the not render.
            /// </summary>
            /// <param name="context">The context.</param>
            public override void UpdateNotRender(RenderContext context)
            {
                base.UpdateNotRender(context);
                if (OctreeManager != null)
                {
                    OctreeManager.ProcessPendingItems();
                    if (OctreeManager.RequestUpdateOctree)
                    {
                        OctreeManager?.RebuildTree(this.ItemsInternal);
                    }
                }
            }

            /// <summary>
            /// Called when [hit test].
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="totalModelMatrix">The total model matrix.</param>
            /// <param name="ray">The ray.</param>
            /// <param name="hits">The hits.</param>
            /// <returns></returns>
            protected override bool OnHitTest(RenderContext context, global::SharpDX.Matrix totalModelMatrix, ref global::SharpDX.Ray ray, ref List<HitTestResult> hits)
            {
                bool isHit = false;
                if (octreeManager != null)
                {
                    isHit = octreeManager.HitTest(context, this.WrapperSource, totalModelMatrix, ray, ref hits);
    #if DEBUG
                    if (isHit)
                    {
                        Debug.WriteLine("Octree hit test, hit at " + hits[0].PointHit);
                    }
    #endif
                }
                else
                {
                    isHit = base.OnHitTest(context, totalModelMatrix, ref ray, ref hits);
                }
                return isHit;
            }
        }
    }

}