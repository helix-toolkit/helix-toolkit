using HelixToolkit.SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

public sealed class MouseDown3DEventArgs : Mouse3DEventArgs
{
    public MouseDown3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
        : base(hitTestResult, position, viewport, originalInputEventArgs)
    { }
}
