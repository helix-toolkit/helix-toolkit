using SharpDX.Mathematics.Interop;

namespace HelixToolkit.SharpDX.Core2D;

public class RectangleRenderCore2D : ShapeRenderCore2DBase
{
    protected override void OnRender(RenderContext2D context)
    {
        if (FillBrush != null)
        {
            context.DeviceContext.FillRectangle(LayoutBound.ToStruct<RectangleF, RawRectangleF>(), FillBrush);
        }
        if (StrokeBrush != null && StrokeStyle != null)
        {
            context.DeviceContext.DrawRectangle(LayoutBound.ToStruct<RectangleF, RawRectangleF>(), StrokeBrush, StrokeWidth, StrokeStyle);
        }
    }
}
