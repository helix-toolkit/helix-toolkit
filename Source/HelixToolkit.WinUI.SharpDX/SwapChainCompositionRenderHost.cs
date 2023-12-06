using HelixToolkit.Logger;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Render;
using Microsoft.Extensions.Logging;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// 
/// </summary>
public class SwapChainCompositionRenderHost : DefaultRenderHost
{
    private static readonly ILogger logger = LogManager.Create<SwapChainCompositionRenderHost>();
    /// <summary>
    /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
    /// </summary>
    public SwapChainCompositionRenderHost()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
    /// </summary>
    /// <param name="createRenderer">The create renderer.</param>
    public SwapChainCompositionRenderHost(Func<IDevice3DResources, IRenderer> createRenderer)
        : base(createRenderer)
    {
    }
    /// <summary>
    /// Creates the render buffer.
    /// </summary>
    /// <returns></returns>
    protected override DX11RenderBufferProxyBase? CreateRenderBuffer()
    {
        logger.LogInformation("DX11SwapChainCompositionRenderBufferProxy");
        return EffectsManager is null ? null : new DX11SwapChainCompositionRenderBufferProxy(EffectsManager);
    }
}
