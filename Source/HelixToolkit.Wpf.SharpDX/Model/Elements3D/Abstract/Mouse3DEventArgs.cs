using System.Windows.Input;
using System.Windows;
using HelixToolkit.SharpDX;

namespace HelixToolkit.Wpf.SharpDX;

public abstract class Mouse3DEventArgs : RoutedEventArgs
{
    public HitTestResult? HitTestResult
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
    /// <summary>
    /// The original mouse/touch event that generated this one.
    /// 
    /// Useful for knowing what mouse button got pressed.
    /// </summary>
    public InputEventArgs? OriginalInputEventArgs
    {
        get; private set;
    }

    public new bool Handled
    { // not overridable
        get
        {
            return base.Handled;
        }
        set
        {
            if (OriginalInputEventArgs != null)
                OriginalInputEventArgs.Handled = value; // ensuring that the original input event is also marked as Handled
            base.Handled = value;
        }
    }

    public Mouse3DEventArgs(RoutedEvent routedEvent, object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, InputEventArgs? originalInputEventArgs = null)
        : base(routedEvent, source)
    {
        this.HitTestResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        this.OriginalInputEventArgs = originalInputEventArgs;
    }
}
