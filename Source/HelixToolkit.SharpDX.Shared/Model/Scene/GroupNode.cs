/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using System.Diagnostics;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    public class GroupNode : GroupNodeBase, IHitable
    {
        public IOctreeManager OctreeManager { set; get; }

        public IOctree Octree
        {
            get
            {
                return OctreeManager != null ? OctreeManager.Octree : null;
            }
        }

        public override bool AddChildNode(SceneNode node)
        {
            if (base.AddChildNode(node))
            {
                if (OctreeManager != null)
                {
                    OctreeManager.AddPendingItem(node);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveChildNode(SceneNode node)
        {
            if(base.RemoveChildNode(node))
            {
                if(OctreeManager != null)
                {
                    OctreeManager.RemoveItem(node);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Clear()
        {
            OctreeManager?.Clear();
            OctreeManager?.RequestRebuild();
            base.Clear();
        }

        public override void UpdateNotRender(IRenderContext context)
        {
            base.UpdateNotRender(context);
            if (OctreeManager != null)
            {
                OctreeManager.ProcessPendingItems();
                if (OctreeManager.RequestUpdateOctree)
                {
                    OctreeManager?.RebuildTree(this.Items);
                }
            }
        }

        protected override bool OnHitTest(IRenderContext context, global::SharpDX.Matrix totalModelMatrix, ref global::SharpDX.Ray ray, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (Octree != null)
            {
                isHit = Octree.HitTest(context, this, totalModelMatrix, ray, ref hits);
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
