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
using System.Text;

namespace HelixToolkit.SharpDX.Shared.Utilities
{
    public interface IOctree<T>
    {
        byte ActiveNodes { get; }
        bool HasChildren { get; }
        bool IsRoot { get; }
        List<T> Objects { get; }
        IOctree<T> Parent { get; }
        BoundingBox Bound { get; }
        IOctree<T>[] ChildNodes { get; }
        bool IsEmpty { get; }
        bool HitTest(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);
        void UpdateTree();
    }

    public abstract class OctreeBase<T> : IOctree<T>
    {
        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public const int MIN_SIZE = 1;

        public BoundingBox Bound { protected set; get; }

        public List<T> Objects { protected set; get; }

        /// <summary>
        /// These are all of the possible child octants for this node in the tree.
        /// </summary>
        private readonly IOctree<T>[] childNodes = new IOctree<T>[8];
        public IOctree<T>[] ChildNodes { get { return childNodes; } }
        /// <summary>
        /// This is a bitmask indicating which child nodes are actively being used.
        /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
        /// </summary>
        public byte ActiveNodes { protected set; get; }

        /// <summary>
        /// A reference to the parent node is sometimes required. If we are a node and we realize that we no longer have items contained within ourselves,
        /// we need to let our parent know that we're empty so that it can delete us.
        /// </summary>
        public IOctree<T> Parent { protected set; get; }

        protected bool treeReady = false;       //the tree has a few objects which need to be inserted before it is complete
        protected bool treeBuilt = false;       //there is no pre-existing tree yet.


        /*Note: we want to avoid allocating memory for as long as possible since there can be lots of nodes.*/
        /// <summary>
        /// Creates an oct tree which encloses the given region and contains the provided objects.
        /// </summary>
        /// <param name="bound">The bounding region for the oct tree.</param>
        /// <param name="objList">The list of objects contained within the bounding region</param>
        protected OctreeBase(BoundingBox bound, List<T> objList)
        {
            Bound = bound;
            Objects = objList;
        }

        public OctreeBase()
        {
            Objects = new List<T>();
            Bound = new BoundingBox(Vector3.Zero, Vector3.Zero);
        }

        /// <summary>
        /// Creates an octTree with a suggestion for the bounding region containing the items.
        /// </summary>
        /// <param name="bound">The suggested dimensions for the bounding region. 
        /// Note: if items are outside this region, the region will be automatically resized.</param>
        public OctreeBase(BoundingBox bound)
            : this()
        {
            Bound = bound;
        }

        /// <summary>
        /// Naively builds an oct tree from scratch.
        /// </summary>
        protected abstract void BuildTree();    //complete & tested

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="objList"></param>
        /// <returns></returns>
        protected abstract IOctree<T> CreateNode(BoundingBox bound, List<T> objList);  //complete & tested

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bound"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        protected IOctree<T> CreateNode(BoundingBox bound, T Item)
        {
            return CreateNode(bound, new List<T> { Item });
        }

        /// <summary>
        /// Processes all pending insertions by inserting them into the tree.
        /// </summary>
        /// <remarks>Consider deprecating this?</remarks>
        public void UpdateTree()   //complete & tested
        {
            /*I think I can just directly insert items into the tree instead of using a queue.*/
            if (!treeBuilt)
            {
                BuildTree();
            }
            treeReady = true;
        }

        /// <summary>
        /// This finds the dimensions of the bounding box necessary to tightly enclose all items in the object list.
        /// </summary>
        protected abstract void FindEnclosingBox();

        /// <summary>
        /// This finds the smallest enclosing cube which is a power of 2, for all objects in the list.
        /// </summary>
        protected abstract void FindEnclosingCube();

        public abstract bool HitTest(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);

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

        /// <summary>
        /// Returns true if this node tree and all children have no content
        /// </summary>
        public bool IsEmpty    //untested
        {
            get
            {
                if (Objects.Count != 0)
                    return false;
                else
                {
                    for (int a = 0; a < 8; a++)
                    {
                        //note that we have to do this recursively. 
                        //Just checking child nodes for the current node doesn't mean that their children won't have objects.
                        if (ChildNodes[a] != null && !ChildNodes[a].IsEmpty)
                            return false;
                    }

                    return true;
                }
            }
        }
        #endregion
    }


    public class GeometryOctree
        : OctreeBase<Tuple<int, BoundingBox>>
    {
        public IList<Vector3> Positions { private set; get; }
        public IList<int> Indices { private set; get; }
        public GeometryOctree(Vector3Collection positions, IList<int> indices)
        {
            Positions = positions;
            Indices = indices;
            Bound = BoundingBox.FromPoints(positions.Array);
            Objects = new List<Tuple<int, BoundingBox>>(indices.Count / 3);
            foreach (var i in Enumerable.Range(0, indices.Count / 3))
            {
                Objects.Add(new Tuple<int, BoundingBox>(i, GetBoundingBox(i)));
            }
        }

        public GeometryOctree(IList<Vector3> positions, IList<int> indices, BoundingBox bound, List<Tuple<int, BoundingBox>> triIndex)
            : base(bound, triIndex)
        {
            Positions = positions;
            Indices = indices;
        }

        protected GeometryOctree(BoundingBox bound, List<Tuple<int, BoundingBox>> list)
            : base(bound, list)
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
        /// Build Tree subroutine
        /// </summary>
        protected override void BuildTree()
        {
            //terminate the recursion if we're a leaf node
            if (Objects.Count <= 1)   //doubt: is this really right? needs testing.
            {
                treeBuilt = true;
                treeReady = true;
                return;
            }

            Vector3 dimensions = Bound.Maximum - Bound.Minimum;

            if (dimensions == Vector3.Zero)
            {
                FindEnclosingCube();
                dimensions = Bound.Maximum - Bound.Minimum;
            }

            //Check to see if the dimensions of the box are greater than the minimum dimensions
            if (dimensions.X <= MIN_SIZE && dimensions.Y <= MIN_SIZE && dimensions.Z <= MIN_SIZE)
            {
                treeBuilt = true;
                treeReady = true;
                return;
            }

            Vector3 half = dimensions / 2.0f;
            Vector3 center = Bound.Minimum + half;

            //Create subdivided regions for each octant
            var octant = new BoundingBox[8] {
                new BoundingBox(Bound.Minimum, center),
                new BoundingBox(new Vector3(center.X, Bound.Minimum.Y, Bound.Minimum.Z), new Vector3(Bound.Maximum.X, center.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, Bound.Minimum.Y, center.Z), new Vector3(Bound.Maximum.X, center.Y, Bound.Maximum.Z)),
                new BoundingBox(new Vector3(Bound.Minimum.X, Bound.Minimum.Y, center.Z), new Vector3(center.X, center.Y, Bound.Maximum.Z)),
                new BoundingBox(new Vector3(Bound.Minimum.X, center.Y, Bound.Minimum.Z), new Vector3(center.X, Bound.Maximum.Y, center.Z)),
                new BoundingBox(new Vector3(center.X, center.Y, Bound.Minimum.Z), new Vector3(Bound.Maximum.X, Bound.Maximum.Y, center.Z)),
                new BoundingBox(center, Bound.Maximum),
                new BoundingBox(new Vector3(Bound.Minimum.X, center.Y, center.Z), new Vector3(center.X, Bound.Maximum.Y, Bound.Maximum.Z))
            };


            //This will contain all of our objects which fit within each respective octant.
            var octList = new List<Tuple<int, BoundingBox>>[8];
            for (int i = 0; i < 8; i++)
                octList[i] = new List<Tuple<int, BoundingBox>>(Objects.Count / 8);

            //this list contains all of the objects which got moved down the tree and can be delisted from this node.
            var delist = new List<int>(Objects.Count);
            int idx = 0;
            foreach (var obj in Objects)
            {
                var box = obj.Item2;
                if (box.Minimum != box.Maximum)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (octant[i].Contains(box) == ContainmentType.Contains)
                        {
                            octList[i].Add(obj);
                            delist.Add(idx);// Add index instead of object to allow fast swap and resize operation
                            break;
                        }
                    }
                }
                ++idx;
            }

            //delist every moved object from this node.
            //foreach (int obj in delist)
            //    Objects.Remove(obj);
            //To avoid list memory operation during remove, use swap and resize to improve performance
            int end = Objects.Count - 1;
            delist.Reverse();
            foreach (var i in delist)
            {
                Objects[i] = Objects[end--];
            }
            ++end;
            if (end < Objects.Count)
                Objects.RemoveRange(end, Objects.Count - end);
            Objects.TrimExcess();
            //Create child nodes where there are items contained in the bounding region
            for (int i = 0; i < 8; i++)
            {
                if (octList[i].Count != 0)
                {
                    ChildNodes[i] = CreateNode(octant[i], octList[i]);
                    ActiveNodes |= (byte)(1 << i);
                    (ChildNodes[i] as GeometryOctree).BuildTree();
                }
            }

            treeBuilt = true;
            treeReady = true;
        }

        protected override IOctree<Tuple<int, BoundingBox>> CreateNode(BoundingBox region, List<Tuple<int, BoundingBox>> objList)
        {
            return new GeometryOctree(Positions, Indices, region, objList);
        }

        protected override void FindEnclosingBox()
        {
            Vector3 global_min = Bound.Minimum, global_max = Bound.Maximum;

            //go through all the objects in the list and find the extremes for their bounding areas.
            foreach (var obj in Objects)
            {
                Vector3 local_min = Vector3.Zero, local_max = Vector3.Zero;
                var bound = obj.Item2;
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

        protected override void FindEnclosingCube()
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

        private static int SigBit(int x)
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

        public override bool HitTest(GeometryModel3D model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
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
                    if (hits.Count > 0)
                    {
                        if (hits[0].Distance > result.Distance)
                        {
                            hits[0] = result;
                        }
                    }
                    else
                    {
                        hits.Add(result);
                    }
                }
                if (HasChildren)
                {
                    foreach (GeometryOctree child in ChildNodes)
                    {
                        if (child != null)
                            child.HitTest(model, modelMatrix, rayWS, ref hits);
                    }
                }
            }

            return hits.Count > 0;
        }
    }
}
