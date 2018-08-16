/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using System;
    using Utilities;
    using Components;

    public interface IPostEffectBloom : IPostEffect
    {
        Color4 ThresholdColor { set; get; }
        float BloomExtractIntensity { set; get; }
        float BloomPassIntensity { set; get; }

        float BloomCombineSaturation { set; get; }

        float BloomCombineIntensity { set; get; }
        int NumberOfBlurPass { set; get; }
        int MaximumDownSamplingStep { set; get; }
    }
    /// <summary>
    /// Outline blur effect
    /// <para>Must not put in shared model across multiple viewport, otherwise may causes performance issue if each viewport sizes are different.</para>
    /// </summary>
    public class PostEffectBloomCore : RenderCoreBase<BorderEffectStruct>, IPostEffectBloom
    {
        #region Variables
        private SamplerStateProxy sampler;
        private ShaderPass screenQuadPass;

        private ShaderPass screenQuadCopy;

        private ShaderPass blurPassVertical;

        private ShaderPass blurPassHorizontal;

        private ShaderPass screenOutlinePass;

        private readonly List<PostEffectBlurCore> offScreenRenderTargets = new List<PostEffectBlurCore>();

        private int textureSlot;

        private int samplerSlot;
      
        private int width, height;

        private readonly ConstantBufferComponent modelCB;
        #endregion
        #region Properties   
        private string effectName = DefaultRenderTechniqueNames.PostEffectBloom;
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            set { SetAffectsCanRenderFlag(ref effectName, value); }
            get { return effectName; }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        public Color4 ThresholdColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.Color, value);
            }
            get { return modelStruct.Color; }
        }

        public float BloomExtractIntensity
        {
            set
            {
                SetAffectsRender(ref modelStruct.Param.M11, value);
            }
            get { return modelStruct.Param.M11; }
        }

        public float BloomPassIntensity
        {
            set
            {
                SetAffectsRender(ref modelStruct.Param.M12, value);
            }
            get { return modelStruct.Param.M12; }
        }

        public float BloomCombineSaturation
        {
            set
            {
                SetAffectsRender(ref modelStruct.Param.M13, value);
            }
            get { return modelStruct.Param.M13; }
        }

        public float BloomCombineIntensity
        {
            set
            {
                SetAffectsRender(ref modelStruct.Param.M14, value);
            }
            get { return modelStruct.Param.M14; }
        }

        private int numberOfBlurPass = 2;
        /// <summary>
        /// Gets or sets the number of blur pass.
        /// </summary>
        /// <value>
        /// The number of blur pass.
        /// </value>
        public int NumberOfBlurPass
        {
            set
            {
                SetAffectsRender(ref numberOfBlurPass, value);
            }
            get { return numberOfBlurPass; }
        }

        private int maximumDownSamplingStep = 3;
        /// <summary>
        /// Gets or sets the maximum down sampling step.
        /// </summary>
        /// <value>
        /// The maximum down sampling step.
        /// </value>
        public int MaximumDownSamplingStep
        {
            set
            {
                SetAffectsRender(ref maximumDownSamplingStep, value);
            }
            get { return maximumDownSamplingStep; }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
        /// </summary>
        public PostEffectBloomCore() : base(RenderType.PostProc)
        {
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
            ThresholdColor = new Color4(0.8f, 0.8f, 0.8f, 0f);
            BloomExtractIntensity = 1f;
            BloomPassIntensity = 0.95f;
            BloomCombineIntensity = 0.7f;
            BloomCombineSaturation = 0.7f;
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            screenQuadPass = technique.GetPass(DefaultPassNames.ScreenQuad);
            screenQuadCopy = technique.GetPass(DefaultPassNames.ScreenQuadCopy);
            blurPassVertical = technique.GetPass(DefaultPassNames.EffectBlurVertical);
            blurPassHorizontal = technique.GetPass(DefaultPassNames.EffectBlurHorizontal);
            screenOutlinePass = technique.GetPass(DefaultPassNames.MeshOutline);
            textureSlot = screenOutlinePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
            samplerSlot = screenOutlinePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
            sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
            return true;
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return IsAttached && !string.IsNullOrEmpty(EffectName);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            var buffer = context.RenderHost.RenderBuffer;
            #region Initialize textures
            if (offScreenRenderTargets.Count == 0
                || width != (int)(context.ActualWidth)
                || height != (int)(context.ActualHeight))
            {
                width = (int)(context.ActualWidth);
                height = (int)(context.ActualHeight);
                for (int i = 0; i < offScreenRenderTargets.Count; ++i)
                {
                    var target = offScreenRenderTargets[i];
                    RemoveAndDispose(ref target);
                }
                offScreenRenderTargets.Clear();

                int w = width;
                int h = height;
                int count = 0;
                while(w > 1 && h > 1 && count < Math.Max(0, MaximumDownSamplingStep) + 1)
                {
                    var target = Collect(new PostEffectBlurCore(global::SharpDX.DXGI.Format.B8G8R8A8_UNorm, blurPassVertical, blurPassHorizontal, textureSlot, samplerSlot,
                        DefaultSamplers.LinearSamplerClampAni1, EffectTechnique.EffectsManager));
                    target.Resize(Device, w, h);
                    offScreenRenderTargets.Add(target);
                    w >>= 2;
                    h >>= 2;
                    ++count;
                }
                //Skip this frame to avoid performance hit due to texture creation
                InvalidateRenderer();
                return;
            }
            #endregion

            #region Render objects onto offscreen texture    

            using (var resource2 = offScreenRenderTargets[0].CurrentRTV.Resource)
            {                   
                deviceContext.CopyResource(buffer.FullResPPBuffer.CurrentTexture, resource2);
            }
            #endregion
            #region Do Bloom Pass
            modelCB.Upload(deviceContext, ref modelStruct);
            //Extract bloom samples
            BindTarget(null, offScreenRenderTargets[0].NextRTV, deviceContext, offScreenRenderTargets[0].Width, offScreenRenderTargets[0].Height, false);
            screenQuadPass.PixelShader.BindTexture(deviceContext, textureSlot, offScreenRenderTargets[0].CurrentSRV);
            screenQuadPass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            screenQuadPass.BindShader(deviceContext);
            screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.Draw(4, 0);
            offScreenRenderTargets[0].SwapTargets();

            // Down sampling
            screenQuadCopy.BindShader(deviceContext);
            screenQuadCopy.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            for (int i = 1; i < offScreenRenderTargets.Count; ++i)
            {
                BindTarget(null, offScreenRenderTargets[i].CurrentRTV, deviceContext, offScreenRenderTargets[i].Width, offScreenRenderTargets[i].Height, false);
                screenQuadCopy.PixelShader.BindTexture(deviceContext, textureSlot, offScreenRenderTargets[i - 1].CurrentSRV);
                deviceContext.Draw(4, 0);
            }

            for (int i = offScreenRenderTargets.Count - 1; i >= 1; --i)
            {
                //Run blur pass
                offScreenRenderTargets[i].Run(deviceContext, NumberOfBlurPass);

                //Up sampling
                screenOutlinePass.BindShader(deviceContext);
                screenOutlinePass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                BindTarget(null, offScreenRenderTargets[i - 1].CurrentRTV, deviceContext, offScreenRenderTargets[i - 1].Width, offScreenRenderTargets[i - 1].Height, false);
                screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, offScreenRenderTargets[i].CurrentSRV);
                deviceContext.Draw(4, 0);
            }
            offScreenRenderTargets[0].Run(deviceContext, NumberOfBlurPass);
            #endregion

            #region Draw outline onto original target
            BindTarget(null, buffer.FullResPPBuffer.CurrentRTV, deviceContext, buffer.TargetWidth, buffer.TargetHeight, false);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, offScreenRenderTargets[0].CurrentSRV);
            screenOutlinePass.BindShader(deviceContext);
            screenOutlinePass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.Draw(4, 0);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, null);
            #endregion
        }

        protected override void OnDetach()
        {
            sampler = null;
            width = height = 0;
            offScreenRenderTargets.Clear();
            base.OnDetach();
        }

        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContextProxy context, int width, int height, bool clear = true)
        {
            if (clear)
            {
                context.ClearRenderTargetView(targetView, Color.Transparent);
            }
            context.SetRenderTargets(dsv, new RenderTargetView[] { targetView });
            context.SetViewport(0, 0, width, height);
            context.SetScissorRectangle(0, 0, width, height);
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, RenderContext context)
        {
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
