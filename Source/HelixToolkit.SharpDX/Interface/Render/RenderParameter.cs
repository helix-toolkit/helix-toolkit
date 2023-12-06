using SharpDX.Direct3D11;
using SharpDX;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public struct RenderParameter
{
    /// <summary>
    /// The render target view
    /// </summary>
    public RenderTargetView?[] RenderTargetView;
    /// <summary>
    /// The depth stencil view
    /// </summary>
    public DepthStencilView? DepthStencilView;
    /// <summary>
    /// Current rendered texture
    /// </summary>
    public Resource? CurrentTargetTexture;
    /// <summary>
    /// 
    /// </summary>
    public bool IsMSAATexture;
    /// <summary>
    /// The viewport regions
    /// </summary>
    public ViewportF ViewportRegion;
    /// <summary>
    /// The scissor region
    /// </summary>
    public Rectangle ScissorRegion;
    /// <summary>
    /// 
    /// </summary>
    public bool RenderLight;
    /// <summary>
    /// 
    /// </summary>
    public bool UpdatePerFrameData;
}
