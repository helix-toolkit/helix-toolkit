using SharpDX;
using SharpDX.Mathematics.Interop;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// 
/// </summary>
public abstract class RenderCore2DBase : RenderCore2D
{
#if DEBUGBOUNDS
            /// <summary>
            /// 
            /// </summary>
            public bool ShowDrawingBorder { set; get; } = true;
#else
    /// <summary>
    /// 
    /// </summary>
    public bool ShowDrawingBorder { set; get; } = false;
#endif

    /// <summary>
    /// Renders the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Render(RenderContext2D context)
    {
        if (CanRender(context))
        {
            context.DeviceContext.Transform = Transform.ToStruct<Matrix3x2, RawMatrix3x2>();
            if (ShowDrawingBorder)
            {
                using var borderBrush = new D2D.SolidColorBrush(context.DeviceContext, new RawColor4(0, 1, 0, 1));
                using var borderDotStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.DashDot });
                using var borderLineStyle = new D2D.StrokeStyle(context.DeviceContext.Factory, new D2D.StrokeStyleProperties() { DashStyle = D2D.DashStyle.Solid });
                context.DeviceContext.DrawRectangle(LayoutBound.ToStruct<RectangleF, RawRectangleF>(), borderBrush, 1f, IsMouseOver ? borderLineStyle : borderDotStyle);
                context.DeviceContext.DrawRectangle(LayoutClippingBound.ToStruct<RectangleF, RawRectangleF>(), borderBrush, 0.5f, borderDotStyle);
            }
            OnRender(context);
        }
    }
    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="context">The context.</param>
    protected abstract void OnRender(RenderContext2D context);
    /// <summary>
    /// Determines whether this instance can render the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>
    ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanRender(RenderContext2D context)
    {
        return IsAttached && IsRendering;
    }
}
