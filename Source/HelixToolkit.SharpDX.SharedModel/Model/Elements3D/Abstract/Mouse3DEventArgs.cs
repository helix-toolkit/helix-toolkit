using HelixToolkit.SharpDX;
#if false
#elif WINUI
#elif WPF
using System.Windows;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public abstract class Mouse3DEventArgs
#if false
#elif WINUI
#elif WPF
    : RoutedEventArgs
#else
#error Unknown framework
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

#if false
#elif WINUI
    private bool handled;
#elif WPF
#else
#error Unknown framework
#endif

#if false
#elif WINUI
#elif WPF
    new
#else
#error Unknown framework
#endif
    public bool Handled
    { // not overridable
        get
        {
#if false
#elif WINUI
            return handled;
#elif WPF
            return base.Handled;
#else
#error Unknown framework
#endif
        }
        set
        {
            if (OriginalInputEventArgs != null)
                OriginalInputEventArgs.Handled = value; // ensuring that the original input event is also marked as Handled

#if false
#elif WINUI
            handled = value;
#elif WPF
            base.Handled = value;
#else
#error Unknown framework
#endif
        }
    }

#if false
#elif WINUI
    public Mouse3DEventArgs(HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
#elif WPF
    public Mouse3DEventArgs(RoutedEvent routedEvent, object? source, HitTestResult? hitTestResult, Point position, Viewport3DX? viewport = null, UIInputEventArgs? originalInputEventArgs = null)
        : base(routedEvent, source)
#else
#error Unknown framework
#endif
    {
        this.HitTestResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        this.OriginalInputEventArgs = originalInputEventArgs;
    }
}
