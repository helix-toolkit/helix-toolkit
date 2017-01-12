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
                if (mOctree == value)
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
        public readonly OctreeBuildParameter Parameter = new OctreeBuildParameter() { MinSize = 1f };

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
            var item = sender as GeometryModel3D;
            if (Octree == null)
            {
                item.OnBoundChanged -= Item_OnBoundChanged;
                return;
            }
            var arg = e;
            int index;
            var node = this.Octree.FindChildByItemBound(item, arg.OldBound, out index);
            bool rootAdd = true;
            if (node != null)
            {
                var tree = Octree;
                Octree = null;
                var geoNode = node as GeometryModel3DOctree;
                if (geoNode.Bound.Contains(arg.NewBound) == ContainmentType.Contains)
                {
                    Debug.WriteLine("new bound inside current node");
                    if (geoNode.Add(item))
                    {
                        geoNode.RemoveAt(index); //remove old item from node after adding successfully.
                        rootAdd = false;
                    }
                }
                else
                {
                    geoNode.RemoveAt(index);
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
            Clear();
            mRequestUpdateOctree = false;
            if (items == null || items.Count == 0)
            {
                return null;
            }
            var list = items.Where(x => x is GeometryModel3D).Select(x => x as GeometryModel3D).ToList();
            foreach (var item in list)
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
            if (Enabled && item is GeometryModel3D)
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
                foreach (var item in items)
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
            //if(Octree == null)
            //{
            //    return;
            //}
            //var queue = new Queue<GeometryModel3DOctree>(256);
            //queue.Enqueue(Octree);
            //while (queue.Count > 0)
            //{
            //    var node = queue.Dequeue();
            //    foreach(var item in node.Objects)
            //    {
            //        UnsubscribeBoundChangeEvent(item);
            //    }
            //    foreach(var child in node.ChildNodes)
            //    {
            //        if (child != null)
            //        {
            //            queue.Enqueue(child as GeometryModel3DOctree);
            //        }
            //    }
            //}           
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
