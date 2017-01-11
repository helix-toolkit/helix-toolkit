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
        public readonly OctreeBuildParameter Parameter = new OctreeBuildParameter();

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
            var item = sender as GeometryModel3D;
            int index;
            var node = this.Octree.FindChildByItemBound(item, arg.OldBound, out index);
            bool rootAdd = true;
            if (node != null)
            {
                var tree = Octree;
                Octree = null;
                var geoNode = node as GeometryModel3DOctree;
                var parent = geoNode.Parent;// Need to get its parent first before remove, otherwise it may remove itself from tree
                geoNode.RemoveAt(index);
                if (geoNode.Bound.Contains(arg.NewBound) == ContainmentType.Contains)
                {
                    Debug.WriteLine("new bound inside current node");
                    if (geoNode.Parent != null && geoNode.Add(item))
                    {
                        rootAdd = false;
                    }
                    else if (parent != null && (parent as GeometryModel3DOctree).Add(item))
                    {
                        rootAdd = false;
                    }
                }
                else
                {                    
                    Debug.WriteLine("new bound outside current node");
                }
                Octree = tree;
            }
            if (rootAdd)
            {
                AddItem(item);
            }
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
            var tree = new GeometryModel3DOctree(list, Parameter);
            tree.BuildTree();
            return tree;
        }

        private static readonly BoundingBox ZeroBound = new BoundingBox();
        public bool AddPendingItem(Model3D item)
        {
            if(Enabled && item is GeometryModel3D)
            {
                var model = item as GeometryModel3D;
                model.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                model.OnBoundChanged += GeometryModel3DOctreeManager_OnBoundInitialized;
                if (model.Bounds != ZeroBound)
                {
                    AddItem(model);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GeometryModel3DOctreeManager_OnBoundInitialized(object sender, BoundChangedEventArgs e)
        {
            var item = sender as GeometryModel3D;
            item.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
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
                    item.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                    UnsubscribeBoundChangeEvent(item);
                    if (!tree.RemoveByBound(item))
                    {
                        Console.WriteLine("Remove failed.");
                    }
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
                item.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                UnsubscribeBoundChangeEvent(item);
                if (!tree.RemoveByBound(item))
                {
                    Console.WriteLine("Remove failed.");
                }
                Octree = tree;
            }
        }

        public void Clear()
        {
            mRequestUpdateOctree = false;
            Octree = null;
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
