/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Render;


    public abstract class GroupNodeBase : SceneNode
    {
        protected readonly Dictionary<Guid, SceneNode> itemHashSet = new Dictionary<Guid, SceneNode>();

        public override IList<IRenderable> Items { get; } = new ObservableCollection<IRenderable>();

        public virtual bool AddChildNode(SceneNode node)
        {
            if (!itemHashSet.ContainsKey(node.GUID))
            {
                itemHashSet.Add(node.GUID, node);
                Items.Add(node);
                if (IsAttached)
                {
                    node.Attach(RenderHost);
                }
                forceUpdateTransform = true;
                return true;
            }
            else { return false; }
        }

        public virtual bool RemoveChildNode(SceneNode node)
        {
            if (itemHashSet.Remove(node.GUID))
            {
                node.Detach();
                Items.Remove(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetNode(Guid guid, out SceneNode node)
        {
            return itemHashSet.TryGetValue(guid, out node);
        }

        public virtual void Clear()
        {
            for(int i=0; i< Items.Count; ++i)
            {
                Items[i].Detach();
            }
            itemHashSet.Clear();
        }

        protected override bool OnAttach(IRenderHost host)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].Attach(host);
            }
            return true;
        }

        protected override void OnDetach()
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].Detach();
            }
            base.OnDetach();
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            foreach (var c in this.Items)
            {
                c.Render(context, deviceContext);
            }
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;
            foreach (var c in this.Items)
            {
                if (c is IHitable h)
                {
                    if (h.HitTest(context, ray, ref hits))
                    {
                        hit = true;
                    }
                }
            }
            if (hit)
            {
                var pos = ray.Position;
                hits = hits.OrderBy(x => Vector3.DistanceSquared(pos, x.PointHit)).ToList();
            }
            return hit;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            itemHashSet.Clear();
            base.OnDispose(disposeManagedResources);
        }
    }
}
