/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Render;
    using Shaders;
    using Model;
    using Components;

    /// <summary>
    /// 
    /// </summary>
    public interface IPostEffectOutlineBlur : IPostEffect
    {
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        Color4 Color { set; get; }
        /// <summary>
        /// Gets or sets the scale x.
        /// </summary>
        /// <value>
        /// The scale x.
        /// </value>
        float ScaleX { set; get; }
        /// <summary>
        /// Gets or sets the scale y.
        /// </summary>
        /// <value>
        /// The scale y.
        /// </value>
        float ScaleY { set; get; }
        /// <summary>
        /// Gets or sets the number of blur pass.
        /// </summary>
        /// <value>
        /// The number of blur pass.
        /// </value>
        int NumberOfBlurPass { set; get; }
    }

    /// <summary>
    /// Outline blur effect
    /// <para>Must not put in shared model across multiple viewport, otherwise may causes performance issue if each viewport sizes are different.</para>
    /// </summary>
    public class PostEffectMeshOutlineBlurCore : RenderCoreBase<BorderEffectStruct>, IPostEffectOutlineBlur
    {
        #region Variables
        private SamplerStateProxy sampler;
        private PostEffectBlurCore blurCore;
        private ShaderPass screenQuadPass;

        private ShaderPass blurPassVertical;

        private ShaderPass blurPassHorizontal;

        private ShaderPass screenOutlinePass;

        private int textureSlot;

        private int samplerSlot;

        private const int downSamplingScale = 2;

        private Texture2DDescription depthdesc = new Texture2DDescription
        {
            BindFlags = BindFlags.DepthStencil,
            Format = global::SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
            MipLevels = 1,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            OptionFlags = ResourceOptionFlags.None,
            CpuAccessFlags = CpuAccessFlags.None,
            ArraySize = 1,
        };

        private readonly ConstantBufferComponent modelCB;
        #endregion
        #region Properties
        private string effectName = DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur;
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            set
            {
                SetAffectsCanRenderFlag(ref effectName, value);
            }
            get
            {
                return effectName;
            }
        }

        private Color4 color = Mathematics.Color.Red;
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        public Color4 Color
        {
            set
            {
                SetAffectsRender(ref color, value);
            }
            get { return color; }
        }

        private float scaleX = 1;
        /// <summary>
        /// Gets or sets the scale x.
        /// </summary>
        /// <value>
        /// The scale x.
        /// </value>
        public float ScaleX
        {
            set
            {
                SetAffectsRender(ref scaleX, value);
            }
            get { return scaleX; }
        }

        private float scaleY = 1;
        /// <summary>
        /// Gets or sets the scale y.
        /// </summary>
        /// <value>
        /// The scale y.
        /// </value>
        public float ScaleY
        {
            set
            {
                SetAffectsRender(ref scaleY, value);
            }
            get { return scaleY; }
        }

        private int numberOfBlurPass = 1;
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

        private OutlineMode drawMode = OutlineMode.Merged;
        
        public OutlineMode DrawMode
        {
            set
            {
                SetAffectsRender(ref drawMode, value);
            }
            get { return drawMode; }
        }
        #endregion        

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
        /// </summary>
        public PostEffectMeshOutlineBlurCore() : base(RenderType.PostProc)
        {
            Color = Mathematics.Color.Red;
            modelCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes)));
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            screenQuadPass = technique.GetPass(DefaultPassNames.ScreenQuad);
            blurPassVertical = technique.GetPass(DefaultPassNames.EffectBlurVertical);
            blurPassHorizontal = technique.GetPass(DefaultPassNames.EffectBlurHorizontal);
            screenOutlinePass = technique.GetPass(DefaultPassNames.MeshOutline);
            textureSlot = screenOutlinePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
            samplerSlot = screenOutlinePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
            sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
            blurCore = Collect(new PostEffectBlurCore(global::SharpDX.DXGI.Format.B8G8R8A8_UNorm, blurPassVertical,
                blurPassHorizontal, textureSlot, samplerSlot, DefaultSamplers.LinearSamplerClampAni1, technique.EffectsManager));
            return true;
        }

        protected override bool OnUpdateCanRenderFlag()
        {
            return IsAttached && !string.IsNullOrEmpty(EffectName);
        }

        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            #region Initialize textures
            var buffer = context.RenderHost.RenderBuffer;
            if (depthdesc.Width != buffer.TargetWidth
                || depthdesc.Height != buffer.TargetHeight)
            {
                depthdesc.Width = buffer.TargetWidth;
                depthdesc.Height = buffer.TargetHeight;

                blurCore.Resize(deviceContext,
                    depthdesc.Width / downSamplingScale,
                    depthdesc.Height / downSamplingScale);
                //Skip this frame to avoid performance hit due to texture creation
                InvalidateRenderer();
                return;
            }
            #endregion

            var depthStencilBuffer = buffer.FullResDepthStencilPool.Get(depthdesc.Format);            
            var renderTargetFull = buffer.FullResPPBuffer.NextRTV;
            
            var frustum = context.BoundingFrustum;
            if (drawMode == OutlineMode.Separated)
            {
                for (int i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                {                    
                    #region Render objects onto offscreen texture
                    var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                    if (context.EnableBoundingFrustum && !mesh.TestViewFrustum(ref frustum))
                    {
                        continue;
                    }
                    BindTarget(depthStencilBuffer, renderTargetFull, deviceContext, buffer.TargetWidth, buffer.TargetHeight);
                    deviceContext.ClearDepthStencilView(depthStencilBuffer, DepthStencilClearFlags.Stencil, 0, 0);
                    modelCB.Upload(deviceContext, ref modelStruct);
                    if (mesh.TryGetPostEffect(EffectName, out IEffectAttributes effect))
                    {
                        var color = Color;
                        if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out object attribute) && attribute is string colorStr)
                        {
                            color = colorStr.ToColor4();
                        }
                        if (modelStruct.Color != color)
                        {
                            modelStruct.Color = color;
                            modelCB.Upload(deviceContext, ref modelStruct);
                        }
                        context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                        var pass = mesh.EffectTechnique[DefaultPassNames.EffectOutlineP1];
                        if (pass.IsNULL) { continue; }
                        pass.BindShader(deviceContext);
                        pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                        mesh.RenderCustom(context, deviceContext);
                        DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetFull);
                    }                             
                    #endregion   
                }
            }
            else
            {
                BindTarget(depthStencilBuffer, renderTargetFull, deviceContext, buffer.TargetWidth, buffer.TargetHeight);
                #region Render objects onto offscreen texture
                deviceContext.ClearDepthStencilView(depthStencilBuffer, DepthStencilClearFlags.Stencil, 0, 0);
                bool hasMesh = false;
                for (int i = 0; i < context.RenderHost.PerFrameNodesWithPostEffect.Count; ++i)
                {
                    var mesh = context.RenderHost.PerFrameNodesWithPostEffect[i];
                    if (context.EnableBoundingFrustum && !mesh.TestViewFrustum(ref frustum))
                    {
                        continue;
                    }
                    if (mesh.TryGetPostEffect(EffectName, out IEffectAttributes effect))
                    {
                        var color = Color;
                        if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out object attribute) && attribute is string colorStr)
                        {
                            color = colorStr.ToColor4();
                        }
                        if (modelStruct.Color != color)
                        {
                            modelStruct.Color = color;
                            modelCB.Upload(deviceContext, ref modelStruct);
                        }
                        context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                        var pass = mesh.EffectTechnique[DefaultPassNames.EffectOutlineP1];
                        if (pass.IsNULL) { continue; }
                        pass.BindShader(deviceContext);
                        pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                        mesh.RenderCustom(context, deviceContext);
                        hasMesh = true;
                    }
                }
                #endregion
                if (hasMesh)
                {
                    DrawOutline(context, deviceContext, depthStencilBuffer, renderTargetFull);
                }
            }
            buffer.FullResDepthStencilPool.Put(depthdesc.Format, depthStencilBuffer);
        }

        private void DrawOutline(RenderContext context, DeviceContextProxy deviceContext, 
            ShaderResourceViewProxy depthStencilBuffer, ShaderResourceViewProxy renderTargetFull)
        {
            var buffer = context.RenderHost.RenderBuffer;
            #region Do Blur Pass
            BindTarget(null, blurCore.CurrentRTV, deviceContext, blurCore.Width, blurCore.Height, true);
            blurPassVertical.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            blurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, buffer.FullResPPBuffer.NextSRV);
            blurPassVertical.BindShader(deviceContext);
            blurPassVertical.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.Draw(4, 0);

            blurCore.Run(deviceContext, NumberOfBlurPass, 1, 0);//Already blur once on vertical, pass 1 as initial index.            
            #endregion

            #region Draw back with stencil test
            BindTarget(depthStencilBuffer, renderTargetFull, deviceContext, buffer.TargetWidth, buffer.TargetHeight);
            screenQuadPass.PixelShader.BindTexture(deviceContext, textureSlot, blurCore.CurrentSRV);
            screenQuadPass.BindShader(deviceContext);
            screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.Draw(4, 0);
            #endregion
            #region Draw outline onto original target
            BindTarget(null, buffer.FullResPPBuffer.CurrentRTV, deviceContext, buffer.TargetWidth, buffer.TargetHeight, false);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, buffer.FullResPPBuffer.NextSRV);
            screenOutlinePass.BindShader(deviceContext);
            screenOutlinePass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.Draw(4, 0);
            screenOutlinePass.PixelShader.BindTexture(deviceContext, textureSlot, null);
            #endregion
        }

        protected override void OnDetach()
        {
            depthdesc.Width = depthdesc.Height = 0;
            blurCore = null;
            sampler = null;
            base.OnDetach();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContextProxy context, int width, int height, bool clear = true)
        {
            if (clear)
            {
                context.ClearRenderTargetView(targetView, Mathematics.Color.Transparent);
            }
            context.SetRenderTargets(dsv, new RenderTargetView[] { targetView });
            context.SetViewport(0, 0, width, height);
            context.SetScissorRectangle(0, 0, width, height);
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, RenderContext context)
        {
            model.Param.M11 = scaleX;
            model.Param.M12 = ScaleY;
            modelStruct.Color = color;
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
