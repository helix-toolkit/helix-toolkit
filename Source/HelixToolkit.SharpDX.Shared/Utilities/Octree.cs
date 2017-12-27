/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public sealed class OnHitEventArgs : EventArgs
    {

    }
    public delegate void OnHitEventHandler(object sender, OnHitEventArgs args);
    public interface IOctree<ModelType>
    {
        event OnHitEventHandler OnHit;
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        byte ActiveNodes { set; get; }
        bool HasChildren { get; }
        bool IsRoot { get; }
        IOctree<ModelType> Parent { get; set; }
        BoundingBox Bound { get; }
        IOctree<ModelType>[] ChildNodes { get; }
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
        bool HitTest(IRenderMatrices context, ModelType model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);

        /// <summary>
        /// Hit test for only this node, not its child node
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        bool HitTestCurrentNodeExcludeChild(IRenderMatrices context, ModelType model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect);

        /// <summary>
        /// Search nearest point by a search sphere at this node only
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        bool FindNearestPointBySphereExcludeChild(IRenderMatrices context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect);

        /// <summary>
        /// Search nearest point by a search sphere for whole octree
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool FindNearestPointBySphere(IRenderMatrices context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result);

        /// <summary>
        /// Search nearest point from point on mesh
        /// </summary>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="heuristic">Use huristic search, return proximated nearest point. Set to 1.0f to disable heuristic. Value must be 0.1f ~ 1.0f</param>
        /// <returns></returns>
        bool FindNearestPointFromPoint(IRenderMatrices context, ref Vector3 point, ref List<HitTestResult> result, float heuristicSearchFactor = 1f);
        /// <summary>
        /// Search nearest point by a point and search radius
        /// </summary>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        bool FindNearestPointByPointAndSearchRadius(IRenderMatrices context, ref Vector3 point, float radius, ref List<HitTestResult> points);
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
        void RemoveChild(IOctree<ModelType> child);
    }

    public interface IOctreeBase<T, ModelType> : IOctree<ModelType>
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
        bool Add(T item, out IOctree<ModelType> octant);
        /// <summary>
        /// Expand the octree to direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        IOctree<ModelType> Expand(ref Vector3 direction);

        /// <summary>
        /// Shrink root if there is no objects
        /// </summary>
        /// <returns></returns>
        IOctree<ModelType> Shrink();

        /// <summary>
        /// Remove item(fast). Search using its bounding box. <see cref="FindChildByItemBound(T, out int)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return false if item not found</returns>
        bool RemoveByBound(T item);
        /// <summary>
        /// Remove item(fast). Search using manual bounding box, this is useful if the item's bound has been changed, use its old bound. <see cref="FindChildByItemBound(T, ref BoundingBox, out int)"/>
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
        IOctree<ModelType> FindChildByItemBound(T item, out int index);

        /// <summary>
        /// Fast search node by item bound
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="bound">The bounding-box.</param>
        /// <param name="index">The item index in Objects, if not found, output -1</param>
        /// <returns></returns>
        IOctree<ModelType> FindChildByItemBound(T item, ref BoundingBox bound, out int index);

        /// <summary>
        /// Exhaust search, slow.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">The item index in Objects, if not found, output -1</param>
        /// <returns></returns>
        IOctree<ModelType> FindChildByItem(T item, out int index);
    }

    public abstract class OctreeBase<T, ModelType> : IOctreeBase<T, ModelType>
    {
        public delegate IOctree<ModelType> CreateNodeDelegate(ref BoundingBox bound, List<T> objects, IOctree<ModelType> parent);
        protected readonly Queue<IOctree<ModelType>> queue;
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
        private readonly IOctree<ModelType>[] childNodes = new IOctree<ModelType>[8];
        public IOctree<ModelType>[] ChildNodes { get { return childNodes; } }

        public byte ActiveNodes { set; get; }

        public IOctree<ModelType> Parent { set; get; }

        private BoundingBox[] octants = null;
        public BoundingBox[] Octants { get { return octants; } }

        protected List<HitTestResult> modelHits = new List<HitTestResult>();

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

        private OctreeBase(OctreeBuildParameter parameter, Queue<IOctree<ModelType>> queueCache)
        {
            queue = queueCache ?? new Queue<IOctree<ModelType>>(64);
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
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        /// <param name="queueCache"></param>
        protected OctreeBase(ref BoundingBox bound, List<T> objList, IOctree<ModelType> parent, OctreeBuildParameter parameter, Queue<IOctree<ModelType>> queueCache)
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
        /// <param name="parameter"></param>
        /// <param name="queueCache"></param>
        protected OctreeBase(IOctree<ModelType> parent, OctreeBuildParameter parameter, Queue<IOctree<ModelType>> queueCache)
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
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        /// <param name="queueCache"></param>
        protected OctreeBase(ref BoundingBox bound, IOctree<ModelType> parent, OctreeBuildParameter parameter, Queue<IOctree<ModelType>> queueCache)
            : this(parent, parameter, queueCache)
        {
            Bound = bound;
        }

        private IOctree<ModelType> CreateNode(ref BoundingBox bound, List<T> objList)
        {
            return CreateNodeWithParent(ref bound, objList, this);
        }

        protected abstract IOctree<ModelType> CreateNodeWithParent(ref BoundingBox bound, List<T> objList, IOctree<ModelType> parent);

        protected IOctree<ModelType> CreateNode(ref BoundingBox bound, T Item)
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

        public void BuildTree(IOctree<ModelType> root, Queue<IOctree<ModelType>> queue)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            TreeTraversal(root, queue, null, (node) => { node.BuildCurretNodeOnly(); }, null, Parameter.EnableParallelBuild);
#if DEBUG
            sw.Stop();
            if (sw.ElapsedMilliseconds > 0)
                Debug.WriteLine("Buildtree time =" + sw.ElapsedMilliseconds);
#endif
            // queue.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TreeTraversal(IOctree<ModelType> root, Queue<IOctree<ModelType>> queue, Func<IOctree<ModelType>, bool> criteria, Action<IOctree<ModelType>> process,
            Func<bool> breakCriteria = null, bool useParallel = false)
        {
            if (useParallel)
            {
                if (criteria == null || criteria(root))
                {
                    process(root);
                    if (breakCriteria != null && breakCriteria())
                    {
                        return;
                    }
                    if (root.HasChildren)
                    {
                        Parallel.ForEach(root.ChildNodes, (subTree) =>
                        {
                            TreeTraversal(subTree, new Queue<IOctree<ModelType>>(), criteria, process, breakCriteria, false);
                        });
                    }
                }
            }
            else
            {
                queue.Clear();
                queue.Enqueue(root);
                while (queue.Count > 0)
                {
                    int count = queue.Count;
                    var tree = queue.Dequeue();
                    if (criteria == null || criteria(tree))
                    {
                        process(tree);
                        if (breakCriteria != null && breakCriteria())
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
            var minimum = box.Minimum;
            var maximum = box.Maximum;
            //Create subdivided regions for each octant
            return new BoundingBox[8] {
                new BoundingBox(minimum, center),
                new BoundingBox(new Vector3(center.X, minimum.Y, minimum.Z), new Vector3(maximum.X, center.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, minimum.Y, center.Z), new Vector3(maximum.X, center.Y, maximum.Z)),
                new BoundingBox(new Vector3(minimum.X, minimum.Y, center.Z), new Vector3(center.X, center.Y, maximum.Z)),
                new BoundingBox(new Vector3(minimum.X, center.Y, minimum.Z), new Vector3(center.X, maximum.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, center.Y, minimum.Z), new Vector3(maximum.X, maximum.Y, center.Z)),
                new BoundingBox(center, maximum),
                new BoundingBox(new Vector3(minimum.X, center.Y, center.Z), new Vector3(center.X, maximum.Y, maximum.Z))
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
                for (int x = 0; x < 8; ++x)
                {                      
                    if(IsContains(Octants[x], obj))
                    {
                        octList[x].Add(obj);
                        Objects[i] = Objects[--count]; //Disard the existing object from location i, replaced with last valid object.
                        break;
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
            if (Objects.Count == 0)
            {
                return Bound;
            }
            var b = GetBoundingBoxFromItem(Objects[0]);
            foreach (var obj in Objects)
            {
                var bound = GetBoundingBoxFromItem(obj);
                BoundingBox.Merge(ref b, ref bound, out b);
            }
            return b;
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

        public virtual bool HitTest(IRenderMatrices context, ModelType model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (hits == null)
            {
                hits = new List<HitTestResult>();
            }
            hitPathBoundingBoxes.Clear();
            var hitQueue = queue;
            hitQueue.Clear();
            hitQueue.Enqueue(this);
            bool isHit = false;
            modelHits.Clear();
            var modelInv = modelMatrix.Inverted();
            if(modelInv == Matrix.Zero) { return false; }//Cannot be inverted
            var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInv), Vector3.TransformNormal(rayWS.Direction, modelInv));
            while (hitQueue.Count > 0)
            {
                var node = hitQueue.Dequeue();
                bool isIntersect = false;
                bool nodeHit = node.HitTestCurrentNodeExcludeChild(context, model, modelMatrix, ref rayWS, ref rayModel, ref modelHits, ref isIntersect);
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
                hits.AddRange(modelHits);
                OnHit?.Invoke(this, new OnHitEventArgs());
            }
            hitQueue.Clear();
            return isHit;
        }


        public abstract bool HitTestCurrentNodeExcludeChild(IRenderMatrices context, ModelType model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect);

        public virtual bool FindNearestPointBySphere(IRenderMatrices context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points)
        {
            if (points == null)
            {
                points = new List<HitTestResult>();
            }
            var hitQueue = queue;
            hitQueue.Clear();
            hitQueue.Enqueue(this);
            bool isHit = false;
            while (hitQueue.Count > 0)
            {
                var node = hitQueue.Dequeue();
                bool isIntersect = false;
                bool nodeHit = node.FindNearestPointBySphereExcludeChild(context, ref sphere, ref points, ref isIntersect);
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
            }
            hitQueue.Clear();
            return isHit;
        }

        public virtual bool FindNearestPointFromPoint(IRenderMatrices context, ref Vector3 point, ref List<HitTestResult> results, float heuristicSearchFactor = 1f)
        {
            if (results == null)
            {
                results = new List<HitTestResult>();
            }
            var hitQueue = queue;
            hitQueue.Clear();
            hitQueue.Enqueue(this);

            var sphere = new global::SharpDX.BoundingSphere(point, float.MaxValue);
            bool isIntersect = false;
            bool isHit = false;
            heuristicSearchFactor = Math.Min(1.0f, Math.Max(0.1f, heuristicSearchFactor));
            while (hitQueue.Count > 0)
            {
                var node = hitQueue.Dequeue();
                isHit |= node.FindNearestPointBySphereExcludeChild(context, ref sphere, ref results, ref isIntersect);

                if (isIntersect)
                {
                    if (results.Count > 0)
                    {
                        sphere.Radius = (float)results[0].Distance * heuristicSearchFactor;
                    }
                    if (node.HasChildren)
                    {
                        foreach (var child in node.ChildNodes)
                        {
                            if (child != null)
                            {
                                hitQueue.Enqueue(child);
                            }
                        }
                    }
                }
            }
            return isHit;
        }

        public abstract bool FindNearestPointBySphereExcludeChild(IRenderMatrices context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect);

        public bool Add(T item)
        {
            IOctree<ModelType> octant;
            return Add(item, out octant);
        }

        public virtual bool Add(T item, out IOctree<ModelType> octant)
        {
            var bound = GetBoundingBoxFromItem(item);
            var node = FindSmallestNodeContainsBoundingBox(ref bound, item);
            octant = node;
            if (node == null)
            {
                return false;
            }
            else
            {
                var nodeBase = node as IOctreeBase<T, ModelType>;
                nodeBase.Objects.Add(item);
                if (nodeBase.Objects.Count > Parameter.MinObjectSizeToSplit)
                {
                    int index = (node as IOctreeBase<T, ModelType>).Objects.Count - 1;
                    PushExistingToChild(nodeBase, index, IsContains, CreateNodeWithParent, out octant);
                }
                return true;
            }
        }

        public bool PushExistingToChild(int index)
        {
            IOctree<ModelType> octant;
            return PushExistingToChild(index, out octant);
        }

        public virtual bool PushExistingToChild(int index, out IOctree<ModelType> octant)
        {
            octant = this;
            if (this.Objects.Count > Parameter.MinObjectSizeToSplit)
            {
                return PushExistingToChild(this, index, IsContains, CreateNodeWithParent, out octant);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Push existing item into child
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        /// <param name="getBound"></param>
        /// <param name="createNodeFunc"></param>
        /// <param name="octant"></param>
        /// <returns>True: Pushed into child. Otherwise false.</returns>
        public static bool PushExistingToChild(IOctreeBase<T, ModelType> node, int index, Func<BoundingBox, T, bool> isContains,
            CreateNodeDelegate createNodeFunc, out IOctree<ModelType> octant)
        {
            var item = node.Objects[index];
            octant = node;
            bool pushToChild = false;
            for (int i = 0; i < node.Octants.Length; ++i)
            {
                if (isContains(node.Octants[i], item))
                {
                    node.Objects.RemoveAt(index);
                    if (node.ChildNodes[i] != null)
                    {
                        (node.ChildNodes[i] as IOctreeBase<T, ModelType>).Objects.Add(item);
                        octant = node.ChildNodes[i];
                    }
                    else
                    {
                        node.ChildNodes[i] = createNodeFunc(ref node.Octants[i], new List<T>() { item }, node);
                        node.ActiveNodes |= (byte)(1 << i);
                        node.ChildNodes[i].BuildTree();
                        int idx = -1;
                        octant = (node.ChildNodes[i] as IOctreeBase<T, ModelType>).FindChildByItemBound(item, out idx);
                    }
                    pushToChild = true;
                    break;
                }
            }
            return pushToChild;
        }

        protected bool IsContains(BoundingBox source, T targetObj)
        {
            var bound = GetBoundingBoxFromItem(targetObj);
            return IsContains(source, bound, targetObj);
        }

        protected virtual bool IsContains(BoundingBox source, BoundingBox target, T targetObj)
        {
            return source.Contains(ref target) == ContainmentType.Contains;
        }
        /// <summary>
        /// Return new root
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual IOctree<ModelType> Expand(ref Vector3 direction)
        {
            return Expand(this, ref direction, CreateNodeWithParent);
        }

        private static Vector3 epsilon = new Vector3(float.Epsilon, float.Epsilon, float.Epsilon);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static float CorrectFloatError(float value)
        //{
        //    if (value > 0)
        //    {
        //        return value + float.Epsilon;
        //    }
        //    else if (value < 0)
        //    {
        //        return value - float.Epsilon;
        //    }else
        //    {
        //        return value;
        //    }
        //}
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static void CorrectFloatError(ref Vector3 v)
        //{
        //    v.X = CorrectFloatError(v.X);
        //    v.Y = CorrectFloatError(v.Y);
        //    v.Z = CorrectFloatError(v.Z);
        //}
        /// <summary>
        /// Return new root
        /// </summary>
        /// <param name="oldRoot"></param>
        /// <param name="direction"></param>
        /// <param name="createNodeFunc"></param>
        /// <returns></returns>
        public static IOctree<ModelType> Expand(IOctree<ModelType> oldRoot, ref Vector3 direction, CreateNodeDelegate createNodeFunc)
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
            var half = dimension / 2 + epsilon;
            var center = rootBound.Minimum + half;
            var newSize = dimension * 2;
            var newCenter = center + new Vector3(xDirection * Math.Abs(half.X), yDirection * Math.Abs(half.Y), zDirection * Math.Abs(half.Z));
            var bound = new BoundingBox(newCenter - dimension, newCenter + dimension);
            BoundingBox.Merge(ref rootBound, ref bound, out bound);
            var newRoot = createNodeFunc(ref bound, new List<T>(), oldRoot);
            newRoot.Parent = null;
            newRoot.BuildTree();
            bool succ = false;
            if (!oldRoot.IsEmpty)
            {
                int idx = -1;
                float diff = float.MaxValue;
                for (int i = 0; i < newRoot.Octants.Length; ++i)
                {
                    var d = (newRoot.Octants[i].Minimum - rootBound.Minimum).LengthSquared();
                    if (d < diff)
                    {
                        diff = d;
                        idx = i;
                        if (diff < 10e-8)
                        {
                            break;
                        }
                    }
                }
                if (idx >= 0 && idx < newRoot.Octants.Length)
                {
                    newRoot.ChildNodes[idx] = oldRoot;
                    newRoot.Octants[idx] = oldRoot.Bound;
                    newRoot.ActiveNodes |= (byte)(1 << idx);
                    oldRoot.Parent = newRoot;
                    succ = true;
                }

                if (!succ)
                {
                    throw new Exception("Expand failed.");
                }
            }
            return newRoot;
        }

        /// <summary>
        /// Return new root
        /// </summary>
        /// <returns></returns>
        public virtual IOctree<ModelType> Shrink()
        {
            return Shrink(this);
        }

        public static IOctree<ModelType> Shrink(IOctree<ModelType> root)
        {
            if (root.Parent != null)
            { throw new ArgumentException("Input node is not a root node."); }
            if (root.IsEmpty)
            {
                return root;
            }
            else if ((root as IOctreeBase<T, ModelType>).Objects.Count == 0 && (root.ActiveNodes & (root.ActiveNodes - 1)) == 0)
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

        public IOctree<ModelType> FindSmallestNodeContainsBoundingBox(ref BoundingBox bound, T item)
        {
            return FindSmallestNodeContainsBoundingBox<T>(bound, item, IsContains, this, this.queue);
        }

        private static IOctree<ModelType> FindSmallestNodeContainsBoundingBox<E>(BoundingBox bound, E item, Func<BoundingBox, E, bool> isContains, IOctreeBase<E, ModelType> root, Queue<IOctree<ModelType>> queueCache)
        {
            IOctree<ModelType> result = null;
            TreeTraversal(root, queueCache,
                (node) => { return isContains(node.Bound, item); },
                (node) => { result = node; });
            return result;
        }

        public IOctree<ModelType> FindChildByItem(T item, out int index)
        {
            return FindChildByItem<T>(item, this, this.queue, out index);
        }

        public static IOctree<ModelType> FindChildByItem<E>(E item, IOctreeBase<E, ModelType> root, Queue<IOctree<ModelType>> queueCache, out int index)
        {
            IOctree<ModelType> result = null;
            int idx = -1;
            TreeTraversal(root, queueCache, null,
                (node) =>
                {
                    idx = (node as IOctreeBase<E, ModelType>).Objects.IndexOf(item);
                    result = idx != -1 ? node : null;
                },
                () => { return idx != -1; });
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
                var nodeBase = node as IOctreeBase<T, ModelType>;
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
                (node as IOctreeBase<T, ModelType>).Objects.RemoveAt(index);
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

        public void RemoveChild(IOctree<ModelType> child)
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

        public virtual IOctree<ModelType> FindChildByItemBound(T item, out int index)
        {
            return FindChildByItemBound(item, ref bound, out index);
        }

        public virtual IOctree<ModelType> FindChildByItemBound(T item, ref BoundingBox bound, out int index)
        {
            return FindChildByItemBound<T>(item, bound, IsContains, this, this.queue, out index);
        }

        public static IOctree<ModelType> FindChildByItemBound<E>(E item, BoundingBox bound, Func<BoundingBox, BoundingBox, E, bool> isContains, IOctreeBase<E, ModelType> root, Queue<IOctree<ModelType>> queueCache, out int index)
        {
            int idx = -1;
            IOctree<ModelType> result = null;
            IOctreeBase<E, ModelType> lastNode = null;
            TreeTraversal(root, queueCache,
                (node) => { return isContains(node.Bound, bound, item); },
                (node) =>
                {
                    lastNode = node as IOctreeBase<E, ModelType>;
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
                        lastNode = lastNode.Parent as IOctreeBase<E, ModelType>;
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

        public static IOctree<ModelType> FindRoot(IOctree<ModelType> node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }

        public bool FindNearestPointByPointAndSearchRadius(IRenderMatrices context, ref Vector3 point, float radius, ref List<HitTestResult> result)
        {
            var sphere = new global::SharpDX.BoundingSphere(point, radius);
            return FindNearestPointBySphere(context, ref sphere, ref result);
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
        /// <summary>
        /// Use parallel tree traversal to build the octree
        /// </summary>
        public bool EnableParallelBuild = false;
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
