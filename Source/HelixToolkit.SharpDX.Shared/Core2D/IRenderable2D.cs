using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelixToolkit.SharpDX.Core2D
{
    public interface IRenderable2D : IDisposable
    {
        RectangleF Rect { set; get; }
        Matrix3x2 Transform { set; get; }
        bool IsRendering { set; get; }
        void Render(IRenderMatrices matrics, RenderTarget target);
    }
}
