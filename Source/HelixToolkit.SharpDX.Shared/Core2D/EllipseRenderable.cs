using System;
using System.Collections.Generic;
using System.Text;
using HelixToolkit.Wpf.SharpDX;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D
{
    public class EllipseRenderable : ShapeRenderable2DBase
    {
        public D2D.Ellipse ellipse { set; get; } = new D2D.Ellipse();

        protected override void OnRender(IRenderMatrices matrices)
        {
            if (StrokeBrush != null && StrokeStyle != null)
            {
                RenderTarget.DrawEllipse(ellipse, StrokeBrush, StrokeWidth, StrokeStyle);
            }
            if (FillBrush != null)
            {
                RenderTarget.FillEllipse(ellipse, FillBrush);
            }
        }
    }
}
