using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public sealed class EmptyRenderCore2D : IRenderCore2D
    {
        public event EventHandler<bool> OnInvalidateRenderer;
        public bool IsEmpty { get; } = true;
        public RectangleF Bound { set; get; }
        public bool IsRendering { set; get; }
        public bool IsMouseOver { set; get; }
        public Matrix3x2 Transform { set; get; }

        public void Attach(IRenderHost target)
        {

        }

        public void Detach()
        {

        }

        public void Dispose()
        {

        }

        public void Render(IRenderContext2D context)
        {

        }
    }
}
