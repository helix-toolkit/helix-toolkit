/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using Mathematics;
    /// <summary>
    /// 
    /// </summary>
    public class EllipseRenderCore2D : ShapeRenderCore2DBase
    {
        private D2D.Ellipse ellipse = new D2D.Ellipse();

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnRender(RenderContext2D context)
        {
            ellipse.Point = LayoutBound.Center.ToRaw();
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
}
