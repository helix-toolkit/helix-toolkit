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

        private ShaderResourceViewProxy boneSkinSBView;
        private DynamicBufferProxy boneSkinSB;
        private int boneSkinSBSlot;
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
                preComputeBoneSkinPass = technique[DefaultPassNames.PreComputeMeshBoneSkinned];
                boneSkinSBSlot = preComputeBoneSkinPass.VertexShader.ShaderResourceViewMapping.GetMapping(DefaultBufferNames.BoneSkinSB).Slot;
                boneSkinSB = Collect(new DynamicBufferProxy(Matrix.SizeInBytes, global::SharpDX.Direct3D11.BindFlags.ShaderResource, global::SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured));
                boneSkinSB.Initialize(Device, BoneMatricesStruct.DefaultBones, BoneMatricesStruct.NumberOfBones);
                boneSkinSBView = Collect(new ShaderResourceViewProxy(Device, boneSkinSB.Buffer));
                boneSkinSBView.CreateTextureView();
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
            boneSkinSB.UploadDataToBuffer(deviceContext, BoneMatrices.Bones ?? BoneMatricesStruct.DefaultBones, BoneMatricesStruct.NumberOfBones);
            preComputeBoneSkinPass.BindShader(deviceContext);
            deviceContext.SetShaderResource(VertexShader.Type, boneSkinSBSlot, boneSkinSBView);
            deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
            preComputeBoneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            matricsChanged = false;         
        }

        protected override void OnDetach()
        {
            preComputeBoneBuffer = null;
            boneSkinSBView = null;
            boneSkinSB = null;
            base.OnDetach();
        }
    }
}
