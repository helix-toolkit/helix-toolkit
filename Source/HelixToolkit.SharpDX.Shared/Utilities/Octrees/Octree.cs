/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Model;
    /// <summary>
    /// Base class template implementation for <see cref="IDynamicOctree"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DynamicOctreeBase<T> : IDynamicOctree
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objects"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public delegate IDynamicOctree CreateNodeDelegate(ref BoundingBox bound, List<T> objects, IDynamicOctree parent);
        /// <summary>
        /// internal stack for tree traversal
        /// </summary>
        protected readonly Stack<KeyValuePair<int, IDynamicOctree[]>> stack;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> Hit;
        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public float MIN_SIZE { get { return Parameter.MinimumOctantSize; } }
        /// <summary>
        /// <see cref="IOctreeBasic.TreeBuilt"/>
        /// </summary>
        public bool TreeBuilt { get { return treeBuilt; } }
        /// <summary>
        /// 
        /// </summary>
        protected bool treeBuilt = false;       //there is no pre-existing tree yet.
        /// <summary>
        /// <see cref="IOctreeBasic.Parameter"/>
        /// </summary>
        public OctreeBuildParameter Parameter { private set; get; }

        private BoundingBox bound;

        /// <summary>
        /// <see cref="IOctreeBasic.Bound"/>
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
        /// <see cref="DynamicOctreeBase{T}.Objects"/>
        /// </summary>
        public List<T> Objects { protected set; get; }

        private readonly List<BoundingBox> hitPathBoundingBoxes = new List<BoundingBox>();
        /// <summary>
        /// <see cref="IOctreeBasic.HitPathBoundingBoxes"/>
        /// </summary>
        public IList<BoundingBox> HitPathBoundingBoxes { get { return hitPathBoundingBoxes.AsReadOnly(); } }
        /// <summary>
        /// These are all of the possible child octants for this node in the tree.
        /// </summary>
        private readonly IDynamicOctree[] childNodes = new IDynamicOctree[8];
        /// <summary>
        /// <see cref="IDynamicOctree.ChildNodes"/>
        /// </summary>
        public IDynamicOctree[] ChildNodes { get { return childNodes; } }
        /// <summary>
        /// <see cref="IDynamicOctree.ActiveNodes"/>
        /// </summary>
        public byte ActiveNodes { set; get; }
        /// <summary>
        /// <see cref="IDynamicOctree.Parent"/>
        /// </summary>
        public IDynamicOctree Parent { set; get; }

        private BoundingBox[] octants = null;
        /// <summary>
        /// <see cref="IDynamicOctree.Octants"/>
        /// </summary>
        public BoundingBox[] Octants { get { return octants; } }
        /// <summary>
        /// 
        /// </summary>
        protected List<HitTestResult> modelHits = new List<HitTestResult>();
        /// <summary>
        /// Gets the self array.
        /// </summary>
        /// <value>
        /// The self array.
        /// </value>
        public IDynamicOctree[] SelfArray { get; private set; }
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

        private DynamicOctreeBase(OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
        {
            SelfArray = new IDynamicOctree[] { this };
            stack = stackCache ?? new Stack<KeyValuePair<int, IDynamicOctree[]>>(64);
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
        protected DynamicOctreeBase(ref BoundingBox bound, List<T> objList, IDynamicOctree parent, OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        protected DynamicOctreeBase(IDynamicOctree parent, OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        protected DynamicOctreeBase(ref BoundingBox bound, IDynamicOctree parent, OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
            : this(parent, parameter, stackCache)
        {
            Bound = bound;
        }

        private IDynamicOctree CreateNode(ref BoundingBox bound, List<T> objList)
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
        protected abstract IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<T> objList, IDynamicOctree parent);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected IDynamicOctree CreateNode(ref BoundingBox bound, T Item)
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
        public void BuildTree(IDynamicOctree root, Stack<KeyValuePair<int, IDynamicOctree[]>> stack)
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
        public static void TreeTraversal(IDynamicOctree root, Stack<KeyValuePair<int, IDynamicOctree[]>> stack, Func<IDynamicOctree, bool> criteria, Action<IDynamicOctree> process,
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
                            if (subTree == null) { return; }
                            TreeTraversal(subTree, new Stack<KeyValuePair<int, IDynamicOctree[]>>(), criteria, process, breakCriteria, false);
                        });
                    }
                }
            }
            else
            {
                int i = -1;
                IDynamicOctree[] treeArray = root.SelfArray;
                while (true)
                {
                    while (++i < treeArray.Length)
                    {
                        var tree = treeArray[i];
                        if (tree != null && (criteria == null || criteria(tree)))
                        {
                            process(tree);
                            if (breakCriteria != null && breakCriteria())
                            {
                                break;
                            }
                            if (tree.HasChildren)
                            {
                                stack.Push(new KeyValuePair<int, IDynamicOctree[]>(i, treeArray));
                                treeArray = tree.ChildNodes;
                                i = -1;
                            }
                        }
                    }
                    if (stack.Count == 0) { break; }
                    var pair = stack.Pop();
                    i = pair.Key;
                    treeArray = pair.Value;
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
                    if (IsContains(Octants[x], obj))
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
            for (int i = 0; i < Objects.Count; ++i)
            {
                var bound = GetBoundingBoxFromItem(Objects[i]);
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
        /// <see cref="IDynamicOctree.Clear"/>
        /// </summary>
        public virtual void Clear()
        {
            Objects.Clear();
            for (int i = 0; i < ChildNodes.Length; ++i)
            {
                ChildNodes[i]?.Clear();
            }
            Array.Clear(ChildNodes, 0, ChildNodes.Length);
        }
        /// <summary>
        /// <see cref="IOctreeBasic.HitTest(RenderContext, object, Geometry3D, Matrix, Ray, ref List{HitTestResult})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public bool HitTest(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
            return HitTest(context, model, geometry, modelMatrix, rayWS, ref hits, 0);
        }
        /// <summary>
        /// <see cref="IOctreeBasic.HitTest(RenderContext, object, Geometry3D, Matrix, Ray, ref List{HitTestResult}, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public virtual bool HitTest(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness)
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
            if (modelInv == Matrix.Zero) { return false; }//Cannot be inverted
            var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInv), Vector3.Normalize(Vector3.TransformNormal(rayWS.Direction, modelInv)));
            var treeArray = SelfArray;
            int i = -1;
            while (true)
            {
                while (++i < treeArray.Length)
                {
                    var node = treeArray[i];
                    if (node == null) { continue; }
                    bool isIntersect = false;
                    bool nodeHit = node.HitTestCurrentNodeExcludeChild(context, model, geometry, modelMatrix, ref rayWS, ref rayModel, ref modelHits, ref isIntersect, hitThickness);
                    isHit |= nodeHit;
                    if (isIntersect && node.HasChildren)
                    {
                        hitStack.Push(new KeyValuePair<int, IDynamicOctree[]>(i, treeArray));
                        treeArray = node.ChildNodes;
                        i = -1;
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
                var pair = hitStack.Pop();
                i = pair.Key;
                treeArray = pair.Value;
            }
            if (!isHit)
            {
                hitPathBoundingBoxes.Clear();
            }
            else
            {
                hits.AddRange(modelHits);
                Hit?.Invoke(this, EventArgs.Empty);
            }
            return isHit;
        }

        /// <summary>
        /// Hit test for current node.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public abstract bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public virtual bool FindNearestPointBySphere(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points)
        {
            if (points == null)
            {
                points = new List<HitTestResult>();
            }
            var hitStack = stack;
            bool isHit = false;
            var treeArray = SelfArray;
            int i = -1;
            while (true)
            {
                while (++i < treeArray.Length)
                {
                    var node = treeArray[i];
                    if (node == null) { continue; }
                    bool isIntersect = false;
                    bool nodeHit = node.FindNearestPointBySphereExcludeChild(context, ref sphere, ref points, ref isIntersect);
                    isHit |= nodeHit;
                    if (isIntersect && node.HasChildren)
                    {
                        hitStack.Push(new KeyValuePair<int, IDynamicOctree[]>(i, treeArray));
                        treeArray = node.ChildNodes;
                        i = -1;
                    }
                }
                if (hitStack.Count == 0) { break; }
                var pair = hitStack.Pop();
                i = pair.Key;
                treeArray = pair.Value;
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
        public virtual bool FindNearestPointFromPoint(RenderContext context, ref Vector3 point, ref List<HitTestResult> results, float heuristicSearchFactor = 1f)
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
            var treeArray = SelfArray;
            int i = -1;
            while (true)
            {
                while (++i < treeArray.Length)
                {
                    var node = treeArray[i];
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
                            hitStack.Push(new KeyValuePair<int, IDynamicOctree[]>(i, treeArray));
                            treeArray = node.ChildNodes;
                            i = -1;
                        }
                    }
                }
                if (hitStack.Count == 0) { break; }
                var pair = hitStack.Pop();
                i = pair.Key;
                treeArray = pair.Value;
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
        public abstract bool FindNearestPointBySphereExcludeChild(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect);
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.Add(T)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            IDynamicOctree octant;
            return Add(item, out octant);
        }
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.Add(T, out IDynamicOctree)"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="octant"></param>
        /// <returns></returns>
        public virtual bool Add(T item, out IDynamicOctree octant)
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
                var nodeBase = node as DynamicOctreeBase<T>;
                nodeBase.Objects.Add(item);
                if (nodeBase.Objects.Count > Parameter.MinObjectSizeToSplit)
                {
                    int index = (node as DynamicOctreeBase<T>).Objects.Count - 1;
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
            IDynamicOctree octant;
            return PushExistingToChild(index, out octant);
        }
        /// <summary>
        /// Push one of object belongs to current node into its child octant
        /// </summary>
        /// <param name="index"></param>
        /// <param name="octant"></param>
        /// <returns></returns>
        public virtual bool PushExistingToChild(int index, out IDynamicOctree octant)
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
        public static bool PushExistingToChild(DynamicOctreeBase<T> node, int index, Func<BoundingBox, T, bool> isContains,
            CreateNodeDelegate createNodeFunc, out IDynamicOctree octant)
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
                        (node.ChildNodes[i] as DynamicOctreeBase<T>).Objects.Add(item);
                        octant = node.ChildNodes[i];
                    }
                    else
                    {
                        node.ChildNodes[i] = createNodeFunc(ref node.Octants[i], new List<T>() { item }, node);
                        node.ActiveNodes |= (byte)(1 << i);
                        node.ChildNodes[i].BuildTree();
                        int idx = -1;
                        octant = (node.ChildNodes[i] as DynamicOctreeBase<T>).FindChildByItemBound(item, out idx);
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
        public virtual IDynamicOctree Expand(ref Vector3 direction)
        {
            return Expand(this, ref direction, CreateNodeWithParent);
        }

        private readonly static Vector3 epsilon = new Vector3(float.Epsilon, float.Epsilon, float.Epsilon);

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
        public static IDynamicOctree Expand(IDynamicOctree oldRoot, ref Vector3 direction, CreateNodeDelegate createNodeFunc)
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
        public virtual IDynamicOctree Shrink()
        {
            return Shrink(this);
        }
        /// <summary>
        /// Shrink the root bound to contains all items inside
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IDynamicOctree Shrink(IDynamicOctree root)
        {
            if (root.Parent != null)
            { throw new ArgumentException("Input node is not a root node."); }
            if (root.IsEmpty)
            {
                return root;
            }
            else if ((root as DynamicOctreeBase<T>).Objects.Count == 0 && (root.ActiveNodes & (root.ActiveNodes - 1)) == 0)
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
        public IDynamicOctree FindSmallestNodeContainsBoundingBox(ref BoundingBox bound, T item)
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
        private static IDynamicOctree FindSmallestNodeContainsBoundingBox<E>(BoundingBox bound, E item, Func<BoundingBox, E, bool> isContains, DynamicOctreeBase<E> root,
            Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
        {
            IDynamicOctree result = null;
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
        public IDynamicOctree FindChildByItem(T item, out int index)
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
        public static IDynamicOctree FindChildByItem<E>(E item, DynamicOctreeBase<E> root, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache, out int index)
        {
            IDynamicOctree result = null;
            int idx = -1;
            TreeTraversal(root, stackCache, null,
                (node) =>
                {
                    idx = (node as DynamicOctreeBase<E>).Objects.IndexOf(item);
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
                var nodeBase = node as DynamicOctreeBase<T>;
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
                (node as DynamicOctreeBase<T>).Objects.RemoveAt(index);
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
        public void RemoveChild(IDynamicOctree child)
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
        public virtual IDynamicOctree FindChildByItemBound(T item, out int index)
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
        public virtual IDynamicOctree FindChildByItemBound(T item, ref BoundingBox bound, out int index)
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
        public static IDynamicOctree FindChildByItemBound<E>(E item, BoundingBox bound, Func<BoundingBox, BoundingBox, E, bool> isContains, DynamicOctreeBase<E> root, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache, out int index)
        {
            int idx = -1;
            IDynamicOctree result = null;
            DynamicOctreeBase<E> lastNode = null;
            TreeTraversal(root, stackCache,
                (node) => { return isContains(node.Bound, bound, item); },
                (node) =>
                {
                    lastNode = node as DynamicOctreeBase<E>;
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
                        lastNode = lastNode.Parent as DynamicOctreeBase<E>;
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
        public static IDynamicOctree FindRoot(IDynamicOctree node)
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
        public bool FindNearestPointByPointAndSearchRadius(RenderContext context, ref Vector3 point, float radius, ref List<HitTestResult> result)
        {
            var sphere = new global::SharpDX.BoundingSphere(point, radius);
            return FindNearestPointBySphere(context, ref sphere, ref result);
        }

        #region Accessors
        /// <summary>
        /// <see cref="IDynamicOctree.IsRoot"/>
        /// </summary>
        public bool IsRoot
        {
            //The root node is the only node without a parent.
            get { return Parent == null; }
        }
        /// <summary>
        /// <see cref="IDynamicOctree.HasChildren"/>
        /// </summary>
        public bool HasChildren
        {
            get
            {
                return ActiveNodes != 0;
            }
        }
        /// <summary>
        /// <see cref="IDynamicOctree.IsEmpty"/>
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return !HasChildren && Objects.Count == 0;
            }
        }
        #endregion

        public LineGeometry3D CreateOctreeLineModel()
        {
            return OctreeHelper.CreateOctreeLineModel(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class OctreeBuildParameter : ObservableObject
    {
        private float minimumOctantSize = 1f;
        /// <summary>
        /// Minimum Octant size.
        /// </summary>
        public float MinimumOctantSize
        {
            set
            {
                Set(ref minimumOctantSize, value);
            }
            get { return minimumOctantSize; }
        }

        private int minObjectSizeToSplit = 2;
        /// <summary>
        /// Minimum object in each octant to start splitting into smaller octant during build
        /// </summary>
        public int MinObjectSizeToSplit
        {
            set { Set(ref minObjectSizeToSplit, value); }
            get { return minObjectSizeToSplit; }
        }

        private bool autoDeleteIfEmpty = true;
        /// <summary>
        /// Delete empty octant automatically
        /// </summary>
        public bool AutoDeleteIfEmpty
        {
            set { Set(ref autoDeleteIfEmpty, value); }
            get { return autoDeleteIfEmpty; }
        }

        private bool cubify = false;
        /// <summary>
        /// Generate cube octants instead of rectangle octants
        /// </summary>
        public bool Cubify
        {
            set { Set(ref cubify, value); }
            get { return cubify; }
        }
        /// <summary>
        /// Record hit path bounding boxes for debugging or display purpose only
        /// </summary>
        public bool RecordHitPathBoundingBoxes { set; get; } = false;
        /// <summary>
        /// Use parallel tree traversal to build the octree
        /// </summary>
        public bool EnableParallelBuild { set; get; } = false;
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
    /// MeshGeometryOctree slices mesh geometry by triangles into octree. Objects are KeyValuePair of each triangle index and its bounding box.
    /// </summary>
    [Obsolete("Please use StaticMeshGeometryOctree for better performance")]
    public class MeshGeometryOctree
        : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
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
        public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
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
            OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
            : base(null, parameter, stackCache)
        {
            Positions = positions;
            Indices = indices;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = new List<KeyValuePair<int, BoundingBox>>(indices.Count / 3);
            // Construct triangle index and its bounding box KeyValuePair
            for (int i = 0; i < indices.Count / 3; ++i)
            {
                Objects.Add(new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i)));
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
        protected MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> triIndex,
            IDynamicOctree parent, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        protected MeshGeometryOctree(BoundingBox bound, List<KeyValuePair<int, BoundingBox>> list, IDynamicOctree parent, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
        {
            return item.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox region, List<KeyValuePair<int, BoundingBox>> objList, IDynamicOctree parent)
        {
            return new MeshGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.stack);
        }
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.HitTestCurrentNodeExcludeChild(RenderContext, object, Geometry3D, Matrix, ref Ray, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS,
            ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            var isHit = false;
            var bound = Bound;
            //Hit test in local space.
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                if (Objects.Count == 0)
                {
                    return false;
                }
                var result = new HitTestResult();
                result.Distance = double.MaxValue;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    var idx = Objects[i].Key * 3;
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
                            result.TriangleIndices = new Tuple<int, int, int>(t1, t2, t3);
                            result.Tag = Objects[i].Key;
                            result.Geometry = geometry;
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
        /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(RenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var containment = Bound.Contains(ref sphere);
            if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
            {
                isIntersect = true;
                if (Objects.Count == 0)
                {
                    return false;
                }
                var tempResult = new HitTestResult();
                tempResult.Distance = float.MaxValue;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    containment = Objects[i].Value.Contains(sphere);
                    if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                    {
                        Vector3 cloestPoint;

                        var idx = Objects[i].Key * 3;
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
                            tempResult.Tag = Objects[i].Key;
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
    /// 
    /// </summary>
    [Obsolete("Please use StaticLineGeometryOctree for better performance")]
    public class LineGeometryOctree : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
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
        public LineGeometryOctree(IList<Vector3> positions, IList<int> indices, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
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
        public LineGeometryOctree(IList<Vector3> positions, IList<int> indices,
            OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
            : base(null, parameter, stackCache)
        {
            Positions = positions;
            Indices = indices;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = new List<KeyValuePair<int, BoundingBox>>(indices.Count / 2);
            // Construct triangle index and its bounding box KeyValuePair
            for (int i = 0; i < indices.Count / 2; ++i)
            {
                Objects.Add(new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i)));
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
        protected LineGeometryOctree(IList<Vector3> positions, IList<int> indices, ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> triIndex,
            IDynamicOctree parent, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        protected LineGeometryOctree(BoundingBox bound, List<KeyValuePair<int, BoundingBox>> list, IDynamicOctree parent, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
            : base(ref bound, list, parent, paramter, stackCache)
        { }

        private BoundingBox GetBoundingBox(int triangleIndex)
        {
            var actual = triangleIndex * 2;
            var v1 = Positions[Indices[actual++]];
            var v2 = Positions[Indices[actual]];
            var maxX = Math.Max(v1.X, v2.X);
            var maxY = Math.Max(v1.Y, v2.Y);
            var maxZ = Math.Max(v1.Z, v2.Z);

            var minX = Math.Min(v1.X, v2.X);
            var minY = Math.Min(v1.Y, v2.Y);
            var minZ = Math.Min(v1.Z, v2.Z);

            return new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
        {
            return item.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="region"></param>
        /// <param name="objList"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox region, List<KeyValuePair<int, BoundingBox>> objList, IDynamicOctree parent)
        {
            return new LineGeometryOctree(Positions, Indices, ref region, objList, parent, parent.Parameter, this.stack);
        }
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.HitTestCurrentNodeExcludeChild(RenderContext, object, Geometry3D, Matrix, ref Ray, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS,
            ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            var isHit = false;

            var bound = Bound;
            bound.Maximum += new Vector3(hitThickness);
            bound.Minimum -= new Vector3(hitThickness);
            var lastDist = double.MaxValue;
            //Hit test in local space.
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                if (Objects.Count == 0)
                {
                    return false;
                }
                var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
                result.Distance = double.MaxValue;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    var idx = Objects[i].Key * 2;
                    var idx1 = Indices[idx];
                    var idx2 = Indices[idx + 1];
                    var v0 = Positions[idx1];
                    var v1 = Positions[idx2];

                    var t0 = Vector3.TransformCoordinate(v0, modelMatrix);
                    var t1 = Vector3.TransformCoordinate(v1, modelMatrix);
                    Vector3 sp, tp;
                    float sc, tc;
                    var rayToLineDistance = LineBuilder.GetRayToLineDistance(rayWS, t0, t1, out sp, out tp, out sc, out tc);
                    var svpm = context.ScreenViewProjectionMatrix;
                    Vector4 sp4;
                    Vector4 tp4;
                    Vector3.Transform(ref sp, ref svpm, out sp4);
                    Vector3.Transform(ref tp, ref svpm, out tp4);
                    var sp3 = sp4.ToVector3();
                    var tp3 = tp4.ToVector3();
                    var tv2 = new Vector2(tp3.X - sp3.X, tp3.Y - sp3.Y);
                    var dist = tv2.Length();
                    if (dist < lastDist && dist <= hitThickness)
                    {
                        lastDist = dist;
                        result.PointHit = sp;
                        result.NormalAtHit = sp - tp; // not normalized to get length
                        result.Distance = (rayWS.Position - sp).Length();
                        result.RayToLineDistance = rayToLineDistance;
                        result.ModelHit = model;
                        result.IsValid = true;
                        result.Tag = Objects[i].Key; // For compatibility
                        result.LineIndex = Objects[i].Key;
                        result.TriangleIndices = null; // Since triangles are shader-generated
                        result.RayHitPointScalar = sc;
                        result.LineHitPointScalar = tc;
                        result.Geometry = geometry;
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
        /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(RenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var containment = Bound.Contains(ref sphere);
            if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
            {
                isIntersect = true;
                if (Objects.Count == 0)
                {
                    return false;
                }
                var tempResult = new LineHitTestResult();
                tempResult.Distance = float.MaxValue;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    containment = Objects[i].Value.Contains(sphere);
                    if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                    {
                        Vector3 cloestPoint;

                        var idx = Objects[i].Key * 3;
                        var t1 = Indices[idx];
                        var t2 = Indices[idx + 1];
                        var v0 = Positions[t1];
                        var v1 = Positions[t2];
                        float t;
                        float distance = LineBuilder.GetPointToLineDistance2D(ref sphere.Center, ref v0, ref v1, out cloestPoint, out t);
                        if (tempResult.Distance > distance)
                        {
                            tempResult.Distance = distance;
                            tempResult.IsValid = true;
                            tempResult.PointHit = cloestPoint;
                            tempResult.TriangleIndices = null;
                            tempResult.RayHitPointScalar = t;
                            tempResult.Tag = tempResult.LineIndex = Objects[i].Key;
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
    [Obsolete("Please use StaticPointGeometryOctree for better performance")]
    public class PointGeometryOctree : DynamicOctreeBase<int>
    {
        private IList<Vector3> Positions;
        private static readonly Vector3 BoundOffset = new Vector3(0.0001f);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="stackCache"></param>
        public PointGeometryOctree(IList<Vector3> positions, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
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
            OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
               : base(null, parameter, stackCache)
        {
            Positions = positions;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = new List<int>(Positions.Count);
            for(int i=0; i < Positions.Count; ++i)
            {
                Objects.Add(i);
            }
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
        protected PointGeometryOctree(BoundingBox bound, IList<Vector3> positions, List<int> list, IDynamicOctree parent, OctreeBuildParameter paramter,
            Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache)
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
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="context"></param>
        /// <param name="rayModel"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix,
            ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt || context == null)
            {
                return false;
            }
            var isHit = false;

            var bound = Bound;

            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                if(Objects.Count == 0)
                {
                    return false;
                }
                var result = new HitTestResult();
                result.Distance = double.MaxValue;
                var svpm = context.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;
                var clickPoint3 = rayWS.Position + rayWS.Direction;
                var pos3 = rayWS.Position;
                Vector3.TransformCoordinate(ref clickPoint3, ref svpm, out var clickPoint);
                Vector3.TransformCoordinate(ref pos3, ref svpm, out pos3);
                
                var dist = hitThickness;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    var v0 = Positions[Objects[i]];
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
                        result.Tag = Objects[i];
                        result.Geometry = geometry;
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
        protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<int> objList, IDynamicOctree parent)
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
        /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(RenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var containment = Bound.Contains(ref sphere);
            if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
            {
                isIntersect = true;
                if (Objects.Count == 0)
                {
                    return false;
                }
                var resultTemp = new HitTestResult();
                resultTemp.Distance = float.MaxValue;
                for (int i = 0; i < Objects.Count; ++i)
                {
                    var p = Positions[Objects[i]];
                    containment = sphere.Contains(ref p);
                    if (containment == ContainmentType.Contains || containment == ContainmentType.Intersects)
                    {
                        var d = (p - sphere.Center).Length();
                        if (resultTemp.Distance > d)
                        {
                            resultTemp.Distance = d;
                            resultTemp.IsValid = true;
                            resultTemp.PointHit = p;
                            resultTemp.Tag = Objects[i];
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
    [Obsolete("Please use StaticInstancingOctree for better performance")]
    public class InstancingModel3DOctree : DynamicOctreeBase<KeyValuePair<int, BoundingBox>>
    {
        private IList<Matrix> InstanceMatrix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceMatrix"></param>
        /// <param name="geometryBound"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public InstancingModel3DOctree(IList<Matrix> instanceMatrix, BoundingBox geometryBound, OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
            : base(ref geometryBound, null, parameter, stackCache)
        {
            InstanceMatrix = instanceMatrix;
            int counter = 0;
            var totalBound = geometryBound.Transform(instanceMatrix[0]);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, instanceMatrix[0])).ToArray());
            for (int i = 0; i < instanceMatrix.Count; ++i)
            {
                var b = geometryBound.Transform(instanceMatrix[i]);// BoundingBox.FromPoints(geometryBound.GetCorners().Select(x => Vector3.TransformCoordinate(x, m)).ToArray());
                Objects.Add(new KeyValuePair<int, BoundingBox>(counter, b));
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
        protected InstancingModel3DOctree(ref BoundingBox bound, IList<Matrix> instanceMatrix, List<KeyValuePair<int, BoundingBox>> objects, IDynamicOctree parent, OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
            : base(ref bound, objects, parent, parameter, stackCache)
        {
            InstanceMatrix = instanceMatrix;
        }
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.FindNearestPointBySphereExcludeChild(RenderContext, ref global::SharpDX.BoundingSphere, ref List{HitTestResult}, ref bool)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        public override bool FindNearestPointBySphereExcludeChild(RenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            return false;
        }
        /// <summary>
        /// <see cref="DynamicOctreeBase{T}.HitTestCurrentNodeExcludeChild(RenderContext, object, Geometry3D, Matrix, ref Ray, ref Ray, ref List{HitTestResult}, ref bool, float)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="geometry"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
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
                for (int i = 0; i < this.Objects.Count; ++i)
                {
                    var b = Objects[i].Value.Transform(modelMatrix);// BoundingBox.FromPoints(t.Item2.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
                    if (b.Intersects(ref rayWS))
                    {
                        var result = new HitTestResult()
                        {
                            Tag = Objects[i].Key
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
        protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<KeyValuePair<int, BoundingBox>> objList, IDynamicOctree parent)
        {
            return new InstancingModel3DOctree(ref bound, this.InstanceMatrix, objList, parent, this.Parameter, this.stack);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
        {
            return item.Value;
        }
    }
}
