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
        private ShaderPass boneSkinPass;

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
                boneSkinPass = technique[DefaultPassNames.MeshBoneSkinned];
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (boneSkinPass.IsNULL || !matricsChanged)
            {
                return;
            }
            GeometryBuffer.UpdateBuffers(deviceContext, EffectTechnique.EffectsManager);
            if (GeometryBuffer is IBoneSkinMeshBufferModel boneBuffer)
            {
                boneBuffer.BindSkinnedVertexBufferToOutput(deviceContext);
                if (BoneMatrices.Bones != null)
                {
                    boneCB.UploadDataToBuffer(deviceContext, BoneMatrices.Bones, BoneMatricesStruct.NumberOfBones);
                }
                boneSkinPass.BindShader(deviceContext);
                deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
                boneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            }
            matricsChanged = false;
        }
    }
}
