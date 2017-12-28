/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class RectangleRenderable : ShapeRenderable2DBase
    {
        protected override void OnRender(IRenderContext matrices)
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
