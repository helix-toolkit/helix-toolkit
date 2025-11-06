using HelixToolkit.SharpDX;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Media = Microsoft.UI.Xaml.Media;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using Media = System.Windows.Media;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Core2D;
using HelixToolkit.Avalonia.SharpDX.Extensions;
using Avalonia.Interactivity;
using Media = Avalonia.Media;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public abstract class Element2D : Element2DCore, ITransformable2D, IHitable2D
{
    #region Dependency Properties
    /// <summary>
    /// 
    /// </summary>
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty VisibilityProperty =
        HelixProperty.Register<Element2D, UIVisibility>("Visibility",
#if AVALONIA
            true,
#else
            Visibility.Visible,
#endif
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Visibility = ((UIVisibility)e.NewValue!).ToD2DVisibility();
                }
            });

    /// <summary>
    /// 
    /// </summary>
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public UIVisibility Visibility
    {
        set
        {
            SetValue(VisibilityProperty, value);
        }
        get
        {
            return (UIVisibility)GetValue(VisibilityProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty IsHitTestVisibleProperty =
        HelixProperty.Register<Element2D, bool>("IsHitTestVisible",
            true,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.IsHitTestVisible = (bool)e.NewValue!;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public bool IsHitTestVisible
    {
        set
        {
            SetValue(IsHitTestVisibleProperty, value);
        }
        get
        {
            return (bool)GetValue(IsHitTestVisibleProperty)!;
        }
    }

    /// <summary>
    /// The is mouse over2 d property
    /// </summary>
#if false
#elif WINUI
#elif WPF
    new
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty IsMouseOverProperty =
        HelixProperty.Register<Element2D, bool>("IsMouseOver",
            false, (d, e) =>
            {
                if (d is Element2D model)
                {
                    model.SceneNode.IsMouseOver = (bool)e.NewValue!;
                    model.OnMouseOverChanged((bool)e.NewValue!, (bool)e.OldValue!);
                    model.InvalidateRender();
                }
            });

    /// <summary>
    /// Gets or sets a value indicating whether this instance is mouse over2 d.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is mouse over2 d; otherwise, <c>false</c>.
    /// </value>
#if false
#elif WINUI
#elif WPF
    new
#elif AVALONIA
#else
#error Unknown framework
#endif
    public bool IsMouseOver
    {
        get
        {
            return (bool)GetValue(IsMouseOverProperty)!;
        }
        set
        {
            SetValue(IsMouseOverProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty WidthProperty =
        HelixProperty.Register<Element2D, double>("Width",
            double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Width = (float)(double)e.NewValue!;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public double Width
    {
        set
        {
            SetValue(WidthProperty, value);
        }
        get
        {
            return (double)GetValue(WidthProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty HeightProperty =
        HelixProperty.Register<Element2D, double>("Height",
            double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Height = (float)(double)e.NewValue!;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public double Height
    {
        set
        {
            SetValue(HeightProperty, value);
        }
        get
        {
            return (double)GetValue(HeightProperty)!;
        }
    }

    public static readonly DependencyProperty MinimumWidthProperty =
        HelixProperty.Register<Element2D, double>("MinimumWidth",
            0.0,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MinimumWidth = (float)(double)e.NewValue!;
                }
            });

    public double MinimumWidth
    {
        set
        {
            SetValue(MinimumWidthProperty, value);
        }
        get
        {
            return (double)GetValue(MinimumWidthProperty)!;
        }
    }

    public static readonly DependencyProperty MinimumHeightProperty =
        HelixProperty.Register<Element2D, double>("MinimumHeight",
            0.0,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MinimumHeight = (float)(double)e.NewValue!;
                }
            });

    public double MinimumHeight
    {
        set
        {
            SetValue(MinimumHeightProperty, value);
        }
        get
        {
            return (double)GetValue(MinimumHeightProperty)!;
        }
    }

    public static readonly DependencyProperty MaximumWidthProperty =
        HelixProperty.Register<Element2D, double>("MaximumWidth",
            double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MaximumWidth = (float)(double)e.NewValue!;
                }
            });

    public double MaximumWidth
    {
        set
        {
            SetValue(MaximumWidthProperty, value);
        }
        get
        {
            return (double)GetValue(MaximumWidthProperty)!;
        }
    }

    public static readonly DependencyProperty MaximumHeightProperty =
        HelixProperty.Register<Element2D, double>("MaximumHeight",
            double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MaximumHeight = (float)(double)e.NewValue!;
                }
            });

    public double MaximumHeight
    {
        set
        {
            SetValue(MaximumHeightProperty, value);
        }
        get
        {
            return (double)GetValue(MaximumHeightProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public UIHorizontalAlignment HorizontalAlignment
    {
        get
        {
            return (UIHorizontalAlignment)GetValue(HorizontalAlignmentProperty)!;
        }
        set
        {
            SetValue(HorizontalAlignmentProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        HelixProperty.Register<Element2D, UIHorizontalAlignment>("HorizontalAlignment",
            UIHorizontalAlignment.Stretch,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.HorizontalAlignment = ((UIHorizontalAlignment)e.NewValue!).ToD2DHorizontalAlignment();
                    }
                });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public UIVerticalAlignment VerticalAlignment
    {
        get
        {
            return (UIVerticalAlignment)GetValue(VerticalAlignmentProperty)!;
        }
        set
        {
            SetValue(VerticalAlignmentProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty VerticalAlignmentProperty =
        HelixProperty.Register<Element2D, UIVerticalAlignment>("VerticalAlignment",
            UIVerticalAlignment.Stretch,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.VerticalAlignment = ((UIVerticalAlignment)e.NewValue!).ToD2DVerticalAlignment();
                    }
                });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public UIThickness Margin
    {
        get
        {
            return (UIThickness)GetValue(MarginProperty)!;
        }
        set
        {
            SetValue(MarginProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty MarginProperty =
        HelixProperty.Register<Element2D, UIThickness>("Margin",
            new UIThickness(),
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Margin = ((UIThickness)e.NewValue!).ToD2DThickness();
                }
            });

#if false
#elif WINUI
    public static readonly DependencyProperty TransformProperty =
        HelixProperty.Register<Element2D, Media.Transform?>("Transform",
        new Media.MatrixTransform { Matrix = Media.Matrix.Identity }, (d, e) =>
        {
            if (d is Element2DCore core)
            {
                core.SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.MatrixTransform)e.NewValue).Matrix.ToMatrix3x2();
            }
        });
#elif WPF
    public static readonly DependencyProperty TransformProperty =
        HelixProperty.Register<Element2D, Media.Transform?>("Transform", Media.Transform.Identity, (d, e) =>
        {
            if (d is Element2DCore core)
            {
                core.SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
            }
        });
#elif AVALONIA
    public static readonly DependencyProperty TransformProperty =
        HelixProperty.Register<Element2D, Media.Transform?>("Transform", new Media.MatrixTransform(), (d, e) =>
        {
            if (d is Element2DCore core)
            {
                core.SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
            }
        });
#else
#error Unknown framework
#endif

    /// <summary>
    /// Render transform
    /// </summary>
    public Media.Transform? Transform
    {
        get
        {
            return (Media.Transform?)GetValue(TransformProperty);
        }

        set
        {
            SetValue(TransformProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public Point RenderTransformOrigin
    {
        get
        {
            return (Point)GetValue(RenderTransformOriginProperty)!;
        }
        set
        {
            SetValue(RenderTransformOriginProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
    new
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty RenderTransformOriginProperty =
        HelixProperty.Register<Element2D, Point>("RenderTransformOrigin",
            new Point(0.5, 0.5),
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.RenderTransformOrigin = ((Point)e.NewValue!).ToVector2();
                }
            });

    public bool EnableBitmapCache
    {
        get
        {
            return (bool)GetValue(EnableBitmapCacheProperty)!;
        }
        set
        {
            SetValue(EnableBitmapCacheProperty, value);
        }
    }

    public static readonly DependencyProperty EnableBitmapCacheProperty =
        HelixProperty.Register<Element2D, bool>("EnableBitmapCache",
            false,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.EnableBitmapCache = (bool)e.NewValue!;
                    }
                });

    #endregion

    #region Events
    public delegate void Mouse2DRoutedEventHandler(object? sender, Mouse2DEventArgs e);

#if false
#elif WINUI
    public static readonly RoutedEvent MouseDown2DEvent = PointerPressedEvent;

    public static readonly RoutedEvent MouseUp2DEvent = PointerReleasedEvent;

    public static readonly RoutedEvent MouseMove2DEvent = PointerMovedEvent;

    public static readonly RoutedEvent MouseEnter2DEvent = PointerEnteredEvent;

    public static readonly RoutedEvent MouseLeave2DEvent = PointerExitedEvent;
#elif WPF
    public static readonly RoutedEvent MouseDown2DEvent =
        EventManager.RegisterRoutedEvent("MouseDown2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

    public static readonly RoutedEvent MouseUp2DEvent =
        EventManager.RegisterRoutedEvent("MouseUp2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

    public static readonly RoutedEvent MouseMove2DEvent =
        EventManager.RegisterRoutedEvent("MouseMove2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

    public static readonly RoutedEvent MouseEnter2DEvent =
        EventManager.RegisterRoutedEvent("MouseEnter2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

    public static readonly RoutedEvent MouseLeave2DEvent =
        EventManager.RegisterRoutedEvent("MouseLeave2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));
#elif AVALONIA
    public static readonly RoutedEvent<Mouse2DEventArgs> MouseDown2DEvent =
        RoutedEvent.Register<Element2D, Mouse2DEventArgs>("MouseDown2D", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<Mouse2DEventArgs> MouseUp2DEvent =
        RoutedEvent.Register<Element2D, Mouse2DEventArgs>("MouseUp2D", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<Mouse2DEventArgs> MouseMove2DEvent =
        RoutedEvent.Register<Element2D, Mouse2DEventArgs>("MouseMove2D", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<Mouse2DEventArgs> MouseEnter2DEvent =
        RoutedEvent.Register<Element2D, Mouse2DEventArgs>("MouseEnter2D", RoutingStrategies.Bubble);

    public static readonly RoutedEvent<Mouse2DEventArgs> MouseLeave2DEvent =
        RoutedEvent.Register<Element2D, Mouse2DEventArgs>("MouseLeave2D", RoutingStrategies.Bubble);
#else
#error Unknown framework
#endif

    public event Mouse2DRoutedEventHandler MouseDown2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(MouseDown2DEvent, value);
#elif AVALONIA
            AddHandler(MouseDown2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(MouseDown2DEvent, value);
#elif AVALONIA
            RemoveHandler(MouseDown2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseUp2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(MouseUp2DEvent, value);
#elif AVALONIA
            AddHandler(MouseUp2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(MouseUp2DEvent, value);
#elif AVALONIA
            RemoveHandler(MouseUp2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseMove2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(MouseMove2DEvent, value);
#elif AVALONIA
            AddHandler(MouseMove2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(MouseMove2DEvent, value);
#elif AVALONIA
            RemoveHandler(MouseMove2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseEnter2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(MouseEnter2DEvent, value);
#elif AVALONIA
            AddHandler(MouseEnter2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(MouseEnter2DEvent, value);
#elif AVALONIA
            RemoveHandler(MouseEnter2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseLeave2D
    {
        add
        {
#if false
#elif WINUI
#elif WPF
            AddHandler(MouseLeave2DEvent, value);
#elif AVALONIA
            AddHandler(MouseLeave2DEvent, value);
#else
#error Unknown framework
#endif
        }
        remove
        {
#if false
#elif WINUI
#elif WPF
            RemoveHandler(MouseLeave2DEvent, value);
#elif AVALONIA
            RemoveHandler(MouseLeave2DEvent, value);
#else
#error Unknown framework
#endif
        }
    }
    #endregion

    public Element2D()
    {
        this.MouseEnter2D += Element2D_MouseEnter2D;
        this.MouseLeave2D += Element2D_MouseLeave2D;
    }

    private void Element2D_MouseEnter2D(object? sender, RoutedEventArgs e)
    {
        OnMouseEnter2D(e as Mouse2DEventArgs);
    }

    private void Element2D_MouseLeave2D(object? sender, RoutedEventArgs e)
    {
        OnMouseLeave2D(e as Mouse2DEventArgs);
    }

    public virtual void OnMouseEnter2D(Mouse2DEventArgs? e)
    {
        if (!IsAttached)
        {
            return;
        }
        IsMouseOver = true;
#if DEBUGMOUSEEVENT
        Console.WriteLine("Element2D_MouseEnter2D");
#endif
    }

    public virtual void OnMouseLeave2D(Mouse2DEventArgs? e)
    {
        if (!IsAttached)
        {
            return;
        }
        IsMouseOver = false;
#if DEBUGMOUSEEVENT
        Console.WriteLine("Element2D_MouseLeave2D");
#endif
    }

    protected virtual void OnMouseOverChanged(bool newValue, bool oldValue)
    {
#if DEBUGMOUSEEVENT
        Debug.WriteLine("OnMouseOverChanged:"+newValue);
#endif
    }

    public static implicit operator Element2D?(HelixToolkit.SharpDX.Model.Scene2D.SceneNode2D s)
    {
        return s.WrapperSource as Element2D;
    }
}
