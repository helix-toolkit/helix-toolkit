using HelixToolkit.SharpDX;
using System.Windows;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public class Mouse2DEventArgs : RoutedEventArgs
{
    public HitTest2DResult? HitTest2DResult
    {
        get; private set;
    }

    public Viewport3DX? Viewport
    {
        get; private set;
    }

    public Point Position
    {
        get; private set;
    }

    public InputEventArgs? InputArgs
    {
        get; private set;
    }

    public Mouse2DEventArgs(RoutedEvent routedEvent, object? source, HitTest2DResult? hitTestResult, Point position, Viewport3DX? viewport = null, InputEventArgs? inputArgs = null)
        : base(routedEvent, source)
    {
        this.HitTest2DResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        InputArgs = inputArgs;
    }

    public Mouse2DEventArgs(RoutedEvent routedEvent, object? source, Viewport3DX? viewport = null)
        : base(routedEvent, source)
    {
        this.Viewport = viewport;
    }
}
