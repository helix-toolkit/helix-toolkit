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

        private ConstantBufferProxy boneCB;
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
                boneCB = technique.ConstantBufferPool.Register(new ConstantBufferDescription(DefaultBufferNames.BoneCB, BoneMatricesStruct.SizeInBytes));
                preComputeBoneSkinPass = technique[DefaultPassNames.PreComputeMeshBoneSkinned];
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
            boneCB.UploadDataToBuffer(deviceContext, BoneMatrices.Bones ?? BoneMatricesStruct.DefaultBones, BoneMatricesStruct.NumberOfBones);
            preComputeBoneSkinPass.BindShader(deviceContext);
            deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
            preComputeBoneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            matricsChanged = false;         
        }

        protected override void OnDetach()
        {
            preComputeBoneBuffer = null;
            base.OnDetach();
        }
    }
}
