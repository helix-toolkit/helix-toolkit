/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
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
        private readonly VolumeTextureMaterialCoreBase<T> material;
        private readonly ShaderPass volumePass;
        private readonly int texSlot, gradientSlot;
        private readonly int samplerSlot;
        private ShaderResourceViewProxy texture;
        private ShaderResourceViewProxy gradientMap;
        private SamplerStateProxy sampler;
       

        public Func<VolumeTextureMaterialCoreBase<T>, IEffectsManager, ShaderResourceViewProxy> OnCreateTexture;

        public VolumeMaterialVariable(IEffectsManager manager, IRenderTechnique technique, VolumeTextureMaterialCoreBase<T> material, 
            string volumePassName = DefaultPassNames.Default)
            : base(manager, technique, DefaultVolumeConstantBufferDesc, material)
        {
            this.material = material;
            volumePass = technique[volumePassName];
            texSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.VolumeTB);
            gradientSlot = volumePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.ColorStripe1DXTB);
            samplerSlot = volumePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.VolumeSampler);
        }

        protected override void OnInitialPropertyBindings()
        {
            base.OnInitialPropertyBindings();
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.VolumeTexture), () => { UpdateTexture(material); });
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.Sampler), () => 
            {
                RemoveAndDispose(ref sampler);
                sampler = Collect(EffectsManager.StateManager.Register(material.Sampler));
            });
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.SampleDistance),
                () => UpdateStepSize());
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.MaxIterations),
                () => UpdateStepSize());
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.Color),
                () => WriteValue(VolumeParamsStruct.Color, material.Color));
            AddPropertyBinding(nameof(VolumeTextureMaterialCoreBase<T>.GradientMap),
                () => UpdateGradientMap());
        }

        private void UpdateStepSize()
        {
            if(texture != null && texture.TextureView != null && texture.Resource is Texture3D res)
            {
                var desc = res.Description;
                var steps = new Vector3(1f / desc.Width, 1f / desc.Height, 1f / desc.Depth);
                steps *= material.SampleDistance;
                var maxSize = Math.Max(desc.Width, Math.Max(desc.Height, desc.Depth));
                var iteration = (int)Math.Min(material.MaxIterations, maxSize / material.SampleDistance);
                var scale = Vector4.One / ((Vector4.One * maxSize) / new Vector4(desc.Width, desc.Height, desc.Depth, 1));
                scale.W = 1;
                WriteValue(VolumeParamsStruct.Iterations, iteration);
                WriteValue(VolumeParamsStruct.StepSize, steps);
                WriteValue(VolumeParamsStruct.ActualSampleDistance, material.SampleDistance);
                WriteValue(VolumeParamsStruct.BaseSampleDistance, 1.0f);
                WriteValue(VolumeParamsStruct.ScaleFactor, scale);
            }            
        }

        private void UpdateTexture(VolumeTextureMaterialCoreBase<T> material)
        {
            RemoveAndDispose(ref texture);
            texture = Collect(OnCreateTexture(material, EffectsManager));
            if(texture != null)
            {
                UpdateStepSize();
            }
        }

        public void UpdateGradientMap()
        {
            RemoveAndDispose(ref gradientMap);
            if(material.GradientMap != null)
            {
                gradientMap = Collect(ShaderResourceViewProxy.CreateViewFromColorArray(EffectsManager.Device, material.GradientMap));
            }
            WriteValue(VolumeParamsStruct.HasGradientMapX, material.GradientMap != null);
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            if(texture != null)
            {
                shaderPass.PixelShader.BindTexture(deviceContext, texSlot, texture);
                shaderPass.PixelShader.BindTexture(deviceContext, gradientSlot, gradientMap);
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
