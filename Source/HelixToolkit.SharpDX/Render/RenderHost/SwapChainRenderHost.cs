﻿using HelixToolkit.Logger;
using Microsoft.Extensions.Logging;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public class SwapChainRenderHost : DefaultRenderHost
{
    private static readonly ILogger logger = LogManager.Create<SwapChainRenderHost>();
    protected readonly IntPtr surface;
    /// <summary>
    /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
    /// </summary>
    /// <param name="surface">The window PTR.</param>
    public SwapChainRenderHost(IntPtr surface)
    {
        this.surface = surface;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
    /// </summary>
    /// <param name="surface">The surface.</param>
    /// <param name="createRenderer">The create renderer.</param>
    public SwapChainRenderHost(IntPtr surface, Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
    {
        this.surface = surface;
    }

    /// <summary>
    /// Creates the render buffer.
    /// </summary>
    /// <returns></returns>
    protected override DX11RenderBufferProxyBase? CreateRenderBuffer()
    {
        logger.LogInformation("Creating DX11SwapChainRenderBufferProxy");

        if (EffectsManager is null)
        {
            return null;
        }

        return new DX11SwapChainRenderBufferProxy(surface, EffectsManager);
    }
}
