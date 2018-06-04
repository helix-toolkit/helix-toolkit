using SharpDX.Direct3D;
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
    using Shaders;
    using System.Runtime.CompilerServices;
    using Utilities;

    /// <summary>
    ///
    /// </summary>
    public sealed class DeviceContextProxy : DisposeObject
    {
        public static bool AutoSkipRedundantStateSetting = false;
        private readonly DeviceContext deviceContext;
        private readonly Device device;
        private RasterizerStateProxy currRasterState = null;
        private DepthStencilStateProxy currDepthStencilState = null;
        private int currStencilRef;
        private BlendStateProxy currBlendState = null;
        private Color4? currBlendFactor = null;
        private uint currSampleMask = uint.MaxValue;

        public readonly bool IsDeferred = false;

        #region Properties

        /// <summary>
        /// Gets or sets the last shader pass.
        /// </summary>
        /// <value>
        /// The last shader pass.
        /// </value>
        public ShaderPass LastShaderPass { set; get; }

        /// <summary>
        /// Gets the number of draw calls.
        /// </summary>
        /// <value>
        /// The number of draw calls.
        /// </value>
        public int NumberOfDrawCalls { private set; get; } = 0;

        /// <summary>
        /// Gets or sets the primitive topology.
        /// </summary>
        /// <value>
        /// The primitive topology.
        /// </value>
        public PrimitiveTopology PrimitiveTopology
        {
            set
            {
                deviceContext.InputAssembler.PrimitiveTopology = value;
            }
            get
            {
                return deviceContext.InputAssembler.PrimitiveTopology;
            }
        }

        /// <summary>
        /// Gets or sets the input layout.
        /// </summary>
        /// <value>
        /// The input layout.
        /// </value>
        public InputLayout InputLayout
        {
            set
            {
                deviceContext.InputAssembler.InputLayout = value;
            }
            get
            {
                return deviceContext.InputAssembler.InputLayout;
            }
        }

        #endregion Properties

        /// <summary>
        /// Initializes a new deferred context
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
            this.device = device;
            IsDeferred = true;
        }

        /// <summary>
        /// Muse pass immediate context for this constructor
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="device">device</param>
        public DeviceContextProxy(DeviceContext context, Device device)
        {
            deviceContext = context;
            this.device = device;
            IsDeferred = false;
        }

        #region Clear Targets

        /// <summary>
        /// Clears the render targets.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="color">The color.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargets(DX11RenderBufferProxyBase buffer, Color4 color)
        {
            buffer.ClearRenderTarget(this, color);
        }

        /// <summary>
        /// Clears the depth stencil view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="clearFlag">The clear flag.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="stencil">The stencil.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDepthStencilView(DepthStencilView view, DepthStencilClearFlags clearFlag, float depth, byte stencil)
        {
            deviceContext.ClearDepthStencilView(view, clearFlag, depth, stencil);
        }

        /// <summary>
        /// Clears the render target view.
        /// </summary>
        /// <param name="renderTargetViewRef">The render target view reference.</param>
        /// <param name="colorRGBA">A 4-component array that represents the color to fill the render target with.</param>
        /// <remarks>
        ///     Applications that wish to clear a render target to a specific integer value bit
        ///     pattern should render a screen-aligned quad instead of using this method. The
        ///     reason for this is because this method accepts as input a floating point value,
        ///     which may not have the same bit pattern as the original integer. Differences
        ///     between Direct3D 9 and Direct3D 11/10: Unlike Direct3D 9, the full extent of
        ///     the resource view is always cleared. Viewport and scissor settings are not applied.
        ///     ?When using D3D_FEATURE_LEVEL_9_x, ClearRenderTargetView only clears the first
        ///     array slice in the render target view. This can impact (for example) cube map
        ///     rendering scenarios. Applications should create a render target view for each
        ///     face or array slice, then clear each view individually.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(RenderTargetView renderTargetViewRef, Color4 colorRGBA)
        {
            deviceContext.ClearRenderTargetView(renderTargetViewRef, colorRGBA);
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTagetBindings()
        {
            deviceContext.OutputMerger.ResetTargets();
        }

        #endregion Clear Targets

        #region Implicit cast

        /// <summary>
        /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="DeviceContext"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator DeviceContext(DeviceContextProxy proxy)
        {
            return proxy.deviceContext;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="Device"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Device(DeviceContextProxy proxy)
        {
            return proxy.device;
        }

        #endregion Implicit cast

        #region Set states and targets

        /// <summary>
        ///
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(DX11RenderBufferProxyBase buffer)
        {
            buffer.SetDefaultRenderTargets(this);
        }

        /// <summary>
        /// Sets the state of the raster.
        /// </summary>
        /// <param name="rasterState">State of the raster.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRasterState(RasterizerStateProxy rasterState)
        {
            if (AutoSkipRedundantStateSetting && currRasterState == rasterState)
            {
                return;
            }
            deviceContext.Rasterizer.State = rasterState;
            currRasterState = rasterState;
        }

        /// <summary>
        /// Sets the state of the depth stencil.
        /// </summary>
        /// <param name="depthStencilState">State of the depth stencil.</param>
        /// <param name="stencilRef">The stencil reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(DepthStencilStateProxy depthStencilState, int stencilRef = 0)
        {
            if (AutoSkipRedundantStateSetting && currDepthStencilState == depthStencilState && currStencilRef == stencilRef)
            {
                return;
            }
            deviceContext.OutputMerger.SetDepthStencilState(depthStencilState, stencilRef);
            currDepthStencilState = depthStencilState;
            currStencilRef = stencilRef;
        }

        /// <summary>
        /// Sets the render target.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        /// <param name="renderTarget">The render target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(DepthStencilView dsv, RenderTargetView renderTarget)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, renderTarget);
        }

        /// <summary>
        /// Sets the render targets.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        /// <param name="renderTarget">The render target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(DepthStencilView dsv, RenderTargetView[] renderTarget)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, renderTarget);
        }

        private static readonly RenderTargetView[] ZeroRenderTargetArray = new RenderTargetView[0];

        /// <summary>
        /// Sets the depth stencil only. This will clear all render target bindings and only binds depth stencil view to output merger.
        /// </summary>
        /// <param name="dsv">The DSV.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilOnly(DepthStencilView dsv)
        {
            deviceContext.OutputMerger.SetRenderTargets(dsv, ZeroRenderTargetArray);
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
            if (AutoSkipRedundantStateSetting && currBlendState == blendState && blendFactor == currBlendFactor && currSampleMask == sampleMask)
            {
                return;
            }
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState;
            currBlendFactor = blendFactor;
            currSampleMask = sampleMask == -1 ? int.MaxValue : (uint)sampleMask;
        }

        /// <summary>
        /// Sets the state of the blend.
        /// </summary>
        /// <param name="blendState">State of the blend.</param>
        /// <param name="blendFactor">The blend factor.</param>
        /// <param name="sampleMask">The sample mask.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendStateProxy blendState, Color4? blendFactor = null, uint sampleMask = uint.MaxValue)
        {
            if (AutoSkipRedundantStateSetting && currBlendState == blendState && blendFactor == currBlendFactor && currSampleMask == sampleMask)
            {
                return;
            }
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState;
            currBlendFactor = blendFactor;
            currSampleMask = sampleMask;
        }
        #endregion Set states and targets

        #region Set Shaders and Constant Buffers

        /// <summary>
        /// Sets the vertex shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(VertexShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.VertexShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.VertexShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        /// <summary>
        /// Sets the hull shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(HullShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.HullShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.HullShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        /// <summary>
        /// Sets the domain shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(DomainShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.DomainShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.DomainShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        /// <summary>
        /// Sets the geometry shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(GeometryShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.GeometryShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.GeometryShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        /// <summary>
        /// Sets the pixel shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(PixelShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.PixelShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.PixelShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        /// <summary>
        /// Sets the compute shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="bindConstantBuffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShader(ComputeShader shader, bool bindConstantBuffer = true)
        {
            deviceContext.ComputeShader.Set(shader.Shader);
            if (bindConstantBuffer)
            {
                foreach (var buff in shader.ConstantBufferMapping.Mappings)
                {
                    deviceContext.ComputeShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
                }
            }
        }

        #endregion Set Shaders and Constant Buffers

        #region Set Get ShaderResources

        #region Vertex Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(VertexShaderType shader, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(VertexShaderType shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(VertexShaderType shader, int startSlot, int num)
        {
            return deviceContext.VertexShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(VertexShaderType shader, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(VertexShaderType shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetSamplers(slot, samplers);
        }
        /// <summary>
        /// Gets the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(VertexShaderType shader, int startSlot, int num)
        {
            return deviceContext.VertexShader.GetSamplers(startSlot, num);
        }
        #endregion Vertex Shader

        #region Domain Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(DomainShaderType shader, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(DomainShaderType shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(DomainShaderType shader, int startSlot, int num)
        {
            return deviceContext.DomainShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(DomainShaderType shader, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(DomainShaderType shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetSamplers(slot, samplers);
        }
        /// <summary>
        /// Gets the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(DomainShaderType shader, int startSlot, int num)
        {
            return deviceContext.DomainShader.GetSamplers(startSlot, num);
        }
        #endregion Domain Shader

        #region Pixel Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(PixelShaderType shader, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(PixelShaderType shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(PixelShaderType shader, int startSlot, int num)
        {
            return deviceContext.PixelShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(PixelShaderType shader, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(PixelShaderType shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetSamplers(slot, samplers);
        }
        /// <summary>
        /// Gets the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(PixelShaderType shader, int startSlot, int num)
        {
            return deviceContext.PixelShader.GetSamplers(startSlot, num);
        }
        #endregion Pixel Shader

        #region Compute Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResource(ComputeShaderType shader, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetShaderResource(slot, texture);
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResources(ComputeShaderType shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResources(ComputeShaderType shader, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetShaderResources(startSlot, num);
        }
        /// <summary>
        /// Binds the unordered access view.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnorderedAccessView(ComputeShaderType shader, int slot, UnorderedAccessView uav)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetUnorderedAccessView(slot, uav);
        }

        /// <summary>
        /// Binds the unordered access views.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="UAVs">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnorderedAccessViews(ComputeShaderType shader, int slot, UnorderedAccessView[] UAVs)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetUnorderedAccessViews(slot, UAVs);
        }
        /// <summary>
        /// Gets the unordered access view.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnorderedAccessView[] GetUnorderedAccessView(ComputeShaderType shader, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetUnorderedAccessViews(startSlot, num);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSampler(ComputeShaderType shader, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="samplers">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplers(ComputeShaderType shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetSamplers(slot, samplers);
        }
        /// <summary>
        /// Gets the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSampler(ComputeShaderType shader, int startSlot, int num)
        {
            return deviceContext.ComputeShader.GetSamplers(startSlot, num);
        }
        #endregion Compute Shader

        #endregion Set ShaderResources

        #region Get targets

        /// <summary>
        /// Gets the depth stencil view.
        /// </summary>
        /// <param name="depthStencilViewRef">The depth stencil view reference.</param>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetDepthStencilView(out DepthStencilView depthStencilViewRef)
        {
            deviceContext.OutputMerger.GetRenderTargets(out depthStencilViewRef);
        }

        /// <summary>
        /// Gets the render targets.
        /// </summary>
        /// <param name="numViews">The number views.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderTargetView[] GetRenderTargets(int numViews)
        {
            return deviceContext.OutputMerger.GetRenderTargets(numViews);
        }

        /// <summary>
        /// Gets the render targets.
        /// </summary>
        /// <param name="numViews">The number views.</param>
        /// <param name="depthStencilViewRef">The depth stencil view reference.</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call {{IUnknown::Release}} on the returned interfaces when they are no
        ///     longer needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RenderTargetView[] GetRenderTargets(int numViews, out DepthStencilView depthStencilViewRef)
        {
            return deviceContext.OutputMerger.GetRenderTargets(numViews, out depthStencilViewRef);
        }

        /// <summary>
        /// Gets the unordered access views.
        /// </summary>
        /// <param name="startSlot">Index of the first element in the zero-based array to return (ranges from 0 to D3D11_PS_CS_UAV_REGISTER_COUNT - 1).</param>
        /// <param name="count"> Number of views to get (ranges from 0 to D3D11_PS_CS_UAV_REGISTER_COUNT - StartSlot).</param>
        /// <returns></returns>
        /// <remarks>
        ///     Any returned interfaces will have their reference count incremented by one. Applications
        ///     should call IUnknown::Release on the returned interfaces when they are no longer
        ///     needed to avoid memory leaks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnorderedAccessView[] GetUnorderedAccessViews(int startSlot, int count)
        {
            return deviceContext.OutputMerger.GetUnorderedAccessViews(startSlot, count);
        }

        #endregion Get targets

        #region DrawCall

        /// <summary>
        /// Draw non-indexed, non-instanced primitives.
        /// </summary>
        /// <param name="vertexCount">Number of vertices to draw.</param>
        /// <param name="startVertexLocation">Index of the first vertex, which is usually an offset in a vertex buffer.</param>
        /// <remarks>
        ///     Draw submits work to the rendering pipeline.The vertex data for a draw call normally
        ///    comes from a vertex buffer that is bound to the pipeline.Even without any vertex
        ///     buffer bound to the pipeline, you can generate your own vertex data in your vertex
        ///     shader by using the SV_VertexID system-value semantic to determine the current
        ///     vertex that the runtime is processing.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(int vertexCount, int startVertexLocation)
        {
            ++NumberOfDrawCalls;
            deviceContext.Draw(vertexCount, startVertexLocation);
        }

        /// <summary>
        /// Draw geometry of an unknown size.
        /// </summary>
        /// <remarks>
        ///     A draw API submits work to the rendering pipeline. This API submits work of an
        ///     unknown size that was processed by the input assembler, vertex shader, and stream-output
        ///     stages; the work may or may not have gone through the geometry-shader stage.After
        ///     data has been streamed out to stream-output stage buffers, those buffers can
        ///     be again bound to the Input Assembler stage at input slot 0 and DrawAuto will
        ///     draw them without the application needing to know the amount of data that was
        ///     written to the buffers. A measurement of the amount of data written to the SO
        ///     stage buffers is maintained internally when the data is streamed out. This means
        ///     that the CPU does not need to fetch the measurement before re-binding the data
        ///     that was streamed as input data. Although this amount is tracked internally,
        ///     it is still the responsibility of applications to use input layouts to describe
        ///     the format of the data in the SO stage buffers so that the layouts are available
        ///     when the buffers are again bound to the input assembler.The following diagram
        ///     shows the DrawAuto process.Calling DrawAuto does not change the state of the
        ///     streaming-output buffers that were bound again as inputs.DrawAuto only works
        ///     when drawing with one input buffer bound as an input to the IA stage at slot
        ///     0. Applications must create the SO buffer resource with both binding flags, SharpDX.Direct3D11.BindFlags.VertexBuffer
        ///     and SharpDX.Direct3D11.BindFlags.StreamOutput.This API does not support indexing
        ///     or instancing.If an application needs to retrieve the size of the streaming-output
        ///     buffer, it can query for statistics on streaming output by using SharpDX.Direct3D11.QueryType.StreamOutputStatistics.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawAuto()
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawAuto();
        }

        /// <summary>
        /// Draw indexed, non-instanced primitives.
        /// </summary>
        /// <param name="indexCount">Number of indices to draw.</param>
        /// <param name="startIndexLocation">The location of the first index read by the GPU from the index buffer.</param>
        /// <param name="baseVertexLocation">A value added to each index before reading a vertex from the vertex buffer.</param>
        /// <remarks>
        ///     A draw API submits work to the rendering pipeline.If the sum of both indices
        ///     is negative, the result of the function call is undefined.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(int indexCount, int startIndexLocation, int baseVertexLocation)
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        }

        /// <summary>
        /// Draw indexed, instanced primitives.
        /// </summary>
        /// <param name="indexCountPerInstance"> Number of indices read from the index buffer for each instance.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startIndexLocation">The location of the first index read by the GPU from the index buffer.</param>
        /// <param name="baseVertexLocation">TA value added to each index before reading a vertex from the vertex buffer.</param>
        /// <param name="startInstanceLocation">A value added to each index before reading per-instance data from a vertex buffer.</param>
        /// <remarks>
        ///     A draw API submits work to the rendering pipeline.Instancing may extend performance
        ///     by reusing the same geometry to draw multiple objects in a scene. One example
        ///     of instancing could be to draw the same object with different positions and colors.
        ///     Instancing requires multiple vertex buffers: at least one for per-vertex data
        ///     and a second buffer for per-instance data.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexedInstanced(int indexCountPerInstance, int instanceCount, int startIndexLocation, int baseVertexLocation, int startInstanceLocation)
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startIndexLocation);
        }

        /// <summary>
        /// Draw indexed, instanced, GPU-generated primitives.
        /// </summary>
        /// <param name="bufferForArgsRef">A reference to an SharpDX.Direct3D11.Buffer, which is a buffer containing the GPU generated primitives.</param>
        /// <param name="alignedByteOffsetForArgs">Offset in pBufferForArgs to the start of the GPU generated primitives.</param>
        /// <remarks>
        ///     When an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
        ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
        ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
        ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
        ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
        ///     in the pDesc parameter. Windows?Phone?8: This API is supported.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexedInstancedIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawIndexedInstancedIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
        }

        /// <summary>
        /// Draw non-indexed, instanced primitives.
        /// </summary>
        /// <param name="vertexCountPerInstance">Number of vertices to draw.</param>
        /// <param name="instanceCount">Number of instances to draw.</param>
        /// <param name="startVertexLocation">Index of the first vertex.</param>
        /// <param name="startInstanceLocation">A value added to each index before reading per-instance data from a vertex buffer.</param>
        /// <remarks>
        ///     A draw API submits work to the rendering pipeline.Instancing may extend performance
        ///     by reusing the same geometry to draw multiple objects in a scene. One example
        ///     of instancing could be to draw the same object with different positions and colors.The
        ///     vertex data for an instanced draw call normally comes from a vertex buffer that
        ///     is bound to the pipeline. However, you could also provide the vertex data from
        ///     a shader that has instanced data identified with a system-value semantic (SV_InstanceID).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawInstanced(int vertexCountPerInstance, int instanceCount, int startVertexLocation, int startInstanceLocation)
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawInstanced(vertexCountPerInstance, instanceCount, startVertexLocation, startInstanceLocation);
        }

        /// <summary>
        /// Draw instanced, GPU-generated primitives.
        /// </summary>
        /// <param name="bufferForArgsRef">A reference to an SharpDX.Direct3D11.Buffer, which is a buffer containing the GPU generated primitives.</param>
        /// <param name="alignedByteOffsetForArgs">Offset in pBufferForArgs to the start of the GPU generated primitives.</param>
        /// <remarks>
        ///     When an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
        ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
        ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
        ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
        ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
        ///     in the pDesc parameter.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawInstancedIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
        {
            ++NumberOfDrawCalls;
            deviceContext.DrawInstancedIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
        }

        #endregion DrawCall

        /// <summary>
        /// Restore all default settings.
        /// </summary>
        /// <remarks>
        ///     This method resets any device context to the default settings. This sets all
        ///     input/output resource slots, shaders, input layouts, predications, scissor rectangles,
        ///     depth-stencil state, rasterizer state, blend state, sampler state, and viewports
        ///     to null. The primitive topology is set to UNDEFINED.For a scenario where you
        ///     would like to clear a list of commands recorded so far, call SharpDX.Direct3D11.DeviceContext.FinishCommandListInternal(SharpDX.Mathematics.Interop.RawBool,SharpDX.Direct3D11.CommandList@)
        ///     and throw away the resulting SharpDX.Direct3D11.CommandList.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearState()
        {
            deviceContext.ClearState();
        }

        /// <summary>
        /// Clears an unordered access resource with bit-precise values.
        /// </summary>
        /// <param name="unorderedAccessViewRef">The unordered access view reference.</param>
        /// <param name="values">The values.</param>
        /// <remarks>
        ///     This API copies the lower ni bits from each array element i to the corresponding
        ///     channel, where ni is the number of bits in the ith channel of the resource format
        ///     (for example, R8G8B8_FLOAT has 8 bits for the first 3 channels). This works on
        ///     any UAV with no format conversion. For a raw or structured buffer view, only
        ///     the first array element value is used.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearUnorderedAccessView(UnorderedAccessView unorderedAccessViewRef, Int4 values)
        {
            deviceContext.ClearUnorderedAccessView(unorderedAccessViewRef, values);
        }

        /// <summary>
        /// Clears an unordered access resource with a float value.
        /// </summary>
        /// <param name="unorderedAccessViewRef">The unordered access view reference.</param>
        /// <param name="values">The values.</param>
        /// <remarks>
        ///     This API works on FLOAT, UNORM, and SNORM unordered access views (UAVs), with
        ///     format conversion from FLOAT to *NORM where appropriate. On other UAVs, the operation
        ///     is invalid and the call will not reach the driver.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearUnorderedAccessView(UnorderedAccessView unorderedAccessViewRef, Vector4 values)
        {
            deviceContext.ClearUnorderedAccessView(unorderedAccessViewRef, values);
        }

        #region Copy Resources

        /// <summary>
        ///     Copy the entire contents of the source resource to the destination resource using
        ///     the GPU.
        /// </summary>
        /// <param name="source">A reference to the source resource (see SharpDX.Direct3D11.Resource).</param>
        /// <param name="destination">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
        /// <remarks>
        ///     This method is unusual in that it causes the GPU to perform the copy operation
        ///     (similar to a memcpy by the CPU). As a result, it has a few restrictions designed
        ///     for improving performance. For instance, the source and destination resources:
        ///     Must be different resources. Must be the same type. Must have identical dimensions
        ///     (including width, height, depth, and size as appropriate). Will only be copied.
        ///     CopyResource does not support any stretch, color key, blend, or format conversions.
        ///     Must have compatible DXGI formats, which means the formats must be identical
        ///     or at least from the same type group. For example, a DXGI_FORMAT_R32G32B32_FLOAT
        ///     texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of
        ///     these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. Might not be currently
        ///     mapped. You cannot use an {{Immutable}} resource as a destination. You can use
        ///     a {{depth-stencil}} resource as either a source or a destination. Resources created
        ///     with multisampling capability (see SharpDX.DXGI.SampleDescription) can be used
        ///     as source and destination only if both source and destination have identical
        ///     multisampled count and quality. If source and destination differ in multisampled
        ///     count and quality or if one is multisampled and the other is not multisampled
        ///     the call to ID3D11DeviceContext::CopyResource fails. The method is an asynchronous
        ///     call which may be added to the command-buffer queue. This attempts to remove
        ///     pipeline stalls that may occur when copying data. An application that only needs
        ///     to copy a portion of the data in a resource should use SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion})
        ///     instead.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyResource(Resource source, Resource destination)
        {
            deviceContext.CopyResource(source, destination);
        }

        /// <summary>
        /// Copies data from a buffer holding variable length data.
        /// </summary>
        /// <param name="dstBufferRef">
        ///     Pointer to SharpDX.Direct3D11.Buffer. This can be any buffer resource that other
        ///     copy commands, such as SharpDX.Direct3D11.DeviceContext.CopyResource_(SharpDX.Direct3D11.Resource,SharpDX.Direct3D11.Resource)
        ///     or SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion}),
        ///     are able to write to.
        /// </param>
        /// <param name="dstAlignedByteOffset">
        ///     Offset from the start of pDstBuffer to write 32-bit UINT structure (vertex) count
        ///     from pSrcView.
        /// </param>
        /// <param name="srcViewRef">
        ///     Pointer to an SharpDX.Direct3D11.UnorderedAccessView of a Structured Buffer resource
        ///     created with either SharpDX.Direct3D11.UnorderedAccessViewBufferFlags.Append
        ///     or SharpDX.Direct3D11.UnorderedAccessViewBufferFlags.Counter specified when the
        ///     UAV was created. These types of resources have hidden counters tracking "how
        ///     many" records have been written.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyStructureCount(Buffer dstBufferRef, int dstAlignedByteOffset, UnorderedAccessView srcViewRef)
        {
            deviceContext.CopyStructureCount(dstBufferRef, dstAlignedByteOffset, srcViewRef);
        }

        /// <summary>
        /// Copy a region from a source resource to a destination resource.
        /// </summary>
        /// <param name="source">A reference to the source resource (see SharpDX.Direct3D11.Resource).</param>
        /// <param name="sourceSubresource">Source subresource index.</param>
        /// <param name="sourceRegion">
        ///     A reference to a 3D box (see SharpDX.Direct3D11.ResourceRegion) that defines
        ///     the source subresources that can be copied. If NULL, the entire source subresource
        ///     is copied. The box must fit within the source resource.
        /// </param>
        /// <param name="destination">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
        /// <param name="destinationSubResource">Destination subresource index.</param>
        /// <param name="dstX">The x-coordinate of the upper left corner of the destination region.</param>
        /// <param name="dstY">
        ///     The y-coordinate of the upper left corner of the destination region. For a 1D
        ///     subresource, this must be zero.
        /// </param>
        /// <param name="dstZ">
        ///     The z-coordinate of the upper left corner of the destination region. For a 1D
        ///     or 2D subresource, this must be zero.
        /// </param>
        /// <remarks>
        ///     The source box must be within the size of the source resource. The destination
        ///     offsets, (x, y, and z) allow the source box to be offset when writing into the
        ///     destination resource; however, the dimensions of the source box and the offsets
        ///     must be within the size of the resource. If the resources are buffers, all coordinates
        ///     are in bytes; if the resources are textures, all coordinates are in texels. {{D3D11CalcSubresource}}
        ///     is a helper function for calculating subresource indexes. CopySubresourceRegion
        ///     performs the copy on the GPU (similar to a memcpy by the CPU). As a consequence,
        ///     the source and destination resources: Must be different subresources (although
        ///     they can be from the same resource). Must be the same type. Must have compatible
        ///     DXGI formats (identical or from the same type group). For example, a DXGI_FORMAT_R32G32B32_FLOAT
        ///     texture can be copied to an DXGI_FORMAT_R32G32B32_UINT texture since both of
        ///     these formats are in the DXGI_FORMAT_R32G32B32_TYPELESS group. May not be currently
        ///     mapped. CopySubresourceRegion only supports copy; it does not support any stretch,
        ///     color key, blend, or format conversions. An application that needs to copy an
        ///     entire resource should use SharpDX.Direct3D11.DeviceContext.CopyResource_(SharpDX.Direct3D11.Resource,SharpDX.Direct3D11.Resource)
        ///     instead. CopySubresourceRegion is an asynchronous call which may be added to
        ///     the command-buffer queue, this attempts to remove pipeline stalls that may occur
        ///     when copying data. See performance considerations for more details. Note??If
        ///     you use CopySubresourceRegion with a depth-stencil buffer or a multisampled resource,
        ///     you must copy the whole subresource. In this situation, you must pass 0 to the
        ///     DstX, DstY, and DstZ parameters and NULL to the pSrcBox parameter. In addition,
        ///     source and destination resources, which are represented by the pSrcResource and
        ///     pDstResource parameters, should have identical sample count values. Example The
        ///     following code snippet copies a box (located at (120,100),(200,220)) from a source
        ///     texture into a region (10,20),(90,140) in a destination texture. D3D11_BOX sourceRegion;
        ///     sourceRegion.left = 120; sourceRegion.right = 200; sourceRegion.top = 100; sourceRegion.bottom
        ///     = 220; sourceRegion.front = 0; sourceRegion.back = 1; pd3dDeviceContext->CopySubresourceRegion(
        ///     pDestTexture, 0, 10, 20, 0, pSourceTexture, 0, sourceRegion ); Notice, that
        ///     for a 2D texture, front and back are set to 0 and 1 respectively.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopySubresourceRegion(Resource source, int sourceSubresource, ResourceRegion? sourceRegion, Resource destination,
            int destinationSubResource, int dstX = 0, int dstY = 0, int dstZ = 0)
        {
            deviceContext.CopySubresourceRegion(source, sourceSubresource, sourceRegion, destination, destinationSubResource, dstX, dstY, dstZ);
        }

        #endregion Copy Resources

        #region Dispatch

        /// <summary>
        /// Execute a command list from a thread group.
        /// </summary>
        /// <param name="threadGroupCountX">
        ///     The number of groups dispatched in the x direction. ThreadGroupCountX must be
        ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
        ///     (65535).
        ///  </param>
        ///  <param name="threadGroupCountY">
        ///     The number of groups dispatched in the y direction. ThreadGroupCountY must be
        ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
        ///     (65535).
        ///  </param>
        ///  <param name="threadGroupCountZ">
        ///     The number of groups dispatched in the z direction. ThreadGroupCountZ must be
        ///     less than or equal to SharpDX.Direct3D11.ComputeShaderStage.DispatchMaximumThreadGroupsPerDimension
        ///     (65535). In feature level 10 the value for ThreadGroupCountZ must be 1.
        ///  </param>
        ///  <remarks>
        ///     You call the Dispatch method to execute commands in a compute shader. A compute
        ///     shader can be run on many threads in parallel, within a thread group. Index a
        ///     particular thread, within a thread group using a 3D vector given by (x,y,z).In
        ///     the following illustration, assume a thread group with 50 threads where the size
        ///     of the group is given by (5,5,2). A single thread is identified from a thread
        ///     group with 50 threads in it, using the vector (4,1,1).The following illustration
        ///     shows the relationship between the parameters passed to SharpDX.Direct3D11.DeviceContext.Dispatch(System.Int32,System.Int32,System.Int32),
        ///     Dispatch(5,3,2), the values specified in the numthreads attribute, numthreads(10,8,3),
        ///     and values that will passed to the compute shader for the thread-related system
        ///     values (SV_GroupIndex,SV_DispatchThreadID,SV_GroupThreadID,SV_GroupID).
        ///  </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispatch(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
        {
            ++NumberOfDrawCalls;
            deviceContext.Dispatch(threadGroupCountX, threadGroupCountY, threadGroupCountZ);
        }

        /// <summary>
        /// Execute a command list over one or more thread groups.
        /// </summary>
        /// <param name="bufferForArgsRef">
        ///     A reference to an SharpDX.Direct3D11.Buffer, which must be loaded with data that
        ///     matches the argument list for SharpDX.Direct3D11.DeviceContext.Dispatch(System.Int32,System.Int32,System.Int32).
        /// </param>
        /// <param name="alignedByteOffsetForArgs">A byte-aligned offset between the start of the buffer and the arguments.</param>
        /// <remarks>
        ///     You call the DispatchIndirect method to execute commands in a compute shader.When
        ///     an application creates a buffer that is associated with the SharpDX.Direct3D11.Buffer
        ///     interface that pBufferForArgs points to, the application must set the SharpDX.Direct3D11.ResourceOptionFlags.DrawIndirectArguments
        ///     flag in the MiscFlags member of the SharpDX.Direct3D11.BufferDescription structure
        ///     that describes the buffer. To create the buffer, the application calls the SharpDX.Direct3D11.Device.CreateBuffer(SharpDX.Direct3D11.BufferDescription@,System.Nullable{SharpDX.DataBox},SharpDX.Direct3D11.Buffer)
        ///     method and in this call passes a reference to SharpDX.Direct3D11.BufferDescription
        ///     in the pDesc parameter.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DispatchIndirect(Buffer bufferForArgsRef, int alignedByteOffsetForArgs)
        {
            ++NumberOfDrawCalls;
            deviceContext.DispatchIndirect(bufferForArgsRef, alignedByteOffsetForArgs);
        }

        #endregion Dispatch

        #region Map/Unmap resources

        /// <summary>
        ///     Maps the data contained in a subresource to a memory pointer, and denies the
        ///     GPU access to that subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="mipSlice">The mip slice.</param>
        /// <param name="arraySlice">The array slice.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The locked SharpDX.DataBox</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Texture2D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
        {
            return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
        }

        /// <summary>
        ///     Maps the data contained in a subresource to a memory pointer, and denies the
        ///     GPU access to that subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="mipSlice">The mip slice.</param>
        /// <param name="arraySlice">The array slice.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The locked SharpDX.DataBox   </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Texture1D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
        {
            return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
        }

        /// <summary>
        ///     Maps the data contained in a subresource to a memory pointer, and denies the
        ///     GPU access to that subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="mipSlice">The mip slice.</param>
        /// <param name="arraySlice">The array slice.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The locked SharpDX.DataBox</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Texture3D resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out DataStream stream)
        {
            return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out stream);
        }

        /// <summary>
        ///     Maps the data contained in a subresource to a memory pointer, and denies the
        ///     GPU access to that subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The locked SharpDX.DataBox      </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Buffer resource, MapMode mode, MapFlags flags, out DataStream stream)
        {
            return deviceContext.MapSubresource(resource, mode, flags, out stream);
        }

        /// <summary>
        /// Maps the subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="mipSlice">The mip slice.</param>
        /// <param name="arraySlice">The array slice.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="mipSizeOut">Size of the mip.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Resource resource, int mipSlice, int arraySlice, MapMode mode, MapFlags flags, out int mipSizeOut)
        {
            return deviceContext.MapSubresource(resource, mipSlice, arraySlice, mode, flags, out mipSizeOut);
        }

        /// <summary>
        /// Maps the subresource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Resource resource, int subresource, MapMode mode, MapFlags flags, out DataStream stream)
        {
            return deviceContext.MapSubresource(resource, subresource, mode, flags, out stream);
        }

        /// <summary>
        /// Maps the subresource.
        /// </summary>
        /// <param name="resourceRef">The resource reference.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="mapType">Type of the map.</param>
        /// <param name="mapFlags">The map flags.</param>
        /// <returns></returns>
        /// <remarks>
        ///     If you call Map on a deferred context, you can only pass SharpDX.Direct3D11.MapMode.WriteDiscard,
        ///     SharpDX.Direct3D11.MapMode.WriteNoOverwrite, or both to the MapType parameter.
        ///     Other SharpDX.Direct3D11.MapMode-typed values are not supported for a deferred
        ///     context.The Direct3D 11.1 runtime, which is available starting with Windows Developer
        ///     Preview, can map shader resource views (SRVs) of dynamic buffers with SharpDX.Direct3D11.MapMode.WriteNoOverwrite.
        ///     The Direct3D 11 and earlier runtimes limited mapping to vertex or index buffers.
        ///     If SharpDX.Direct3D11.MapFlags.DoNotWait is used and the resource is still being
        ///     used by the GPU, this method return an empty DataBox whose property SharpDX.DataBox.IsEmpty
        ///     returns true.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataBox MapSubresource(Resource resourceRef, int subresource, MapMode mapType, MapFlags mapFlags)
        {
            return deviceContext.MapSubresource(resourceRef, subresource, mapType, mapFlags);
        }

        /// <summary>
        /// Invalidate the reference to a resource and reenable the GPU's access to that resource.
        /// </summary>
        /// <param name="resourceRef">The resource reference.</param>
        /// <param name="subresource">The subresource.</param>
        /// <remarks>
        ///     For info about how to use Unmap, see How to: Use dynamic resources. Windows?Phone?8:
        ///     This API is supported.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnmapSubresource(Resource resourceRef, int subresource)
        {
            deviceContext.UnmapSubresource(resourceRef, subresource);
        }

        /// <summary>
        /// Copy a multisampled resource into a non-multisampled resource.
        /// </summary>
        /// <param name="source">Source resource. Must be multisampled.</param>
        /// <param name="sourceSubresource">The source subresource.</param>
        /// <param name="destination">
        ///     Destination resource. Must be a created with the SharpDX.Direct3D11.ResourceUsage.Default
        ///     flag and be single-sampled. See SharpDX.Direct3D11.Resource.
        /// </param>
        /// <param name="destinationSubresource">A zero-based index, that identifies the destination subresource. Use {{D3D11CalcSubresource}} to calculate the index.</param>
        /// <param name="format"> A SharpDX.DXGI.Format that indicates how the multisampled resource will be resolved to a single-sampled resource. See remarks.</param>
        /// <remarks>
        ///     This API is most useful when re-using the resulting render target of one render
        ///     pass as an input to a second render pass. The source and destination resources
        ///     must be the same resource type and have the same dimensions. In addition, they
        ///     must have compatible formats. There are three scenarios for this: ScenarioRequirements
        ///     Source and destination are prestructured and typedBoth the source and destination
        ///     must have identical formats and that format must be specified in the Format parameter.
        ///     One resource is prestructured and typed and the other is prestructured and typelessThe
        ///     typed resource must have a format that is compatible with the typeless resource
        ///     (i.e. the typed resource is DXGI_FORMAT_R32_FLOAT and the typeless resource is
        ///     DXGI_FORMAT_R32_TYPELESS). The format of the typed resource must be specified
        ///     in the Format parameter. Source and destination are prestructured and typelessBoth
        ///     the source and destination must have the same typeless format (i.e. both must
        ///     have DXGI_FORMAT_R32_TYPELESS), and the Format parameter must specify a format
        ///     that is compatible with the source and destination (i.e. if both are DXGI_FORMAT_R32_TYPELESS
        ///     then DXGI_FORMAT_R32_FLOAT could be specified in the Format parameter). For example,
        ///     given the DXGI_FORMAT_R16G16B16A16_TYPELESS format: The source (or dest) format
        ///     could be DXGI_FORMAT_R16G16B16A16_UNORM The dest (or source) format could be
        ///     DXGI_FORMAT_R16G16B16A16_FLOAT ?
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResolveSubresource(Resource source, int sourceSubresource, Resource destination, int destinationSubresource, global::SharpDX.DXGI.Format format)
        {
            deviceContext.ResolveSubresource(source, sourceSubresource, destination, destinationSubresource, format);
        }

        #endregion Map/Unmap resources

        #region Upload sub resources

        /// <summary>
        /// The CPU copies data from memory to a subresource created in non-mappable memory.
        /// </summary>
        /// <param name="dstResourceRef">A reference to the destination resource (see SharpDX.Direct3D11.Resource).</param>
        /// <param name="dstSubresource">A zero-based index, that identifies the destination subresource. See D3D11CalcSubresource for more details.</param>
        /// <param name="dstBoxRef">
        ///     A reference to a box that defines the portion of the destination subresource
        ///     to copy the resource data into. Coordinates are in bytes for buffers and in texels
        ///     for textures. If null, the data is written to the destination subresource with
        ///     no offset. The dimensions of the source must fit the destination (see SharpDX.Direct3D11.ResourceRegion).
        ///     An empty box results in a no-op. A box is empty if the top value is greater than
        ///     or equal to the bottom value, or the left value is greater than or equal to the
        ///     right value, or the front value is greater than or equal to the back value. When
        ///     the box is empty, UpdateSubresource doesn't perform an update operation.
        /// </param>
        /// <param name="srcDataRef">A reference to the source data in memory.</param>
        /// <param name="srcRowPitch">The size of one row of the source data.</param>
        /// <param name="srcDepthPitch">The size of one depth slice of source data.</param>
        /// <remarks>
        /// Remarks:
        ///     For a shader-constant buffer; set pDstBox to null. It is not possible to use
        ///     this method to partially update a shader-constant buffer.A resource cannot be
        ///     used as a destination if: the resource is created with immutable or dynamic usage.
        ///     the resource is created as a depth-stencil resource. the resource is created
        ///     with multisampling capability (see SharpDX.DXGI.SampleDescription). When UpdateSubresource
        ///     returns, the application is free to change or even free the data pointed to by
        ///     pSrcData because the method has already copied/snapped away the original contents.The
        ///     performance of UpdateSubresource depends on whether or not there is contention
        ///     for the destination resource. For example, contention for a vertex buffer resource
        ///     occurs when the application executes a Draw call and later calls UpdateSubresource
        ///     on the same vertex buffer before the Draw call is actually executed by the GPU.
        ///     When there is contention for the resource, UpdateSubresource will perform 2 copies
        ///     of the source data. First, the data is copied by the CPU to a temporary storage
        ///     space accessible by the command buffer. This copy happens before the method returns.
        ///     A second copy is then performed by the GPU to copy the source data into non-mappable
        ///     memory. This second copy happens asynchronously because it is executed by GPU
        ///     when the command buffer is flushed. When there is no resource contention, the
        ///     behavior of UpdateSubresource is dependent on which is faster (from the CPU's
        ///     perspective): copying the data to the command buffer and then having a second
        ///     copy execute when the command buffer is flushed, or having the CPU copy the data
        ///     to the final resource location. This is dependent on the architecture of the
        ///     underlying system. Note??Applies only to feature level 9_x hardware If you use
        ///     UpdateSubresource or SharpDX.Direct3D11.DeviceContext.CopySubresourceRegion_(SharpDX.Direct3D11.Resource,System.Int32,System.Int32,System.Int32,System.Int32,SharpDX.Direct3D11.Resource,System.Int32,System.Nullable{SharpDX.Direct3D11.ResourceRegion})
        ///     to copy from a staging resource to a default resource, you can corrupt the destination
        ///     contents. This occurs if you pass a null source box and if the source resource
        ///     has different dimensions from those of the destination resource or if you use
        ///     destination offsets, (x, y, and z). In this situation, always pass a source box
        ///     that is the full size of the source resource.?To better understand the source
        ///     row pitch and source depth pitch parameters, the following illustration shows
        ///     a 3D volume texture.Each block in this visual represents an element of data,
        ///     and the size of each element is dependent on the resource's format. For example,
        ///     if the resource format is SharpDX.DXGI.Format.R32G32B32A32_Float, the size of
        ///     each element would be 128 bits, or 16 bytes. This 3D volume texture has a width
        ///     of two, a height of three, and a depth of four.To calculate the source row pitch
        ///     and source depth pitch for a given resource, use the following formulas: Source
        ///     Row Pitch = [size of one element in bytes] * [number of elements in one row]
        ///     Source Depth Pitch = [Source Row Pitch] * [number of rows (height)] In the case
        ///     of this example 3D volume texture where the size of each element is 16 bytes,
        ///     the formulas are as follows: Source Row Pitch = 16 * 2 = 32 Source Depth Pitch
        ///     = 16 * 2 * 3 = 96 The following illustration shows the resource as it is laid
        ///     out in memory.For example, the following code snippet shows how to specify a
        ///     destination region in a 2D texture. Assume the destination texture is 512x512
        ///     and the operation will copy the data pointed to by pData to [(120,100)..(200,220)]
        ///     in the destination texture. Also assume that rowPitch has been initialized with
        ///     the proper value (as explained above). front and back are set to 0 and 1 respectively,
        ///     because by having front equal to back, the box is technically empty. SharpDX.Direct3D11.ResourceRegion
        ///     destRegion; destRegion.left = 120; destRegion.right = 200; destRegion.top = 100;
        ///     destRegion.bottom = 220; destRegion.front = 0; destRegion.back = 1; pd3dDeviceContext->UpdateSubresource(
        ///     pDestTexture, 0, &destRegion, pData, rowPitch, 0 ); The 1D case is similar. The
        ///     following snippet shows how to specify a destination region in a 1D texture.
        ///     Use the same assumptions as above, except that the texture is 512 in length.
        ///     SharpDX.Direct3D11.ResourceRegion destRegion; destRegion.left = 120; destRegion.right
        ///     = 200; destRegion.top = 0; destRegion.bottom = 1; destRegion.front = 0; destRegion.back
        ///     = 1; pd3dDeviceContext->UpdateSubresource( pDestTexture, 0, &destRegion, pData,
        ///     rowPitch, 0 ); For info about various resource types and how UpdateSubresource
        ///     might work with each resource type, see Introduction to a Resource in Direct3D
        ///     11.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresource(Resource dstResourceRef, int dstSubresource, ResourceRegion? dstBoxRef, System.IntPtr srcDataRef, int srcRowPitch, int srcDepthPitch)
        {
            deviceContext.UpdateSubresource(dstResourceRef, dstSubresource, dstBoxRef, srcDataRef, srcRowPitch, srcDepthPitch);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="region">The region.</param>
        /// <remarks>This method is implementing the workaround for deferred context.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresource(DataBox source, Resource resource, int subresource, ref ResourceRegion region)
        {
            deviceContext.UpdateSubresource(source, resource, subresource, region);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="subresource">The subresource.</param>
        /// <remarks>This method is implementing the workaround for deferred context.       </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresource(DataBox source, Resource resource, int subresource = 0)
        {
            deviceContext.UpdateSubresource(source, resource, subresource);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="rowPitch">The row pitch.</param>
        /// <param name="depthPitch">The depth pitch.</param>
        /// <param name="region">
        /// A region that defines the portion of the destination subresource to copy the
        /// resource data into. Coordinates are in bytes for buffers and in texels for textures.
        /// </param>
        /// <remarks>This method is implementing the workaround for deferred context.     </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresource<T>(T[] data, Resource resource, int subresource = 0, int rowPitch = 0, int depthPitch = 0, ResourceRegion? region = null) where T : struct
        {
            deviceContext.UpdateSubresource(data, resource, subresource, rowPitch, depthPitch, region);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="rowPitch">The row pitch.</param>
        /// <param name="depthPitch">The depth pitch.</param>
        /// <param name="region">The region.</param>
        /// <remarks>This method is implementing the workaround for deferred context.        </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresource<T>(ref T data, Resource resource, int subresource = 0, int rowPitch = 0, int depthPitch = 0, ResourceRegion? region = null) where T : struct
        {
            deviceContext.UpdateSubresource(ref data, resource, subresource, rowPitch, depthPitch, region);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
        /// <remarks>This method is implementing the workaround for deferred context. </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresourceSafe(ref DataBox source, Resource resource, int srcBytesPerElement, int subresource = 0, bool isCompressedResource = false)
        {
            deviceContext.UpdateSubresourceSafe(source, resource, srcBytesPerElement, subresource, isCompressedResource);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="region">The region.</param>
        /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
        /// <remarks>This method is implementing the workaround for deferred context.    </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresourceSafe(ref DataBox source, Resource resource, int srcBytesPerElement, int subresource, ResourceRegion region, bool isCompressedResource = false)
        {
            deviceContext.UpdateSubresourceSafe(source, resource, srcBytesPerElement, subresource, region, isCompressedResource);
        }

        /// <summary>
        /// Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="rowPitch">The row pitch.</param>
        /// <param name="depthPitch">The depth pitch.</param>
        /// <param name="isCompressedResource">if set to true the resource is a block/compressed resource</param>
        /// <remarks>This method is implementing the workaround for deferred context.       </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresourceSafe<T>(T[] data, Resource resource, int srcBytesPerElement, int subresource = 0, int rowPitch = 0, int depthPitch = 0,
            bool isCompressedResource = false) where T : struct
        {
            deviceContext.UpdateSubresourceSafe(data, resource, srcBytesPerElement, subresource, rowPitch, depthPitch);
        }

        /// <summary>
        ///  Copies data from the CPU to to a non-mappable subresource region.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="srcBytesPerElement">The size in bytes per pixel/block element.</param>
        /// <param name="subresource">The subresource.</param>
        /// <param name="rowPitch">The row pitch.</param>
        /// <param name="depthPitch">The depth pitch.</param>
        /// <param name="isCompressedResource">if set to <c>true</c> [is compressed resource].</param>
        /// <remarks>This method is implementing the workaround for deferred context.  </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateSubresourceSafe<T>(ref T data, Resource resource, int srcBytesPerElement, int subresource = 0, int rowPitch = 0, int depthPitch = 0,
            bool isCompressedResource = false) where T : struct
        {
            deviceContext.UpdateSubresourceSafe(ref data, resource, srcBytesPerElement, subresource, rowPitch, depthPitch, isCompressedResource);
        }

        #endregion Upload sub resources

        #region set buffers 
        /// <summary>
        /// Bind an index buffer to the input-assembler stage.
        /// </summary>
        /// <param name="indexBufferRef">
        ///     A reference to an SharpDX.Direct3D11.Buffer object, that contains indices. The
        ///     index buffer must have been created with the SharpDX.Direct3D11.BindFlags.IndexBuffer
        ///     flag. 
        /// </param>
        /// <param name="format">
        ///     A SharpDX.DXGI.Format that specifies the format of the data in the index buffer.
        ///     The only formats allowed for index buffer data are 16-bit (SharpDX.DXGI.Format.R16_UInt)
        ///     and 32-bit (SharpDX.DXGI.Format.R32_UInt) integers.
        /// </param>
        /// <param name="offset">Offset (in bytes) from the start of the index buffer to the first index to use.</param>
        /// <remarks>
        ///     For information about creating index buffers, see How to: Create an Index Buffer.
        ///     Calling this method using a buffer that is currently bound for writing (i.e.
        ///     bound to the stream output pipeline stage) will effectively bind null instead
        ///     because a buffer cannot be bound as both an input and an output at the same time.
        ///     The debug layer will generate a warning whenever a resource is prevented from
        ///     being bound simultaneously as an input and an output, but this will not prevent
        ///     invalid data from being used by the runtime. The method will hold a reference
        ///     to the interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10. Windows?Phone?8: This API is supported.  
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(Buffer indexBufferRef, global::SharpDX.DXGI.Format format, int offset)
        {
            deviceContext.InputAssembler.SetIndexBuffer(indexBufferRef, format, offset);
        }

        /// <summary>
        /// Bind a single vertex buffer to the input-assembler stage.
        /// </summary>
        /// <param name="slot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level. 
        /// </param>
        /// <param name="vertexBufferBinding">
        ///     A SharpDX.Direct3D11.VertexBufferBinding. The vertex buffer must have been created
        ///     with the SharpDX.Direct3D11.BindFlags.VertexBuffer flag. 
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10. 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int slot, ref VertexBufferBinding vertexBufferBinding)
        {
            deviceContext.InputAssembler.SetVertexBuffers(slot, vertexBufferBinding);
        }

        /// <summary>
        /// Bind an array of vertex buffers to the input-assembler stage.
        /// </summary>
        /// <param name="firstSlot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level. 
        /// </param>
        /// <param name="vertexBufferBindings">
        ///     A reference to an array of SharpDX.Direct3D11.VertexBufferBinding. The vertex
        ///     buffers must have been created with the SharpDX.Direct3D11.BindFlags.VertexBuffer
        ///     flag. 
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int firstSlot, params VertexBufferBinding[] vertexBufferBindings)
        {
            deviceContext.InputAssembler.SetVertexBuffers(firstSlot, vertexBufferBindings);
        }
    
        /// <summary>
        /// Sets the vertex buffers.
        /// </summary>
        /// <param name="slot">
        ///     The first input slot for binding. The first vertex buffer is explicitly bound
        ///     to the start slot; this causes each additional vertex buffer in the array to
        ///     be implicitly bound to each subsequent input slot. The maximum of 16 or 32 input
        ///     slots (ranges from 0 to SharpDX.Direct3D11.InputAssemblerStage.VertexInputResourceSlotCount
        ///     - 1) are available; the maximum number of input slots depends on the feature
        ///     level.
        /// </param>
        /// <param name="vertexBuffers">
        ///     A reference to an array of vertex buffers (see SharpDX.Direct3D11.Buffer). The
        ///     vertex buffers must have been created with the SharpDX.Direct3D11.BindFlags.VertexBuffer
        ///     flag.
        /// </param>
        /// <param name="stridesRef">
        ///     Pointer to an array of stride values; one stride value for each buffer in the
        ///     vertex-buffer array. Each stride is the size (in bytes) of the elements that
        ///     are to be used from that vertex buffer.
        /// </param>
        /// <param name="offsetsRef">
        ///     Pointer to an array of offset values; one offset value for each buffer in the
        ///     vertex-buffer array. Each offset is the number of bytes between the first element
        ///     of a vertex buffer and the first element that will be used.
        /// </param>
        /// <remarks>
        ///     For information about creating vertex buffers, see Create a Vertex Buffer.Calling
        ///     this method using a buffer that is currently bound for writing (i.e. bound to
        ///     the stream output pipeline stage) will effectively bind null instead because
        ///     a buffer cannot be bound as both an input and an output at the same time.The
        ///     debug layer will generate a warning whenever a resource is prevented from being
        ///     bound simultaneously as an input and an output, but this will not prevent invalid
        ///     data from being used by the runtime. The method will hold a reference to the
        ///     interfaces passed in. This differs from the device state behavior in Direct3D
        ///     10.     
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffers(int slot, Buffer[] vertexBuffers, int[] stridesRef, int[] offsetsRef)
        {
            deviceContext.InputAssembler.SetVertexBuffers(slot, vertexBuffers, stridesRef, offsetsRef);
        }

        #endregion

        #region CommandList

        /// <summary>
        /// Create a command list and record graphics commands into it.
        /// </summary>
        /// <param name="restoreState">
        /// A flag indicating whether the immediate context state is saved (prior) and restored
        /// (after) the execution of a command list.
        /// </param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandList FinishCommandList(bool restoreState)
        {
            return deviceContext.FinishCommandList(restoreState);
        }

        /// <summary>
        /// Queues commands from a command list onto a device.
        /// </summary>
        /// <param name="commandListRef">The command list reference.</param>
        /// <param name="restoreContextState">
        ///     A Boolean flag that determines whether the target context state is saved prior
        ///     to and restored after the execution of a command list. Use TRUE to indicate that
        ///     the runtime needs to save and restore the state. Use SharpDX.Result.False to
        ///     indicate that no state shall be saved or restored, which causes the target context
        ///     to return to its default state after the command list executes. Applications
        ///     should typically use SharpDX.Result.False unless they will restore the state
        ///     to be nearly equivalent to the state that the runtime would restore if TRUE were
        ///     passed. When applications use SharpDX.Result.False, they can avoid unnecessary
        ///     and inefficient state transitions.
        /// </param>
        /// <remarks>
        ///     Use this method to play back a command list that was recorded by a deferred context
        ///     on any thread. A call to ExecuteCommandList of a command list from a deferred
        ///     context onto the immediate context is required for the recorded commands to be
        ///     executed on the graphics processing unit (GPU). A call to ExecuteCommandList
        ///     of a command list from a deferred context onto another deferred context can be
        ///     used to merge recorded lists. But to run the commands from the merged deferred
        ///     command list on the GPU, you need to execute them on the immediate context. This
        ///     method performs some runtime validation related to queries. Queries that are
        ///     begun in a device context cannot be manipulated indirectly by executing a command
        ///     list (that is, Begin or End was invoked against the same query by the deferred
        ///     context which generated the command list). If such a condition occurs, the ExecuteCommandList
        ///     method does not execute the command list. However, the state of the device context
        ///     is still maintained, as would be expected (SharpDX.Direct3D11.DeviceContext.ClearState
        ///     is performed, unless the application indicates to preserve the device context
        ///     state). Windows?Phone?8: This API is supported.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandList(CommandList commandListRef, bool restoreContextState)
        {
            deviceContext.ExecuteCommandList(commandListRef, restoreContextState);
        }

        #endregion CommandList

        #region Viewport and Scissors

        /// <summary>
        /// Sets the scissor rectangle.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <remarks>
        ///     All scissor rects must be set atomically as one operation. Any scissor rects
        ///     not defined by the call are disabled.The scissor rectangles will only be used
        ///     if ScissorEnable is set to true in the rasterizer state (see SharpDX.Direct3D11.RasterizerStateDescription).Which
        ///     scissor rectangle to use is determined by the SV_ViewportArrayIndex semantic
        ///     output by a geometry shader (see shader semantic syntax). If a geometry shader
        ///     does not make use of the SV_ViewportArrayIndex semantic then Direct3D will use
        ///     the first scissor rectangle in the array.Each scissor rectangle in the array
        ///     corresponds to a viewport in an array of viewports (see SharpDX.Direct3D11.RasterizerStage.SetViewports(SharpDX.Mathematics.Interop.RawViewportF[],System.Int32)).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetScissorRectangle(int left, int top, int right, int bottom)
        {
            deviceContext.Rasterizer.SetScissorRectangle(left, top, right, bottom);
        }

        /// <summary>
        /// Binds a set of scissor rectangles to the rasterizer stage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scissorRectangles">The scissor rectangles.</param>
        /// <remarks>
        ///     All scissor rects must be set atomically as one operation. Any scissor rects
        ///     not defined by the call are disabled.The scissor rectangles will only be used
        ///     if ScissorEnable is set to true in the rasterizer state (see SharpDX.Direct3D11.RasterizerStateDescription).Which
        ///     scissor rectangle to use is determined by the SV_ViewportArrayIndex semantic
        ///     output by a geometry shader (see shader semantic syntax). If a geometry shader
        ///     does not make use of the SV_ViewportArrayIndex semantic then Direct3D will use
        ///     the first scissor rectangle in the array.Each scissor rectangle in the array
        ///     corresponds to a viewport in an array of viewports (see SharpDX.Direct3D11.RasterizerStage.SetViewports(SharpDX.Mathematics.Interop.RawViewportF[],System.Int32)).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetScissorRectangles<T>(params T[] scissorRectangles) where T : struct
        {
            deviceContext.Rasterizer.SetScissorRectangles(scissorRectangles);
        }

        /// <summary>
        /// Binds a single viewport to the rasterizer stage.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="minZ">The minimum z.</param>
        /// <param name="maxZ">The maximum z.</param>
        /// <remarks>
        ///     All viewports must be set atomically as one operation. Any viewports not defined
        ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
        ///     semantic output by a geometry shader; if a geometry shader does not specify the
        ///     semantic, Direct3D will use the first viewport in the array.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewport(float x, float y, float width, float height, float minZ = 0, float maxZ = 1)
        {
            deviceContext.Rasterizer.SetViewport(x, y, width, height, minZ, maxZ);
        }

        /// <summary>
        /// Binds a single viewport to the rasterizer stage.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <remarks>
        ///     All viewports must be set atomically as one operation. Any viewports not defined
        ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
        ///     semantic output by a geometry shader; if a geometry shader does not specify the
        ///     semantic, Direct3D will use the first viewport in the array.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewport(ref Viewport viewport)
        {
            deviceContext.Rasterizer.SetViewport(viewport);
        }

        /// <summary>
        /// Binds a single viewport to the rasterizer stage.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <remarks>
        ///     All viewports must be set atomically as one operation. Any viewports not defined
        ///     by the call are disabled.Which viewport to use is determined by the SV_ViewportArrayIndex
        ///     semantic output by a geometry shader; if a geometry shader does not specify the
        ///     semantic, Direct3D will use the first viewport in the array.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewport(ref ViewportF viewport)
        {
            deviceContext.Rasterizer.SetViewport(viewport);
        }

        #endregion Viewport and Scissors

        /// <summary>
        /// Generates mipmaps for the given shader resource.
        /// </summary>
        /// <param name="shaderResourceViewRef">The shader resource view reference.</param>
        /// <remarks>
        ///     You can call GenerateMips on any shader-resource view to generate the lower mipmap
        ///     levels for the shader resource. GenerateMips uses the largest mipmap level of
        ///     the view to recursively generate the lower levels of the mip and stops with the
        ///     smallest level that is specified by the view. If the base resource wasn't created
        ///     with SharpDX.Direct3D11.BindFlags.RenderTarget, SharpDX.Direct3D11.BindFlags.ShaderResource,
        ///     and SharpDX.Direct3D11.ResourceOptionFlags.GenerateMipMaps, the call to GenerateMips
        ///     has no effect.Feature levels 9.1, 9.2, and 9.3 can't support automatic generation
        ///     of mipmaps for 3D (volume) textures.Video adapters that support feature level
        ///     9.1 and higher support generating mipmaps if you use any of these formats: SharpDX.DXGI.Format.R8G8B8A8_UNorm
        ///     SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb SharpDX.DXGI.Format.B5G6R5_UNorm SharpDX.DXGI.Format.B8G8R8A8_UNorm
        ///     SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb SharpDX.DXGI.Format.B8G8R8X8_UNorm SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb
        ///     Video adapters that support feature level 9.2 and higher support generating mipmaps
        ///     if you use any of these formats in addition to any of the formats for feature
        ///     level 9.1: SharpDX.DXGI.Format.R16G16B16A16_Float SharpDX.DXGI.Format.R16G16B16A16_UNorm
        ///     SharpDX.DXGI.Format.R16G16_Float SharpDX.DXGI.Format.R16G16_UNorm SharpDX.DXGI.Format.R32_Float
        ///     Video adapters that support feature level 9.3 and higher support generating mipmaps
        ///     if you use any of these formats in addition to any of the formats for feature
        ///     levels 9.1 and 9.2: SharpDX.DXGI.Format.R32G32B32A32_Float DXGI_FORMAT_B4G4R4A4
        ///     (optional) Video adapters that support feature level 10 and higher support generating
        ///     mipmaps if you use any of these formats in addition to any of the formats for
        ///     feature levels 9.1, 9.2, and 9.3: SharpDX.DXGI.Format.R32G32B32_Float (optional)
        ///     SharpDX.DXGI.Format.R16G16B16A16_SNorm SharpDX.DXGI.Format.R32G32_Float SharpDX.DXGI.Format.R10G10B10A2_UNorm
        ///     SharpDX.DXGI.Format.R11G11B10_Float SharpDX.DXGI.Format.R8G8B8A8_SNorm SharpDX.DXGI.Format.R16G16_SNorm
        ///     SharpDX.DXGI.Format.R8G8_UNorm SharpDX.DXGI.Format.R8G8_SNorm SharpDX.DXGI.Format.R16_Float
        ///     SharpDX.DXGI.Format.R16_UNorm SharpDX.DXGI.Format.R16_SNorm SharpDX.DXGI.Format.R8_UNorm
        ///     SharpDX.DXGI.Format.R8_SNorm SharpDX.DXGI.Format.A8_UNorm SharpDX.DXGI.Format.B5G5R5A1_UNorm
        ///     (optional) For all other unsupported formats, GenerateMips will silently fail.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GenerateMips(ShaderResourceView shaderResourceViewRef)
        {
            deviceContext.GenerateMips(shaderResourceViewRef);
        }

        /// <summary>
        ///     Sends queued-up commands in the command buffer to the graphics processing unit
        ///     (GPU).
        /// </summary>
        /// <remarks>
        ///     Most applications don't need to call this method. If an application calls this
        ///     method when not necessary, it incurs a performance penalty. Each call to Flush
        ///     incurs a significant amount of overhead.When Microsoft Direct3D state-setting,
        ///     present, or draw commands are called by an application, those commands are queued
        ///     into an internal command buffer. Flush sends those commands to the GPU for processing.
        ///     Typically, the Direct3D runtime sends these commands to the GPU automatically
        ///     whenever the runtime determines that they need to be sent, such as when the command
        ///     buffer is full or when an application maps a resource. Flush sends the commands
        ///     manually.We recommend that you use Flush when the CPU waits for an arbitrary
        ///     amount of time (such as when you call the Sleep function).Because Flush operates
        ///     asynchronously, it can return either before or after the GPU finishes executing
        ///     the queued graphics commands. However, the graphics commands eventually always
        ///     complete. You can call the SharpDX.Direct3D11.Device.CreateQuery(SharpDX.Direct3D11.QueryDescription,SharpDX.Direct3D11.Query)
        ///     method with the SharpDX.Direct3D11.QueryType.Event value to create an event query;
        ///     you can then use that event query in a call to the SharpDX.Direct3D11.DeviceContext.GetDataInternal(SharpDX.Direct3D11.Asynchronous,System.IntPtr,System.Int32,SharpDX.Direct3D11.AsynchronousFlags)
        ///     method to determine when the GPU is finished processing the graphics commands.
        ///     Microsoft Direct3D?11 defers the destruction of objects. Therefore, an application
        ///     can't rely upon objects immediately being destroyed. By calling Flush, you destroy
        ///     any objects whose destruction was deferred. If an application requires synchronous
        ///     destruction of an object, we recommend that the application release all its references,
        ///     call SharpDX.Direct3D11.DeviceContext.ClearState, and then call Flush.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            deviceContext.Flush();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            LastShaderPass = null;
            currRasterState = null;
            currBlendState = null;
            currDepthStencilState = null;
            currBlendFactor = null;
            currSampleMask = uint.MaxValue;
            currStencilRef = 0;
        }

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ResetDrawCalls()
        {
            int total = NumberOfDrawCalls;
            NumberOfDrawCalls = 0;
            return total;
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
    }
}