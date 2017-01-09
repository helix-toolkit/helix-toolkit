using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;
using System.Collections;

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
        private readonly Queue<GeometryModel3D> mAddPendingQueue = new Queue<GeometryModel3D>();
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

        public void UpdateOctree(IList<Element3D> items)
        {
            mRequestUpdateOctree = false;
            if (Enabled)
            {
                if (Octree == null)
                {
                    Octree = RebuildOctree(items);
                }
                else
                {
                    while (mAddPendingQueue.Count > 0)
                    {
                        var model = mAddPendingQueue.Dequeue();

                        var tree = Octree as GeometryModel3DOctree;
                        Octree = null;
                        if (!tree.Add(model))
                        {
                            Octree = RebuildOctree(items);
                        }
                        else
                        {
                            Octree = tree;
                        }
                    }
                }                
            }
            else
            {
                Clear();
            }
        }

        private GeometryModel3DOctree RebuildOctree(IList<Element3D> items)
        {
            var list = items.Where(x => x is GeometryModel3D).Select(x => x as GeometryModel3D).ToList();
            var tree = new GeometryModel3DOctree(list);
            tree.BuildTree();
            return tree;
        }

        public bool AddPendingItem(Model3D item)
        {
            if(Enabled && item is GeometryModel3D)
            {
                mAddPendingQueue.Enqueue(item as GeometryModel3D);
                mRequestUpdateOctree = true;
                return true;
            }
            else
            {
                return false;
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
                    tree.Remove(item);
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
                tree.Remove(item);
                Octree = tree;
            }
        }

        public void Clear()
        {
            mRequestUpdateOctree = false;
            Octree = null;
            mAddPendingQueue.Clear();
        }

        private void RaiseOctreeChangedEvent()
        {
            var handler = OnOctreeChanged;
            if (handler != null)
            {
                handler(this, new OctreeChangedArgs(this.Octree));
            }
        }
    }
}
