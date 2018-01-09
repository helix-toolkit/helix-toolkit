using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Runtime.CompilerServices;

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
            var tempHits = new List<HitTestResult>();
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var t in this.Objects)
                {
                    //t.PushMatrix(modelMatrix);
                    isHit |= t.HitTest(context, rayWS, ref tempHits);
                    //t.PopMatrix();
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
                    foreach (var item in (node as IOctreeBase<GeometryModel3D>).Objects)
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
}
