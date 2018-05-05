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
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    public class PanelNode2D : SceneNode2D
    {
        protected readonly Dictionary<Guid, SceneNode2D> itemHashSet = new Dictionary<Guid, SceneNode2D>();

        public override IList<SceneNode2D> Items
        {
            get;
        } = new ObservableCollection<SceneNode2D>();

        public virtual bool AddChildNode(SceneNode2D node)
        {
            if (!itemHashSet.ContainsKey(node.GUID))
            {
                itemHashSet.Add(node.GUID, node);
                Items.Add(node);
                node.Parent = this;
                if (IsAttached)
                {
                    node.Attach(RenderHost);
                }
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
        public virtual bool RemoveChildNode(SceneNode2D node)
        {
            if (itemHashSet.Remove(node.GUID))
            {
                node.Detach();
                Items.Remove(node);
                node.Parent = null;
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
        public bool TryGetNode(Guid guid, out SceneNode2D node)
        {
            return itemHashSet.TryGetValue(guid, out node);
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

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (!LayoutBoundWithTransform.Contains(mousePoint))
            {
                return false;
            }
            foreach (var item in Items.Reverse())
            {
                if (item.HitTest(mousePoint, out hitResult))
                { return true; }
            }
            return false;
        }
    }
}