using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface ILineRenderParams
{
    /// <summary>
    /// 
    /// </summary>
    float Thickness
    {
        set; get;
    }

    /// <summary>
    /// 
    /// </summary>
    float Smoothness
    {
        set; get;
    }
    /// <summary>
    /// Final Line Color = LineColor * PerVertexLineColor
    /// </summary>
    Color4 LineColor
    {
        set; get;
    }
}
