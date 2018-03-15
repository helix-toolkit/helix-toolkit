/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

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
        /// <summary>
        /// Gets or sets the name of the effect.
        /// </summary>
        /// <value>
        /// The name of the effect.
        /// </value>
        public string EffectName
        {
            set; get;
        } = DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur;

        private Color4 color = global::SharpDX.Color.Red;
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

        private IShaderPass screenQuadPass;

        private IShaderPass blurPassVertical;

        private IShaderPass blurPassHorizontal;

        private IShaderPass screenOutlinePass;
        #region Texture Resources

        private ShaderResourceViewProxy renderTargetFull;       

        private ShaderResourceViewProxy depthStencilBuffer;

        private int textureSlot;

        private int samplerSlot;

        private SamplerState sampler;

        private const int downSamplingScale = 2;

        private Texture2DDescription renderTargetDesc = new Texture2DDescription()
        {
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Usage = ResourceUsage.Default,
            ArraySize = 1,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
        };

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

        private ShaderResourceViewDescription targetResourceViewDesc = new ShaderResourceViewDescription()
        {
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Dimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new ShaderResourceViewDescription.Texture2DResource()
            {
                MipLevels = 1,
                MostDetailedMip = 0,
            }
        };

        private RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
        {
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Dimension = RenderTargetViewDimension.Texture2D,
            Texture2D = new RenderTargetViewDescription.Texture2DResource() { MipSlice = 0 }
        };

        private DepthStencilViewDescription depthStencilViewDesc = new DepthStencilViewDescription()
        {
            Format = global::SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
            Flags = DepthStencilViewFlags.None,
            Dimension = DepthStencilViewDimension.Texture2D,
            Texture2D = new DepthStencilViewDescription.Texture2DResource() { MipSlice = 0 }
        };
        #endregion

        private PostEffectBlurCore blurCore;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
        /// </summary>
        public PostEffectMeshOutlineBlurCore() : base(RenderType.PostProc)
        {
            Color = global::SharpDX.Color.Red;
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                screenQuadPass = technique.GetPass(DefaultPassNames.ScreenQuad);
                blurPassVertical = technique.GetPass(DefaultPassNames.EffectBlurVertical);
                blurPassHorizontal = technique.GetPass(DefaultPassNames.EffectBlurHorizontal);
                screenOutlinePass = technique.GetPass(DefaultPassNames.MeshOutline);
                textureSlot = screenOutlinePass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerSlot = screenOutlinePass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni4));
                blurCore = Collect(new PostEffectBlurCore(global::SharpDX.DXGI.Format.B8G8R8A8_UNorm, blurPassVertical,
                    blurPassHorizontal, textureSlot, samplerSlot, DefaultSamplers.LinearSamplerClampAni4, technique.EffectsManager));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender(IRenderContext context)
        {
            return IsAttached && !string.IsNullOrEmpty(EffectName);
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            #region Initialize textures
            if (renderTargetFull == null
                || renderTargetDesc.Width != (int)(context.ActualWidth)
                || renderTargetDesc.Height != (int)(context.ActualHeight))
            {
                depthdesc.Width = renderTargetDesc.Width = (int)(context.ActualWidth);
                depthdesc.Height = renderTargetDesc.Height = (int)(context.ActualHeight);

                RemoveAndDispose(ref renderTargetFull);
                RemoveAndDispose(ref depthStencilBuffer);

                renderTargetFull = Collect(new ShaderResourceViewProxy(deviceContext.DeviceContext.Device, renderTargetDesc));
                renderTargetFull.CreateView(renderTargetViewDesc);
                renderTargetFull.CreateView(targetResourceViewDesc);
                depthStencilBuffer = Collect(new ShaderResourceViewProxy(deviceContext.DeviceContext.Device, depthdesc));
                depthStencilBuffer.CreateView(depthStencilViewDesc);

                blurCore.Resize(deviceContext.DeviceContext.Device,
                    renderTargetDesc.Width / downSamplingScale,
                    renderTargetDesc.Height / downSamplingScale);
            }
            #endregion

            #region Render objects onto offscreen texture
            deviceContext.DeviceContext.ClearDepthStencilView(depthStencilBuffer, DepthStencilClearFlags.Stencil, 0, 0);
            BindTarget(depthStencilBuffer, renderTargetFull, deviceContext, renderTargetDesc.Width, renderTargetDesc.Height);

            context.IsCustomPass = true;
            bool hasMesh = false;
            for (int i = 0; i < context.RenderHost.PerFrameGeneralCoresWithPostEffect.Count; ++i)
            {
                IEffectAttributes effect;
                var mesh = context.RenderHost.PerFrameGeneralCoresWithPostEffect[i];
                if (mesh.TryGetPostEffect(EffectName, out effect))
                {
                    object attribute;
                    var color = Color;
                    if (effect.TryGetAttribute(EffectAttributeNames.ColorAttributeName, out attribute) && attribute is string colorStr)
                    {
                        color = colorStr.ToColor4();
                    }
                    if (modelStruct.Color != color)
                    {
                        modelStruct.Color = color;
                        OnUploadPerModelConstantBuffers(deviceContext);
                    }
                    context.CustomPassName = DefaultPassNames.EffectOutlineP1;
                    var pass = mesh.EffectTechnique[DefaultPassNames.EffectOutlineP1];
                    if (pass.IsNULL) { continue; }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.SetDepthStencilState(pass.DepthStencilState, 1);
                    mesh.Render(context, deviceContext);
                    hasMesh = true;
                }
            }
            context.IsCustomPass = false;
            #endregion
            if (hasMesh)
            {
                deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                deviceContext.DeviceContext.PixelShader.SetSampler(samplerSlot, sampler);
                #region Do Blur Pass
                BindTarget(null, blurCore.CurrentRTV, deviceContext, blurCore.Width, blurCore.Height, true);
                blurPassVertical.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetFull);
                blurPassVertical.BindShader(deviceContext);
                blurPassVertical.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                deviceContext.DeviceContext.Draw(4, 0);

                blurCore.Run(deviceContext, NumberOfBlurPass, 1, 0);//Already blur once on vertical, pass 1 as initial index.            
                #endregion

                #region Draw back with stencil test
                BindTarget(depthStencilBuffer, renderTargetFull, deviceContext, renderTargetDesc.Width, renderTargetDesc.Height);
                screenQuadPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, blurCore.CurrentSRV);
                screenQuadPass.BindShader(deviceContext);
                deviceContext.SetDepthStencilState(screenQuadPass.DepthStencilState, 0);
                screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState);
                deviceContext.DeviceContext.Draw(4, 0);
                #endregion

                #region Draw outline onto original target
                context.RenderHost.SetDefaultRenderTargets(false);
                screenOutlinePass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetFull);
                screenOutlinePass.BindShader(deviceContext);
                screenOutlinePass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                deviceContext.DeviceContext.Draw(4, 0);
                screenOutlinePass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, null);
                #endregion
            }
            else
            {
                context.RenderHost.SetDefaultRenderTargets(false);
            }
        }

        protected override void OnDetach()
        {
            renderTargetFull = null;
            renderTargetDesc.Width = renderTargetDesc.Height = 0;
            blurCore = null;
            base.OnDetach();
        }

        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContext context, int width, int height, bool clear = true)
        {
            if (clear)
            {
                context.ClearRenderTargetView(targetView, global::SharpDX.Color.Transparent);
            }
            context.OutputMerger.SetRenderTargets(dsv, new RenderTargetView[] { targetView });
            context.Rasterizer.SetViewport(0, 0, width, height);
            context.Rasterizer.SetScissorRectangle(0, 0, width, height);
        }

        protected override void OnUpdatePerModelStruct(ref BorderEffectStruct model, IRenderContext context)
        {
            model.Param.M11 = scaleX;
            model.Param.M12 = ScaleY;
            modelStruct.Color = color;
        }
    }
}
