/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Render;
    using Shaders;
    using Utilities;
    public class VolumeMaterialVariable<T> : MaterialVariable
    {
        private readonly VolumeTextureMaterialBase<T> material;
        private readonly ShaderPass volumePass;
        private readonly int texSlot;
        private readonly int samplerSlot;
        private ShaderResourceViewProxy texture;
        private SamplerStateProxy sampler;
       

        public Func<VolumeTextureMaterialBase<T>, IEffectsManager, ShaderResourceViewProxy> OnCreateTexture;

        public VolumeMaterialVariable(IEffectsManager manager, IRenderTechnique technique, VolumeTextureMaterialBase<T> material)
            : base(manager, technique, DefaultVolumeConstantBufferDesc, material)
        {
            this.material = material;
            volumePass = technique[DefaultPassNames.Default];
            texSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeTB);
            samplerSlot = volumePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.VolumeSampler);
        }

        protected override void OnInitialPropertyBindings()
        {
            base.OnInitialPropertyBindings();
            AddPropertyBinding(nameof(VolumeTextureMaterialBase<T>.VolumeTexture), () => { UpdateTexture(material); });
            AddPropertyBinding(nameof(VolumeTextureMaterialBase<T>.Sampler), () => 
            {
                RemoveAndDispose(ref sampler);
                sampler = Collect(EffectsManager.StateManager.Register(material.Sampler));
            });
            AddPropertyBinding(nameof(VolumeTextureMaterialBase<T>.StepSize),
                () => WriteValue(VolumeParamsStruct.StepSize, material.StepSize));
            AddPropertyBinding(nameof(VolumeTextureMaterialBase<T>.Iteration),
                () => WriteValue(VolumeParamsStruct.Iterations, material.Iteration));
            AddPropertyBinding(nameof(VolumeTextureMaterialBase<T>.Color),
                () => WriteValue(VolumeParamsStruct.Color, material.Color));
        }

        private void UpdateTexture(VolumeTextureMaterialBase<T> material)
        {
            RemoveAndDispose(ref texture);
            texture = Collect(OnCreateTexture(material, EffectsManager));
        }


        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if(texture != null)
            {
                shaderPass.PixelShader.BindTexture(deviceContext, texSlot, texture);
                shaderPass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return volumePass;
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }
    }
}
