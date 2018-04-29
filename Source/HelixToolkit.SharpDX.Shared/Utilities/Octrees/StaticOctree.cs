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
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface IStaticOctree
    {
        void Build();
        bool HitTest(IRenderContext context, object model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits);
        bool HitTest(IRenderContext context, object model, Matrix modelMatrix, Ray rayWS, ref List<HitTestResult> hits, float hitThickness);
    }

    public abstract class StaticOctree<T> : IStaticOctree
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
            private volatile Octant[] array = new Octant[128];
            public int Count { private set; get; }

            public OctantArray(BoundingBox bound, int length)
            {
                var octant = new Octant(-1, 0, ref bound);
                octant.Start = 0;
                octant.End = length;
                array[0] = octant;
                ++Count;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Octant Add(int parentIndex, int childIndex, BoundingBox bound)
            {
                if (array.Length < Count + OctantSize)
                {
                    var newArray = new Octant[array.Length * 2];
                    Array.Copy(array, newArray, Count);
                    array = newArray;
                }
                ref var parent = ref array[parentIndex];

                array[Count] = new Octant(parent.Index, Count, ref bound);
                parent[childIndex] = Count;
                ++Count;               
                return parent;
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

        private OctantArray octants;
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
        public event EventHandler<EventArgs> OnHit;

        protected T[] Objects;


        public StaticOctree(OctreeBuildParameter parameter)
        {
            Parameter = parameter;
        }
            

        public void Build()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            Objects = GetObjects();
            octants = new OctantArray(GetMaxBound(), Objects.Length);
            TreeTraversal(stack, (index) => { BuildSubTree(index); }, null);
            octants.Compact();
#if DEBUG
            sw.Stop();
             Console.WriteLine($"Build static tree time ={sw.ElapsedTicks}; Total = {octants.Count}");
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
        protected virtual void BuildSubTree(int index)
        {
            var octant = octants[index];
            if (!CheckDimension(octant.Bound) || octant.IsEmpty 
                || octant.IsBuilt || octant.Count < this.Parameter.MinObjectSizeToSplit)
            {
                return;
            }

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
                            octant = octants.Add(index, childOctantIdx, octantBounds[childOctantIdx]);
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
                while(++curr < OctantSize)
                {
                    if(parentOctant.HasChildAtIndex(curr))
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
        /// <see cref="IOctree.HitTest(IRenderContext, object, Matrix, Ray, ref List{HitTestResult})"/>
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
        /// <see cref="IOctree.HitTest(IRenderContext, object, Matrix, Ray, ref List{HitTestResult}, float)"/>
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
            var hitStack = stack;
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
                        bool nodeHit = HitTestCurrentNodeExcludeChild(octant,
                            context, model, modelMatrix, ref rayWS, ref rayModel, ref modelHits, ref isIntersect, hitThickness);
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
                if(parent == -1)
                {
                    break;
                }
                parentOctant = octants[parent];
            }
            if (!isHit)
            {
                //hitPathBoundingBoxes.Clear();
            }
            else
            {
                hits.AddRange(modelHits);
                OnHit?.Invoke(this, EventArgs.Empty);
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
        protected abstract bool HitTestCurrentNodeExcludeChild(Octant octant, IRenderContext context, object model, 
            Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);
    }

    public class StaticMeshGeometryOctree : StaticOctree<KeyValuePair<int, BoundingBox>>
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<Vector3> Positions { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public IList<int> Indices { private set; get; }

        public StaticMeshGeometryOctree(IList<Vector3> positions, IList<int> indices, OctreeBuildParameter parameter) 
            : base(parameter)
        {
            Positions = positions;
            Indices = indices;
        }

        protected override KeyValuePair<int, BoundingBox>[] GetObjects()
        {
            var objects = new KeyValuePair<int, BoundingBox>[Indices.Count / 3];
            // Construct triangle index and its bounding box KeyValuePair
            for (int i = 0; i < Indices.Count / 3; ++i)
            {
                objects[i] = new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i));
            }
            return objects;
        }
        protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
        {
            return item.Value;
        }

        protected override BoundingBox GetMaxBound()
        {
            return BoundingBoxExtensions.FromPoints(Positions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        protected override bool HitTestCurrentNodeExcludeChild(Octant octant, 
            IRenderContext context, object model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, 
            ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt)
            {
                return false;
            }
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = octant.Bound;
            //Hit test in local space.
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                for (int i = octant.Start; i < octant.End; ++i)
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
                            result.Tag = idx;
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

    public class StaticLineGeometryOctree : StaticOctree<KeyValuePair<int, BoundingBox>>
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<Vector3> Positions { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public IList<int> Indices { private set; get; }

        public StaticLineGeometryOctree(IList<Vector3> positions, IList<int> indices, OctreeBuildParameter parameter)
            : base(parameter)
        {
            Positions = positions;
            Indices = indices;
        }

        protected override KeyValuePair<int, BoundingBox>[] GetObjects()
        {
            var objects = new KeyValuePair<int, BoundingBox>[Indices.Count / 2];
            // Construct triangle index and its bounding box KeyValuePair
            for (int i = 0; i < Indices.Count / 2; ++i)
            {
                objects[i] = new KeyValuePair<int, BoundingBox>(i, GetBoundingBox(i));
            }
            return objects;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        protected override BoundingBox GetMaxBound()
        {
            return BoundingBoxExtensions.FromPoints(Positions);
        }

        protected override BoundingBox GetBoundingBoxFromItem(KeyValuePair<int, BoundingBox> item)
        {
            return item.Value;
        }

        protected override bool HitTestCurrentNodeExcludeChild(Octant octant, IRenderContext context, object model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt)
            {
                return false;
            }
            var isHit = false;
            var result = new LineHitTestResult { IsValid = false, Distance = double.MaxValue };
            result.Distance = double.MaxValue;
            var bound = octant.Bound;
            bound.Maximum += new Vector3(hitThickness);
            bound.Minimum -= new Vector3(hitThickness);
            var lastDist = double.MaxValue;
            //Hit test in local space.
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                for (int i = octant.Start; i < octant.End; ++i)
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
                        result.Tag = idx; // For compatibility
                        result.LineIndex = idx;
                        result.TriangleIndices = null; // Since triangles are shader-generated
                        result.RayHitPointScalar = sc;
                        result.LineHitPointScalar = tc;
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
    }

    public class StaticPointGeometryOctree : StaticOctree<int>
    {
        private IList<Vector3> Positions;
        private static readonly Vector3 BoundOffset = new Vector3(0.0001f);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public StaticPointGeometryOctree(IList<Vector3> positions,
            OctreeBuildParameter parameter, Stack<KeyValuePair<int, IOctree[]>> stackCache = null)
               : base(parameter)
        {
            Positions = positions;
        }

        protected override BoundingBox GetBoundingBoxFromItem(int item)
        {
            return new BoundingBox(Positions[item] - BoundOffset, Positions[item] + BoundOffset);
        }

        protected override BoundingBox GetMaxBound()
        {
            return BoundingBoxExtensions.FromPoints(Positions);
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

        protected override int[] GetObjects()
        {
            var objects = new int[Positions.Count];
            for (int i = 0; i < Positions.Count; ++i)
            {
                objects[i] = i;
            }
            return objects;
        }

        protected override bool HitTestCurrentNodeExcludeChild(Octant octant, IRenderContext context, object model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt || context == null)
            {
                return false;
            }
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = octant.Bound;

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
                for (int i = octant.Start; i < octant.End; ++i)
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
    }
}
