using SharpDX;
using SharpDX.Direct3D11;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Utilities;
    using Shaders;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DeviceContextProxy : DisposeObject
    {
        private DeviceContext deviceContext;
        /// <summary>
        /// 
        /// </summary>
        public DeviceContext DeviceContext { get { return deviceContext; } }

        /// <summary>
        /// Gets or sets the last shader pass.
        /// </summary>
        /// <value>
        /// The last shader pass.
        /// </value>
        public ShaderPass LastShaderPass { set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public DeviceContextProxy(DeviceContext context)
        {
            this.deviceContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(DX11RenderBufferProxyBase buffer)
        {
            buffer.SetDefaultRenderTargets(deviceContext);
        }
        /// <summary>
        /// Clears the render targets.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="color">The color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargets(DX11RenderBufferProxyBase buffer, Color4 color)
        {
            buffer.ClearRenderTarget(deviceContext, color);
        }

        /// <summary>
        /// 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTagetBindings()
        {
            deviceContext.OutputMerger.ResetTargets();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            ClearRenderTagetBindings();
            base.OnDispose(disposeManagedResources);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        public static implicit operator DeviceContext(DeviceContextProxy proxy)
        {
            return proxy.DeviceContext;
        }

        /// <summary>
        /// Sets the state of the raster. 
        /// </summary>
        /// <param name="rasterState">State of the raster.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRasterState(RasterizerStateProxy rasterState)
        {
            DeviceContext.Rasterizer.State = rasterState;
        }
        /// <summary>
        /// Sets the state of the depth stencil. 
        /// </summary>
        /// <param name="depthStencilState">State of the depth stencil.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(DepthStencilStateProxy depthStencilState)
        {
            DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState);
        }
        /// <summary>
        /// Sets the state of the depth stencil.
        /// </summary>
        /// <param name="depthStencilState">State of the depth stencil.</param>
        /// <param name="stencilRef">The stencil reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(DepthStencilStateProxy depthStencilState, int stencilRef)
        {
            DeviceContext.OutputMerger.SetDepthStencilState(depthStencilState, stencilRef);
        }
        /// <summary>
        /// Sets the state of the blend. 
        /// </summary>
        /// <param name="blendState">State of the blend.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendStateProxy blendState)
        {
            DeviceContext.OutputMerger.SetBlendState(blendState);
        }

        /// <summary>
        /// Sets the state of the blend.
        /// </summary>
        /// <param name="blendState">State of the blend.</param>
        /// <param name="blendFactor">The blend factor.</param>
        /// <param name="sampleMask">The sample mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendStateProxy blendState, Color4? blendFactor = null, int sampleMask = -1)
        {
            DeviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
        }
        /// <summary>
        /// Sets the state of the blend.
        /// </summary>
        /// <param name="blendState">State of the blend.</param>
        /// <param name="blendFactor">The blend factor.</param>
        /// <param name="sampleMask">The sample mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendStateProxy blendState, Color4? blendFactor, uint sampleMask)
        {
            DeviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
        }
        /// <summary>
        /// Resets this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            LastShaderPass = null;
        }
    }
}
