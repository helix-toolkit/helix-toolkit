using System;
using System.Collections.Generic;
using System.Text;
using HelixToolkit.Wpf.SharpDX;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D
{
    public class EllipseRenderable : ShapeRenderable2DBase
    {
        private D2D.Ellipse ellipse = new D2D.Ellipse();

        protected override void OnRender(IRenderMatrices matrices)
        {
            ellipse.Point = LocalDrawingRect.Center;
            ellipse.RadiusX = LocalDrawingRect.Width / 2;
            ellipse.RadiusY = LocalDrawingRect.Height / 2;
            if (FillBrush != null)
            {
                RenderTarget.FillEllipse(ellipse, FillBrush);
            }
            if (StrokeBrush != null && StrokeStyle != null)
            {
                RenderTarget.DrawEllipse(ellipse, StrokeBrush, StrokeWidth, StrokeStyle);
            }
        }
    }
}
