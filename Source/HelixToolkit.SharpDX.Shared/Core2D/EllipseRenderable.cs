/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
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
            ellipse.Point = LocalDrawingRect.Center;
            ellipse.RadiusX = LocalDrawingRect.Width / 2;
            ellipse.RadiusY = LocalDrawingRect.Height / 2;
            if (FillBrush != null)
            {
                context.D2DTarget.FillEllipse(ellipse, FillBrush);
            }
            if (StrokeBrush != null && StrokeStyle != null)
            {
                context.D2DTarget.DrawEllipse(ellipse, StrokeBrush, StrokeWidth, StrokeStyle);
            }
        }
    }
}
