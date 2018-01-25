using System;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderContext2D : IDisposable
    {
        global::SharpDX.Direct2D1.RenderTarget RenderTarget { set; get; }
        global::SharpDX.Direct2D1.DeviceContext DeviceContext { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RenderContext2D : DisposeObject, IRenderContext2D
    {
        public global::SharpDX.Direct2D1.RenderTarget RenderTarget { set; get; }
        public global::SharpDX.Direct2D1.DeviceContext DeviceContext { private set; get; }
        public RenderContext2D(IRenderHost host)
        {
            DeviceContext = Collect(new global::SharpDX.Direct2D1.DeviceContext(host.Device2D, global::SharpDX.Direct2D1.DeviceContextOptions.EnableMultithreadedOptimizations));
        }
    }
}
