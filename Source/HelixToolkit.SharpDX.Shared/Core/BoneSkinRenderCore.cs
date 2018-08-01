/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;

    public class BoneSkinRenderCore : MeshRenderCore
    {
        private bool matricsChanged = true;
        private BoneMatricesStruct boneMatrices;
        public BoneMatricesStruct BoneMatrices
        {
            set
            {
                if(SetAffectsRender(ref boneMatrices, value))
                {
                    matricsChanged = true;
                }
            }
            get { return boneMatrices; }
        }

        private ShaderResourceViewProxy boneSkinTBView;
        private DynamicBufferProxy boneSkinTB;
        private int boneSkinTBSlot;
        private ShaderPass preComputeBoneSkinPass;
        private IBoneSkinPreComputehBufferModel preComputeBoneBuffer;
        

        public BoneSkinRenderCore()
        {
            NeedUpdate = true;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                matricsChanged = true;
                //boneCB = technique.ConstantBufferPool.Register(new ConstantBufferDescription(DefaultBufferNames.BoneCB, BoneMatricesStruct.SizeInBytes));
                preComputeBoneSkinPass = technique[DefaultPassNames.PreComputeMeshBoneSkinned];
                boneSkinTBSlot = preComputeBoneSkinPass.VertexShader.ShaderResourceViewMapping.GetMapping(DefaultBufferNames.BoneSkinSB).Slot;
                boneSkinTB = Collect(new DynamicBufferProxy(Matrix.SizeInBytes, global::SharpDX.Direct3D11.BindFlags.ShaderResource, global::SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnGeometryBufferChanged(IAttachableBufferModel buffer)
        {
            base.OnGeometryBufferChanged(buffer);
            preComputeBoneBuffer = buffer as IBoneSkinPreComputehBufferModel;
        }

        protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (preComputeBoneSkinPass.IsNULL || preComputeBoneBuffer == null || !preComputeBoneBuffer.CanPreCompute || !matricsChanged)
            {
                return;
            }
            GeometryBuffer.UpdateBuffers(deviceContext, EffectTechnique.EffectsManager);
            preComputeBoneBuffer.BindSkinnedVertexBufferToOutput(deviceContext);
            boneSkinTB.UploadDataToBuffer(deviceContext, BoneMatrices.Bones ?? BoneMatricesStruct.DefaultBones, BoneMatricesStruct.NumberOfBones);
            preComputeBoneSkinPass.BindShader(deviceContext);
            if(boneSkinTBView == null)
            {
                boneSkinTBView = Collect(new ShaderResourceViewProxy(Device, boneSkinTB.Buffer));
                boneSkinTBView.CreateTextureView();
            }
            deviceContext.SetShaderResource(VertexShader.Type, boneSkinTBSlot, boneSkinTBView);
            deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
            preComputeBoneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            matricsChanged = false;         
        }

        protected override void OnDetach()
        {
            preComputeBoneBuffer = null;
            boneSkinTBView = null;
            boneSkinTB = null;
            base.OnDetach();
        }
    }
}
