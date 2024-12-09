using HelixToolkit.SharpDX;
#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public abstract class Mouse3DEventArgs
#if WPF
    : RoutedEventArgs
#endif
{
    public HitTestResult? HitTestResult { get; private set; }

    public Viewport3DX? Viewport { get; private set; }

    public Point Position { get; private set; }

    /// <summary>
    /// The original mouse/touch event that generated this one.
    /// 
    /// Useful for knowing what mouse button got pressed.
    /// </summary>
    public UIInputEventArgs? OriginalInputEventArgs
    {
        get; private set;
    }

#if WINUI
    private bool handled;
#endif

#if WPF
    new
#endif
    public bool Handled
    { // not overridable
        get
        {
#if WINUI
            return handled;
#else
            return base.Handled;
#endif
        }
        set
        {
            if (OriginalInputEventArgs != null)
                OriginalInputEventArgs.Handled = value; // ensuring that the original input event is also marked as Handled

#if WINUI
            handled = value;
#else
            base.Handled = value;
#endif
        }
    }

#if WINUI
    public Mouse3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
#else
    public Mouse3DEventArgs(RoutedEvent routedEvent, object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(routedEvent, source)
#endif
    {
        this.HitTestResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        this.OriginalInputEventArgs = originalInputEventArgs;
    }
}
