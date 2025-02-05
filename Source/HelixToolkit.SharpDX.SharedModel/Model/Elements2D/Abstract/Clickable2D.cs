using HelixToolkit.SharpDX.Model.Scene2D;
using System.Diagnostics;
using System.Windows.Input;
#if false
#elif WINUI
#elif WPF
using System.Windows;
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

public abstract class Clickable2D : Border2D
{
    public static long DoubleClickThreshold { get; set; } = 300;

    #region Dependency Properties
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(Clickable2D), new PropertyMetadata(null));
    public ICommand? Command
    {
        set
        {
            SetValue(CommandProperty, value);
        }
        get
        {
            return (ICommand?)GetValue(CommandProperty);
        }
    }

    #endregion

    #region Events
#if false
#elif WINUI
#elif WPF
    public static readonly RoutedEvent Clicked2DEvent =
        EventManager.RegisterRoutedEvent("Clicked2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Clickable2D));

    public static readonly RoutedEvent DoubleClicked2DEvent =
        EventManager.RegisterRoutedEvent("DoubleClicked2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Clickable2D));

#else
#error Unknown framework
#endif

    public event Mouse2DRoutedEventHandler Clicked2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(Clicked2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(Clicked2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }

    public event Mouse2DRoutedEventHandler DoubleClicked2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(DoubleClicked2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(DoubleClicked2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }
    #endregion

#if false
#elif WINUI
#elif WPF
    private long lastClickedTime = 0;
#else
#error Unknown framework
#endif

    public Clickable2D()
    {
        MouseDown2D += Clickable2D_MouseDown2D;
        MouseEnter2D += Clickable2D_MouseEnter2D;
        MouseLeave2D += Clickable2D_MouseLeave2D;
    }

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new ClickableNode2D();
    }

    private void Clickable2D_MouseLeave2D(object? sender, Mouse2DEventArgs e)
    {

    }

    private void Clickable2D_MouseEnter2D(object? sender, Mouse2DEventArgs e)
    {

    }

    private void Clickable2D_MouseDown2D(object? sender, Mouse2DEventArgs e)
    {
#if false
#elif WINUI
#elif WPF
        if (e.InputArgs is TouchEventArgs || (e.InputArgs is MouseEventArgs m && m.LeftButton == MouseButtonState.Pressed))
        {
            long time = e.InputArgs.Timestamp;
            if (time - lastClickedTime < DoubleClickThreshold)
            {
                RaiseEvent(new Mouse2DEventArgs(DoubleClicked2DEvent, this));
#if DEBUG
                Debug.WriteLine("DoubleClicked2DEvent");
#endif
            }
            else
            {
                RaiseEvent(new Mouse2DEventArgs(Clicked2DEvent, this));
#if DEBUG
                Debug.WriteLine("Clicked2DEvent");
#endif
                Command?.Execute(e);
            }
            lastClickedTime = time;
        }
#else
#error Unknown framework
#endif
    }
}
