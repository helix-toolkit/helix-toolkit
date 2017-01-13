using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Windows;
using SharpDX;
using System.Diagnostics;
using HelixToolkit.SharpDX.Shared.Utilities;
using HelixToolkit.SharpDX.Shared.Model;
using System.Runtime.CompilerServices;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
    /// </summary>
    public sealed class GeometryModel3DOctreeManager : FrameworkElement, IOctreeManager
    {
        public static readonly DependencyProperty OctreeProperty
            = DependencyProperty.Register("Octree", typeof(IOctree), typeof(GeometryModel3DOctreeManager), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ParameterProperty
            = DependencyProperty.Register("Parameter", typeof(OctreeBuildParameter), typeof(GeometryModel3DOctreeManager),
                new PropertyMetadata(new OctreeBuildParameter()));

        public IOctree Octree
        {
            set
            {
                SetValue(OctreeProperty, value);
            }
            get
            {
                return (IOctree)GetValue(OctreeProperty);
            }
        }

        private GeometryModel3DOctree mOctree = null;

        public OctreeBuildParameter Parameter
        {
            set
            {
                SetValue(ParameterProperty, value);
            }
            get
            {
                return (OctreeBuildParameter)GetValue(ParameterProperty);
            }
        }

        public bool RequestUpdateOctree { get { return mRequestUpdateOctree; } }
        private volatile bool mRequestUpdateOctree = false;

        private bool mEnabled = true;
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

        private void UpdateOctree(GeometryModel3DOctree tree)
        {
            Octree = tree;
            mOctree = tree;
        }

        public void RebuildTree(IList<Element3D> items)
        {
            mRequestUpdateOctree = false;
            if (Enabled)
            {
                UpdateOctree(RebuildOctree(items));
            }
            else
            {
                Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnBoundChanged -= Item_OnBoundChanged;
            item.OnBoundChanged += Item_OnBoundChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnBoundChanged -= Item_OnBoundChanged;
        }

        private void Item_OnBoundChanged(object sender, BoundChangedEventArgs e)
        {
            var item = sender as GeometryModel3D;
            if (Octree == null || !item.IsAttached)
            {
                UnsubscribeBoundChangeEvent(item);
                return;
            }
            var arg = e;
            int index;
            var node = mOctree.FindChildByItemBound(item, arg.OldBound, out index);
            bool rootAdd = true;
            if (node != null)
            {
                var tree = mOctree;
                UpdateOctree(null);
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
                UpdateOctree(tree);
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
        public bool AddPendingItem(Element3D item)
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

        private void AddItem(Element3D item)
        {
            if (Enabled && item is GeometryModel3D)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as GeometryModel3D;
                if (tree == null || !tree.Add(model))
                {
                    RequestRebuild();
                }
                else
                {
                    UpdateOctree(tree);
                }
                SubscribeBoundChangeEvent(model);
            }
        }

        //public void RemoveItems(IList<GeometryModel3D> items)
        //{
        //    if (Enabled && Octree != null)
        //    {
        //        var tree = Octree;
        //        UpdateOctree(null);
        //        foreach (var item in items)
        //        {
        //            item.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
        //            UnsubscribeBoundChangeEvent(item);
        //            if (!tree.RemoveByBound(item))
        //            {
        //                Console.WriteLine("Remove failed.");
        //            }
        //        }
        //        UpdateOctree(tree);
        //    }
        //}

        public void RemoveItem(Element3D item)
        {
            if (Enabled && Octree != null && item is GeometryModel3D)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as GeometryModel3D;
                model.OnBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                UnsubscribeBoundChangeEvent(model);
                if (!tree.RemoveByBound(model))
                {
                    Console.WriteLine("Remove failed.");
                }
                UpdateOctree(tree);
            }
        }

        public void Clear()
        {
            mRequestUpdateOctree = false;
            UpdateOctree(null);   
        }

        public void RequestRebuild()
        {
            Clear();
            mRequestUpdateOctree = true;
        }
    }
}
