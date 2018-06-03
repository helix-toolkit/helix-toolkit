using SharpDX;
using SharpDX.Direct3D11;
using global::SharpDX.Direct3D;

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
    using System.Collections.Generic;


    /// <summary>
    /// 
    /// </summary>
    public sealed class DeviceContextProxy : DisposeObject
    {
        private readonly DeviceContext deviceContext;
        private readonly Device device;

        public static bool AutoSkipRedundantStateSetting = false;
        private System.IntPtr currRasterState = System.IntPtr.Zero;
        private System.IntPtr currDepthStencilState = System.IntPtr.Zero;
        private int currStencilRef;
        private System.IntPtr currBlendState = System.IntPtr.Zero;
        private Color4? currBlendFactor = null;
        private uint currSampleMask = uint.MaxValue;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextProxy(Device device)
        {
            deviceContext = Collect(new DeviceContext(device));
            this.device = device;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextProxy"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="device">device</param>
        public DeviceContextProxy(DeviceContext context, Device device)
        {
            deviceContext = context;
            this.device = device;
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
            buffer.ClearRenderTarget(deviceContext, color);
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
        #endregion

        #region Implicit cast
        /// <summary>
        /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="DeviceContext"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DeviceContext(DeviceContextProxy proxy)
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
        #endregion

        #region Set states and targets
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
        /// Sets the state of the raster. 
        /// </summary>
        /// <param name="rasterState">State of the raster.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRasterState(RasterizerStateProxy rasterState)
        {
            if (AutoSkipRedundantStateSetting && currRasterState == rasterState.State.NativePointer)
            {
                return;
            }
            deviceContext.Rasterizer.State = rasterState;
            currRasterState = rasterState.State.NativePointer;
        }


        /// <summary>
        /// Sets the state of the depth stencil.
        /// </summary>
        /// <param name="depthStencilState">State of the depth stencil.</param>
        /// <param name="stencilRef">The stencil reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(DepthStencilStateProxy depthStencilState, int stencilRef = 0)
        {
            if (AutoSkipRedundantStateSetting && currDepthStencilState == depthStencilState.State.NativePointer && currStencilRef == stencilRef)
            {
                return;
            }
            deviceContext.OutputMerger.SetDepthStencilState(depthStencilState, stencilRef);
            currDepthStencilState = depthStencilState.State.NativePointer;
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
            if (AutoSkipRedundantStateSetting && currBlendState == blendState.State.NativePointer && blendFactor == currBlendFactor && currSampleMask == sampleMask)
            {
                return;
            }
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState.State.NativePointer;
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
            if (AutoSkipRedundantStateSetting && currBlendState == blendState.State.NativePointer && blendFactor == currBlendFactor && currSampleMask == sampleMask)
            {
                return;
            }
            deviceContext.OutputMerger.SetBlendState(blendState, blendFactor, sampleMask);
            currBlendState = blendState.State.NativePointer;
            currBlendFactor = blendFactor;
            currSampleMask = sampleMask;
        }

        /// <summary>
        /// Sets the state of the sampler on multiple stage.
        /// </summary>
        /// <param name="shaderStages">The shader stages.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplerStateMultiStages(ShaderStage shaderStages, int slot, SamplerState sampler)
        {
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Vertex))
            {
                deviceContext.VertexShader.SetSampler(slot, sampler);
            }
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Pixel))
            {
                deviceContext.PixelShader.SetSampler(slot, sampler);
            }
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Compute))
            {
                deviceContext.ComputeShader.SetSampler(slot, sampler);
            }
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Hull))
            {
                deviceContext.HullShader.SetSampler(slot, sampler);
            }
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Geometry))
            {
                deviceContext.GeometryShader.SetSampler(slot, sampler);
            }
            if (EnumHelper.HasFlag(shaderStages, ShaderStage.Domain))
            {
                deviceContext.DomainShader.SetSampler(slot, sampler);
            }
        }

        /// <summary>
        /// Sets the sampler state on single stage.
        /// </summary>
        /// <param name="singleShaderStage">The single stage.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplerStateSingleStage(ShaderStage singleShaderStage, int slot, SamplerStateProxy sampler)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                deviceContext.PixelShader.SetSampler(slot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                deviceContext.VertexShader.SetSampler(slot, sampler);
            }            
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                deviceContext.ComputeShader.SetSampler(slot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                deviceContext.HullShader.SetSampler(slot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                deviceContext.GeometryShader.SetSampler(slot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                deviceContext.DomainShader.SetSampler(slot, sampler);
            }
        }
        /// <summary>
        /// Sets the sampler states single stage.
        /// </summary>
        /// <param name="singleShaderStage">The single shader stage.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSamplerStatesSingleStage(ShaderStage singleShaderStage, int startSlot, SamplerState[] sampler)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                deviceContext.PixelShader.SetSamplers(startSlot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                deviceContext.VertexShader.SetSamplers(startSlot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                deviceContext.ComputeShader.SetSamplers(startSlot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                deviceContext.HullShader.SetSamplers(startSlot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                deviceContext.GeometryShader.SetSamplers(startSlot, sampler);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                deviceContext.DomainShader.SetSamplers(startSlot, sampler);
            }
        }

        private static readonly SamplerState[] EmptySamplerStateArray = new SamplerState[0];
        /// <summary>
        /// Gets the sampler states on single shader stage.
        /// </summary>
        /// <param name="singleShaderStage">The single shader stage.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SamplerState[] GetSamplerStatesSingleStage(ShaderStage singleShaderStage, int startSlot, int count)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                return deviceContext.PixelShader.GetSamplers(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                return deviceContext.VertexShader.GetSamplers(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                return deviceContext.ComputeShader.GetSamplers(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                return deviceContext.HullShader.GetSamplers(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                return deviceContext.GeometryShader.GetSamplers(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                return deviceContext.DomainShader.GetSamplers(startSlot, count);
            }
            else
            {
                return EmptySamplerStateArray;
            }
        }

        /// <summary>
        /// Sets the shader resource single.
        /// </summary>
        /// <param name="singleShaderStage">The single shader stage.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="resource">The resource.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResourceSingleStage(ShaderStage singleShaderStage, int slot, ShaderResourceViewProxy resource)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                deviceContext.PixelShader.SetShaderResource(slot, resource);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                deviceContext.VertexShader.SetShaderResource(slot, resource);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                deviceContext.ComputeShader.SetShaderResource(slot, resource);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                deviceContext.HullShader.SetShaderResource(slot, resource);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                deviceContext.GeometryShader.SetShaderResource(slot, resource);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                deviceContext.DomainShader.SetShaderResource(slot, resource);
            }
        }
        /// <summary>
        /// Sets the shader resources single stage.
        /// </summary>
        /// <param name="singleShaderStage">The single shader stage.</param>
        /// <param name="startSlot">The slot.</param>
        /// <param name="resources">The resource.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShaderResourcesSingleStage(ShaderStage singleShaderStage, int startSlot, ShaderResourceView[] resources)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                deviceContext.PixelShader.SetShaderResources(startSlot, resources);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                deviceContext.VertexShader.SetShaderResources(startSlot, resources);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                deviceContext.ComputeShader.SetShaderResources(startSlot, resources);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                deviceContext.HullShader.SetShaderResources(startSlot, resources);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                deviceContext.GeometryShader.SetShaderResources(startSlot, resources);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                deviceContext.DomainShader.SetShaderResources(startSlot, resources);
            }
        }

        private static readonly ShaderResourceView[] EmptyShaderResourceViewArray = new ShaderResourceView[0];
        /// <summary>
        /// Gets the shader resources single stage.
        /// </summary>
        /// <param name="singleShaderStage">The single shader stage.</param>
        /// <param name="startSlot">The start slot.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ShaderResourceView[] GetShaderResourcesSingleStage(ShaderStage singleShaderStage, int startSlot, int count)
        {
            if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Pixel))
            {
                return deviceContext.PixelShader.GetShaderResources(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Vertex))
            {
                return deviceContext.VertexShader.GetShaderResources(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Compute))
            {
                return deviceContext.ComputeShader.GetShaderResources(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Hull))
            {
                return deviceContext.HullShader.GetShaderResources(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Geometry))
            {
                return deviceContext.GeometryShader.GetShaderResources(startSlot, count);
            }
            else if (EnumHelper.HasFlag(singleShaderStage, ShaderStage.Domain))
            {
                return deviceContext.DomainShader.GetShaderResources(startSlot, count);
            }
            else { return EmptyShaderResourceViewArray; }
        }
        #endregion

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
        #endregion

        #region Set ShaderResources
        #region Vertex Shader
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(VertexShader shader, int slot, ShaderResourceView texture)
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
        public void BindTexture(VertexShader shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(VertexShader shader, int slot, SamplerState sampler)
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
        public void BindSampler(VertexShader shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.VertexShader.SetSamplers(slot, samplers);
        }

        #endregion

        #region Domain Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(DomainShader shader, int slot, ShaderResourceView texture)
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
        public void BindTexture(DomainShader shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(DomainShader shader, int slot, SamplerState sampler)
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
        public void BindSampler(DomainShader shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.DomainShader.SetSamplers(slot, samplers);
        }
        #endregion

        #region Pixel Shader

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(PixelShader shader, int slot, ShaderResourceView texture)
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
        public void BindTexture(PixelShader shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetShaderResources(slot, texture);
        }
        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(PixelShader shader, int slot, SamplerState sampler)
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
        public void BindSampler(PixelShader shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.PixelShader.SetSamplers(slot, samplers);
        }
        #endregion

        #region Compute Shader
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindTexture(ComputeShader shader, int slot, ShaderResourceView texture)
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
        public void BindTexture(ComputeShader shader, int slot, ShaderResourceView[] texture)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetShaderResources(slot, texture);
        }

        /// <summary>
        /// Binds the unordered access view.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The texture.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindUnorderedAccessView(ComputeShader shader, int slot, UnorderedAccessView uav)
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
        public void BindUnorderedAccessView(ComputeShader shader, int slot, UnorderedAccessView[] UAVs)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetUnorderedAccessViews(slot, UAVs);
        }

        /// <summary>
        /// Binds the sampler.
        /// </summary>
        /// <param name="shader">The shader.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BindSampler(ComputeShader shader, int slot, SamplerState sampler)
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
        public void BindSampler(ComputeShader shader, int slot, SamplerState[] samplers)
        {
            if (slot < 0)
            { return; }
            deviceContext.ComputeShader.SetSamplers(slot, samplers);
        }
        #endregion
        #endregion

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

        #endregion

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
        #endregion
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

        #endregion

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
        #endregion

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
        #endregion

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
        #endregion

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
        /// Resets this instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            LastShaderPass = null;
            currRasterState = System.IntPtr.Zero;
            currBlendState = System.IntPtr.Zero;
            currDepthStencilState = System.IntPtr.Zero;
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
