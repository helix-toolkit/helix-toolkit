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
        global::SharpDX.Direct2D1.DeviceContext DeviceContext { get; }
        double ActualWidth { get; }
        double ActualHeight { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RenderContext2D : DisposeObject, IRenderContext2D
    {
        public double ActualWidth { get { return renderHost.ActualWidth; } }
        public double ActualHeight { get { return renderHost.ActualHeight; } }
        public global::SharpDX.Direct2D1.DeviceContext DeviceContext { private set; get; }

        private IRenderHost renderHost;
        public RenderContext2D(global::SharpDX.Direct2D1.DeviceContext deviceContext, IRenderHost host)
        {
            DeviceContext = deviceContext;
            renderHost = host;
        }
    }
}
