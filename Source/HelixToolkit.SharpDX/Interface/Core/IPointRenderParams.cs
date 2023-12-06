using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IPointRenderParams
{
    /// <summary>
    /// 
    /// </summary>
    Color4 PointColor
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Width
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Height
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    PointFigure Figure
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float FigureRatio
    {
        set; get;
    }
}
