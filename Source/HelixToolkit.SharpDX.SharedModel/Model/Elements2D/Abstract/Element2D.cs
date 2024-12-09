using HelixToolkit.SharpDX;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Media = Microsoft.UI.Xaml.Media;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows;
using Media = System.Windows.Media;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public abstract class Element2D : Element2DCore, ITransformable2D, IHitable2D
{
    #region Dependency Properties
    /// <summary>
    /// 
    /// </summary>
#if WINUI
    new
#endif
    public static readonly DependencyProperty VisibilityProperty =
        DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element2D), new PropertyMetadata(Visibility.Visible,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Visibility = ((Visibility)e.NewValue).ToD2DVisibility();
                }
            }));

    /// <summary>
    /// 
    /// </summary>
#if WINUI
    new
#endif
    public Visibility Visibility
    {
        set
        {
            SetValue(VisibilityProperty, value);
        }
        get
        {
            return (Visibility)GetValue(VisibilityProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty IsHitTestVisibleProperty =
        DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element2D), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.IsHitTestVisible = (bool)e.NewValue;
                }
            }));

#if WINUI
    new
#endif
    public bool IsHitTestVisible
    {
        set
        {
            SetValue(IsHitTestVisibleProperty, value);
        }
        get
        {
            return (bool)GetValue(IsHitTestVisibleProperty);
        }
    }

    /// <summary>
    /// The is mouse over2 d property
    /// </summary>
#if WPF
    new
#endif
    public static readonly DependencyProperty IsMouseOverProperty =
        DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Element2D),
            new PropertyMetadata(false, (d, e) =>
            {
                if (d is Element2D model)
                {
                    model.SceneNode.IsMouseOver = (bool)e.NewValue;
                    model.OnMouseOverChanged((bool)e.NewValue, (bool)e.OldValue);
                    model.InvalidateRender();
                }
            }));

    /// <summary>
    /// Gets or sets a value indicating whether this instance is mouse over2 d.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is mouse over2 d; otherwise, <c>false</c>.
    /// </value>
#if WPF
    new
#endif
    public bool IsMouseOver
    {
        get
        {
            return (bool)GetValue(IsMouseOverProperty);
        }
        set
        {
            SetValue(IsMouseOverProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Element2D),
        new PropertyMetadata(double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Width = (float)(double)e.NewValue;
                }
            }));

#if WINUI
    new
#endif
    public double Width
    {
        set
        {
            SetValue(WidthProperty, value);
        }
        get
        {
            return (double)GetValue(WidthProperty);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(Element2D),
        new PropertyMetadata(double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Height = (float)(double)e.NewValue;
                }
            }));

#if WINUI
    new
#endif
    public double Height
    {
        set
        {
            SetValue(HeightProperty, value);
        }
        get
        {
            return (double)GetValue(HeightProperty);
        }
    }

    public static readonly DependencyProperty MinimumWidthProperty = DependencyProperty.Register("MinimumWidth", typeof(double), typeof(Element2D),
        new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MinimumWidth = (float)(double)e.NewValue;
                }
            }));

    public double MinimumWidth
    {
        set
        {
            SetValue(MinimumWidthProperty, value);
        }
        get
        {
            return (double)GetValue(MinimumWidthProperty);
        }
    }

    public static readonly DependencyProperty MinimumHeightProperty = DependencyProperty.Register("MinimumHeight", typeof(double), typeof(Element2D),
        new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MinimumHeight = (float)(double)e.NewValue;
                }
            }));

    public double MinimumHeight
    {
        set
        {
            SetValue(MinimumHeightProperty, value);
        }
        get
        {
            return (double)GetValue(MinimumHeightProperty);
        }
    }

    public static readonly DependencyProperty MaximumWidthProperty = DependencyProperty.Register("MaximumWidth", typeof(double), typeof(Element2D),
        new PropertyMetadata(double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MaximumWidth = (float)(double)e.NewValue;
                }
            }));

    public double MaximumWidth
    {
        set
        {
            SetValue(MaximumWidthProperty, value);
        }
        get
        {
            return (double)GetValue(MaximumWidthProperty);
        }
    }

    public static readonly DependencyProperty MaximumHeightProperty = DependencyProperty.Register("MaximumHeight", typeof(double), typeof(Element2D),
        new PropertyMetadata(double.PositiveInfinity,
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.MaximumHeight = (float)(double)e.NewValue;
                }
            }));

    public double MaximumHeight
    {
        set
        {
            SetValue(MaximumHeightProperty, value);
        }
        get
        {
            return (double)GetValue(MaximumHeightProperty);
        }
    }

#if WINUI
    new
#endif
    public HorizontalAlignment HorizontalAlignment
    {
        get
        {
            return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
        }
        set
        {
            SetValue(HorizontalAlignmentProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Element2D),
            new PropertyMetadata(HorizontalAlignment.Stretch,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.HorizontalAlignment = ((HorizontalAlignment)e.NewValue).ToD2DHorizontalAlignment();
                    }
                }));

#if WINUI
    new
#endif
    public VerticalAlignment VerticalAlignment
    {
        get
        {
            return (VerticalAlignment)GetValue(VerticalAlignmentProperty);
        }
        set
        {
            SetValue(VerticalAlignmentProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(Element2D),
            new PropertyMetadata(VerticalAlignment.Stretch,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.VerticalAlignment = ((VerticalAlignment)e.NewValue).ToD2DVerticalAlignment();
                    }
                }));

#if WINUI
    new
#endif
    public Thickness Margin
    {
        get
        {
            return (Thickness)GetValue(MarginProperty);
        }
        set
        {
            SetValue(MarginProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register("Margin", typeof(Thickness), typeof(Element2D), new PropertyMetadata(new Thickness(),
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.Margin = ((Thickness)e.NewValue).ToD2DThickness();
                }
            }));

#if WINUI
    public static readonly DependencyProperty TransformProperty =
        DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Element2D), new PropertyMetadata(new Media.MatrixTransform { Matrix = Media.Matrix.Identity }, (d, e) =>
        {
            if (d is Element2DCore core)
            {
                core.SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.MatrixTransform)e.NewValue).Matrix.ToMatrix3x2();
            }
        }));
#else
    public static readonly DependencyProperty TransformProperty =
        DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Element2D), new PropertyMetadata(Media.Transform.Identity, (d, e) =>
        {
            if (d is Element2DCore core)
            {
                core.SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
            }
        }));
#endif

    /// <summary>
    /// Render transform
    /// </summary>
    public Media.Transform Transform
    {
        get
        {
            return (Media.Transform)GetValue(TransformProperty);
        }

        set
        {
            SetValue(TransformProperty, value);
        }
    }

#if WINUI
    new
#endif
    public Point RenderTransformOrigin
    {
        get
        {
            return (Point)GetValue(RenderTransformOriginProperty);
        }
        set
        {
            SetValue(RenderTransformOriginProperty, value);
        }
    }

#if WINUI
    new
#endif
    public static readonly DependencyProperty RenderTransformOriginProperty =
        DependencyProperty.Register("RenderTransformOrigin", typeof(Point), typeof(Element2D), new PropertyMetadata(new Point(0.5, 0.5),
            (d, e) =>
            {
                if (d is Element2DCore core)
                {
                    core.SceneNode.RenderTransformOrigin = ((Point)e.NewValue).ToVector2();
                }
            }));

    public bool EnableBitmapCache
    {
        get
        {
            return (bool)GetValue(EnableBitmapCacheProperty);
        }
        set
        {
            SetValue(EnableBitmapCacheProperty, value);
        }
    }

    public static readonly DependencyProperty EnableBitmapCacheProperty =
        DependencyProperty.Register("EnableBitmapCache", typeof(bool), typeof(Element2D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    if (d is Element2DCore core)
                    {
                        core.SceneNode.EnableBitmapCache = (bool)e.NewValue;
                    }
                }));

    #endregion

    #region Events
    public delegate void Mouse2DRoutedEventHandler(object? sender, Mouse2DEventArgs e);

#if WINUI
    public static readonly RoutedEvent MouseDown2DEvent = PointerPressedEvent;

    public static readonly RoutedEvent MouseUp2DEvent = PointerReleasedEvent;

    public static readonly RoutedEvent MouseMove2DEvent = PointerMovedEvent;

    public static readonly RoutedEvent MouseEnter2DEvent = PointerEnteredEvent;

    public static readonly RoutedEvent MouseLeave2DEvent = PointerExitedEvent;
#else
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
#endif

    public event Mouse2DRoutedEventHandler MouseDown2D
    {
        add
        {
#if WPF
            AddHandler(MouseDown2DEvent, value);
#endif
        }
        remove
        {
#if WPF
            RemoveHandler(MouseDown2DEvent, value);
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseUp2D
    {
        add
        {
#if WPF
            AddHandler(MouseUp2DEvent, value);
#endif
        }
        remove
        {
#if WPF
            RemoveHandler(MouseUp2DEvent, value);
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseMove2D
    {
        add
        {
#if WPF
            AddHandler(MouseMove2DEvent, value);
#endif
        }
        remove
        {
#if WPF
            RemoveHandler(MouseMove2DEvent, value);
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseEnter2D
    {
        add
        {
#if WPF
            AddHandler(MouseEnter2DEvent, value);
#endif
        }
        remove
        {
#if WPF
            RemoveHandler(MouseEnter2DEvent, value);
#endif
        }
    }

    public event Mouse2DRoutedEventHandler MouseLeave2D
    {
        add
        {
#if WPF
            AddHandler(MouseLeave2DEvent, value);
#endif
        }
        remove
        {
#if WPF
            RemoveHandler(MouseLeave2DEvent, value);
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
