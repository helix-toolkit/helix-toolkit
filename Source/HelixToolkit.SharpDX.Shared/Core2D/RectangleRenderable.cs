using System;
using System.Collections.Generic;
using System.Text;
using HelixToolkit.Wpf.SharpDX;

namespace HelixToolkit.SharpDX.Core2D
{
    public class RectangleRenderable : ShapeRenderable2DBase
    {
        protected override void OnRender(IRenderMatrices matrices)
        {
            if (FillBrush != null)
            {
                RenderTarget.FillRectangle(LocalDrawingRect, FillBrush);
            }
            if (StrokeBrush != null && StrokeStyle != null)
            {
                RenderTarget.DrawRectangle(LocalDrawingRect, StrokeBrush, StrokeWidth, StrokeStyle);
            }
        }
    }
}
