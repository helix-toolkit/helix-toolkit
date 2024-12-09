using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class MouseDown3DEventArgs : Mouse3DEventArgs
{
#if WINUI
    public MouseDown3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
#else
    public MouseDown3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseDown3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
#endif
    {
    }
}
