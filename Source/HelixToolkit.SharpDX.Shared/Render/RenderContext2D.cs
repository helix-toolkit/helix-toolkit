using System;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core2D;
    using global::SharpDX.Direct2D1;

    public interface IRenderContext2D : IDisposable
    {
        ID2DTarget RenderTarget { get; }
        RenderTarget D2DTarget
        {
            get;
        }
    }

    public class RenderContext2D : DisposeObject, IRenderContext2D
    {
        public ID2DTarget RenderTarget { private set; get; }

        public RenderTarget D2DTarget { get { return RenderTarget.D2DTarget; } }

        public RenderContext2D(ID2DTarget target)
        {
            RenderTarget = target;
        }
    }
}
