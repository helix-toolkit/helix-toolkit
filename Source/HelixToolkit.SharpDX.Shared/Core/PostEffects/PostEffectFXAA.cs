/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;
    using Components;

    public sealed class PostEffectFXAA : RenderCoreBase<BorderEffectStruct>, IPostEffect
    {
        private string effectName = DefaultRenderTechniqueNames.PostEffectFXAA;
        public string EffectName
        {
            set { SetAffectsCanRenderFlag(ref effectName, value); }
            get { return effectName; }
        }

        private FXAALevel fxaaLevel = FXAALevel.None;
        /// <summary>
        /// Gets or sets the fxaa level.
        /// </summary>
        /// <value>
        /// The fxaa level.
        /// </value>
        public FXAALevel FXAALevel
        {
            set { SetAffectsCanRenderFlag(ref fxaaLevel, value); }
            get { return fxaaLevel; }
        }

        private int textureSlot;
        private int samplerSlot;
        private SamplerStateProxy sampler;
        private ShaderPass FXAAPass;
        private ShaderPass LUMAPass;
        private readonly ConstantBufferComponent modelCB;

        public PostEffectFXAA() : base(RenderType.PostProc)
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            FXAAPass = technique[DefaultPassNames.FXAAPass];
            LUMAPass = technique[DefaultPassNames.LumaPass];
            textureSlot = FXAAPass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
            samplerSlot = FXAAPass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
            sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
            return true;
        }

        protected override void OnDetach()
        {
            sampler = null;
            base.OnDetach();
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return IsAttached && !string.IsNullOrEmpty(EffectName) && FXAALevel != FXAALevel.None;
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            var buffer = context.RenderHost.RenderBuffer;
            deviceContext.SetRenderTargets(null, new RenderTargetView[] { buffer.FullResPPBuffer.NextRTV });
            deviceContext.SetViewport(0, 0, buffer.TargetWidth, buffer.TargetHeight, 0.0f, 1.0f);
            deviceContext.SetScissorRectangle(0, 0, buffer.TargetWidth, buffer.TargetHeight);
            modelCB.Upload(deviceContext, ref modelStruct);
            LUMAPass.BindShader(deviceContext);
            LUMAPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState | StateType.RasterState);
            LUMAPass.PixelShader.BindTexture(deviceContext, textureSlot, buffer.FullResPPBuffer.CurrentSRV);
            LUMAPass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            deviceContext.Draw(4, 0);
           
            deviceContext.SetRenderTargets(null, new RenderTargetView[] { buffer.FullResPPBuffer.CurrentRTV });
            FXAAPass.BindShader(deviceContext);
            FXAAPass.PixelShader.BindTexture(deviceContext, textureSlot, buffer.FullResPPBuffer.NextSRV);
            deviceContext.Draw(4, 0);
            FXAAPass.PixelShader.BindTexture(deviceContext, textureSlot, null);
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, RenderContext context)
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
        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
