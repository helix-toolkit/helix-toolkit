namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[Flags]
public enum StateType
{
    None = 0,
    RasterState = 1,
    DepthStencilState = 1 << 2,
    BlendState = 1 << 3,
    All = RasterState | DepthStencilState | BlendState
}
