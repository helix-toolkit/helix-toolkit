using HelixToolkit.SharpDX;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

public class MouseUp3DEventArgs : Mouse3DEventArgs
{
    public MouseUp3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, InputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseUp3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
    {
    }
}
