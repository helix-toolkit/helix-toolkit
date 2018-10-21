/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    public sealed class VolumeMaterialVariable : MaterialVariable
    {
        private readonly VolumeTextureMaterial material;
        private readonly ShaderPass volumePass;
        private readonly int texSlot;
        private readonly int samplerSlot;
        private ShaderResourceViewProxy texture;
        private SamplerStateProxy sampler;

        public VolumeMaterialVariable(IEffectsManager manager, IRenderTechnique technique, VolumeTextureMaterial material)
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
            AddPropertyBinding(nameof(VolumeTextureMaterial.VolumeTexture), () => { UpdateTexture(); });
            AddPropertyBinding(nameof(VolumeTextureMaterial.Sampler), () => 
            {
                RemoveAndDispose(ref sampler);
                sampler = Collect(EffectsManager.StateManager.Register(material.Sampler));
            });
        }

        private void UpdateTexture()
        {
            RemoveAndDispose(ref texture);
            if(material.VolumeTexture != null)
            {
                texture = Collect(ShaderResourceViewProxy.CreateView(EffectsManager.Device, material.VolumeTexture));
            }
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
