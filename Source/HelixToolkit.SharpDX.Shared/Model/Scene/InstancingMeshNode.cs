/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;


    public class InstancingMeshNode : MeshNode
    {
        public IList<Guid> InstanceIdentifiers
        {
            set; get;
        } = null;


        public IList<InstanceParameter> InstanceParamArray
        {
            set { instanceParamBuffer.Elements = value; }
            get { return instanceParamBuffer.Elements; }
        }

        public IOctreeManager OctreeManager
        {
            set; get;
        } = null;

        private bool isInstanceChanged = false;
        protected IOctreeManager octreeManager { private set; get; }

        protected IElementsBufferModel<InstanceParameter> instanceParamBuffer = new InstanceParamsBufferModel<InstanceParameter>(InstanceParameter.SizeInBytes);

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.InstancingBlinn];
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new InstancingMeshRenderCore() { ParameterBuffer = this.instanceParamBuffer };
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                instanceParamBuffer.Initialize();

                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            instanceParamBuffer.DisposeAndClear();
            base.OnDetach();
        }

        public override void UpdateNotRender(IRenderContext context)
        {
            base.UpdateNotRender(context);
            if (isInstanceChanged)
            {
                BuildOctree();
                isInstanceChanged = false;
            }
        }

        protected override void InstancesChanged()
        {
            base.InstancesChanged();
            octreeManager?.Clear();
            isInstanceChanged = true;
        }

        private void BuildOctree()
        {
            if (IsRenderable && InstanceBuffer.HasElements)
            {
                octreeManager?.RebuildTree(Enumerable.Repeat<IRenderable>(this, 1));
            }
            else
            {
                octreeManager?.Clear();
            }
        }

        public override bool HitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
        {
            bool isHit = false;
            if (CanHitTest(context))
            {
                if (octreeManager != null && octreeManager.Octree != null)
                {
                    var boundHits = new List<HitTestResult>();
                    isHit = octreeManager.Octree.HitTest(context, this, TotalModelMatrix, rayWS, ref boundHits);
                    if (isHit)
                    {
                        isHit = false;
                        Matrix instanceMatrix;
                        foreach (var hit in boundHits)
                        {
                            int instanceIdx = (int)hit.Tag;
                            instanceMatrix = InstanceBuffer.Elements[instanceIdx];
                            var h = base.OnHitTest(context, TotalModelMatrix * instanceMatrix, ref rayWS, ref hits);
                            isHit |= h;
                            if (h && hits.Count > 0)
                            {
                                var result = hits.Last();
                                object tag = null;
                                if (InstanceIdentifiers != null && InstanceIdentifiers.Count == InstanceBuffer.Elements.Count)
                                {
                                    tag = InstanceIdentifiers[instanceIdx];
                                }
                                else
                                {
                                    tag = instanceIdx;
                                }
                                result.Tag = tag;
                                hits[hits.Count - 1] = result;
                            }
                        }
                    }
                }
                else
                {
                    isHit = base.HitTest(context, rayWS, ref hits);
                }

            }
            return isHit;
        }
    }
}
