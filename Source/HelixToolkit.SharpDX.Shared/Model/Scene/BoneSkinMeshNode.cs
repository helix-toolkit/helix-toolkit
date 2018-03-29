/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class BoneSkinMeshNode : MeshNode
    {
        public IList<BoneIds> VertexBoneIds
        {
            set
            {
                bonesBufferModel.Elements = value;
            }
            get { return bonesBufferModel.Elements; }
        }

        public BoneMatricesStruct BoneMatrices
        {
            set
            {
                (RenderCore as BoneSkinRenderCore).BoneMatrices = value;
            }
            get
            {
                return (RenderCore as BoneSkinRenderCore).BoneMatrices;
            }
        }

        protected readonly IElementsBufferModel<BoneIds> bonesBufferModel = new VertexBoneIdBufferModel<BoneIds>(BoneIds.SizeInBytes);
        private IBoneSkinRenderParams boneSkinRenderCore
        {
            get { return (IBoneSkinRenderParams)RenderCore; }
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BoneSkinBlinn];
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new BoneSkinRenderCore();
        }

        protected override void AssignDefaultValuesToCore(RenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            boneSkinRenderCore.BoneMatrices = BoneMatrices;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                bonesBufferModel.Initialize();
                boneSkinRenderCore.VertexBoneIdBuffer = bonesBufferModel;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnDetach()
        {
            bonesBufferModel.DisposeAndClear();
            base.OnDetach();
        }

        protected override bool CheckBoundingFrustum(BoundingFrustum boundingFrustum)
        {
            return true;
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;//return base.CanHitTest(context) && !hasBoneParameter;
        }
    }
}
