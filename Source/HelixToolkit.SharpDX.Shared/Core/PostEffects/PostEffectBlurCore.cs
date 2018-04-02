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
    using global::SharpDX;
    using Render;
    using Shaders;
    using Utilities;

    /// <summary>
    ///
    /// </summary>
    public class PostEffectBlurCore : DisposeObject
    {
        /// <summary>
        /// Gets the current ShaderResourceView.
        /// </summary>
        /// <value>
        /// The current SRV.
        /// </value>
        public ShaderResourceView CurrentSRV { get { return renderTargetBlur[0].TextureView; } }

        /// <summary>
        /// Gets the next SRV.
        /// </summary>
        /// <value>
        /// The next SRV.
        /// </value>
        public ShaderResourceView NextSRV { get { return renderTargetBlur[1].TextureView; } }

        public int Width { get { return texture2DDesc.Width; } }

        public int Height { get { return texture2DDesc.Height; } }

        /// <summary>
        /// Gets the current RenderTargetView.
        /// </summary>
        /// <value>
        /// The current RTV.
        /// </value>
        public RenderTargetView CurrentRTV { get { return renderTargetBlur[0].RenderTargetView; } }

        /// <summary>
        /// Gets the next RTV.
        /// </summary>
        /// <value>
        /// The next RTV.
        /// </value>
        public RenderTargetView NextRTV { get { return renderTargetBlur[1].RenderTargetView; } }

        private IShaderPass screenBlurPassVertical;

        private IShaderPass screenBlurPassHorizontal;

        #region Texture Resources

        private const int NumPingPongBlurBuffer = 2;

        private ShaderResourceViewProxy[] renderTargetBlur = new ShaderResourceViewProxy[NumPingPongBlurBuffer];

        private int textureSlot;

        private int samplerSlot;

        private SamplerState sampler;

        private Texture2DDescription texture2DDesc = new Texture2DDescription()
        {
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            Usage = ResourceUsage.Default,
            ArraySize = 1,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
        };

        private ShaderResourceViewDescription targetResourceViewDesc = new ShaderResourceViewDescription()
        {
            Dimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new ShaderResourceViewDescription.Texture2DResource()
            {
                MipLevels = 1,
                MostDetailedMip = 0,
            }
        };

        private RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
        {
            Dimension = RenderTargetViewDimension.Texture2D,
            Texture2D = new RenderTargetViewDescription.Texture2DResource() { MipSlice = 0 }
        };

        #endregion Texture Resources

        /// <summary>
        /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
        /// </summary>
        public PostEffectBlurCore(global::SharpDX.DXGI.Format textureFormat,
            IShaderPass blurVerticalPass, IShaderPass blurHorizontalPass, int textureSlot, int samplerSlot,
            SamplerStateDescription sampler, IEffectsManager manager)
        {
            screenBlurPassVertical = blurVerticalPass;
            screenBlurPassHorizontal = blurHorizontalPass;
            this.textureSlot = textureSlot;
            this.samplerSlot = samplerSlot;
            this.sampler = Collect(manager.StateManager.Register(sampler));
            texture2DDesc.Format = targetResourceViewDesc.Format = renderTargetViewDesc.Format = textureFormat;
        }
        /// <summary>
        /// Resizes the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Resize(Device device, int width, int height)
        {
            if (texture2DDesc.Width != width || texture2DDesc.Height != height)
            {
                texture2DDesc.Width = width;
                texture2DDesc.Height = height;

                for (int i = 0; i < NumPingPongBlurBuffer; ++i)
                {
                    RemoveAndDispose(ref renderTargetBlur[i]);
                    renderTargetBlur[i] = Collect(new ShaderResourceViewProxy(device, texture2DDesc));
                    renderTargetBlur[i].CreateView(renderTargetViewDesc);
                    renderTargetBlur[i].CreateView(targetResourceViewDesc);
                }
            }
        }
        /// <summary>
        /// Runs the specified device context.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="iteration">The iteration.</param>
        /// <param name="initVerticalIter">The initialize vertical iter.</param>
        /// <param name="initHorizontalIter">The initialize horizontal iter.</param>
        public virtual void Run(DeviceContextProxy deviceContext, int iteration, int initVerticalIter = 0, int initHorizontalIter = 0)
        {
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            deviceContext.DeviceContext.PixelShader.SetSampler(samplerSlot, sampler);
            if (!screenBlurPassVertical.IsNULL)
            {
                screenBlurPassVertical.BindShader(deviceContext);
                screenBlurPassVertical.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                for (int i = initVerticalIter; i < iteration; ++i)
                {
                    SwapTargets();
                    BindTarget(null, renderTargetBlur[0], deviceContext, texture2DDesc.Width, texture2DDesc.Height);
                    screenBlurPassVertical.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetBlur[1].TextureView);
                    deviceContext.DeviceContext.Draw(4, 0);
                }
            }

            if (!screenBlurPassHorizontal.IsNULL)
            {
                screenBlurPassHorizontal.BindShader(deviceContext);
                screenBlurPassHorizontal.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
                for (int i = initHorizontalIter; i < iteration; ++i)
                {
                    SwapTargets();
                    BindTarget(null, renderTargetBlur[0], deviceContext, texture2DDesc.Width, texture2DDesc.Height);
                    screenBlurPassHorizontal.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetBlur[1].TextureView);
                    deviceContext.DeviceContext.Draw(4, 0);
                }
            }
            deviceContext.DeviceContext.PixelShader.SetShaderResource(textureSlot, null);
        }
        /// <summary>
        /// Swaps the targets.
        /// </summary>
        public void SwapTargets()
        {
            //swap buffer
            var current = renderTargetBlur[0];
            renderTargetBlur[0] = renderTargetBlur[1];
            renderTargetBlur[1] = current;
        }

        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContext context, int width, int height)
        {
            //context.ClearRenderTargetView(targetView, Color.White);
            context.OutputMerger.SetRenderTargets(dsv, new RenderTargetView[] { targetView });
            context.Rasterizer.SetViewport(0, 0, width, height);
            context.Rasterizer.SetScissorRectangle(0, 0, width, height);
        }
        /// <summary>
        /// Clears the targets.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="c">The c.</param>
        public void ClearTargets(DeviceContext context, Color c)
        {
            foreach (var target in renderTargetBlur)
            {
                context.ClearRenderTargetView(target, c);
            }
        }
    }
}