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

namespace HelixToolkit.SharpDX.Shared.Utilities
{
    public interface IOctree
    {
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        byte ActiveNodes { set; get; }
        bool HasChildren { get; }
        bool IsRoot { get; }
        IOctree Parent { get; }
        BoundingBox Bound { get; }
        IOctree[] ChildNodes { get; }

        bool RecordHitPathBoundingBoxes { set; get; }
        IList<BoundingBox> HitPathBoundingBoxes { get; }
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
        void RemoveChild(IOctree child);

        /// <summary>
        /// Update bounding box. Returns true if bounding box has been changed. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        bool UpdateBoundingBox();
    }

    public interface IOctreeBase<T> : IOctree
    {
        List<T> Objects { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        bool Add(T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);
    }

    public abstract class OctreeBase<T> : IOctreeBase<T>
    {
        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public readonly int MIN_SIZE = 1;

        public BoundingBox Bound { protected set; get; }

        public List<T> Objects { protected set; get; }

        public bool RecordHitPathBoundingBoxes { set; get; } = false;
        private readonly List<BoundingBox> hitPathBoundingBoxes = new List<BoundingBox>();
        public IList<BoundingBox> HitPathBoundingBoxes { get { return hitPathBoundingBoxes.AsReadOnly(); } }
        /// <summary>
        /// These are all of the possible child octants for this node in the tree.
        /// </summary>
        private readonly IOctree[] childNodes = new IOctree[8];
        public IOctree[] ChildNodes { get { return childNodes; } }

        public byte ActiveNodes { set; get; }

        public IOctree Parent { protected set; get; }

        protected bool treeReady = false;       //the tree has a few objects which need to be inserted before it is complete
        protected bool treeBuilt = false;       //there is no pre-existing tree yet.

        /*Note: we want to avoid allocating memory for as long as possible since there can be lots of nodes.*/
        /// <summary>
        /// Creates an oct tree which encloses the given region and contains the provided objects.
        /// </summary>
        /// <param name="bound">The bounding region for the oct tree.</param>
        /// <param name="objList">The list of objects contained within the bounding region</param>
        protected OctreeBase(BoundingBox bound, List<T> objList, IOctree parent, int minSize)
        {
            Bound = bound;
            Objects = objList;
            Parent = parent;
            MIN_SIZE = minSize;
        }

        protected OctreeBase(IOctree parent, int minSize)
        {
            Objects = new List<T>();
            Bound = new BoundingBox(Vector3.Zero, Vector3.Zero);
            Parent = parent;
            MIN_SIZE = minSize;
        }

        /// <summary>
        /// Creates an octTree with a suggestion for the bounding region containing the items.
        /// </summary>
        /// <param name="bound">The suggested dimensions for the bounding region. 
        /// Note: if items are outside this region, the region will be automatically resized.</param>
        protected OctreeBase(BoundingBox bound, IOctree parent, int minSize)
            : this(parent, minSize)
        {
            Bound = bound;
        }

        private IOctree CreateNode(BoundingBox bound, List<T> objList)
        {
            return CreateNodeWithParent(bound, objList, this);
        }

        protected abstract IOctree CreateNodeWithParent(BoundingBox bound, List<T> objList, IOctree parent);

        protected IOctree CreateNode(BoundingBox bound, T Item)
        {
            return CreateNode(bound, new List<T> { Item });
        }

        public void BuildTree()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            var queue = new Queue<IOctree>(256);
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                var tree = queue.Dequeue();
                tree.BuildCurretNodeOnly();
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
#if DEBUG
            sw.Stop();
            Debug.WriteLine("Buildtree time =" + sw.ElapsedMilliseconds);
#endif
        }

        public void BuildCurretNodeOnly()
        {
            /*I think I can just directly insert items into the tree instead of using a queue.*/
            if (!treeBuilt)
            {            //terminate the recursion if we're a leaf node
                if (Objects.Count <= 1)   //doubt: is this really right? needs testing.
                {
                    treeBuilt = true;
                    treeReady = true;
                    return;
                }
                BuildSubTree();
                treeBuilt = true;
                treeReady = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BoundingBox[] CreateOctants(BoundingBox box)
        {
            Vector3 dimensions = box.Maximum - box.Minimum;

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
        private bool CheckDimension()
        {
            Vector3 dimensions = Bound.Maximum - Bound.Minimum;

            if (dimensions == Vector3.Zero)
            {
                FindEnclosingCube();
                dimensions = Bound.Maximum - Bound.Minimum;
            }

            //Check to see if the dimensions of the box are greater than the minimum dimensions
            if (dimensions.X <= MIN_SIZE && dimensions.Y <= MIN_SIZE && dimensions.Z <= MIN_SIZE)
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
            if (!CheckDimension())
            {
                treeBuilt = true;
                treeReady = true;
                return;
            }

            //Create subdivided regions for each octant
            var octants = CreateOctants(Bound);

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
                        if (octants[x].Contains(box) == ContainmentType.Contains)
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
                    ChildNodes[i] = CreateNode(octants[i], octList[i]);
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
        protected virtual void FindEnclosingBox()
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
            Bound = new BoundingBox(global_min, global_max);
        }
        /// <summary>
        /// This finds the smallest enclosing cube which is a power of 2, for all objects in the list.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void FindEnclosingCube()
        {
            FindEnclosingBox();

            //find the min offset from (0,0,0) and translate by it for a short while
            Vector3 offset = Bound.Minimum - Vector3.Zero;
            Bound = new BoundingBox(Bound.Minimum + offset, Bound.Maximum + offset);

            //find the nearest power of two for the max values
            int highX = (int)Math.Floor(Math.Max(Math.Max(Bound.Maximum.X, Bound.Maximum.Y), Bound.Maximum.Z));

            //see if we're already at a power of 2
            for (int bit = 0; bit < 32; bit++)
            {
                if (highX == 1 << bit)
                {
                    Bound = new BoundingBox(Bound.Minimum - offset, new Vector3(highX, highX, highX) - offset);
                    return;
                }
            }

            //gets the most significant bit value, so that we essentially do a Ceiling(X) with the 
            //ceiling result being to the nearest power of 2 rather than the nearest integer.
            int x = SigBit(highX);

            Bound = new BoundingBox(Bound.Minimum - offset, new Vector3(x, x, x) - offset);
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
            var hitQueue = new Queue<IOctree>(256);
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
                if (RecordHitPathBoundingBoxes && nodeHit)
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
            return isHit;
        }

        public abstract bool HitTestCurrentNodeExcludeChild(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, ref bool isIntersect);

        public bool Add(T item)
        {
            var bound = GetBoundingBoxFromItem(item);
            var node = FindSmallestNodeContainsBoundingBox(bound);
            if (node == null)
            {
                return false;
            }
            else
            {
                var octants = CreateOctants(node.Bound);
                bool pushToChild = false;
                for(int i = 0; i < octants.Length; ++i)
                {
                    if(octants[i].Contains(bound)== ContainmentType.Contains)
                    {
                        if (node.ChildNodes[i] != null)
                        {
                            (node.ChildNodes[i] as IOctreeBase<T>).Add(item);
                        }
                        else
                        {                            
                            node.ChildNodes[i] = CreateNodeWithParent(octants[i], new List<T>() { item }, node);
                            node.ActiveNodes |= (byte)(1 << i);
                            node.ChildNodes[i].BuildTree();
                        }
                        pushToChild = true;
                        break;
                    }
                }
                if (!pushToChild)
                {
                    (node as IOctreeBase<T>).Objects.Add(item);
                }
                return true;
            }
        }       

        public IOctree FindSmallestNodeContainsBoundingBox(BoundingBox bound)
        {
            var queue = new Queue<IOctreeBase<T>>(64);
            queue.Enqueue(this);
            IOctree result = null;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Bound.Contains(bound) != ContainmentType.Contains)
                {
                    continue;
                }
                else
                {
                    result = node;
                    foreach (var child in node.ChildNodes)
                    {
                        if (child != null)
                        {
                            queue.Enqueue(child as IOctreeBase<T>);
                        }
                    }
                }
            }
            return result;
        }

        public void Remove(T item)
        {
            var node = FindChildByItem(item);
            if (node == null)
            {
                return;
            }
            else
            {
                var nodeBase = node as IOctreeBase<T>;
                nodeBase.Objects.Remove(item);
                if (nodeBase.IsEmpty)
                {
                    nodeBase.RemoveSelf();
                }
            }
        }

        public virtual void RemoveSelf()
        {
            if (Parent == null)
            { return; }

            Clear();
            Parent.RemoveChild(this);
            UpdateParentBoundingBoxToRoot();
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
            if (IsEmpty)
            {
                RemoveSelf();
            }
        }

        protected virtual IOctree FindChildByItem(T item)
        {
            var bound = GetBoundingBoxFromItem(item);
            var queue = new Queue<IOctreeBase<T>>(64);
            queue.Enqueue(this);
            IOctree result = null;
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Bound.Contains(bound) != ContainmentType.Contains)
                {
                    continue;
                }
                var found = node.Objects.Where(x => x.Equals(item)).FirstOrDefault();
                if (found == null)
                {
                    foreach (var child in node.ChildNodes)
                    {
                        if (child != null)
                        {
                            queue.Enqueue(child as IOctreeBase<T>);
                        }
                    }
                }
                else
                {
                    queue.Clear();
                    result = node;
                    break;
                }
            }
            return result;
        }


        private void UpdateParentBoundingBoxToRoot()
        {
            var parent = Parent;
            while (parent != null)
            {
                if (!parent.UpdateBoundingBox()) //If bounding box does not change, don't need to propergate to upper level
                {
                    break;
                }
                parent = parent.Parent;
            }
        }

        public bool UpdateBoundingBox()
        {
            var box = new BoundingBox();
            if (Objects.Count > 0)
            {
                box = GetBoundingBoxFromItem(Objects[0]);
            }
            else if (HasChildren)
            {
                foreach (var child in ChildNodes)
                {
                    if (child != null)
                    {
                        box = child.Bound;
                        break;
                    }
                }
            }

            foreach (var item in Objects)
            {
                box = BoundingBox.Merge(box, GetBoundingBoxFromItem(item));
            }
            if (HasChildren)
            {
                foreach (var child in ChildNodes)
                {
                    if (child != null)
                    {
                        box = BoundingBox.Merge(box, child.Bound);
                    }
                }
            }
            if (Bound == box)
            {
                return false;
            }
            else
            {
                Bound = box;
                return true;
            }
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

        public MeshGeometryOctree(Vector3Collection positions, IList<int> indices, int minSize = 1)
            : base(null, minSize)
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

        protected MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, BoundingBox bound, List<Tuple<int, BoundingBox>> triIndex, IOctree parent, int minSize)
            : base(bound, triIndex, parent, minSize)
        {
            Positions = positions;
            Indices = indices;
        }

        protected MeshGeometryOctree(BoundingBox bound, List<Tuple<int, BoundingBox>> list, IOctree parent, int minSize)
            : base(bound, list, parent, minSize)
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

        protected override IOctree CreateNodeWithParent(BoundingBox region, List<Tuple<int, BoundingBox>> objList, IOctree parent)
        {
            return new MeshGeometryOctree(Positions, Indices, region, objList, parent, this.MIN_SIZE);
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
        public GeometryModel3DOctree(List<GeometryModel3D> objList, int minSize = 1)
            : base(null, minSize)
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
        protected GeometryModel3DOctree(BoundingBox bound, List<GeometryModel3D> objList, IOctree parent, int minSize)
            : base(bound, objList, parent, minSize)
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
            return item.Bounds;
        }

        protected override IOctree CreateNodeWithParent(BoundingBox bound, List<GeometryModel3D> objList, IOctree parent)
        {
            return new GeometryModel3DOctree(bound, objList, parent, this.MIN_SIZE);
        }
    }
}
