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
    using global::SharpDX.DXGI;

    public sealed class PostEffectFXAA : RenderCoreBase<BorderEffectStruct>, IPostEffect
    {
        public string EffectName { set; get; } = DefaultRenderTechniqueNames.PostEffectFXAA;

        public FXAALevel FXAALevel
        {
            set; get;
        } = FXAALevel.Medium;

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
                FXAAPass = technique[DefaultPassNames.Default];
                textureSlot = FXAAPass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerSlot = FXAAPass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            deviceContext.DeviceContext.Flush();
            var buffer = context.RenderHost.RenderBuffer;
            
            if (buffer.ColorBufferSampleDesc.Count > 1 || FXAALevel == FXAALevel.None)
            {
                Device.ImmediateContext.ResolveSubresource(buffer.ColorBuffer.Resource, 0, buffer.BackBuffer.Resource, 0, Format.B8G8R8A8_UNorm);
            }
            else
            {
                //deviceContext.DeviceContext.ClearRenderTargetView(buffer.BackBuffer, new Color4(0, 0, 0, 1));
                buffer.SetDefaultRenderTargets(deviceContext, false);
                FXAAPass.BindShader(deviceContext);
                FXAAPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
                FXAAPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, buffer.ColorBuffer);
                FXAAPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerSlot, sampler);
                deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                deviceContext.DeviceContext.Draw(4, 0);
                FXAAPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, null);
            }
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, IRenderContext context)
        {
            model.Color.Red = (float)(1 / context.ActualWidth);
            model.Color.Green = (float)(1 / context.ActualHeight);
            switch (FXAALevel)
            {
                case FXAALevel.Low:
                    model.Param.M11 = 0.25f; //fxaaQualitySubpix
                    model.Param.M12 = 0.250f; // FxaaFloat fxaaQualityEdgeThreshold,
                    model.Param.M13 = 0.0833f; // FxaaFloat fxaaQualityEdgeThresholdMin,
                    break;
                case FXAALevel.Medium:
                    model.Param.M11 = 0.50f;
                    model.Param.M12 = 0.166f;
                    model.Param.M13 = 0.0625f;
                    break;
                case FXAALevel.High:
                    model.Param.M11 = 0.75f;
                    model.Param.M12 = 0.125f;
                    model.Param.M13 = 0.0625f;
                    break;
                case FXAALevel.Ultra:
                    model.Param.M11 = 1.00f;
                    model.Param.M12 = 0.063f;
                    model.Param.M13 = 0.0312f;
                    break;
            }
        }
    }
}
