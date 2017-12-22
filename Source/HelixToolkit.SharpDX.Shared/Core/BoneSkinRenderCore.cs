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
    public class BoneSkinRenderCore : PatchMeshRenderCore
    {
        public IElementsBufferModel VertexBoneIdBuffer { set; get; }

        public BoneMatricesStruct BoneMatrices
        {
            set;get;
        }

        private IBufferProxy boneCB;

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

        protected override bool CanRender()
        {
            return base.CanRender()&& VertexBoneIdBuffer != null && VertexBoneIdBuffer.HasElements;
        }

        protected override void OnAttachBuffers(DeviceContext context)
        {
            base.OnAttachBuffers(context);         
            VertexBoneIdBuffer?.AttachBuffer(context, 2);
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            base.OnUpdatePerModelStruct(ref model, context);     
            model.HasBones = BoneMatrices.Bones != null ? 1 : 0;                 
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            if (BoneMatrices.Bones != null)
            {
                boneCB.UploadDataToBuffer(context, BoneMatrices.Bones);
            }
        }
    }
}
