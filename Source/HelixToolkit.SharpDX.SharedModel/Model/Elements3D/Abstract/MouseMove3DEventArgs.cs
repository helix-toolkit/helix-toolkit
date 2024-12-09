using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class MouseMove3DEventArgs : Mouse3DEventArgs
{
#if WINUI
    public MouseMove3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
#else
    public MouseMove3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseMove3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
#endif
    {
    }
}
