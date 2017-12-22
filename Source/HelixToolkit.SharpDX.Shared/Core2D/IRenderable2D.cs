/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct2D1;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public interface IRenderable2D : IDisposable
    {
        RectangleF Rect { set; get; }
        Matrix3x2 Transform { set; get; }
        bool IsRendering { set; get; }
        void Render(IRenderMatrices matrics, RenderTarget target);

        bool IsMouseOver { set; get; }
    }
}
