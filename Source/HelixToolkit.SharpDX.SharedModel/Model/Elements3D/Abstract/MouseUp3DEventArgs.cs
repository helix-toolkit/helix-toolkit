using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class MouseUp3DEventArgs : Mouse3DEventArgs
{
#if false
#elif WINUI
    public MouseUp3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
#elif WPF
    public MouseUp3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseUp3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
#else
#error Unknown framework
#endif
    {
    }
}
