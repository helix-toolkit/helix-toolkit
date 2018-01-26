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
    public class EllipseRenderable : ShapeRenderable2DBase
    {
        private D2D.Ellipse ellipse = new D2D.Ellipse();

        protected override void OnRender(IRenderContext2D context)
        {
            ellipse.Point = LayoutBound.Center;
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
