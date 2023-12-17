using SharpDX.Mathematics.Interop;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// 
/// </summary>
public class EllipseRenderCore2D : ShapeRenderCore2DBase
{
    private D2D.Ellipse ellipse = new();

    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    protected override void OnRender(RenderContext2D context)
    {
        ellipse.Point = LayoutBound.Center.ToStruct<Vector2, RawVector2>();
        ellipse.RadiusX = LayoutBound.Width / 2;
        ellipse.RadiusY = LayoutBound.Height / 2;
        if (FillBrush != null)
        {
            context.DeviceContext.FillEllipse(ellipse, FillBrush);
        }
        if (StrokeBrush != null && StrokeStyle != null)
        {
            context.DeviceContext.DrawEllipse(ellipse, StrokeBrush, StrokeWidth, StrokeStyle);
        }
    }
}
