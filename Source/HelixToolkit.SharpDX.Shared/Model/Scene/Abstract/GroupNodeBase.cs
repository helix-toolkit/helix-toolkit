/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Render;
    /// <summary>
    /// 
    /// </summary>
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
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].Detach();
            }
            itemHashSet.Clear();
        }
        /// <summary>
        /// Removes the child node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Tries the get node.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public bool TryGetNode(Guid guid, out SceneNode node)
        {
            return itemHashSet.TryGetValue(guid, out node);
        }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderHost host)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].Attach(host);
            }
            return true;
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].Detach();
            }
            base.OnDetach();
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            foreach (var c in this.Items)
            {
                c.Render(context, deviceContext);
            }
        }

        /// <summary>
        /// Called when [dispose].
        /// </summary>
        /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            itemHashSet.Clear();
            base.OnDispose(disposeManagedResources);
        }
    }
}