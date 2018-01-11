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
    /// <summary>
    /// 
    /// </summary>
    public sealed class OnHitEventArgs : EventArgs
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void OnHitEventHandler(object sender, OnHitEventArgs args);

    /// <summary>
    /// General interface for octree
    /// </summary>
    public interface IOctree
    {
        /// <summary>
        /// 
        /// </summary>
        event OnHitEventHandler OnHit;
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        byte ActiveNodes { set; get; }
        /// <summary>
        /// Has child octants
        /// </summary>
        bool HasChildren { get; }
        /// <summary>
        /// If this node is root node
        /// </summary>
        bool IsRoot { get; }
        /// <summary>
        /// Parent node
        /// </summary>
        IOctree Parent { get; set; }
        /// <summary>
        /// Node bound
        /// </summary>
        BoundingBox Bound { get; }
        /// <summary>
        /// Child octants
        /// </summary>
        IOctree[] ChildNodes { get; }
        /// <summary>
        /// Octant bounds
        /// </summary>
        BoundingBox[] Octants { get; }
        /// <summary>
        /// Output the hit path of the tree traverse. Only for debugging
        /// </summary>
        IList<BoundingBox> HitPathBoundingBoxes { get; }
        /// <summary>
        /// Octree parameter
        /// </summary>
        OctreeBuildParameter Parameter { get; }
        /// <summary>
        /// Whether the tree has been built.
        /// </summary>
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
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        bool HitTest(IRenderContext context, IRenderable model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);
        /// <summary>
        /// Normal hit test from top to bottom
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="hitTestThickness"></param>
        /// <returns></returns>
        bool HitTest(IRenderContext context, IRenderable model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitTestThickness);

        /// <summary>
        /// Hit test for only this node, not its child node
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness">Only used for point/line hit test</param>
        /// <param name="rayModel"></param>
        /// <returns></returns>
        bool HitTestCurrentNodeExcludeChild(IRenderContext context, IRenderable model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, 
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

        /// <summary>
        /// Search nearest point by a search sphere at this node only
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect);

        /// <summary>
        /// Search nearest point by a search sphere for whole octree
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool FindNearestPointBySphere(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result);

        /// <summary>
        /// Search nearest point from point on mesh
        /// </summary>
        /// <param name="context"></param>
        /// <param name="point"></param>
        /// <param name="result"></param>
        /// <param name="heuristicSearchFactor">Use huristic search, return proximated nearest point. Set to 1.0f to disable heuristic. Value must be 0.1f ~ 1.0f</param>
        /// <returns></returns>
        bool FindNearestPointFromPoint(IRenderContext context, ref Vector3 point, ref List<HitTestResult> result, float heuristicSearchFactor = 1f);
        /// <summary>
        /// Search nearest point by a point and search radius
        /// </summary>
        /// <param name="context"></param>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        bool FindNearestPointByPointAndSearchRadius(IRenderContext context, ref Vector3 point, float radius, ref List<HitTestResult> points);
        /// <summary>
        /// Build the whole tree from top to bottom iteratively.
        /// </summary>
        void BuildTree();

        /// <summary>
        /// Build current node level only, this will only build current node and create children, but not build its children. 
        /// To build from top to bottom, call BuildTree
        /// </summary>
        void BuildCurretNodeOnly();
        /// <summary>
        /// Clear the tree
        /// </summary>
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

    /// <summary>
    /// Base class template interface for octree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOctreeBase<T> : IOctree
    {
        /// <summary>
        /// Objects for current node.
        /// </summary>
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
        /// <param name="direction"></param>
        /// <returns></returns>
        IOctree Expand(ref Vector3 direction);

        /// <summary>
        /// Shrink root if there is no objects
        /// </summary>
        /// <returns></returns>
        IOctree Shrink();

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
        IOctree FindChildByItemBound(T item, out int index);

        /// <summary>
        /// Fast search node by item bound
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="bound">The bounding-box.</param>
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
    /// <summary>
    /// Base class template implementation for <see cref="IOctreeBase{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OctreeBase<T> : IOctreeBase<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objects"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public delegate IOctree CreateNodeDelegate(ref BoundingBox bound, List<T> objects, IOctree parent);
        /// <summary>
        /// internal stack for tree traversal
        /// </summary>
        protected readonly Stack<IEnumerator<IOctree>> stack;
        /// <summary>
        /// 
        /// </summary>
        public event OnHitEventHandler OnHit;
        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public float MIN_SIZE { get { return Parameter.MinimumOctantSize; } }
        /// <summary>
        /// <see cref="IOctree.TreeBuilt"/>
        /// </summary>
        public bool TreeBuilt { get { return treeBuilt; } }
        /// <summary>
        /// 
        /// </summary>
        protected bool treeBuilt = false;       //there is no pre-existing tree yet.
        /// <summary>
        /// <see cref="IOctree.Parameter"/>
        /// </summary>
        public OctreeBuildParameter Parameter { private set; get; }

        private BoundingBox bound;

        /// <summary>
        /// <see cref="IOctree.Bound"/>
        /// </summary>
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
        /// <summary>
        /// <see cref="IOctreeBase{T}.Objects"/>
        /// </summary>
        public List<T> Objects { protected set; get; }

        private readonly List<BoundingBox> hitPathBoundingBoxes = new List<BoundingBox>();
        /// <summary>
        /// <see cref="IOctree.HitPathBoundingBoxes"/>
        /// </summary>
        public IList<BoundingBox> HitPathBoundingBoxes { get { return hitPathBoundingBoxes.AsReadOnly(); } }
        /// <summary>
        /// These are all of the possible child octants for this node in the tree.
        /// </summary>
        private readonly IOctree[] childNodes = new IOctree[8];
        /// <summary>
        /// <see cref="IOctree.ChildNodes"/>
        /// </summary>
        public IOctree[] ChildNodes { get { return childNodes; } }
        /// <summary>
        /// <see cref="IOctree.ActiveNodes"/>
        /// </summary>
        public byte ActiveNodes { set; get; }
        /// <summary>
        /// <see cref="IOctree.Parent"/>
        /// </summary>
        public IOctree Parent { set; get; }

        private BoundingBox[] octants = null;
        /// <summary>
        /// <see cref="IOctree.Octants"/>
        /// </summary>
        public BoundingBox[] Octants { get { return octants; } }
        /// <summary>
        /// 
        /// </summary>
        protected List<HitTestResult> modelHits = new List<HitTestResult>();
        /// <summary>
        /// Delete the octant if there is no object or child octant inside it.
        /// </summary>
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

        private OctreeBase(OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache)
        {
            stack = stackCache ?? new Stack<IEnumerator<IOctree>>(64);
#if DEBUG
            if (stackCache == null)
            {
                Debug.WriteLine("stack cache is null");
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
        /// <param name="stackCache"></param>
        protected OctreeBase(ref BoundingBox bound, List<T> objList, IOctree parent, OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache)
            : this(parameter, stackCache)
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
        /// <param name="stackCache"></param>
        protected OctreeBase(IOctree parent, OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache)
            : this(parameter, stackCache)
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
        /// <param name="stackCache"></param>
        protected OctreeBase(ref BoundingBox bound, IOctree parent, OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache)
            : this(parent, parameter, stackCache)
        {
            Bound = bound;
        }

        private IOctree CreateNode(ref BoundingBox bound, List<T> objList)
        {
            return CreateNodeWithParent(ref bound, objList, this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected abstract IOctree CreateNodeWithParent(ref BoundingBox bound, List<T> objList, IOctree parent);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected IOctree CreateNode(ref BoundingBox bound, T Item)
        {
            return CreateNode(ref bound, new List<T> { Item });
        }
        /// <summary>
        /// Build the octree
        /// </summary>
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
            BuildTree(this, this.stack);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="stack"></param>
        public void BuildTree(IOctree root, Stack<IEnumerator<IOctree>> stack)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            TreeTraversal(root, stack, null, (node) => { node.BuildCurretNodeOnly(); }, null, Parameter.EnableParallelBuild);
#if DEBUG
            sw.Stop();
            if (sw.ElapsedMilliseconds > 0)
                Debug.WriteLine("Buildtree time =" + sw.ElapsedMilliseconds);
#endif
        }
        /// <summary>
        /// Common function to traverse the tree
        /// </summary>
        /// <param name="root"></param>
        /// <param name="stack"></param>
        /// <param name="criteria"></param>
        /// <param name="process"></param>
        /// <param name="breakCriteria"></param>
        /// <param name="useParallel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TreeTraversal(IOctree root, Stack<IEnumerator<IOctree>> stack, Func<IOctree, bool> criteria, Action<IOctree> process,
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
                            TreeTraversal(subTree, new Stack<IEnumerator<IOctree>>(), criteria, process, breakCriteria, false);
                        });
                    }
                }
            }
            else
            {
                var e = Enumerable.Repeat(root, 1).GetEnumerator();
                while (true)
                {
                    while (e.MoveNext())
                    {
                        var tree = e.Current;
                        if (e.Current != null && (criteria == null || criteria(tree)))
                        {
                            process(tree);
                            if (breakCriteria != null && breakCriteria())
                            {
                                break;
                            }
                            if (tree.HasChildren)
                            {
                                stack.Push(e);
                                e = tree.ChildNodes.AsEnumerable().GetEnumerator();
                            }
                        }
                    }                   
                    if (stack.Count == 0) { break; }
                    e.Dispose();                   
                    e = stack.Pop();
                }
                e.Dispose();
                while (stack.Count != 0)
                {
                    stack.Pop().Dispose();
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void BuildCurretNodeOnly()
        {
            /*I think I can just directly insert items into the tree instead of using a stack.*/
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <param name="minSize"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
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
        /// <summary>
        /// <see cref="IOctree.Clear"/>
        /// </summary>
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
        /// <summary>
        /// <see cref="IOctree.HitTest(IRenderContext, IRenderable, Matrix, Ray, ref List{HitTestResult})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public bool HitTest(IRenderContext context, IRenderable model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
            return HitTest(context, model, modelMatrix, rayWS, ref hits, 0);
        }
        /// <summary>
        /// <see cref="IOctree.HitTest(IRenderContext, IRenderable, Matrix, Ray, ref List{HitTestResult}, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public virtual bool HitTest(IRenderContext context, IRenderable model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness)
        {
            if (hits == null)
            {
                hits = new List<HitTestResult>();
            }
            hitPathBoundingBoxes.Clear();
            var hitStack = stack;
            bool isHit = false;
            modelHits.Clear();
            var modelInv = modelMatrix.Inverted();
            if(modelInv == Matrix.Zero) { return false; }//Cannot be inverted
            var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInv), Vector3.TransformNormal(rayWS.Direction, modelInv));

            var e = Enumerable.Repeat<IOctree>(this, 1).GetEnumerator();
            while (true)
            {
                while (e.MoveNext())
                {
                    var node = e.Current;
                    if (node == null) { continue; }
                    bool isIntersect = false;
                    bool nodeHit = node.HitTestCurrentNodeExcludeChild(context, model, modelMatrix, ref rayWS, ref rayModel, ref modelHits, ref isIntersect, hitThickness);
                    isHit |= nodeHit;
                    if (isIntersect && node.HasChildren)
                    {
                        hitStack.Push(e);
                        e = node.ChildNodes.AsEnumerable().GetEnumerator();
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
                if (hitStack.Count == 0)
                {
                    break;
                }
                e.Dispose();               
                e = hitStack.Pop();
            }
            e.Dispose();
            while (hitStack.Count > 0)
            {
                hitStack.Pop().Dispose();
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
            return isHit;
        }

        /// <summary>
        /// Hit test for current node.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public abstract bool HitTestCurrentNodeExcludeChild(IRenderContext context, IRenderable model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public virtual bool FindNearestPointBySphere(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points)
        {
            if (points == null)
            {
                points = new List<HitTestResult>();
            }
            var hitStack = stack;
            bool isHit = false;
            var e = Enumerable.Repeat<IOctree>(this, 1).GetEnumerator();
            while (true)
            {
                while (e.MoveNext())
                {
                    var node = e.Current;
                    if (node == null) { continue; }
                    bool isIntersect = false;
                    bool nodeHit = node.FindNearestPointBySphereExcludeChild(context, ref sphere, ref points, ref isIntersect);
                    isHit |= nodeHit;
                    if (isIntersect && node.HasChildren)
                    {
                        hitStack.Push(e);
                        e = node.ChildNodes.AsEnumerable().GetEnumerator();
                    }
                }
                if (hitStack.Count == 0) { break; }
                e.Dispose();
                e = hitStack.Pop();
            }
            e.Dispose();
            while (hitStack.Count > 0)
            {
                hitStack.Pop().Dispose();
            }
            return isHit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="point"></param>
        /// <param name="results"></param>
        /// <param name="heuristicSearchFactor"></param>
        /// <returns></returns>
        public virtual bool FindNearestPointFromPoint(IRenderContext context, ref Vector3 point, ref List<HitTestResult> results, float heuristicSearchFactor = 1f)
        {
            if (results == null)
            {
                results = new List<HitTestResult>();
            }
            var hitStack = stack;

            var sphere = new global::SharpDX.BoundingSphere(point, float.MaxValue);
            bool isIntersect = false;
            bool isHit = false;
            heuristicSearchFactor = Math.Min(1.0f, Math.Max(0.1f, heuristicSearchFactor));

            var e = Enumerable.Repeat<IOctree>(this, 1).GetEnumerator();
            while (true)
            {
                while (e.MoveNext())
                {
                    var node = e.Current;
                    if (node == null) { continue; }
                    isHit |= node.FindNearestPointBySphereExcludeChild(context, ref sphere, ref results, ref isIntersect);

                    if (isIntersect)
                    {
                        if (results.Count > 0)
                        {
                            sphere.Radius = (float)results[0].Distance * heuristicSearchFactor;
                        }
                        if (node.HasChildren)
                        {
                            hitStack.Push(e);
                            e = node.ChildNodes.AsEnumerable().GetEnumerator();
                        }
                    }
                }
                if (hitStack.Count == 0) { break; }
                e.Dispose();
                e = hitStack.Pop();
            }
            e.Dispose();
            while (hitStack.Count > 0)
            {
                hitStack.Pop().Dispose();
            }
            return isHit;
        }
        /// <summary>
        /// Find nearest point by sphere on current node only.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public abstract bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect);
        /// <summary>
        /// <see cref="IOctreeBase{T}.Add(T)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            IOctree octant;
            return Add(item, out octant);
        }
        /// <summary>
        /// <see cref="IOctreeBase{T}.Add(T, out IOctree)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="octant"></param>
        /// <returns></returns>
        public virtual bool Add(T item, out IOctree octant)
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
                var nodeBase = node as IOctreeBase<T>;
                nodeBase.Objects.Add(item);
                if (nodeBase.Objects.Count > Parameter.MinObjectSizeToSplit)
                {
                    int index = (node as IOctreeBase<T>).Objects.Count - 1;
                    PushExistingToChild(nodeBase, index, IsContains, CreateNodeWithParent, out octant);
                }
                return true;
            }
        }
        /// <summary>
        /// Push one of object belongs to current node into its child octant
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool PushExistingToChild(int index)
        {
            IOctree octant;
            return PushExistingToChild(index, out octant);
        }
        /// <summary>
        /// Push one of object belongs to current node into its child octant
        /// </summary>
        /// <param name="index"></param>
        /// <param name="octant"></param>
        /// <returns></returns>
        public virtual bool PushExistingToChild(int index, out IOctree octant)
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
        /// <param name="isContains"></param>
        /// <param name="createNodeFunc"></param>
        /// <param name="octant"></param>
        /// <returns>True: Pushed into child. Otherwise false.</returns>
        public static bool PushExistingToChild(IOctreeBase<T> node, int index, Func<BoundingBox, T, bool> isContains,
            CreateNodeDelegate createNodeFunc, out IOctree octant)
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        protected bool IsContains(BoundingBox source, T targetObj)
        {
            var bound = GetBoundingBoxFromItem(targetObj);
            return IsContains(source, bound, targetObj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        protected virtual bool IsContains(BoundingBox source, BoundingBox target, T targetObj)
        {
            return source.Contains(ref target) == ContainmentType.Contains;
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
        /// Shrink the root bound to contains all items inside, return new root
        /// </summary>
        /// <returns></returns>
        public virtual IOctree Shrink()
        {
            return Shrink(this);
        }
        /// <summary>
        /// Shrink the root bound to contains all items inside
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public IOctree FindSmallestNodeContainsBoundingBox(ref BoundingBox bound, T item)
        {
            return FindSmallestNodeContainsBoundingBox<T>(bound, item, IsContains, this, this.stack);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="bound"></param>
        /// <param name="item"></param>
        /// <param name="isContains"></param>
        /// <param name="root"></param>
        /// <param name="stackCache"></param>
        /// <returns></returns>
        private static IOctree FindSmallestNodeContainsBoundingBox<E>(BoundingBox bound, E item, Func<BoundingBox, E, bool> isContains, IOctreeBase<E> root, Stack<IEnumerator<IOctree>> stackCache)
        {
            IOctree result = null;
            TreeTraversal(root, stackCache,
                (node) => { return isContains(node.Bound, item); },
                (node) => { result = node; });
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public IOctree FindChildByItem(T item, out int index)
        {
            return FindChildByItem<T>(item, this, this.stack, out index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="item"></param>
        /// <param name="root"></param>
        /// <param name="stackCache"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IOctree FindChildByItem<E>(E item, IOctreeBase<E> root, Stack<IEnumerator<IOctree>> stackCache, out int index)
        {
            IOctree result = null;
            int idx = -1;
            TreeTraversal(root, stackCache, null,
                (node) =>
                {
                    idx = (node as IOctreeBase<E>).Objects.IndexOf(item);
                    result = idx != -1 ? node : null;
                },
                () => { return idx != -1; });
            index = idx;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool RemoveByBound(T item)
        {
            var bound = GetBoundingBoxFromItem(item);
            return RemoveByBound(item, ref bound);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        public virtual void RemoveSelf()
        {
            if (Parent == null)
            { return; }

            Clear();
            Parent.RemoveChild(this);
            Parent = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual IOctree FindChildByItemBound(T item, out int index)
        {
            return FindChildByItemBound(item, ref bound, out index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="bound"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual IOctree FindChildByItemBound(T item, ref BoundingBox bound, out int index)
        {
            return FindChildByItemBound<T>(item, bound, IsContains, this, this.stack, out index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="item"></param>
        /// <param name="bound"></param>
        /// <param name="isContains"></param>
        /// <param name="root"></param>
        /// <param name="stackCache"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IOctree FindChildByItemBound<E>(E item, BoundingBox bound, Func<BoundingBox, BoundingBox, E, bool> isContains, IOctreeBase<E> root, Stack<IEnumerator<IOctree>> stackCache, out int index)
        {
            int idx = -1;
            IOctree result = null;
            IOctreeBase<E> lastNode = null;
            TreeTraversal(root, stackCache,
                (node) => { return isContains(node.Bound, bound, item); },
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IOctree FindRoot(IOctree node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool FindNearestPointByPointAndSearchRadius(IRenderContext context, ref Vector3 point, float radius, ref List<HitTestResult> result)
        {
            var sphere = new global::SharpDX.BoundingSphere(point, radius);
            return FindNearestPointBySphere(context, ref sphere, ref result);
        }

#region Accessors
        /// <summary>
        /// <see cref="IOctree.IsRoot"/>
        /// </summary>
        public bool IsRoot
        {
            //The root node is the only node without a parent.
            get { return Parent == null; }
        }
        /// <summary>
        /// <see cref="IOctree.HasChildren"/>
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return ActiveNodes != 0;
            }
        }
        /// <summary>
        /// <see cref="IOctree.IsEmpty"/>
        /// </summary>
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
    /// 
    /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public OctreeBuildParameter()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minSize"></param>
        public OctreeBuildParameter(float minSize)
        {
            MinimumOctantSize = Math.Max(0, minSize);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoDeleteIfEmpty"></param>
        public OctreeBuildParameter(bool autoDeleteIfEmpty)
        {
            AutoDeleteIfEmpty = autoDeleteIfEmpty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minSize"></param>
        /// <param name="autoDeleteIfEmpty"></param>
        public OctreeBuildParameter(int minSize, bool autoDeleteIfEmpty)
            : this(minSize)
        {
            AutoDeleteIfEmpty = autoDeleteIfEmpty;
        }
    }


    /// <summary>
    /// MeshGeometryOctree slices mesh geometry by triangles into octree. Objects are tuple of each triangle index and its bounding box.
    /// </summary>
    public class MeshGeometryOctree
        : OctreeBase<Tuple<int, BoundingBox>>
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<Vector3> Positions { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public IList<int> Indices { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="stackCache"></param>
        public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, Stack<IEnumerator<IOctree>> stackCache = null)
            : this(positions, indices, null, stackCache)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices,
            OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache = null)
            : base(null, parameter, stackCache)
        {
            Positions = positions;
            Indices = indices;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = new List<Tuple<int, BoundingBox>>(indices.Count / 3);
            // Construct triangle index and its bounding box tuple
            foreach (var i in Enumerable.Range(0, indices.Count / 3))
            {
                Objects.Add(new Tuple<int, BoundingBox>(i, GetBoundingBox(i)));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="indices"></param>
        /// <param name="bound"></param>
        /// <param name="triIndex"></param>
        /// <param name="parent"></param>
        /// <param name="paramter"></param>
        /// <param name="stackCache"></param>
        protected MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, ref BoundingBox bound, List<Tuple<int, BoundingBox>> triIndex,
            IOctree parent, OctreeBuildParameter paramter, Stack<IEnumerator<IOctree>> stackCache)
            : base(ref bound, triIndex, parent, paramter, stackCache)
        {
            Positions = positions;
            Indices = indices;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="list"></param>
        /// <param name="parent"></param>
        /// <param name="paramter"></param>
        /// <param name="stackCache"></param>
        protected MeshGeometryOctree(BoundingBox bound, List<Tuple<int, BoundingBox>> list, IOctree parent, OctreeBuildParameter paramter, Stack<IEnumerator<IOctree>> stackCache)
            : base(ref bound, list, parent, paramter, stackCache)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override BoundingBox GetBoundingBoxFromItem(Tuple<int, BoundingBox> item)
        {
            return item.Item2;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IOctree CreateNodeWithParent(ref BoundingBox region, List<Tuple<int, BoundingBox>> objList, IOctree parent)
        {
            return new MeshGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.stack);
        }
        /// <summary>
        /// <see cref="OctreeBase{T}.HitTestCurrentNodeExcludeChild(IRenderContext, IRenderable, Matrix, ref Ray, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, IRenderable model, Matrix modelMatrix, ref Ray rayWS, 
            ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = Bound;
            //Hit test in local space.
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    var idx = t.Item1 * 3;
                    var t1 = Indices[idx];
                    var t2 = Indices[idx + 1];
                    var t3 = Indices[idx + 2];
                    var v0 = Positions[t1];
                    var v1 = Positions[t2];
                    var v2 = Positions[t3];
                    float d;

                    if (Collision.RayIntersectsTriangle(ref rayModel, ref v0, ref v1, ref v2, out d))
                    {
                        if (d >= 0 && d < result.Distance) // If d is NaN, the condition is false.
                        {
                            result.IsValid = true;
                            result.ModelHit = model;
                            // transform hit-info to world space now:
                            var pointWorld = Vector3.TransformCoordinate(rayModel.Position + (rayModel.Direction * d), modelMatrix);
                            result.PointHit = pointWorld;
                            result.Distance = (rayWS.Position - pointWorld).Length();

                            var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                            var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                            var p2 = Vector3.TransformCoordinate(v2, modelMatrix);
                            var n = Vector3.Cross(p1 - p0, p2 - p0);
                            n.Normalize();
                            // transform hit-info to world space now:
                            result.NormalAtHit = n;// Vector3.TransformNormal(n, m).ToVector3D();
                            result.TriangleIndices = new System.Tuple<int, int, int>(t1, t2, t3);
                            result.Tag = t.Item1;
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
        /// <summary>
        /// <see cref="OctreeBase{T}.FindNearestPointBySphereExcludeChild(IRenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var tempResult = new HitTestResult();
            tempResult.Distance = float.MaxValue;
            var containment = Bound.Contains(ref sphere);
            if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
            {
                isIntersect = true;
                foreach (var t in Objects)
                {
                    containment = t.Item2.Contains(sphere);
                    if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                    {
                        Vector3 cloestPoint;

                        var idx = t.Item1 * 3;
                        var t1 = Indices[idx];
                        var t2 = Indices[idx + 1];
                        var t3 = Indices[idx + 2];
                        var v0 = Positions[t1];
                        var v1 = Positions[t2];
                        var v2 = Positions[t3];
                        Collision.ClosestPointPointTriangle(ref sphere.Center, ref v0, ref v1, ref v2, out cloestPoint);
                        var d = (cloestPoint - sphere.Center).Length();
                        if (tempResult.Distance > d)
                        {
                            tempResult.Distance = d;
                            tempResult.IsValid = true;
                            tempResult.PointHit = cloestPoint;
                            tempResult.TriangleIndices = new Tuple<int, int, int>(t1, t2, t3);
                            tempResult.Tag = t.Item1;
                            isHit = true;
                        }
                    }
                }
                if (isHit)
                {
                    isHit = false;
                    if (result.Count > 0)
                    {
                        if (result[0].Distance > tempResult.Distance)
                        {
                            result[0] = tempResult;
                            isHit = true;
                        }
                    }
                    else
                    {
                        result.Add(tempResult);
                        isHit = true;
                    }
                }
            }
            else
            {
                isIntersect = false;
            }
            return isHit;
        }
    }
    /// <summary>
    /// Octree for points
    /// </summary>
    public class PointGeometryOctree : OctreeBase<int>
    {
        private IList<Vector3> Positions;
        private static readonly Vector3 BoundOffset = new Vector3(0.0001f);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="stackCache"></param>
        public PointGeometryOctree(IList<Vector3> positions, Stack<IEnumerator<IOctree>> stackCache = null)
            : this(positions, null, stackCache)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public PointGeometryOctree(IList<Vector3> positions,
            OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache = null)
               : base(null, parameter, stackCache)
        {
            Positions = positions;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = Enumerable.Range(0, Positions.Count).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="positions"></param>
        /// <param name="list"></param>
        /// <param name="parent"></param>
        /// <param name="paramter"></param>
        /// <param name="stackCache"></param>
        protected PointGeometryOctree(BoundingBox bound, IList<Vector3> positions, List<int> list, IOctree parent, OctreeBuildParameter paramter,
            Stack<IEnumerator<IOctree>> stackCache)
            : base(ref bound, list, parent, paramter, stackCache)
        {
            Positions = positions;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override bool IsContains(BoundingBox source, BoundingBox target, int obj)
        {
            return source.Contains(Positions[obj]) != ContainmentType.Disjoint;
        }
        /// <summary>
        /// Get the distance from ray to a point
        /// </summary>
        /// <param name="r"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double DistanceRayToPoint(ref Ray r, ref Vector3 p)
        {
            Vector3 v = r.Direction;
            Vector3 w = p - r.Position;

            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;

            Vector3 pb = r.Position + v * b;
            return (p - pb).Length();
        }
        /// <summary>
        /// Return nearest point it gets hit. And the distance from ray origin to the point it gets hit
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="context"></param>
        /// <param name="rayModel"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, IRenderable model, Matrix modelMatrix, 
            ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt || context == null)
            {
                return false;
            }
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = Bound;

            if (rayModel.Intersects(ref bound))
            {
                var svpm = context.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;
                var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
                var pos4 = new Vector4(rayWS.Position, 1);
                Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
                Vector4.Transform(ref pos4, ref svpm, out pos4);
                var clickPoint = clickPoint4.ToVector3();

                isIntersect = true;
                var dist = hitThickness;
                foreach (var t in this.Objects)
                {
                    var v0 = Positions[t];
                    var p0 = Vector3.TransformCoordinate(v0, smvpm);
                    var pv = p0 - clickPoint;
                    var d = pv.Length();
                    if (d < dist) // If d is NaN, the condition is false.
                    {
                        dist = d;
                        result.IsValid = true;
                        result.ModelHit = model;
                        var px = Vector3.TransformCoordinate(v0, modelMatrix);
                        result.PointHit = px;
                        result.Distance = (rayWS.Position - px).Length();
                        result.Tag = t;
                        isHit = true;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IOctree CreateNodeWithParent(ref BoundingBox bound, List<int> objList, IOctree parent)
        {
            return new PointGeometryOctree(bound, Positions, objList, parent, Parameter, stack);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override BoundingBox GetBoundingBoxFromItem(int item)
        {
            return new BoundingBox(Positions[item] - BoundOffset, Positions[item] + BoundOffset);
        }
        /// <summary>
        /// <see cref="OctreeBase{T}.FindNearestPointBySphereExcludeChild(IRenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var resultTemp = new HitTestResult();
            resultTemp.Distance = float.MaxValue;
            var containment = Bound.Contains(ref sphere);
            if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
            {
                isIntersect = true;
                foreach (var t in Objects)
                {
                    var p = Positions[t];
                    containment = sphere.Contains(ref p);
                    if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                    {
                        var d = (p - sphere.Center).Length();
                        if (resultTemp.Distance > d)
                        {
                            resultTemp.Distance = d;
                            resultTemp.IsValid = true;
                            resultTemp.PointHit = p;
                            resultTemp.Tag = t;
                            isHit = true;
                        }
                    }
                }
                if (isHit)
                {
                    isHit = false;
                    if (result.Count > 0)
                    {
                        if (result[0].Distance > resultTemp.Distance)
                        {
                            result[0] = resultTemp;
                            isHit = true;
                        }
                    }
                    else
                    {
                        result.Add(resultTemp);
                        isHit = true;
                    }
                }
            }
            else
            {
                isIntersect = false;
            }
            return isHit;
        }
    }

    /// <summary>
    /// Octree for instancing
    /// </summary>
    public class InstancingModel3DOctree : OctreeBase<Tuple<int, BoundingBox>>
    {
        private IList<Matrix> InstanceMatrix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceMatrix"></param>
        /// <param name="geometryBound"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public InstancingModel3DOctree(IList<Matrix> instanceMatrix, BoundingBox geometryBound, OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache = null)
            : base(ref geometryBound, null, parameter, stackCache)
        {
            InstanceMatrix = instanceMatrix;
            int counter = 0;
            var totalBound = geometryBound.Transform(instanceMatrix[0]);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, instanceMatrix[0])).ToArray());
            foreach (var m in instanceMatrix)
            {
                var b = geometryBound.Transform(m);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, m)).ToArray());
                Objects.Add(new Tuple<int, BoundingBox>(counter, b));
                BoundingBox.Merge(ref totalBound, ref b, out totalBound);
                ++counter;
            }
            this.Bound = totalBound;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="instanceMatrix"></param>
        /// <param name="objects"></param>
        /// <param name="parent"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        protected InstancingModel3DOctree(ref BoundingBox bound, IList<Matrix> instanceMatrix, List<Tuple<int, BoundingBox>> objects, IOctree parent, OctreeBuildParameter parameter, Stack<IEnumerator<IOctree>> stackCache = null)
            : base(ref bound, objects, parent, parameter, stackCache)
        {
            InstanceMatrix = instanceMatrix;
        }
        /// <summary>
        /// <see cref="OctreeBase{T}.FindNearestPointBySphereExcludeChild(IRenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// <see cref="OctreeBase{T}.HitTestCurrentNodeExcludeChild(IRenderContext, IRenderable, Matrix, ref Ray, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, IRenderable model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            bool isHit = false;
            var bound = Bound.Transform(modelMatrix);// BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    var b = t.Item2.Transform(modelMatrix);// BoundingBox.FromPoints(t.Item2.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
                    if (b.Intersects(ref rayWS))
                    {
                        var result = new HitTestResult()
                        {
                            Tag = t.Item1
                        };
                        hits.Add(result);
                        isHit = true;
                    }
                }
            }
            return isHit;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IOctree CreateNodeWithParent(ref BoundingBox bound, List<Tuple<int, BoundingBox>> objList, IOctree parent)
        {
            return new InstancingModel3DOctree(ref bound, this.InstanceMatrix, objList, parent, this.Parameter, this.stack);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override BoundingBox GetBoundingBoxFromItem(Tuple<int, BoundingBox> item)
        {
            return item.Item2;
        }
    }
}
