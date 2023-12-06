using HelixToolkit.SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

public sealed class MouseUp3DEventArgs : Mouse3DEventArgs
{
    public MouseUp3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
    { }
}
