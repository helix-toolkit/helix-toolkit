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
        IDevice2DProxy RenderTarget { get; }
        RenderTarget D2DTarget
        {
            get;
        }
    }

    public class RenderContext2D : DisposeObject, IRenderContext2D
    {
        public IDevice2DProxy RenderTarget { private set; get; }

        public RenderTarget D2DTarget { get { return RenderTarget.D2DTarget; } }

        public RenderContext2D(IDevice2DProxy target)
        {
            RenderTarget = target;
        }
    }
}
