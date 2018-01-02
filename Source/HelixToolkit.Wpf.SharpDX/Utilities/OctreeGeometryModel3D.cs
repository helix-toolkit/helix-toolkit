using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Runtime.CompilerServices;
    using IOctree = IOctree<GeometryModel3D>;
    /// <summary>
    /// MeshGeometryOctree slices mesh geometry by triangles into octree. Objects are tuple of each triangle index and its bounding box.
    /// </summary>
    public class MeshGeometryOctree
        : OctreeBase<Tuple<int, BoundingBox>, GeometryModel3D>
    {
        public IList<Vector3> Positions { private set; get; }
        public IList<int> Indices { private set; get; }
        public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices, Queue<IOctree> queueCache = null)
            : this(positions, indices, null, queueCache)
        {
        }
        public MeshGeometryOctree(IList<Vector3> positions, IList<int> indices,
            OctreeBuildParameter parameter, Queue<IOctree> queueCache = null)
            : base(null, parameter, queueCache)
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

        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, GeometryModel3D model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect)
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
            bool checkBoundSphere = false;
            global::SharpDX.BoundingSphere boundSphere = new global::SharpDX.BoundingSphere();
            if (model != null)
            {
                checkBoundSphere = true;
                boundSphere = model.BoundsSphere;
            }
            //Hit test in local space.
            if (rayModel.Intersects(ref bound) && (!checkBoundSphere || rayModel.Intersects(ref boundSphere)))
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
                            result.PointHit = pointWorld.ToPoint3D();
                            result.Distance = (rayWS.Position - pointWorld).Length();

                            var p0 = Vector3.TransformCoordinate(v0, modelMatrix);
                            var p1 = Vector3.TransformCoordinate(v1, modelMatrix);
                            var p2 = Vector3.TransformCoordinate(v2, modelMatrix);
                            var n = Vector3.Cross(p1 - p0, p2 - p0);
                            n.Normalize();
                            // transform hit-info to world space now:
                            result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
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
                            tempResult.PointHit = cloestPoint.ToPoint3D();
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

    public class PointGeometryOctree : OctreeBase<int, GeometryModel3D>
    {
        private IList<Vector3> Positions;
        private static readonly Vector3 BoundOffset = new Vector3(0.0001f);
        public PointGeometryOctree(IList<Vector3> positions, Queue<IOctree> queueCache = null)
            : this(positions, null, queueCache)
        {
        }
        public PointGeometryOctree(IList<Vector3> positions,
            OctreeBuildParameter parameter, Queue<IOctree> queueCache = null)
               : base(null, parameter, queueCache)
        {
            Positions = positions;
            Bound = BoundingBoxExtensions.FromPoints(positions);
            Objects = Enumerable.Range(0, Positions.Count).ToList();
        }

        protected PointGeometryOctree(BoundingBox bound, IList<Vector3> positions, List<int> list, IOctree parent, OctreeBuildParameter paramter,
            Queue<IOctree> queueCache)
            : base(ref bound, list, parent, paramter, queueCache)
        {
            Positions = positions;
        }

        protected override bool IsContains(BoundingBox source, BoundingBox target, int obj)
        {
            return source.Contains(Positions[obj]) != ContainmentType.Disjoint;
        }

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
        /// <returns></returns>
        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, GeometryModel3D model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect)
        {
            isIntersect = false;
            if (!this.treeBuilt || !(model is PointGeometryModel3D))
            {
                return false;
            }
            var pointModel = model as PointGeometryModel3D;
            var isHit = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var bound = Bound;

            if (rayModel.Intersects(ref bound) && context != null)
            {
                var svpm = context.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;
                var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
                var pos4 = new Vector4(rayWS.Position, 1);
                Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
                Vector4.Transform(ref pos4, ref svpm, out pos4);
                var clickPoint = clickPoint4.ToVector3();

                isIntersect = true;
                var dist = pointModel.HitTestThickness;
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
                        result.PointHit = px.ToPoint3D();
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

        protected override IOctree CreateNodeWithParent(ref BoundingBox bound, List<int> objList, IOctree parent)
        {
            return new PointGeometryOctree(bound, Positions, objList, parent, Parameter, queue);
        }

        protected override BoundingBox GetBoundingBoxFromItem(int item)
        {
            return new BoundingBox(Positions[item] - BoundOffset, Positions[item] + BoundOffset);
        }

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
                            resultTemp.PointHit = p.ToPoint3D();
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

    public class GeometryModel3DOctree : OctreeBase<GeometryModel3D, GeometryModel3D>
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
                var bound = GetBoundingBoxFromItem(Objects[0]);
                foreach (var item in Objects)
                {
                    var b = GetBoundingBoxFromItem(item);
                    BoundingBox.Merge(ref b, ref bound, out bound);
                }
                this.Bound = bound;
            }
        }

        protected GeometryModel3DOctree(BoundingBox bound, List<GeometryModel3D> objList, IOctree parent, OctreeBuildParameter paramter, Queue<IOctree> queueCache)
            : base(ref bound, objList, parent, paramter, queueCache)
        { }

        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, GeometryModel3D model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            bool isHit = false;
            var bound = Bound.Transform(modelMatrix);// BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            var tempHits = new List<HitTestResult>();
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    t.PushMatrix(modelMatrix);
                    isHit |= t.HitTest(context, rayWS, ref tempHits);
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
            if (IsRoot)
            {
                OctantDictionary = new Dictionary<Guid, IOctree>(Objects.Count);
            }
            base.BuildTree();
            if (IsRoot)
            {
                TreeTraversal(this, queue, null, (node) =>
                {
                    foreach (var item in (node as IOctreeBase<GeometryModel3D, GeometryModel3D>).Objects)
                    {
                        OctantDictionary.Add(item.GUID, node);
                    }
                }, null);
            }
        }

        public IOctree FindItemByGuid(Guid guid, GeometryModel3D item, out int index)
        {
            var root = FindRoot(this) as GeometryModel3DOctree;
            index = -1;
            if (root.OctantDictionary.ContainsKey(guid))
            {
                var node = root.OctantDictionary[guid];
                index = (node as IOctreeBase<GeometryModel3D, GeometryModel3D>).Objects.IndexOf(item);
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
            if (base.Add(item, out octant))
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
            if (base.PushExistingToChild(index, out octant))
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
            //Debug.WriteLine("Expaned");
            var root = this;
            if (!IsRoot)
            {
                root = FindRoot(this) as GeometryModel3DOctree;
            }
            var newRoot = Expand(root, ref direction, CreateNodeWithParent);
            (newRoot as GeometryModel3DOctree).TransferOctantDictionary(root, ref root.OctantDictionary);//Transfer the dictionary to new root
            return newRoot;
        }

        public override IOctree Shrink()
        {
            var root = this;
            if (!IsRoot)
            {
                root = FindRoot(this) as GeometryModel3DOctree;
            }
            var newRoot = Shrink(root);
            (newRoot as GeometryModel3DOctree).TransferOctantDictionary(root, ref root.OctantDictionary);//Transfer the dictionary to new root
            return newRoot;
        }

        private void TransferOctantDictionary(IOctree source, ref Dictionary<Guid, IOctree> dictionary)
        {
            if (source == this)
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

        public override bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            throw new NotImplementedException();
        }
    }

    public class InstancingModel3DOctree : OctreeBase<Tuple<int, BoundingBox>, GeometryModel3D>
    {
        private IList<Matrix> InstanceMatrix;
        public InstancingModel3DOctree(IList<Matrix> instanceMatrix, BoundingBox geometryBound, OctreeBuildParameter parameter, Queue<IOctree> queueCache = null)
            : base(ref geometryBound, null, parameter, queueCache)
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

        protected InstancingModel3DOctree(ref BoundingBox bound, IList<Matrix> instanceMatrix, List<Tuple<int, BoundingBox>> objects, IOctree parent, OctreeBuildParameter parameter, Queue<IOctree> queueCache = null)
            : base(ref bound, objects, parent, parameter, queueCache)
        {
            InstanceMatrix = instanceMatrix;
        }

        public override bool FindNearestPointBySphereExcludeChild(IRenderContext context, ref global::SharpDX.BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            throw new NotImplementedException();
        }

        public override bool HitTestCurrentNodeExcludeChild(IRenderContext context, GeometryModel3D model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect)
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

        protected override IOctree CreateNodeWithParent(ref BoundingBox bound, List<Tuple<int, BoundingBox>> objList, IOctree parent)
        {
            return new InstancingModel3DOctree(ref bound, this.InstanceMatrix, objList, parent, this.Parameter, this.queue);
        }

        protected override BoundingBox GetBoundingBoxFromItem(Tuple<int, BoundingBox> item)
        {
            return item.Item2;
        }
    }
}
