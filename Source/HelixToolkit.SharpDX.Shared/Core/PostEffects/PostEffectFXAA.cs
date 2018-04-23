/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Render;
    using Shaders;

    public sealed class PostEffectFXAA : RenderCoreBase<BorderEffectStruct>, IPostEffect
    {
        public string EffectName { set; get; } = DefaultRenderTechniqueNames.PostEffectFXAA;

        private int textureSlot;
        private int samplerSlot;
        private SamplerStateProxy sampler;
        private IShaderPass FXAAPass;

        public PostEffectFXAA() : base(RenderType.PostProc) { }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                FXAAPass = technique[DefaultRenderTechniqueNames.PostEffectFXAA];
                textureSlot = FXAAPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerSlot = FXAAPass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni4));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, IRenderContext context)
        {
            model.Color.Red = (float)(1 / context.ActualWidth);
            model.Color.Green = (float)(1 / context.ActualHeight);
        }
    }
}
