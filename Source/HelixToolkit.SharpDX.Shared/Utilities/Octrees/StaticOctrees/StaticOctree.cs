/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

//#define DEBUG
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else

namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public abstract class StaticOctree<T> : IOctreeBasic where T : struct
    {
        public const int OctantSize = 8;

        protected struct Octant
        {
            public BoundingBox Bound { private set; get; }
            public int Parent { private set; get; }
            public int Index { private set; get; }
            private int c0, c1, c2, c3, c4, c5, c6, c7;
            public int Start { set; get; }
            public int End { set; get; }
            public bool IsValid { set; get; }
            public bool IsBuilt { set; get; }
            public byte ActiveNode { private set; get; }
            public bool HasChildren { get { return ActiveNode != 0; } }
            public bool IsEmpty { get { return Count == 0; } }
            public int Count { get { return End - Start; } }

            public Octant(int parent, int index, ref BoundingBox bound)
            {
                Parent = parent;
                Index = index;
                Bound = bound;
                Start = End = 0;
                IsValid = true;
                ActiveNode = 0;
                IsBuilt = false;
                c0 = c1 = c2 = c3 = c4 = c5 = c6 = c7 = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int GetChildIndex(int index)
            {
                switch (index)
                {
                    case 0: return c0;
                    case 1: return c1;
                    case 2: return c2;
                    case 3: return c3;
                    case 4: return c4;
                    case 5: return c5;
                    case 6: return c6;
                    case 7: return c7;
                    default:
                        return -1;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetChildIndex(int index, int value)
            {
                switch (index)
                {
                    case 0: c0 = value; break;
                    case 1: c1 = value; break;
                    case 2: c2 = value; break;
                    case 3: c3 = value; break;
                    case 4: c4 = value; break;
                    case 5: c5 = value; break;
                    case 6: c6 = value; break;
                    case 7: c7 = value; break;
                }
                if (value >= 0)
                {
                    ActiveNode |= (byte)(1 << index);
                }
            }

            public int this[int index]
            {
                get { return GetChildIndex(index); }
                set { SetChildIndex(index, value); }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasChildAtIndex(int index)
            {
                return (ActiveNode & (byte)(1 << index)) != 0;
            }
        }

        protected sealed class OctantArray
        {
            private Octant[] array = new Octant[128];
            public int Count { private set; get; }

            public OctantArray(BoundingBox bound, int length)
            {
                var octant = new Octant(-1, 0, ref bound);
                octant.Start = 0;
                octant.End = length;
                array[0] = octant;
                ++Count;
            }
            /// <summary>
            /// Adds the specified parent index.
            /// </summary>
            /// <param name="parentIndex">Index of the parent.</param>
            /// <param name="childIndex">Index of the child.</param>
            /// <param name="bound">The bound.</param>
            /// <param name="newParent">The parent out.</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Add(int parentIndex, int childIndex, BoundingBox bound, ref Octant newParent)
            {
                if (array.Length < Count + OctantSize)
                {
                    var newSize = array.Length * 2;
                    if(newSize > int.MaxValue / 4) //Size is too big
                    {
                        return false;
                    }
                    var newArray = new Octant[array.Length * 2];
                    Array.Copy(array, newArray, Count);
                    array = newArray;
                }
                ref var parent = ref array[parentIndex];

                array[Count] = new Octant(parent.Index, Count, ref bound);
                parent[childIndex] = Count;
                ++Count;
                newParent = parent;
                return true;
            }

            public void Compact()
            {
                if (array.Length > Count)
                {
                    var newArray = new Octant[Count];
                    Array.Copy(array, newArray, Count);
                    array = newArray;
                }
            }

            private Octant InvalidOctant = new Octant() { IsValid = false };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref Octant Get(int i)
            {
                return ref array[i];
            }

            public Octant this[int index]
            {
                get { return array[index]; }
                set { array[index] = value; }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler<EventArgs> OnHit;

        private OctantArray octants;

        private readonly List<BoundingBox> hitPathBoundingBoxes = new List<BoundingBox>();

        /// <summary>
        ///
        /// </summary>
        public IList<BoundingBox> HitPathBoundingBoxes { get { return hitPathBoundingBoxes.AsReadOnly(); } }

        private readonly Stack<KeyValuePair<int, int>> stack = new Stack<KeyValuePair<int, int>>();

        /// <summary>
        ///
        /// </summary>
        protected List<HitTestResult> modelHits = new List<HitTestResult>();

        /// <summary>
        /// The minumum size for enclosing region is a 1x1x1 cube.
        /// </summary>
        public float MIN_SIZE { get { return Parameter.MinimumOctantSize; } }

        public OctreeBuildParameter Parameter { private set; get; }

        /// <summary>
        ///
        /// </summary>
        protected T[] Objects { private set; get; }

        /// <summary>
        ///
        /// </summary>
        public bool TreeBuilt { private set; get; } = false;

        /// <summary>
        ///
        /// </summary>
        public BoundingBox Bound { private set; get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        public StaticOctree(OctreeBuildParameter parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        ///
        /// </summary>
        public void BuildTree()
        {
            if (TreeBuilt)
            {
                return;
            }
#if DEBUG
            var tick = Stopwatch.GetTimestamp();
#endif
            Objects = GetObjects();
            octants = new OctantArray(GetMaxBound(), Objects.Length);
            TreeTraversal(stack, (index) => { BuildSubTree(index); }, null);
            octants.Compact();
            TreeBuilt = true;
            Bound = octants[0].Bound;
#if DEBUG
            tick = Stopwatch.GetTimestamp() - tick;
            Console.WriteLine($"Build static tree time ={(double)tick / Stopwatch.Frequency * 1000}; Total = {octants.Count}");
#endif
        }

        protected abstract T[] GetObjects();

        protected abstract BoundingBox GetMaxBound();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckDimension(BoundingBox bound)
        {
            Vector3 dimensions = bound.Maximum - bound.Minimum;

            if (dimensions == Vector3.Zero)
            {
                return false;
            }
            dimensions = bound.Maximum - bound.Minimum;
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
        protected void BuildSubTree(int index)
        {
            var octant = octants[index];
            if (octant.IsBuilt)
            {
                return;
            }

            if (CheckDimension(octant.Bound) && !octant.IsEmpty
                && octant.Count > this.Parameter.MinObjectSizeToSplit)
            {
                var octantBounds = CreateOctants(octant.Bound, Parameter.MinimumOctantSize);

                int start = octant.Start;
                Octant childOctant = new Octant();
                for (int childOctantIdx = 0; childOctantIdx < OctantSize; ++childOctantIdx)
                {
                    int count = 0;
                    int end = octant.End;
                    bool hasChildOctant = false;
                    int childIdx = -1;

                    for (int i = end - 1; i >= start; --i)
                    {
                        var obj = Objects[i];
                        if (IsContains(octantBounds[childOctantIdx], GetBoundingBoxFromItem(obj), obj))
                        {
                            if (!hasChildOctant)//Add New Child Octant if not having one.
                            {
                                if(!octants.Add(index, childOctantIdx, octantBounds[childOctantIdx], ref octant))
                                {
                                    Debug.WriteLine("Add child failed.");
                                    break;
                                }
                                childIdx = octant[childOctantIdx];
                                childOctant = octants[childIdx];
                                hasChildOctant = true;
                            }
                            ++count;
                            childOctant.End = end;
                            childOctant.Start = end - count;
                            var o = Objects[i];
                            Objects[i] = Objects[end - count]; //swap objects. Move object into parent octant start/end range
                            Objects[end - count] = o; //Move object into child octant start/end range
                        }
                    }

                    if (hasChildOctant)
                    {
                        octants[childIdx] = childOctant;
                    }
                    octant.End = end - count;
                }
            }

            octant.IsBuilt = true;
            octants[index] = octant;
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
        protected BoundingBox FindEnclosingBox(int index)
        {
            var octant = octants[index];
            if (octant.Count == 0)
            {
                return new BoundingBox();
            }
            var b = GetBoundingBoxFromItem(Objects[octant.Start]);
            for (int i = octant.Start + 1; i < octant.End; ++i)
            {
                var bound = GetBoundingBoxFromItem(Objects[i]);
                BoundingBox.Merge(ref b, ref bound, out b);
            }
            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingBox[] CreateOctants(BoundingBox box, float minSize)
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
            return new BoundingBox[] {
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
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        protected virtual bool IsContains(BoundingBox source, BoundingBox target, T targetObj)
        {
            return source.Contains(ref target) == ContainmentType.Contains;
        }

        /// <summary>
        /// Common function to traverse the tree
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="process"></param>
        /// <param name="canVisitChildren"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TreeTraversal(Stack<KeyValuePair<int, int>> stack, Action<int> process,
            Func<int, bool> canVisitChildren = null)
        {
            int parent = -1;
            int curr = -1;
            var dummy = new Octant();
            dummy[0] = 0;
            var parentOctant = dummy;
            while (true)
            {
                while (++curr < OctantSize)
                {
                    if (parentOctant.HasChildAtIndex(curr))
                    {
                        var childIdx = parentOctant[curr];
                        process(childIdx);
                        var octant = octants[childIdx];
                        if (octant.HasChildren && (canVisitChildren == null || canVisitChildren(octant.Index)))
                        {
                            stack.Push(new KeyValuePair<int, int>(parent, curr));
                            parent = octant.Index;
                            curr = -1;
                            parentOctant = octants[parent];
                        }
                    }
                }
                if (stack.Count == 0) { break; }
                var prev = stack.Pop();
                parent = prev.Key;
                curr = prev.Value;
                if (parent == -1)
                {
                    break;
                }
                parentOctant = octants[parent];
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public bool HitTest(IRenderContext context, object model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits)
        {
            return HitTest(context, model, modelMatrix, rayWS, ref hits, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="hits"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        public virtual bool HitTest(IRenderContext context, object model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness)
        {
            if (hits == null)
            {
                hits = new List<HitTestResult>();
            }
            hitPathBoundingBoxes.Clear();
            var hitStack = stack;
            hitStack.Clear();
            bool isHit = false;
            modelHits.Clear();
            var modelInv = modelMatrix.Inverted();
            if (modelInv == Matrix.Zero) { return false; }//Cannot be inverted
            var rayModel = new Ray(Vector3.TransformCoordinate(rayWS.Position, modelInv), Vector3.TransformNormal(rayWS.Direction, modelInv));

            int parent = -1;
            int curr = -1;
            var dummy = new Octant();
            dummy[0] = 0;
            var parentOctant = dummy;
            while (true)
            {
                while (++curr < OctantSize)
                {
                    if (parentOctant.HasChildAtIndex(curr))
                    {
                        var octant = octants[parentOctant[curr]];
                        bool isIntersect = false;
                        bool nodeHit = HitTestCurrentNodeExcludeChild(ref octant,
                            context, model, modelMatrix, ref rayWS, ref rayModel, ref modelHits, ref isIntersect, hitThickness);
                        isHit |= nodeHit;
                        if (isIntersect && octant.HasChildren)
                        {
                            stack.Push(new KeyValuePair<int, int>(parent, curr));
                            parent = octant.Index;
                            curr = -1;
                            parentOctant = octants[parent];
                        }
                        if (Parameter.RecordHitPathBoundingBoxes && nodeHit)
                        {
                            var n = octant;
                            while (n.IsValid)
                            {
                                hitPathBoundingBoxes.Add(n.Bound);
                                if (n.Parent >= 0)
                                {
                                    n = octants[n.Parent];
                                }
                                else { break; }
                            }
                        }
                    }
                }
                if (stack.Count == 0) { break; }
                var prev = stack.Pop();
                parent = prev.Key;
                curr = prev.Value;
                if (parent == -1)
                {
                    break;
                }
                parentOctant = octants[parent];
            }
            if (!isHit)
            {
                hitPathBoundingBoxes.Clear();
            }
            else
            {
                hits.AddRange(modelHits);
                OnHit?.Invoke(this, EventArgs.Empty);
            }
            return isHit;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public virtual bool FindNearestPointBySphere(IRenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> points)
        {
            if (points == null)
            {
                points = new List<HitTestResult>();
            }
            var hitStack = stack;
            hitStack.Clear();
            bool isHit = false;

            int parent = -1;
            int curr = -1;
            var dummy = new Octant();
            dummy[0] = 0;
            var parentOctant = dummy;
            while (true)
            {
                while (++curr < OctantSize)
                {
                    if (parentOctant.HasChildAtIndex(curr))
                    {
                        var octant = octants[parentOctant[curr]];
                        bool isIntersect = false;
                        bool nodeHit = FindNearestPointBySphereExcludeChild(ref octant, context, ref sphere, ref points, ref isIntersect);
                        isHit |= nodeHit;
                        if (octant.HasChildren && isIntersect)
                        {
                            stack.Push(new KeyValuePair<int, int>(parent, curr));
                            parent = octant.Index;
                            curr = -1;
                            parentOctant = octants[parent];
                        }
                    }
                }
                if (stack.Count == 0) { break; }
                var prev = stack.Pop();
                parent = prev.Key;
                curr = prev.Value;
                if (parent == -1)
                {
                    break;
                }
                parentOctant = octants[parent];
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
            hitStack.Clear();
            var sphere = new BoundingSphere(point, float.MaxValue);
            bool isHit = false;
            heuristicSearchFactor = Math.Min(1.0f, Math.Max(0.1f, heuristicSearchFactor));

            int parent = -1;
            int curr = -1;
            var dummy = new Octant();
            dummy[0] = 0;
            var parentOctant = dummy;
            while (true)
            {
                while (++curr < OctantSize)
                {
                    if (parentOctant.HasChildAtIndex(curr))
                    {
                        var octant = octants[parentOctant[curr]];
                        bool isIntersect = false;
                        bool nodeHit = FindNearestPointBySphereExcludeChild(ref octant, context, ref sphere, ref results, ref isIntersect);
                        isHit |= nodeHit;
                        if (isIntersect)
                        {
                            if (results.Count > 0)
                            {
                                sphere.Radius = (float)results[0].Distance * heuristicSearchFactor;
                            }
                            if (octant.HasChildren)
                            {
                                stack.Push(new KeyValuePair<int, int>(parent, curr));
                                parent = octant.Index;
                                curr = -1;
                                parentOctant = octants[parent];
                            }
                        }
                    }
                }
                if (stack.Count == 0) { break; }
                var prev = stack.Pop();
                parent = prev.Key;
                curr = prev.Value;
                if (parent == -1)
                {
                    break;
                }
                parentOctant = octants[parent];
            }
            return isHit;
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
        /// <summary>
        /// Find nearest point by sphere on current node only.
        /// </summary>
        /// <param name="octant"></param>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="points"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        protected abstract bool FindNearestPointBySphereExcludeChild(ref Octant octant, IRenderContext context,
            ref BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect);

        /// <summary>
        /// Hit test for current node.
        /// </summary>
        /// <param name="octant"></param>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        protected abstract bool HitTestCurrentNodeExcludeChild(ref Octant octant, IRenderContext context, object model,
            Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

        public LineGeometry3D CreateOctreeLineModel()
        {
            var builder = new LineBuilder();
            for (int i = 0; i < octants.Count; ++i)
            {
                var box = octants[i].Bound;
                Vector3[] verts = new Vector3[8];
                verts[0] = box.Minimum;
                verts[1] = new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z); //Z
                verts[2] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z); //Y
                verts[3] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z); //X

                verts[7] = box.Maximum;
                verts[4] = new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z); //Z
                verts[5] = new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z); //Y
                verts[6] = new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z); //X
                builder.AddLine(verts[0], verts[1]);
                builder.AddLine(verts[0], verts[2]);
                builder.AddLine(verts[0], verts[3]);
                builder.AddLine(verts[7], verts[4]);
                builder.AddLine(verts[7], verts[5]);
                builder.AddLine(verts[7], verts[6]);

                builder.AddLine(verts[1], verts[6]);
                builder.AddLine(verts[1], verts[5]);
                builder.AddLine(verts[4], verts[2]);
                builder.AddLine(verts[4], verts[3]);
                builder.AddLine(verts[2], verts[6]);
                builder.AddLine(verts[3], verts[5]);
            }
            return builder.ToLineGeometry3D();
        }
    }
}