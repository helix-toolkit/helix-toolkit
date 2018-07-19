using HelixToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP.Utilities
#else
namespace HelixToolkit.Wpf.SharpDX.Utilities
#endif
{
    using Model.Scene;
    using System.Runtime.CompilerServices;

    public class BoundableNodeOctree : DynamicOctreeBase<SceneNode>
    {
        /// <summary>
        /// Only root contains dictionary
        /// </summary>
        private Dictionary<Guid, IDynamicOctree> OctantDictionary = null;
        public BoundableNodeOctree(List<SceneNode> objList, Stack<KeyValuePair<int, IDynamicOctree[]>> queueCache = null)
            : this(objList, null, queueCache)
        {

        }

        public BoundableNodeOctree(List<SceneNode> objList, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> queueCache = null)
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

        protected BoundableNodeOctree(BoundingBox bound, List<SceneNode> objList, IDynamicOctree parent, OctreeBuildParameter paramter, Stack<KeyValuePair<int, IDynamicOctree[]>> queueCache)
            : base(ref bound, objList, parent, paramter, queueCache)
        { }

        public override bool HitTestCurrentNodeExcludeChild(RenderContext context, object model, Geometry3D geometry, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel,
            ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!this.treeBuilt)
            {
                return false;
            }
            bool isHit = false;
            //var bound = Bound.Transform(modelMatrix);// BoundingBox.FromPoints(Bound.GetCorners().Select(x => Vector3.TransformCoordinate(x, modelMatrix)).ToArray());
            var bound = Bound;
            var tempHits = new List<HitTestResult>();
            if (rayWS.Intersects(ref bound))
            {
                isIntersect = true;
                foreach (var r in this.Objects)
                {
                    isHit |= r.HitTest(context, rayWS, ref tempHits);
                    hits.AddRange(tempHits);
                    tempHits.Clear();
                }
            }
            return isHit;
        }

        protected override BoundingBox GetBoundingBoxFromItem(SceneNode item)
        {
            return item.BoundsWithTransform;
        }

        protected override IDynamicOctree CreateNodeWithParent(ref BoundingBox bound, List<SceneNode> objList, IDynamicOctree parent)
        {
            return new BoundableNodeOctree(bound, objList, parent, parent.Parameter, this.stack);
        }

        public override void BuildTree()
        {
            if (IsRoot)
            {
                OctantDictionary = new Dictionary<Guid, IDynamicOctree>(Objects.Count);
            }
            base.BuildTree();
            if (IsRoot)
            {
                TreeTraversal(this, stack, null, (node) =>
                {
                    foreach (var item in (node as DynamicOctreeBase<SceneNode>).Objects)
                    {
                        OctantDictionary.Add(item.GUID, node);
                    }
                }, null);
            }
        }

        public IDynamicOctree FindItemByGuid(Guid guid, SceneNode item, out int index)
        {
            var root = FindRoot(this) as BoundableNodeOctree;
            index = -1;
            if (root.OctantDictionary.ContainsKey(guid))
            {
                var node = root.OctantDictionary[guid];
                index = (node as DynamicOctreeBase<SceneNode>).Objects.IndexOf(item);
                return root.OctantDictionary[guid];
            }
            else
            {
                return null;
            }
        }

        public bool RemoveByGuid(Guid guid, SceneNode item)
        {
            var root = FindRoot(this);
            return RemoveByGuid(guid, item, root as BoundableNodeOctree);
        }

        public bool RemoveByGuid(Guid guid, SceneNode item, BoundableNodeOctree root)
        {
            if (root.OctantDictionary.ContainsKey(guid))
            {
                (OctantDictionary[guid] as BoundableNodeOctree).RemoveSafe(item, root);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Add(SceneNode item, out IDynamicOctree octant)
        {
            if (base.Add(item, out octant))
            {
                if (octant == null)
                { throw new Exception("Output octant is null"); };
                var root = FindRoot(this) as BoundableNodeOctree;
                if (!root.OctantDictionary.ContainsKey(item.GUID))
                {
                    root.OctantDictionary.Add(item.GUID, octant);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool PushExistingToChild(int index, out IDynamicOctree octant)
        {
            var item = Objects[index];
            if (base.PushExistingToChild(index, out octant))
            {
                var root = FindRoot(this) as BoundableNodeOctree;
                root.OctantDictionary[item.GUID] = octant;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool RemoveSafe(SceneNode item)
        {
            var root = FindRoot(this);
            return RemoveSafe(item, root);
        }

        public bool RemoveSafe(SceneNode item, IDynamicOctree root)
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

        public bool RemoveAt(int index, IDynamicOctree root)
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

        public override bool RemoveByBound(SceneNode item, ref BoundingBox bound)
        {
            var root = FindRoot(this);
            return RemoveByBound(item, ref bound, root);
        }

        public bool RemoveByBound(SceneNode item, ref BoundingBox bound, IDynamicOctree root)
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

        public override IDynamicOctree Expand(ref Vector3 direction)
        {
            //Debug.WriteLine("Expaned");
            var root = this;
            if (!IsRoot)
            {
                root = FindRoot(this) as BoundableNodeOctree;
            }
            var newRoot = Expand(root, ref direction, CreateNodeWithParent);
            (newRoot as BoundableNodeOctree).TransferOctantDictionary(root, ref root.OctantDictionary);//Transfer the dictionary to new root
            return newRoot;
        }

        public override IDynamicOctree Shrink()
        {
            var root = this;
            if (!IsRoot)
            {
                root = FindRoot(this) as BoundableNodeOctree;
            }
            var newRoot = Shrink(root);
            (newRoot as BoundableNodeOctree).TransferOctantDictionary(root, ref root.OctantDictionary);//Transfer the dictionary to new root
            return newRoot;
        }

        private void TransferOctantDictionary(IDynamicOctree source, ref Dictionary<Guid, IDynamicOctree> dictionary)
        {
            if (source == this)
            {
                return;
            }
            this.OctantDictionary = dictionary;
            dictionary = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveFromRootDictionary(IDynamicOctree node, Guid guid)
        {
            node = FindRoot(node);
            var root = node as BoundableNodeOctree;
            if (root.OctantDictionary.ContainsKey(guid))
            {
                root.OctantDictionary.Remove(guid);
            }
        }

        public override bool FindNearestPointBySphereExcludeChild(RenderContext context, ref BoundingSphere sphere, ref List<HitTestResult> points, ref bool isIntersect)
        {
            throw new NotImplementedException();
        }
    }
}
