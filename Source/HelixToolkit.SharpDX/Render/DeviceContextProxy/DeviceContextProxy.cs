﻿using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D;
using System.Runtime.CompilerServices;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
///
/// </summary>
public sealed partial class DeviceContextProxy : DisposeObject
{
    public static bool AutoSkipRedundantStateSetting { get; set; } = false;

    private DeviceContext? deviceContext;
    private readonly Device device;
    private RasterizerStateProxy? currRasterState = null;
    private DepthStencilStateProxy? currDepthStencilState = null;
    private int currStencilRef;
    private BlendStateProxy? currBlendState = null;
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
    public ShaderPass? CurrShaderPass
    {
        private set; get;
    }

    /// <summary>
    /// Gets the number of draw calls.
    /// </summary>
    /// <value>
    /// The number of draw calls.
    /// </value>
    public int NumberOfDrawCalls { private set; get; } = 0;
    #endregion Properties

    #region Constructor
    /// <summary>
    /// Initializes a new deferred context
    /// </summary>
    /// <param name="device">The device.</param>
    public DeviceContextProxy(Device device)
    {
        deviceContext = new DeviceContext(device);
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
    #endregion Constructor

    #region Cast

    /// <summary>
    /// Performs an implicit conversion from <see cref="DeviceContextProxy"/> to <see cref="DeviceContext"/>.
    /// </summary>
    /// <param name="proxy">The proxy.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator DeviceContext?(DeviceContextProxy proxy)
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

    #endregion Cast

    /// <summary>
    /// Resets this instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        currRasterState = null;
        currBlendState = null;
        currDepthStencilState = null;
        currBlendFactor = null;
        currSampleMask = uint.MaxValue;
        currStencilRef = 0;
        currInputLayout = null;
        PrimitiveTopology = PrimitiveTopology.Undefined;
        CurrShaderPass = null;
        for (var i = 0; i < ConstantBufferCheck.Length; ++i)
        {
            ConstantBufferCheck[i] = null;
        }
        for (var i = 0; i < SamplerStateCheck.Length; ++i)
        {
            SamplerStateCheck[i] = null;
        }
    }

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
        deviceContext?.ClearState();
        Reset();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposeManagedResources"></param>
    protected override void OnDispose(bool disposeManagedResources)
    {
        if (deviceContext != null && !deviceContext.IsDisposed)
        {
            deviceContext.ClearState();
            deviceContext.OutputMerger.ResetTargets();
        }
        if (IsDeferred)
        {
            RemoveAndDispose(ref deviceContext);
        }
        base.OnDispose(disposeManagedResources);
    }
}
