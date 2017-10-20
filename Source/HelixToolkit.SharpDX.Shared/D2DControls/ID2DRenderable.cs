using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelixToolkit.SharpDX.Shared.D2DControls
{
    public interface ID2DRenderable : IDisposable
    {
        bool IsRendering { set; get; }
        void Render(IRenderMatrices matrics, RenderTarget target);
    }
}
