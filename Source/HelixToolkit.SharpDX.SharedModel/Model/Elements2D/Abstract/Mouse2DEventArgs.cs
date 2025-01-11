using HelixToolkit.SharpDX;
#if false
#elif WINUI
using Microsoft.UI.Input;
#elif WPF
using System.Windows;
using System.Windows.Input;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public class Mouse2DEventArgs : RoutedEventArgs
{
#if false
#elif WINUI
    public RoutedEvent RoutedEvent
    {
        get; private set;
    }
#elif WPF
#else
#error Unknown framework
#endif

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
#if false
#elif WINUI
#elif WPF
        : base(routedEvent, source)
#else
#error Unknown framework
#endif
    {
#if false
#elif WINUI
        this.RoutedEvent = routedEvent;
#elif WPF
#else
#error Unknown framework
#endif

        this.HitTest2DResult = hitTestResult;
        this.Position = position;
        this.Viewport = viewport;
        InputArgs = inputArgs;
    }

    public Mouse2DEventArgs(RoutedEvent routedEvent, object? source, Viewport3DX? viewport = null)
#if false
#elif WINUI
#elif WPF
        : base(routedEvent, source)
#else
#error Unknown framework
#endif
    {
#if false
#elif WINUI
        this.RoutedEvent = routedEvent;
#elif WPF
#else
#error Unknown framework
#endif

        this.Viewport = viewport;
    }
}
