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
        BoundingBox Region { get; }
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

        public BoundingBox Region { protected set; get; }

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
        /// <param name="region">The bounding region for the oct tree.</param>
        /// <param name="objList">The list of objects contained within the bounding region</param>
        protected OctreeBase(BoundingBox region, List<T> objList)
        {
            Region = region;
            Objects = objList;
        }

        public OctreeBase()
        {
            Objects = new List<T>();
            Region = new BoundingBox(Vector3.Zero, Vector3.Zero);
        }

        /// <summary>
        /// Creates an octTree with a suggestion for the bounding region containing the items.
        /// </summary>
        /// <param name="region">The suggested dimensions for the bounding region. 
        /// Note: if items are outside this region, the region will be automatically resized.</param>
        public OctreeBase(BoundingBox region)
            :this()
        {
            Region = region;
        }

        /// <summary>
        /// Naively builds an oct tree from scratch.
        /// </summary>
        protected abstract void BuildTree();    //complete & tested

        protected abstract OctreeBase<T> CreateNode(BoundingBox region, List<T> objList);  //complete & tested


        protected OctreeBase<T> CreateNode(BoundingBox region, T Item)
        {
            return CreateNode(region, new List<T> { Item });
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
        : OctreeBase<int>
    {
        public IList<Vector3> Positions { private set; get; }
        public IList<int> Indices { private set; get; }
        public GeometryOctree(Vector3Collection positions, IList<int> indices)
            : base(BoundingBox.FromPoints(positions.Array), Enumerable.Range(0, indices.Count/3).ToList())
        {
            Positions = positions;
            Indices = indices;
        }

        public GeometryOctree(IList<Vector3> positions, IList<int> indices, BoundingBox box, List<int> triIndex)
            : base(box, triIndex)
        {
            Positions = positions;
            Indices = indices;
        }

        protected GeometryOctree(BoundingBox box, List<int> list):base(box, list) { }

        private BoundingBox GetBoundingBox(int triangleIndex)
        {
            var actual = triangleIndex * 3;
            var v1 = Positions[Indices[actual++]];
            var v2 = Positions[Indices[actual++]];
            var v3 = Positions[Indices[actual++]];
            var maxX = Math.Max(v1.X, Math.Max(v2.X, v3.X));
            var maxY = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y));
            var maxZ = Math.Max(v1.Z, Math.Max(v2.Z, v3.Z));

            var minX = Math.Min(v1.X, Math.Min(v2.X, v3.X));
            var minY = Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
            var minZ = Math.Min(v1.Z, Math.Min(v2.Z, v3.Z));

            return new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }
        protected override void BuildTree()
        {
            //terminate the recursion if we're a leaf node
            if (Objects.Count <= 1)   //doubt: is this really right? needs testing.
                return;

            Vector3 dimensions = Region.Maximum - Region.Minimum;

            if (dimensions == Vector3.Zero)
            {
                FindEnclosingCube();
                dimensions = Region.Maximum - Region.Minimum;
            }

            //Check to see if the dimensions of the box are greater than the minimum dimensions
            if (dimensions.X <= MIN_SIZE && dimensions.Y <= MIN_SIZE && dimensions.Z <= MIN_SIZE)
            {
                return;
            }

            Vector3 half = dimensions / 2.0f;
            Vector3 center = Region.Minimum + half;

            //Create subdivided regions for each octant
            BoundingBox[] octant = new BoundingBox[8];
            octant[0] = new BoundingBox(Region.Minimum, center);
            octant[1] = new BoundingBox(new Vector3(center.X, Region.Minimum.Y, Region.Minimum.Z), new Vector3(Region.Maximum.X, center.Y, center.Z));
            octant[2] = new BoundingBox(new Vector3(center.X, Region.Minimum.Y, center.Z), new Vector3(Region.Maximum.X, center.Y, Region.Maximum.Z));
            octant[3] = new BoundingBox(new Vector3(Region.Minimum.X, Region.Minimum.Y, center.Z), new Vector3(center.X, center.Y, Region.Maximum.Z));
            octant[4] = new BoundingBox(new Vector3(Region.Minimum.X, center.Y, Region.Minimum.Z), new Vector3(center.X, Region.Maximum.Y, center.Z));
            octant[5] = new BoundingBox(new Vector3(center.X, center.Y, Region.Minimum.Z), new Vector3(Region.Maximum.X, Region.Maximum.Y, center.Z));
            octant[6] = new BoundingBox(center, Region.Maximum);
            octant[7] = new BoundingBox(new Vector3(Region.Minimum.X, center.Y, center.Z), new Vector3(center.X, Region.Maximum.Y, Region.Maximum.Z));

            //This will contain all of our objects which fit within each respective octant.
            List<int>[] octList = new List<int>[8];
            for (int i = 0; i < 8; i++) octList[i] = new List<int>();

            //this list contains all of the objects which got moved down the tree and can be delisted from this node.
            List<int> delist = new List<int>();

            foreach (int obj in Objects)
            {
                var box = GetBoundingBox(obj);
                if (box.Minimum != box.Maximum)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (octant[i].Contains(box) == ContainmentType.Contains)
                        {
                            octList[i].Add(obj);
                            delist.Add(obj);
                            break;
                        }
                    }
                }
            }

            //delist every moved object from this node.
            foreach (int obj in delist)
                Objects.Remove(obj);

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

        protected override OctreeBase<int> CreateNode(BoundingBox region, List<int> objList)
        {
            return new GeometryOctree(Positions, Indices, region, objList);
        }

        protected override void FindEnclosingBox()
        {
            Vector3 global_min = Region.Minimum, global_max = Region.Maximum;

            //go through all the objects in the list and find the extremes for their bounding areas.
            foreach (var obj in Objects)
            {
                Vector3 local_min = Vector3.Zero, local_max = Vector3.Zero;
                var BoundingBox = GetBoundingBox(obj);
                if (BoundingBox != null && BoundingBox.Maximum != BoundingBox.Minimum)
                {
                    local_min = BoundingBox.Minimum;
                    local_max = BoundingBox.Maximum;
                }

                if (local_min.X < global_min.X) global_min.X = local_min.X;
                if (local_min.Y < global_min.Y) global_min.Y = local_min.Y;
                if (local_min.Z < global_min.Z) global_min.Z = local_min.Z;

                if (local_max.X > global_max.X) global_max.X = local_max.X;
                if (local_max.Y > global_max.Y) global_max.Y = local_max.Y;
                if (local_max.Z > global_max.Z) global_max.Z = local_max.Z;
            }
            Region = new BoundingBox(global_min, global_max);
        }

        protected override void FindEnclosingCube()
        {
            FindEnclosingBox();

            //find the min offset from (0,0,0) and translate by it for a short while
            Vector3 offset = Region.Minimum - Vector3.Zero;
            Region = new BoundingBox(Region.Minimum + offset, Region.Maximum + offset);

            //find the nearest power of two for the max values
            int highX = (int)Math.Floor(Math.Max(Math.Max(Region.Maximum.X, Region.Maximum.Y), Region.Maximum.Z));

            //see if we're already at a power of 2
            for (int bit = 0; bit < 32; bit++)
            {
                if (highX == 1 << bit)
                {
                    Region = new BoundingBox(Region.Minimum - offset, new Vector3(highX, highX, highX) - offset);
                    return;
                }
            }

            //gets the most significant bit value, so that we essentially do a Ceiling(X) with the 
            //ceiling result being to the nearest power of 2 rather than the nearest integer.
            int x = SigBit(highX);

            Region = new BoundingBox(Region.Minimum - offset, new Vector3(x, x, x) - offset);
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
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = BoundingBox.FromPoints(Region.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            if(rayWS.Intersects(ref bound))
            {
                foreach(var t in this.Objects)
                {
                    var idx = t*3;
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
                    foreach(GeometryOctree child in ChildNodes)
                    {
                        if(child != null)
                            child.HitTest(model, modelMatrix, rayWS, ref hits);
                    }
                }
            }

            return hits.Count>0;
        }
    }
}
