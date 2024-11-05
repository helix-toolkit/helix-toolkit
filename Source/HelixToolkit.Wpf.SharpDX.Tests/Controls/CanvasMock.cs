using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using HelixToolkit.Wpf.SharpDX.Controls;

namespace HelixToolkit.Wpf.SharpDX.Tests.Controls;

class CanvasMock : IRenderCanvas
{
    public IRenderHost RenderHost { private set; get; } = new DefaultRenderHost();

    public double DpiScale
    {
        set; get;
    }
    public bool EnableDpiScale
    {
        set; get;
    }

    public event EventHandler<RelayExceptionEventArgs>? ExceptionOccurred { add { } remove { } }

    public CanvasMock()
    {
        RenderHost.EffectsManager = new DefaultEffectsManager();
    }
}
