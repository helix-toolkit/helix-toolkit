/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;

    public class BoneSkinRenderCore : MeshRenderCore, IBoneSkinRenderParams
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

        private ConstantBufferProxy boneCB;

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

        protected override bool CanRender(RenderContext context)
        {
            return base.CanRender(context) && VertexBoneIdBuffer != null && VertexBoneIdBuffer.HasElements;
        }

        protected override bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            if (base.OnAttachBuffers(context, ref vertStartSlot))
            {
                VertexBoneIdBuffer?.AttachBuffer(context, ref vertStartSlot);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);     
            model.HasBones = BoneMatrices.Bones != null ? 1 : 0;                 
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContextProxy context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            if (BoneMatrices.Bones != null)
            {
                boneCB.UploadDataToBuffer(context, BoneMatrices.Bones, BoneMatricesStruct.NumberOfBones);
            }
        }
    }
}
