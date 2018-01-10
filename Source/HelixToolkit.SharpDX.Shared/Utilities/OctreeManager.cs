// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OctreeManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

#if NETFX_CORE
namespace HelixToolkit.UWP.Utilities
#else
namespace HelixToolkit.Wpf.SharpDX.Utilities
#endif
{
    using Model;
    public abstract class OctreeManagerBase : ObservableObject, IOctreeManager
    {
        private IOctree octree;
        public IOctree Octree
        {
            protected set
            {
                Set(ref octree, value);
            }
            get
            { return octree; }
        }
        protected RenderableBoundingOctree mOctree = null;

        public OctreeBuildParameter Parameter { set; get; } = new OctreeBuildParameter();

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

        public abstract bool AddPendingItem(IRenderable item);

        public abstract void Clear();

        public abstract void RebuildTree(IEnumerable<IRenderable> items);

        public abstract void RemoveItem(IRenderable item);

        public abstract void RequestRebuild();
    }

    /// <summary>
    /// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
    /// </summary>
    public sealed class GroupRenderableBoundOctreeManager : OctreeManagerBase
    {
        private void UpdateOctree(RenderableBoundingOctree tree)
        {
            Octree = tree;
            mOctree = tree;
        }
        public override void RebuildTree(IEnumerable<IRenderable> items)
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
        private void SubscribeBoundChangeEvent(IRenderable item)
        {
            item.OnTransformBoundChanged -= Item_OnBoundChanged;
            item.OnTransformBoundChanged += Item_OnBoundChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeBoundChangeEvent(IRenderable item)
        {
            item.OnTransformBoundChanged -= Item_OnBoundChanged;
        }

        private void Item_OnBoundChanged(object sender,  BoundChangeArgs<BoundingBox> args)
        {
            var item = sender as IRenderable;
            if (item == null)
            { return; }
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
                var geoNode = node as RenderableBoundingOctree;
                if (geoNode.Bound.Contains(args.NewBound) == ContainmentType.Contains)
                {
                    if (geoNode.PushExistingToChild(index))
                    {
                        tree = tree.Shrink() as RenderableBoundingOctree;
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

        private RenderableBoundingOctree RebuildOctree(IEnumerable<IRenderable> items)
        {
            Clear();
            if (items == null)
            {
                return null;
            }
            var list = items.Where(x => x is IRenderable).Select(x => x as IRenderable).ToList();
            var array = list.ToArray();
            var tree = new RenderableBoundingOctree(list, Parameter);
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
        public override bool AddPendingItem(IRenderable item)
        {
            if (Enabled && item is IRenderable)
            {
                var model = item as IRenderable;
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

        private void GeometryModel3DOctreeManager_OnBoundInitialized(object sender, BoundChangeArgs<BoundingBox> args)
        {
            var item = sender as IRenderable;
            item.OnTransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
            AddItem(item);
        }

        private void AddItem(IRenderable item)
        {
            if (Enabled && item is IRenderable)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as IRenderable;
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
                        var direction = (model.BoundsWithTransform.Minimum + model.BoundsWithTransform.Maximum)
                            - (tree.Bound.Minimum + tree.Bound.Maximum);
                        tree = tree.Expand(ref direction) as RenderableBoundingOctree;
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

        public override void RemoveItem(IRenderable item)
        {
            if (Enabled && Octree != null && item is IRenderable)
            {
                var tree = mOctree;
                UpdateOctree(null);
                var model = item as IRenderable;
                model.OnTransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                UnsubscribeBoundChangeEvent(model);
                if (!tree.RemoveByBound(model))
                {
                    //Console.WriteLine("Remove failed.");
                }
                else
                {
                    tree = tree.Shrink() as RenderableBoundingOctree;
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

    public sealed class InstancingRenderableOctreeManager : OctreeManagerBase
    {
        public override bool AddPendingItem(IRenderable item)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            Octree = null;
        }

        public override void RebuildTree(IEnumerable<IRenderable> items)
        {
            Clear();
            if (items == null)
            { return; }
            var model3D = items.Where(x => x is IInstancing && x is IRenderable).FirstOrDefault() as IInstancing;
            if (model3D == null)
            { return; }
            IList<Matrix> instMatrix = model3D.Instances;
            var octree = new InstancingModel3DOctree(instMatrix, (model3D as IRenderable).BoundsWithTransform, this.Parameter, new Queue<IOctree>(256));
            octree.BuildTree();
            Octree = octree;
        }

        public override void RemoveItem(IRenderable item)
        {
            throw new NotImplementedException();
        }

        public override void RequestRebuild()
        {
            throw new NotImplementedException();
        }
    }
}
