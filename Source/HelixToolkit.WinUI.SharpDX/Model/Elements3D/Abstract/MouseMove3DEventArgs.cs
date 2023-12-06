using HelixToolkit.SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

public sealed class MouseMove3DEventArgs : Mouse3DEventArgs
{
    public MouseMove3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
    { }
}
