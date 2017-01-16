// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Octree.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
// </copyright>
// <summary>
// An octree implementation reference from https://www.gamedev.net/resources/_/technical/game-programming/introduction-to-octrees-r3529
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HelixToolkit.SharpDX.Shared.Utilities
{
    public sealed class OnHitEventArgs : EventArgs
    {

    }
    public delegate void OnHitEventHandler(object sender, OnHitEventArgs args);
    public interface IOctree
    {
        event OnHitEventHandler OnHit;
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        byte ActiveNodes { set; get; }
        bool HasChildren { get; }
        bool IsRoot { get; }
        IOctree Parent { get; set; }
        BoundingBox Bound { get; }
        IOctree[] ChildNodes { get; }
        BoundingBox[] Octants { get; }
        IList<BoundingBox> HitPathBoundingBoxes { get; }
        OctreeBuildParameter Parameter { get; }

        bool TreeBuilt { get; }
        /// <summary>
        /// Delete self if is empty;
        /// </summary>
        bool AutoDeleteIfEmpty { set; get; }

        /// <summary>
        /// Returns true if this node tree and all children have no content
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Normal hit test from top to bottom
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        bool HitTest(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);

        /// <summary>
        /// Hit test for only this node, not its child node
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        bool HitTestCurrentNodeExcludeChild(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, ref bool isIntersect);

        /// <summary>
        /// Build the whole tree from top to bottom iteratively.
        /// </summary>
        void BuildTree();

        /// <summary>
        /// Build current node level only, this will only build current node and create children, but not build its children. 
        /// To build from top to bottom, call BuildTree
        /// </summary>
        void BuildCurretNodeOnly();
        void Clear();
        /// <summary>
        /// Remove self from parent node
        /// </summary>
        void RemoveSelf();
        /// <summary>
        /// Remove child from ChildNodes
        /// </summary>
        /// <param name="child"></param>
        void RemoveChild(IOctree child);
    }

    public interface IOctreeBase<T> : IOctree
    {
        List<T> Objects { get; }

        /// <summary>
        /// <para>Add item into octree. Return true if successful, otherwise return false to indicate the tree needs to be recreated.</para>
        /// <para>Note: When return false, it usually indicates the bound of new object is outside the max bound of current octree. </para>
        /// </summary>
        /// <param name="item"></param>
        bool Add(T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="octant">The octant object belongs</param>
        /// <returns></returns>
        bool Add(T item, out IOctree octant);
        /// <summary>
        /// Expand the octree to direction
        /// </summary>
        /// <param name="root"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IOctree Expand(ref Vector3 direction);

        /// <summary>
        /// Shrink root if there is no objects
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        IOctree Shrink();

        /// <summary>
        /// Remove item(fast). Search using its bounding box. <see cref="FindChildByItemBound(T, out int)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return false if item not found</returns>
        bool RemoveByBound(T item);
        /// <summary>
        /// Remove item(fast). Search using manual bounding box, this is useful if the item's bound has been changed, use its old bound. <see cref="FindChildByItemBound(T, BoundingBox, out int)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bound"></param>
        /// <returns>Return false if item not found</returns>
        bool RemoveByBound(T item, ref BoundingBox bound);

        /// <summary>
        /// Remove item using exhaust search(Slow). <see cref="FindChildByItem(T, out int)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return false if item not found</returns>
        bool RemoveSafe(T item);

        /// <summary>
        /// Remove item from current node by its index in Objects
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Return false if index out of bound</returns>
        bool RemoveAt(int index);
        /// <summary>
        /// Fast search node by item bound
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">The item index in Objects, if not found, output -1</param>
        /// <returns></returns>
        IOctree FindChildByItemBound(T item, out int index);

        /// <summary>
        /// Fast search node by item bound
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">The item index in Objects, if not found, output -1</param>
        /// <returns></returns>
        IOctree FindChildByItemBound(T item, ref BoundingBox bound, out int index);

        /// <summary>
        /// Exhaust search, slow.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">The item index in Objects, if not found, output -1</param>
        /// <returns></returns>
        IOctree FindChildByItem(T item, out int index);
    }

    public abstract class OctreeBase<T> : IOctreeBase<T>
    {
        public delegate IOctree CreateNodeDelegate(ref BoundingBox bound, List<T> objects, IOctree parent);
        protected readonly Queue<IOctree> queue;
        public event OnHitEventHandler OnHit;
        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public float MIN_SIZE { get { return Parameter.MinimumOctantSize; } }

        public bool TreeBuilt { get { return treeBuilt; } }
        protected bool treeBuilt = false;       //there is no pre-existing tree yet.
        public OctreeBuildParameter Parameter { private set; get; }

        private BoundingBox bound;
        public BoundingBox Bound
        {
            protected set
            {
                if (bound == value)
                {
                    return;
                }
                bound = value;
                octants = CreateOctants(ref value, MIN_SIZE);
            }
            get
            {
                return bound;
            }
        }

        public List<T> Objects { protected set; get; }

        private readonly List<BoundingBox> hitPathBoundingBoxes = new List<BoundingBox>();
        public IList<BoundingBox> HitPathBoundingBoxes { get { return hitPathBoundingBoxes.AsReadOnly(); } }
        /// <summary>
        /// These are all of the possible child octants for this node in the tree.
        /// </summary>
        private readonly IOctree[] childNodes = new IOctree[8];
        public IOctree[] ChildNodes { get { return childNodes; } }

        public byte ActiveNodes { set; get; }

        public IOctree Parent { set; get; }

        private BoundingBox[] octants = null;
        public BoundingBox[] Octants { get { return octants; } }

        public bool AutoDeleteIfEmpty
        {
            get
            {
                return Parameter.AutoDeleteIfEmpty;
            }
            set
            {
                Parameter.AutoDeleteIfEmpty = value;
            }
        }

        private OctreeBase(OctreeBuildParameter parameter, Queue<IOctree> queueCache)
        {
            queue = queueCache ?? new Queue<IOctree>(64);
#if DEBUG
            if (queueCache == null)
            {
                Debug.WriteLine("queue cache is null");
            }
#endif
            if (parameter != null)
                Parameter = parameter;
            else
                Parameter = new OctreeBuildParameter();
        }

        /// <summary>
        /// Creates an oct tree which encloses the given region and contains the provided objects.
        /// </summary>
        /// <param name="bound">The bounding region for the oct tree.</param>
        /// <param name="objList">The list of objects contained within the bounding region</param>
        /// <param name="autoDeleteIfEmpty">Delete self if becomes empty</param>
        protected OctreeBase(ref BoundingBox bound, List<T> objList, IOctree parent, OctreeBuildParameter parameter, Queue<IOctree> queueCache)
            : this(parameter, queueCache)
        {
            Bound = bound;
            Objects = objList;
            Parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="minSize"></param>
        /// <param name="autoDeleteIfEmpty">Delete self if becomes empty</param>
        protected OctreeBase(IOctree parent, OctreeBuildParameter parameter, Queue<IOctree> queueCache)
            : this(parameter, queueCache)
        {
            Objects = new List<T>();
            Bound = new BoundingBox(Vector3.Zero, Vector3.Zero);
            Parent = parent;
        }

        /// <summary>
        /// Creates an octTree with a suggestion for the bounding region containing the items.
        /// </summary>
        /// <param name="bound">The suggested dimensions for the bounding region. 
        /// Note: if items are outside this region, the region will be automatically resized.</param>
        protected OctreeBase(ref BoundingBox bound, IOctree parent, OctreeBuildParameter parameter, Queue<IOctree> queueCache)
            : this(parent, parameter, queueCache)
        {
            Bound = bound;
        }

        private IOctree CreateNode(ref BoundingBox bound, List<T> objList)
        {
            return CreateNodeWithParent(ref bound, objList, this);
        }

        protected abstract IOctree CreateNodeWithParent(ref BoundingBox bound, List<T> objList, IOctree parent);

        protected IOctree CreateNode(ref BoundingBox bound, T Item)
        {
            return CreateNode(ref bound, new List<T> { Item });
        }

        public virtual void BuildTree()
        {
            if (Bound.Maximum == Bound.Minimum || !CheckDimension())
            {
                treeBuilt = false;
                return;
            }
            if (Parameter.Cubify)
            {
                Bound = FindEnclosingCube(ref bound);
            }
            BuildTree(this, this.queue);
        }

        public void BuildTree(IOctree root, Queue<IOctree> queue)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            TreeTraversal(root, queue, null, (node) => { node.BuildCurretNodeOnly(); });

            //queue.Clear();
            //queue.Enqueue(root);
            //while (queue.Count > 0)
            //{
            //    var tree = queue.Dequeue();
            //    tree.BuildCurretNodeOnly();
            //    if (tree.HasChildren)
            //    {
            //        foreach (var subTree in tree.ChildNodes)
            //        {
            //            if (subTree != null)
            //            {
            //                queue.Enqueue(subTree);
            //            }
            //        }
            //    }
            //}
#if DEBUG
            sw.Stop();
            if (sw.ElapsedMilliseconds > 0)
                Debug.WriteLine("Buildtree time =" + sw.ElapsedMilliseconds);
#endif
           // queue.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TreeTraversal(IOctree root, Queue<IOctree> queue, Func<IOctree, bool> criteria, Action<IOctree> process,
            Func<bool> breakCriteria = null)
        {
            queue.Clear();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var tree = queue.Dequeue();
                if (criteria == null || criteria(tree))
                {
                    process(tree);
                    if(breakCriteria !=null && breakCriteria())
                    {
                        break;
                    } 
                    if (tree.HasChildren)
                    {
                        foreach (var subTree in tree.ChildNodes)
                        {
                            if (subTree != null)
                            {
                                queue.Enqueue(subTree);
                            }
                        }
                    }                 
                }
            }
            queue.Clear();
        }

        public void BuildCurretNodeOnly()
        {
            /*I think I can just directly insert items into the tree instead of using a queue.*/
            if (!treeBuilt)
            {            //terminate the recursion if we're a leaf node
                if (Objects.Count <= 1)   //doubt: is this really right? needs testing.
                {
                    treeBuilt = true;
                    return;
                }
                BuildSubTree();
                treeBuilt = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingBox[] CreateOctants(ref BoundingBox box, float minSize)
        {
            Vector3 dimensions = box.Maximum - box.Minimum;
            if (dimensions == Vector3.Zero || (dimensions.X < minSize && dimensions.Y < minSize && dimensions.Z < minSize))
            {
                return new BoundingBox[0];
            }
            Vector3 half = dimensions / 2.0f;
            Vector3 center = box.Minimum + half;

            //Create subdivided regions for each octant
            return new BoundingBox[8] {
                new BoundingBox(box.Minimum, center),
                new BoundingBox(new Vector3(center.X, box.Minimum.Y, box.Minimum.Z), new Vector3(box.Maximum.X, center.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, box.Minimum.Y, center.Z), new Vector3(box.Maximum.X, center.Y, box.Maximum.Z)),
                new BoundingBox(new Vector3(box.Minimum.X, box.Minimum.Y, center.Z), new Vector3(center.X, center.Y, box.Maximum.Z)),
                new BoundingBox(new Vector3(box.Minimum.X, center.Y, box.Minimum.Z), new Vector3(center.X, box.Maximum.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, center.Y, box.Minimum.Z), new Vector3(box.Maximum.X, box.Maximum.Y, center.Z)),
                new BoundingBox(center, box.Maximum),
                new BoundingBox(new Vector3(box.Minimum.X, center.Y, center.Z), new Vector3(center.X, box.Maximum.Y, box.Maximum.Z))
                };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckDimension()
        {
            Vector3 dimensions = Bound.Maximum - Bound.Minimum;

            if (dimensions == Vector3.Zero)
            {
                Bound = FindEnclosingBox();
            }
            dimensions = Bound.Maximum - Bound.Minimum;
            //Check to see if the dimensions of the box are greater than the minimum dimensions
            if (dimensions.X < MIN_SIZE && dimensions.Y < MIN_SIZE && dimensions.Z < MIN_SIZE)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Build sub tree nodes
        /// </summary>
        protected virtual void BuildSubTree()
        {
            if (!CheckDimension() || Objects.Count < this.Parameter.MinObjectSizeToSplit)
            {
                treeBuilt = true;
                return;
            }

            //This will contain all of our objects which fit within each respective octant.
            var octList = new List<T>[8];
            for (int i = 0; i < 8; ++i)
                octList[i] = new List<T>(Objects.Count / 8);

            int count = Objects.Count;
            for (int i = Objects.Count - 1; i >= 0; --i)
            {
                var obj = Objects[i];
                var box = GetBoundingBoxFromItem(obj);
                if (box.Minimum != box.Maximum)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        if (Octants[x].Contains(box) == ContainmentType.Contains)
                        {
                            octList[x].Add(obj);
                            Objects[i] = Objects[--count]; //Disard the existing object from location i, replaced with last valid object.
                            break;
                        }
                    }
                }
            }

            Objects.RemoveRange(count, Objects.Count - count);
            Objects.TrimExcess();

            //Create child nodes where there are items contained in the bounding region
            for (int i = 0; i < 8; ++i)
            {
                if (octList[i].Count != 0)
                {
                    ChildNodes[i] = CreateNode(ref octants[i], octList[i]);
                    ActiveNodes |= (byte)(1 << i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract BoundingBox GetBoundingBoxFromItem(T item);

        /// <summary>
        /// This finds the dimensions of the bounding box necessary to tightly enclose all items in the object list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected BoundingBox FindEnclosingBox()
        {
            Vector3 global_min = Bound.Minimum, global_max = Bound.Maximum;

            //go through all the objects in the list and find the extremes for their bounding areas.
            foreach (var obj in Objects)
            {
                Vector3 local_min = Vector3.Zero, local_max = Vector3.Zero;
                var bound = GetBoundingBoxFromItem(obj);
                if (bound != null && bound.Maximum != bound.Minimum)
                {
                    local_min = bound.Minimum;
                    local_max = bound.Maximum;
                }

                if (local_min.X < global_min.X) global_min.X = local_min.X;
                if (local_min.Y < global_min.Y) global_min.Y = local_min.Y;
                if (local_min.Z < global_min.Z) global_min.Z = local_min.Z;

                if (local_max.X > global_max.X) global_max.X = local_max.X;
                if (local_max.Y > global_max.Y) global_max.Y = local_max.Y;
                if (local_max.Z > global_max.Z) global_max.Z = local_max.Z;
            }
            return new BoundingBox(global_min, global_max);
        }
        /// <summary>
        /// This finds the smallest enclosing cube which is a power of 2, for all objects in the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingBox FindEnclosingCube(ref BoundingBox bound)
        {
            var v = (bound.Maximum - bound.Minimum) / 2 + bound.Minimum;
            bound = new BoundingBox(bound.Minimum - v, bound.Maximum - v);
            var max = Math.Max(bound.Maximum.X, Math.Max(bound.Maximum.Y, bound.Maximum.Z));
            return new BoundingBox(new Vector3(-max, -max, -max) + v, new Vector3(max, max, max) + v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int SigBit(int x)
        {
            if (x >= 0)
            {
                return (int)Math.Pow(2, Math.Ceiling(Math.Log(x) / Math.Log(2)));
            }
            else
            {
                x = Math.Abs(x);
                return -(int)Math.Pow(2, Math.Ceiling(Math.Log(x) / Math.Log(2)));
            }
        }

        public virtual void Clear()
        {
            Objects.Clear();
            foreach (var item in ChildNodes)
            {
                if (item != null)
                {
                    item.Clear();
                }
            }
            Array.Clear(ChildNodes, 0, ChildNodes.Length);
        }

        public virtual bool HitTest(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
            hitPathBoundingBoxes.Clear();
            var hitQueue = queue;
            hitQueue.Clear();
            hitQueue.Enqueue(this);
            bool isHit = false;
            while (hitQueue.Count > 0)
            {
                var node = hitQueue.Dequeue();
                bool isIntersect = false;
                bool nodeHit = node.HitTestCurrentNodeExcludeChild(model, modelMatrix, rayWS, ref hits, ref isIntersect);
                isHit |= nodeHit;
                if (isIntersect && node.HasChildren)
                {
                    foreach (var child in node.ChildNodes)
                    {
                        if (child != null)
                        {
                            hitQueue.Enqueue(child);
                        }
                    }
                }
                if (Parameter.RecordHitPathBoundingBoxes && nodeHit)
                {
                    var n = node;
                    while (n != null)
                    {
                        hitPathBoundingBoxes.Add(n.Bound);
                        n = n.Parent;
                    }
                }
            }
            if (!isHit)
            {
                hitPathBoundingBoxes.Clear();
            }
            else
            {
                OnHit?.Invoke(this, new OnHitEventArgs());
            }
            hitQueue.Clear();
            return isHit;
        }


        public abstract bool HitTestCurrentNodeExcludeChild(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, ref bool isIntersect);

        public bool Add(T item)
        {
            IOctree octant;
            return Add(item, out octant);
        }

        public virtual bool Add(T item, out IOctree octant)
        {
            var bound = GetBoundingBoxFromItem(item);
            var node = FindSmallestNodeContainsBoundingBox(ref bound);
            octant = node;
            if (node == null)
            {
                return false;
            }
            else
            {
                var nodeBase = node as IOctreeBase<T>;
                nodeBase.Objects.Add(item);
                if (nodeBase.Objects.Count > Parameter.MinObjectSizeToSplit)
                {
                    int index = (node as IOctreeBase<T>).Objects.Count - 1;
                    PushExistingToChild(nodeBase, index, GetBoundingBoxFromItem, CreateNodeWithParent, out octant);
                }
                return true;
            }
        }

        public bool PushExistingToChild(int index)
        {
            IOctree octant;
            return PushExistingToChild(index, out octant);
        }

        public virtual bool PushExistingToChild(int index, out IOctree octant)
        {
            octant = this;
            if (this.Objects.Count > Parameter.MinObjectSizeToSplit)
            {
                return PushExistingToChild(this, index, GetBoundingBoxFromItem, CreateNodeWithParent, out octant);
            }
            else
            {
                return false;
            }
        }

        /// Push existing item into child
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <param name="getBound"></param>
        /// <param name="createNodeFunc"></param>
        /// <param name="octant"></param>
        /// <returns>True: Pushed into child. Otherwise false.</returns>
        public static bool PushExistingToChild(IOctreeBase<T> node, int index, Func<T, BoundingBox> getBound, 
            CreateNodeDelegate createNodeFunc, out IOctree octant)
        {
            var item = node.Objects[index];
            octant = node;
            bool pushToChild = false;
            var bound = getBound(item);
            for (int i = 0; i < node.Octants.Length; ++i)
            {
                if (node.Octants[i].Contains(bound) == ContainmentType.Contains)
                {
                    node.Objects.RemoveAt(index);                   
                    if (node.ChildNodes[i] != null)
                    {
                        (node.ChildNodes[i] as IOctreeBase<T>).Objects.Add(item);
                        octant = node.ChildNodes[i];
                    }
                    else
                    {
                        node.ChildNodes[i] = createNodeFunc(ref node.Octants[i], new List<T>() { item }, node);
                        node.ActiveNodes |= (byte)(1 << i);
                        node.ChildNodes[i].BuildTree();
                        int idx = -1;
                        octant = (node.ChildNodes[i] as IOctreeBase<T>).FindChildByItemBound(item, out idx);
                    }
                    pushToChild = true;
                    break;
                }
            }
            return pushToChild;
        }
        /// <summary>
        /// Return new root
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual IOctree Expand(ref Vector3 direction)
        {
            return Expand(this, ref direction, CreateNodeWithParent);
        }

        /// <summary>
        /// Return new root
        /// </summary>
        /// <param name="oldRoot"></param>
        /// <returns></returns>
        public static IOctree Expand(IOctree oldRoot, ref Vector3 direction, CreateNodeDelegate createNodeFunc)
        {
            if (oldRoot.Parent != null)
            {
                throw new ArgumentException("Input node is not root node");
            }
            var rootBound = oldRoot.Bound;
            int xDirection = direction.X >= 0 ? 1 : -1;
            int yDirection = direction.Y >= 0 ? 1 : -1;
            int zDirection = direction.Z >= 0 ? 1 : -1;
            var dimension = rootBound.Maximum - rootBound.Minimum;
            var half = dimension / 2;
            var center = rootBound.Minimum + half;
            var newSize = dimension * 2;
            var newCenter = center + new Vector3(xDirection * Math.Abs(half.X), yDirection * Math.Abs(half.Y), zDirection * Math.Abs(half.Z));
            var bound = new BoundingBox(newCenter - dimension, newCenter + dimension);
            var newRoot = createNodeFunc(ref bound, new List<T>(), oldRoot);
            newRoot.Parent = null;
            newRoot.BuildTree();
            if (!oldRoot.IsEmpty)
            {
                for (int i=0; i< newRoot.Octants.Length;++i)
                {
                    if (newRoot.Octants[i] == rootBound)
                    {
                        newRoot.ChildNodes[i] = oldRoot;
                        newRoot.ActiveNodes |= (byte)(1 << i);
                        oldRoot.Parent = newRoot;
                        break;
                    }
                }
            }
            return newRoot;
        }

        /// <summary>
        /// Return new root
        /// </summary>
        /// <returns></returns>
        public virtual IOctree Shrink()
        {
            return Shrink(this);
        }

        public static IOctree Shrink(IOctree root)
        {
            if (root.Parent != null)
            { throw new ArgumentException("Input node is not a root node."); }
            if (root.IsEmpty)
            {
                return root;
            }
            else if ((root as IOctreeBase<T>).Objects.Count == 0 && (root.ActiveNodes & (root.ActiveNodes - 1)) == 0)
            {
                for (int i = 0; i < root.ChildNodes.Length; ++i)
                {
                    if (root.ChildNodes[i] != null)
                    {
                        var newRoot = root.ChildNodes[i];
                        newRoot.Parent = null;
                        root.ChildNodes[i] = null;
                        return newRoot;
                    }
                }
                return null;
            }
            else
            {
                return root;
            }
        }

        public IOctree FindSmallestNodeContainsBoundingBox(ref BoundingBox bound)
        {
            return FindSmallestNodeContainsBoundingBox<T>(bound, this, this.queue);
        }

        private static IOctree FindSmallestNodeContainsBoundingBox<E>(BoundingBox bound, IOctreeBase<E> root, Queue<IOctree> queueCache)
        {
            IOctree result = null;
            TreeTraversal(root, queueCache,
                (node) => { return node.Bound.Contains(bound) == ContainmentType.Contains; }, 
                (node) => { result = node; });
            return result;
        }

        public IOctree FindChildByItem(T item, out int index)
        {
            return FindChildByItem<T>(item, this, this.queue, out index);
        }

        public static IOctree FindChildByItem<E>(E item, IOctreeBase<E> root, Queue<IOctree> queueCache, out int index)
        {
            IOctree result = null;
            int idx = -1;
            TreeTraversal(root, queueCache, null, 
                (node) => 
                {
                    idx = (node as IOctreeBase<E>).Objects.IndexOf(item);
                    result = idx != -1 ? node : null;
                }, 
                ()=> { return idx != -1; });
            index = idx;
            return result;
        }

        public virtual bool RemoveByBound(T item, ref BoundingBox bound)
        {
            int index;
            var node = FindChildByItemBound(item, ref bound, out index);
            if (node == null)
            {
#if DEBUG
                if (!RemoveSafe(item))
                {
                    throw new Exception("item not found using bound.");
                }
                return true;
#else
                return RemoveSafe(item);
#endif
            }
            else
            {
                var nodeBase = node as IOctreeBase<T>;
                nodeBase.Objects.RemoveAt(index);
                if (nodeBase.IsEmpty && nodeBase.AutoDeleteIfEmpty)
                {
                    nodeBase.RemoveSelf();
                }
                return true;
            }
        }

        public virtual bool RemoveByBound(T item)
        {
            var bound = GetBoundingBoxFromItem(item);
            return RemoveByBound(item, ref bound);
        }

        public virtual bool RemoveSafe(T item)
        {
            Debug.WriteLine("RemoveSafe");
            int index;
            var node = FindChildByItem(item, out index);
            if (node != null)
            {
                (node as IOctreeBase<T>).Objects.RemoveAt(index);
                if (node.IsEmpty && node.AutoDeleteIfEmpty)
                {
                    node.RemoveSelf();
                }
                return true;
            }
            return false;
        }

        public virtual bool RemoveAt(int index)
        {
            if (index < 0 || index >= this.Objects.Count)
            {
                return false;
            }
            this.Objects.RemoveAt(index);
            if (this.IsEmpty && this.AutoDeleteIfEmpty)
            {
                this.RemoveSelf();
            }
            return true;
        }

        public virtual void RemoveSelf()
        {
            if (Parent == null)
            { return; }

            Clear();
            Parent.RemoveChild(this);
            Parent = null;
        }

        public void RemoveChild(IOctree child)
        {
            for (int i = 0; i < ChildNodes.Length; ++i)
            {
                if (ChildNodes[i] == child)
                {
                    ChildNodes[i] = null;
                    ActiveNodes ^= (byte)(1 << i);
                    break;
                }
            }
            if (IsEmpty && AutoDeleteIfEmpty)
            {
                RemoveSelf();
            }
        }

        public virtual IOctree FindChildByItemBound(T item, out int index)
        {
            var bound = GetBoundingBoxFromItem(item);
            return FindChildByItemBound(item, ref bound, out index);
        }

        public virtual IOctree FindChildByItemBound(T item, ref BoundingBox bound, out int index)
        {
            return FindChildByItemBound<T>(item, bound, this, this.queue, out index);
        }

        public static IOctree FindChildByItemBound<E>(E item, BoundingBox bound, IOctreeBase<E> root, Queue<IOctree> queueCache, out int index)
        {
            int idx = -1;
            IOctree result = null;
            IOctreeBase<E> lastNode = null;
            TreeTraversal(root, queueCache,
                (node) => { return node.Bound.Contains(bound) == ContainmentType.Contains; },
                (node) =>
                {
                    lastNode = node as IOctreeBase<E>;
                    idx = lastNode.Objects.IndexOf(item);
                    result = idx != -1 ? node : null;
                }, 
                () => { return idx != -1; });
            index = idx;
            //If not found, traverse from bottom to top to find the item.
            if (result == null)
            {
                while (lastNode != null)
                {
                    index = lastNode.Objects.IndexOf(item);
                    if (index == -1)
                    {
                        lastNode = lastNode.Parent as IOctreeBase<E>;
                    }
                    else
                    {
                        result = lastNode;
                        break;
                    }
                }
            }
            return result;
        }

        public static IOctree FindRoot(IOctree node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }
        #region Accessors
        public bool IsRoot
        {
            //The root node is the only node without a parent.
            get { return Parent == null; }
        }

        public bool HasChildren
        {
            get
            {
                return ActiveNodes != 0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !HasChildren && Objects.Count == 0;
            }
        }
        #endregion
    }

    /// <summary>
    /// MeshGeometryOctree slices mesh geometry by triangles into octree. Objects are tuple of each triangle index and its bounding box.
    /// </summary>
    public class MeshGeometryOctree
        : OctreeBase<Tuple<int, BoundingBox>>
    {
        public IList<Vector3> Positions { private set; get; }
        public IList<int> Indices { private set; get; }
        public MeshGeometryOctree(Vector3Collection positions, IList<int> indices, Queue<IOctree> queueCache = null)
            : this(positions, indices, null, queueCache)
        {
        }
        public MeshGeometryOctree(Vector3Collection positions, IList<int> indices,
            OctreeBuildParameter parameter, Queue<IOctree> queueCache = null)
            : base(null, parameter, queueCache)
        {
            Positions = positions;
            Indices = indices;
            Bound = BoundingBox.FromPoints(positions.Array);
            Objects = new List<Tuple<int, BoundingBox>>(indices.Count / 3);
            // Construct triangle index and its bounding box tuple
            foreach (var i in Enumerable.Range(0, indices.Count / 3))
            {
                Objects.Add(new Tuple<int, BoundingBox>(i, GetBoundingBox(i)));
            }
        }

        protected MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, ref BoundingBox bound, List<Tuple<int, BoundingBox>> triIndex,
            IOctree parent, OctreeBuildParameter paramter, Queue<IOctree> queueCache)
            : base(ref bound, triIndex, parent, paramter, queueCache)
        {
            Positions = positions;
            Indices = indices;
        }

        protected MeshGeometryOctree(BoundingBox bound, List<Tuple<int, BoundingBox>> list, IOctree parent, OctreeBuildParameter paramter, Queue<IOctree> queueCache)
            : base(ref bound, list, parent, paramter, queueCache)
        { }

        private BoundingBox GetBoundingBox(int triangleIndex)
        {
            var actual = triangleIndex * 3;
            var v1 = Positions[Indices[actual++]];
            var v2 = Positions[Indices[actual++]];
            var v3 = Positions[Indices[actual]];
            var maxX = Math.Max(v1.X, Math.Max(v2.X, v3.X));
            var maxY = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y));
            var maxZ = Math.Max(v1.Z, Math.Max(v2.Z, v3.Z));

            var minX = Math.Min(v1.X, Math.Min(v2.X, v3.X));
            var minY = Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
            var minZ = Math.Min(v1.Z, Math.Min(v2.Z, v3.Z));

            return new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }

        protected override BoundingBox GetBoundingBoxFromItem(Tuple<int, BoundingBox> item)
        {
            return item.Item2;
        }

        protected override IOctree CreateNodeWithParent(ref BoundingBox region, List<Tuple<int, BoundingBox>> objList, IOctree parent)
        {
            return new MeshGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.queue);
        }

        public override bool HitTestCurrentNodeExcludeChild(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, ref bool isIntersect)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    var idx = t.Item1 * 3;
                    var v0 = Positions[Indices[idx]];
                    var v1 = Positions[Indices[idx + 1]];
                    var v2 = Positions[Indices[idx + 2]];
                    float d;
                    var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                    var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                    var p2 = Vector3.TransformCoordinate(v2, modelMatrix);

                    if (Collision.RayIntersectsTriangle(ref rayWS, ref p0, ref p1, ref p2, out d))
                    {
                        if (d > 0 && d < result.Distance) // If d is NaN, the condition is false.
                        {
                            result.IsValid = true;
                            result.ModelHit = model;
                            // transform hit-info to world space now:
                            result.PointHit = (rayWS.Position + (rayWS.Direction * d)).ToPoint3D();
                            result.Distance = d;

                            var n = Vector3.Cross(p1 - p0, p2 - p0);
                            n.Normalize();
                            // transform hit-info to world space now:
                            result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
                            result.TriangleIndices = new System.Tuple<int, int, int>(Indices[idx], Indices[idx + 1], Indices[idx + 2]);
                            isHit = true;
                        }
                    }
                }

                if (isHit)
                {
                    isHit = false;
                    if (hits.Count > 0)
                    {
                        if (hits[0].Distance > result.Distance)
                        {
                            hits[0] = result;
                            isHit = true;
                        }
                    }
                    else
                    {
                        hits.Add(result);
                        isHit = true;
                    }
                }
            }

            return isHit;
        }
    }

    public class GeometryModel3DOctree : OctreeBase<GeometryModel3D>
    {
        /// <summary>
        /// Only root contains dictionary
        /// </summary>
        private Dictionary<Guid, IOctree> OctantDictionary = null;
        public GeometryModel3DOctree(List<GeometryModel3D> objList, Queue<IOctree> queueCache = null)
            : this(objList, null, queueCache)
        {

        }

        public GeometryModel3DOctree(List<GeometryModel3D> objList, OctreeBuildParameter paramter, Queue<IOctree> queueCache = null)
            : base(null, paramter, queueCache)
        {
            Objects = objList;
            if (Objects != null && Objects.Count > 0)
            {
                var bound = Objects[0].Bounds;
                foreach (var item in Objects)
                {
                    bound = BoundingBox.Merge(item.Bounds, bound);
                }
                this.Bound = bound;
            }
        }

        protected GeometryModel3DOctree(BoundingBox bound, List<GeometryModel3D> objList, IOctree parent, OctreeBuildParameter paramter, Queue<IOctree> queueCache)
            : base(ref bound, objList, parent, paramter, queueCache)
        { }

        public override bool HitTestCurrentNodeExcludeChild(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, ref bool isIntersect)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            bool isHit = false;
            var bound = BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            var tempHits = new List<HitTestResult>();
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    t.PushMatrix(modelMatrix);
                    isHit |= t.HitTest(rayWS, ref tempHits);
                    t.PopMatrix();
                    hits.AddRange(tempHits);
                    tempHits.Clear();
                }
            }
            return isHit;
        }

        protected override BoundingBox GetBoundingBoxFromItem(GeometryModel3D item)
        {
            return item.BoundsWithTransform;
        }

        protected override IOctree CreateNodeWithParent(ref BoundingBox bound, List<GeometryModel3D> objList, IOctree parent)
        {
            return new GeometryModel3DOctree(bound, objList, parent, parent.Parameter, this.queue);
        }

        public override void BuildTree()
        {
            OctantDictionary = new Dictionary<Guid, IOctree>(Objects.Count);
            base.BuildTree();
            TreeTraversal(this, queue, null, (node) =>
            {
                foreach(var item in (node as IOctreeBase<GeometryModel3D>).Objects)
                {
                    OctantDictionary.Add(item.GUID, node);
                }
            }, null);
        }

        public IOctree FindItemByGuid(Guid guid, GeometryModel3D item, out int index)
        {
            var root = FindRoot(this) as GeometryModel3DOctree;
            index = -1;
            if (root.OctantDictionary.ContainsKey(guid))
            {
                var node = root.OctantDictionary[guid];
                index = (node as IOctreeBase<GeometryModel3D>).Objects.IndexOf(item);
                return root.OctantDictionary[guid];
            }
            else
            {
                return null;
            }
        }

        public bool RemoveByGuid(Guid guid, GeometryModel3D item)
        {
            var root = FindRoot(this);
            return RemoveByGuid(guid, item, root as GeometryModel3DOctree);
        }

        public bool RemoveByGuid(Guid guid, GeometryModel3D item, GeometryModel3DOctree root)
        {
            if (root.OctantDictionary.ContainsKey(guid))
            {
                (OctantDictionary[guid] as GeometryModel3DOctree).RemoveSafe(item, root);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Add(GeometryModel3D item, out IOctree octant)
        {
            if(base.Add(item, out octant))
            {
                if (octant == null)
                { throw new Exception("Output octant is null"); };
                var root = FindRoot(this) as GeometryModel3DOctree;
                root.OctantDictionary.Add(item.GUID, octant);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool PushExistingToChild(int index, out IOctree octant)
        {
            var item = Objects[index];
            if(base.PushExistingToChild(index, out octant))
            {
                var root = FindRoot(this) as GeometryModel3DOctree;
                root.OctantDictionary[item.GUID] = octant;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveSafe(GeometryModel3D item)
        {
            var root = FindRoot(this);
            return RemoveSafe(item, root);
        }

        public bool RemoveSafe(GeometryModel3D item, IOctree root)
        {
            if (base.RemoveSafe(item))
            {
                RemoveFromRootDictionary(root, item.GUID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveAt(int index)
        {
            var root = FindRoot(this);
            return RemoveAt(index, root);
        }

        public bool RemoveAt(int index, IOctree root)
        {
            var id = this.Objects[index].GUID;
            if (base.RemoveAt(index))
            {
                RemoveFromRootDictionary(root, id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveByBound(GeometryModel3D item, ref BoundingBox bound)
        {
            var root = FindRoot(this);
            return RemoveByBound(item, ref bound, root);
        }

        public bool RemoveByBound(GeometryModel3D item, ref BoundingBox bound, IOctree root)
        {
            if (base.RemoveByBound(item, ref bound))
            {
                RemoveFromRootDictionary(root, item.GUID);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override IOctree Expand(ref Vector3 direction)
        {
            var root = base.Expand(ref direction);
            (root as GeometryModel3DOctree).TransferOctantDictionary(this, ref this.OctantDictionary);//Transfer the dictionary to new root
            return root;
        }

        public override IOctree Shrink()
        {
            var root = base.Shrink();
            (root as GeometryModel3DOctree).TransferOctantDictionary(this, ref this.OctantDictionary);//Transfer the dictionary to new root
            return root;
        }

        private void TransferOctantDictionary(IOctree source, ref Dictionary<Guid, IOctree> dictionary)
        {
            if(source == this)
            {
                return;
            }
            this.OctantDictionary = dictionary;
            dictionary = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveFromRootDictionary(IOctree node, Guid guid)
        {
            node = FindRoot(node);
            var root = node as GeometryModel3DOctree;
            if (root.OctantDictionary.ContainsKey(guid))
            {
                root.OctantDictionary.Remove(guid);
            }
        }
    }

    public sealed class OctreeBuildParameter
    {
        /// <summary>
        /// Minimum Octant size.
        /// </summary>
        public float MinimumOctantSize = 1f;
        /// <summary>
        /// Minimum object in each octant to start splitting into smaller octant during build
        /// </summary>
        public int MinObjectSizeToSplit = 2;
        /// <summary>
        /// Delete empty octant automatically
        /// </summary>
        public bool AutoDeleteIfEmpty = true;
        /// <summary>
        /// Generate cube octants instead of rectangle octants
        /// </summary>
        public bool Cubify = false;
        /// <summary>
        /// Record hit path bounding boxes for debugging or display purpose only
        /// </summary>
        public bool RecordHitPathBoundingBoxes = false;
        public OctreeBuildParameter()
        {
        }

        public OctreeBuildParameter(float minSize)
        {
            MinimumOctantSize = Math.Max(0, minSize);
        }

        public OctreeBuildParameter(bool autoDeleteIfEmpty)
        {
            AutoDeleteIfEmpty = autoDeleteIfEmpty;
        }

        public OctreeBuildParameter(int minSize, bool autoDeleteIfEmpty)
            : this(minSize)
        {
            AutoDeleteIfEmpty = autoDeleteIfEmpty;
        }
    }
}
