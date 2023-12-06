using HelixToolkit.SharpDX;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

public class MouseDown3DEventArgs : Mouse3DEventArgs
{
    public MouseDown3DEventArgs(object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, InputEventArgs? originalInputEventArgs = null)
        : base(Element3D.MouseDown3DEvent, source, hitTestResult, position, viewport, originalInputEventArgs)
    {
    }
}
