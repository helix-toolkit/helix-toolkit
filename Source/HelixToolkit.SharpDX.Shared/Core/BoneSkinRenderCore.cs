/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;

    public class BoneSkinRenderCore : MeshRenderCore
    {       
        private bool matricsChanged = true;
        public Matrix[] BoneMatrices
        {
            set
            {
                internalBoneBuffer.BoneMatrices = value;
            }
            get { return internalBoneBuffer.BoneMatrices; }
        }

        private BoneUploaderCore sharedBoneBuffer;
        public BoneUploaderCore SharedBoneBuffer
        {
            set
            {
                var old = sharedBoneBuffer;
                if(Set(ref sharedBoneBuffer, value))
                {
                    if (old != null)
                    {
                        old.OnBoneChanged -= OnBoneChanged;
                    }
                    if (value != null)
                    {
                        value.OnBoneChanged += OnBoneChanged;
                    }
                    matricsChanged = true;
                }
            }
            get { return sharedBoneBuffer; }
        }

        private int boneSkinSBSlot;
        private ShaderPass preComputeBoneSkinPass;
        private IBoneSkinPreComputehBufferModel preComputeBoneBuffer;
        private readonly BoneUploaderCore internalBoneBuffer = new BoneUploaderCore();

        public BoneSkinRenderCore()
        {
            NeedUpdate = true;
            internalBoneBuffer.OnBoneChanged += OnBoneChanged;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                matricsChanged = true;
                preComputeBoneSkinPass = technique[DefaultPassNames.PreComputeMeshBoneSkinned];
                boneSkinSBSlot = preComputeBoneSkinPass.VertexShader.ShaderResourceViewMapping.GetMapping(DefaultBufferNames.BoneSkinSB).Slot;
                internalBoneBuffer.Attach(technique);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnBoneChanged(object sender, System.EventArgs e)
        {
            matricsChanged = true;
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
            var buffer = sharedBoneBuffer ?? internalBoneBuffer;

            if(buffer.BoneMatrices.Length == 0)
            {
                preComputeBoneBuffer.ResetSkinnedVertexBuffer(deviceContext);
            }
            else
            {
                GeometryBuffer.UpdateBuffers(deviceContext, EffectTechnique.EffectsManager);
                preComputeBoneBuffer.BindSkinnedVertexBufferToOutput(deviceContext);
                buffer.Update(context, deviceContext);
                preComputeBoneSkinPass.BindShader(deviceContext);
                buffer.BindBuffer(deviceContext, boneSkinSBSlot);
                deviceContext.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
                preComputeBoneBuffer.UnBindSkinnedVertexBufferToOutput(deviceContext);
            }
            matricsChanged = false;         
        }

        protected override void OnDetach()
        {
            preComputeBoneBuffer = null;
            internalBoneBuffer.Detach();
            base.OnDetach();
        }
    }
}
