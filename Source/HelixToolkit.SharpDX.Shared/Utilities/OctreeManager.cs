using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;
using System.Collections;
using System.Windows;
using SharpDX;
using System.Diagnostics;

namespace HelixToolkit.SharpDX.Shared.Utilities
{
    public sealed class OctreeChangedArgs : EventArgs
    {
        public IOctree Octree { private set; get; }
        public OctreeChangedArgs(IOctree tree)
        {
            Octree = tree;
        }
    }

    public delegate void OnOctreeChangedEventHandler(object sender, OctreeChangedArgs args);

    public sealed class GeometryModel3DOctreeManager
    {
        public event OnOctreeChangedEventHandler OnOctreeChanged;

        private GeometryModel3DOctree mOctree = null;
        public GeometryModel3DOctree Octree
        {
            private set
            {
                if(mOctree == value)
                {
                    return;
                }
                mOctree = value;
                RaiseOctreeChangedEvent();
            }
            get
            {
                return mOctree;
            }
        }

        public bool RequestUpdateOctree { get { return mRequestUpdateOctree; } }
        private volatile bool mRequestUpdateOctree = false;
       // private readonly Queue<GeometryModel3D> mAddPendingQueue = new Queue<GeometryModel3D>();
        private bool mEnabled = false;
        public bool Enabled
        {
            set
            {
                mEnabled = value;
                if (!mEnabled)
                {
                    Clear();
                }
            }
            get
            {
                return mEnabled;
            }
        }

        public void RebuildTree(IList<Element3D> items)
        {
            mRequestUpdateOctree = false;
            if (Enabled)
            {
                Octree = RebuildOctree(items);      
            }
            else
            {
                Clear();
            }
        }

        private void SubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnBoundChanged -= Item_OnBoundChanged;
            item.OnBoundChanged += Item_OnBoundChanged;
        }

        private void UnsubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnBoundChanged -= Item_OnBoundChanged;
        }

        private void Item_OnBoundChanged(object sender, BoundChangedEventArgs e)
        {
            if(Octree == null)
            {
                return;
            }
            var arg = e;
            if (arg.OldBound.Contains(arg.NewBound) == ContainmentType.Contains)
            {
                return;
            }
            var item = sender as GeometryModel3D;
            var node = this.Octree.FindChildByItemBound(item, arg.OldBound);
            if (node != null && node.Bound.Contains(arg.NewBound) == ContainmentType.Contains)
            {
                Debug.WriteLine("new bound inside current node. Do nothing.");
                return;
            }
            else if (node != null && node is GeometryModel3DOctree)
            {
                Debug.WriteLine("new bound outside current node, remove it.");
                (node as GeometryModel3DOctree).RemoveByBound(item, arg.OldBound);
            }
            AddItem(item);
        }

        //private RoutedEventHandler MakeWeakHandler(Action<object, BoundChangedEventArgs> action, Action<RoutedEventHandler> remove)
        //{
        //    var reference = new WeakReference(action.Target);
        //    var method = action.Method;
        //    RoutedEventHandler handler = null;
        //    handler = delegate (object sender, RoutedEventArgs e)
        //    {
        //        var target = reference.Target;
        //        if (reference.IsAlive)
        //        {
        //            method.Invoke(target, null);
        //        }
        //        else
        //        {
        //            remove(handler);
        //        }
        //    };
        //    return handler;
        //}

        private GeometryModel3DOctree RebuildOctree(IList<Element3D> items)
        {
            var list = items.Where(x => x is GeometryModel3D).Select(x => x as GeometryModel3D).ToList();
            foreach(var item in list)
            {
                SubscribeBoundChangeEvent(item);
            }
            var tree = new GeometryModel3DOctree(list);
            tree.BuildTree();
            return tree;
        }

        public bool AddPendingItem(Model3D item)
        {
            if(Enabled && item is GeometryModel3D)
            {
                (item as GeometryModel3D).OnBoundChanged += GeometryModel3DOctreeManager_OnBoundChanged;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GeometryModel3DOctreeManager_OnBoundChanged(object sender, BoundChangedEventArgs e)
        {
            var item = sender as GeometryModel3D;
            item.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundChanged;
            AddItem(item);
        }

        private void AddItem(GeometryModel3D item)
        {
            if (Enabled)
            {
                var tree = Octree;
                Octree = null;
                if (tree == null || !tree.Add(item))
                {
                    RequestRebuild();
                }
                else
                {
                    Octree = tree;
                }
                SubscribeBoundChangeEvent(item);
            }
        }

        public void RemoveItems(IList<GeometryModel3D> items)
        {
            if (Enabled && Octree != null)
            {
                var tree = Octree;
                Octree = null;
                foreach(var item in items)
                {
                    UnsubscribeBoundChangeEvent(item);
                    tree.RemoveByBound(item);
                }
                Octree = tree;
            }
        }

        public void RemoveItem(GeometryModel3D item)
        {
            if (Enabled && Octree != null)
            {
                var tree = Octree;
                Octree = null;
                UnsubscribeBoundChangeEvent(item);
                tree.RemoveByBound(item);
                Octree = tree;
            }
        }

        public void Clear()
        {
            mRequestUpdateOctree = false;
            Octree = null;
            //mAddPendingQueue.Clear();
        }

        private void RaiseOctreeChangedEvent()
        {
            OnOctreeChanged?.Invoke(this, new OctreeChangedArgs(this.Octree));
        }

        public void RequestRebuild()
        {
            Clear();
            mRequestUpdateOctree = true;
        }
    }
}
