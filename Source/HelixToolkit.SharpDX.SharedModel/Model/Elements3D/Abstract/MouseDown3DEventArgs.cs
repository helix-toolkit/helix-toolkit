using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class MouseDown3DEventArgs : Mouse3DEventArgs
{
#if false
#elif WINUI
    public MouseDown3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
#elif WPF
    public MouseDown3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseDown3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
#else
#error Unknown framework
#endif
    {
    }
}
