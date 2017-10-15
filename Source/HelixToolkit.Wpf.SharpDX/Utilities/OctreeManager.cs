// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OctreeManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Windows;
using SharpDX;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HelixToolkit.Wpf.SharpDX
{
    public abstract class OctreeManagerBase : FrameworkContentElement, IOctreeManager
    {
        public static readonly DependencyProperty OctreeProperty
            = DependencyProperty.Register("Octree", typeof(IOctree), typeof(OctreeManagerBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MinSizeProperty
            = DependencyProperty.Register("MinSize", typeof(float), typeof(OctreeManagerBase),
                new PropertyMetadata(1f, (s, e) => { (s as OctreeManagerBase).Parameter.MinimumOctantSize = (float)e.NewValue; }));

        public static readonly DependencyProperty AutoDeleteIfEmptyProperty
            = DependencyProperty.Register("AutoDeleteIfEmpty", typeof(bool), typeof(OctreeManagerBase),
                new PropertyMetadata(true, (s, e) => { (s as OctreeManagerBase).Parameter.AutoDeleteIfEmpty = (bool)e.NewValue; }));

        public static readonly DependencyProperty CubifyPropertyProperty
            = DependencyProperty.Register("Cubify", typeof(bool), typeof(OctreeManagerBase),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBase).Parameter.Cubify = (bool)e.NewValue; }));

        public static readonly DependencyProperty RecordHitPathBoundingBoxesProperty
            = DependencyProperty.Register("RecordHitPathBoundingBoxes", typeof(bool), typeof(OctreeManagerBase),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBase).Parameter.RecordHitPathBoundingBoxes = (bool)e.NewValue; }));

        public static readonly DependencyProperty MinObjectSizeToSplitProperty
            = DependencyProperty.Register("MinObjectSizeToSplit", typeof(int), typeof(OctreeManagerBase),
                new PropertyMetadata(0, (s, e) => { (s as OctreeManagerBase).Parameter.MinObjectSizeToSplit = (int)e.NewValue; }));


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

        /// <summary>
        /// Minimum octant size
        /// </summary>
        public float MinSize
        {
            set
            {
                SetValue(MinSizeProperty, value);
            }
            get
            {
                return (float)GetValue(MinSizeProperty);
            }
        }
        /// <summary>
        /// Delete octant node if its empty
        /// </summary>
        public bool AutoDeleteIfEmpty
        {
            set
            {
                SetValue(AutoDeleteIfEmptyProperty, value);
            }
            get
            {
                return (bool)GetValue(AutoDeleteIfEmptyProperty);
            }
        }
        /// <summary>
        /// Create cube octree
        /// </summary>
        public bool Cubify
        {
            set
            {
                SetValue(CubifyPropertyProperty, value);
            }
            get
            {
                return (bool)GetValue(CubifyPropertyProperty);
            }
        }
        /// <summary>
        /// Record the hit path bounding box for debugging
        /// </summary>
        public bool RecordHitPathBoundingBoxes
        {
            set
            {
                SetValue(RecordHitPathBoundingBoxesProperty, value);
            }
            get
            {
                return (bool)GetValue(RecordHitPathBoundingBoxesProperty);
            }
        }
        /// <summary>
        /// Minimum object in each octant to start splitting into smaller octant during build
        /// </summary>
        public int MinObjectSizeToSplit
        {
            set
            {
                SetValue(MinObjectSizeToSplitProperty, value);
            }
            get
            {
                return (int)GetValue(MinObjectSizeToSplitProperty);
            }
        }

        protected GeometryModel3DOctree mOctree = null;

        public OctreeBuildParameter Parameter { private set; get; } = new OctreeBuildParameter();

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

        public bool RequestUpdateOctree { get { return mRequestUpdateOctree; } protected set { mRequestUpdateOctree = value; } }
        private volatile bool mRequestUpdateOctree = false;

        public abstract bool AddPendingItem(Element3D item);

        public abstract void Clear();

        public abstract void RebuildTree(IList<Element3D> items);

        public abstract void RemoveItem(Element3D item);

        public abstract void RequestRebuild();
    }

    /// <summary>
    /// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
    /// </summary>
    public sealed class GeometryModel3DOctreeManager : OctreeManagerBase
    {
        public GeometryModel3DOctreeManager()
        {
        }
        private void UpdateOctree(GeometryModel3DOctree tree)
        {
            Octree = tree;
            mOctree = tree;
        }
        public override void RebuildTree(IList<Element3D> items)
        {
            RequestUpdateOctree = false;
            if (Enabled)
            {
                UpdateOctree(RebuildOctree(items));
                if (Octree == null)
                {
                    RequestRebuild();
                }
            }
            else
            {
                Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnTransformBoundChanged -= Item_OnBoundChanged;
            item.OnTransformBoundChanged += Item_OnBoundChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeBoundChangeEvent(GeometryModel3D item)
        {
            item.OnTransformBoundChanged -= Item_OnBoundChanged;
        }

        private void Item_OnBoundChanged(object sender, ref BoundingBox newBound, ref BoundingBox oldBound)
        {
            var item = sender as GeometryModel3D;
            if (mOctree == null || !item.IsAttached)
            {
                UnsubscribeBoundChangeEvent(item);
                return;
            }
            int index;
            var node = mOctree.FindItemByGuid(item.GUID, item, out index);
            bool rootAdd = true;
            if (node != null)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var geoNode = node as GeometryModel3DOctree;
                if (geoNode.Bound.Contains(newBound) == ContainmentType.Contains)
                {
                    if (geoNode.PushExistingToChild(index))
                    {
                        tree = tree.Shrink() as GeometryModel3DOctree;
                    }
                    rootAdd = false;
                }
                else
                {
                    geoNode.RemoveAt(index, tree);
                }
                UpdateOctree(tree);
            }
            else
            {
                mOctree.RemoveByGuid(item.GUID, item, mOctree);
            }
            if (rootAdd)
            {
                AddItem(item);
            }
        }

        private GeometryModel3DOctree RebuildOctree(IList<Element3D> items)
        {
            Clear();
            if (items == null || items.Count == 0)
            {
                return null;
            }
            var list = items.Where(x => x is GeometryModel3D).Select(x => x as GeometryModel3D).ToList();
            var array = list.ToArray();
            var tree = new GeometryModel3DOctree(list, Parameter);
            tree.BuildTree();
            if (tree.TreeBuilt)
            {
                foreach (var item in array)
                {
                    SubscribeBoundChangeEvent(item);
                }
            }
            return tree.TreeBuilt ? tree : null;
        }

        private static readonly BoundingBox ZeroBound = new BoundingBox();
        public override bool AddPendingItem(Element3D item)
        {
            if (Enabled && item is GeometryModel3D)
            {
                var model = item as GeometryModel3D;
                model.OnTransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                model.OnTransformBoundChanged += GeometryModel3DOctreeManager_OnBoundInitialized;
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

        private void GeometryModel3DOctreeManager_OnBoundInitialized(object sender, ref BoundingBox newBound, ref BoundingBox oldBound)
        {
            var item = sender as GeometryModel3D;
            item.OnTransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
            AddItem(item);
        }

        private void AddItem(Element3D item)
        {
            if (Enabled && item is GeometryModel3D)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as GeometryModel3D;
                if (tree == null)
                {
                    RequestRebuild();
                }
                else
                {
                    bool succeed = true;
                    int counter = 0;
                    while (!tree.Add(model))
                    {
                        var direction = (model.Bounds.Minimum + model.Bounds.Maximum)
                            - (tree.Bound.Minimum + tree.Bound.Maximum);
                        tree = tree.Expand(ref direction) as GeometryModel3DOctree;
                        ++counter;
                        if (counter > 10)
                        {
#if DEBUG
                            throw new Exception("Expand tree failed");
#else
                            succeed = false;
                            break;
#endif
                        }
                    }
                    if (succeed)
                    {
                        UpdateOctree(tree);
                        SubscribeBoundChangeEvent(model);
                    }
                    else
                    {
                        RequestRebuild();
                    }
                }
            }
        }

        public override void RemoveItem(Element3D item)
        {
            if (Enabled && Octree != null && item is GeometryModel3D)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as GeometryModel3D;
                model.OnTransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                UnsubscribeBoundChangeEvent(model);
                if (!tree.RemoveByBound(model))
                {
                    Console.WriteLine("Remove failed.");
                }
                else
                {
                    tree = tree.Shrink() as GeometryModel3DOctree;
                }
                UpdateOctree(tree);
            }
        }

        public override void Clear()
        {
            RequestUpdateOctree = false;
            UpdateOctree(null);
        }

        public override void RequestRebuild()
        {
            Clear();
            RequestUpdateOctree = true;
        }
    }

    public sealed class InstancingModel3DOctreeManager : OctreeManagerBase
    {
        public override bool AddPendingItem(Element3D item)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            Octree = null;
        }

        public override void RebuildTree(IList<Element3D> items)
        {
            Clear();
            if (items == null || items.Count == 0)
            {
                return;
            }
            var model3D = items.Where(x => x is InstancingMeshGeometryModel3D).FirstOrDefault() as InstancingMeshGeometryModel3D;
            if (model3D == null)
            { return; }
            IList<Matrix> instMatrix = model3D.Instances;
            var octree = new InstancingModel3DOctree(instMatrix, model3D.BoundsWithTransform, this.Parameter, new Queue<IOctree>(256));
            octree.BuildTree();
            Octree = octree;
        }

        public override void RemoveItem(Element3D item)
        {
            throw new NotImplementedException();
        }

        public override void RequestRebuild()
        {
            throw new NotImplementedException();
        }
    }
}
