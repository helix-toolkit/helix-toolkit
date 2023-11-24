using HelixToolkit.SharpDX;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

public class MouseMove3DEventArgs : Mouse3DEventArgs
{
    public MouseMove3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, InputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseMove3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
    {
    }
}
