/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    public class BoneSkinRenderCore : PatchMeshRenderCore, IBoneSkinRenderParams
    {
        public IElementsBufferModel VertexBoneIdBuffer { set; get; }

        private BoneMatricesStruct boneMatrices;
        public BoneMatricesStruct BoneMatrices
        {
            set
            {
                SetAffectsRender(ref boneMatrices, value);
            }
            get { return boneMatrices; }
        }

        private IConstantBufferProxy boneCB;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                boneCB = technique.ConstantBufferPool.Register(new ConstantBufferDescription(DefaultBufferNames.BoneCB, BoneMatricesStruct.SizeInBytes));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && VertexBoneIdBuffer != null && VertexBoneIdBuffer.HasElements;
        }

        protected override void OnAttachBuffers(DeviceContext context, ref int vertStartSlot)
        {
            base.OnAttachBuffers(context, ref vertStartSlot);         
            VertexBoneIdBuffer?.AttachBuffer(context, ref vertStartSlot);
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);     
            model.HasBones = BoneMatrices.Bones != null ? 1 : 0;                 
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            if (BoneMatrices.Bones != null)
            {
                boneCB.UploadDataToBuffer(context, BoneMatrices.Bones, BoneMatricesStruct.NumberOfBones);
            }
        }
    }
}
