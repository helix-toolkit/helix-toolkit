using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI.Xaml.Media;
using DashStyle = SharpDX.Direct2D1.DashStyle;
using DashStyles = SharpDX.Direct2D1.DashStyle;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows.Media;
using DashStyle = System.Windows.Media.DashStyle;
using DashStyles = System.Windows.Media.DashStyles;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Core2D;
using HelixToolkit.Avalonia.SharpDX.Extensions;
using Avalonia.Media;
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

public class Border2D : ContentElement2D
{
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public double CornerRadius
    {
        get
        {
            return (double)GetValue(CornerRadiusProperty)!;
        }
        set
        {
            SetValue(CornerRadiusProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty CornerRadiusProperty =
        HelixProperty.Register<Border2D, double>("CornerRadius",
            0.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.CornerRadius = (float)(double)e.NewValue!;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public UIThickness Padding
    {
        get
        {
            return (UIThickness)GetValue(PaddingProperty)!;
        }
        set
        {
            SetValue(PaddingProperty, value);
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty PaddingProperty =
        HelixProperty.Register<Border2D, UIThickness>("Padding",
            new UIThickness(0, 0, 0, 0),
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.Padding = ((UIThickness)e.NewValue!).ToD2DThickness();
                }
            });

    #region Stroke properties
#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty BorderBrushProperty =
        HelixProperty.Register<Border2D, Brush>("BorderBrush",
            new SolidColorBrush(UIColors.Black),
            (d, e) =>
            {
                if (d is Border2D node)
                {
                    node.strokeChanged = true;
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public Brush BorderBrush
    {
        set
        {
            SetValue(BorderBrushProperty, value);
        }
        get
        {
            return (Brush)GetValue(BorderBrushProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeDashCapProperty =
        HelixProperty.Register<Border2D, PenLineCap>("StrokeDashCap",
            PenLineCap.Flat,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: BorderNode2D node })
            {
                node.StrokeDashCap = ((PenLineCap)e.NewValue!).ToD2DCapStyle();
            }
        });

    public PenLineCap StrokeDashCap
    {
        set
        {
            SetValue(StrokeDashCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeDashCapProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeStartLineCapProperty =
        HelixProperty.Register<Border2D, PenLineCap>("StrokeStartLineCap",
            PenLineCap.Flat,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeStartLineCap = ((PenLineCap)e.NewValue!).ToD2DCapStyle();
                }
            });

    public PenLineCap StrokeStartLineCap
    {
        set
        {
            SetValue(StrokeStartLineCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeStartLineCapProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeEndLineCapProperty =
        HelixProperty.Register<Border2D, PenLineCap>("StrokeEndLineCap",
            PenLineCap.Flat,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeEndLineCap = ((PenLineCap)e.NewValue!).ToD2DCapStyle();
                }
            });

    public PenLineCap StrokeEndLineCap
    {
        set
        {
            SetValue(StrokeEndLineCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeEndLineCapProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeDashStyleProperty =
        HelixProperty.Register<Border2D, DashStyle>("StrokeDashStyle",
#if WINUI || WPF
            DashStyles.Solid,
#elif AVALONIA
            new DashStyle(),
#endif
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeDashStyle = ((DashStyle)e.NewValue!).ToD2DDashStyle();
                }
            });

    public DashStyle StrokeDashStyle
    {
        set
        {
            SetValue(StrokeDashStyleProperty, value);
        }
        get
        {
            return (DashStyle)GetValue(StrokeDashStyleProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeDashOffsetProperty =
        HelixProperty.Register<Border2D, double>("StrokeDashOffset",
            0.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeDashOffset = (float)(double)e.NewValue!;
                }
            });

    public double StrokeDashOffset
    {
        set
        {
            SetValue(StrokeDashOffsetProperty, value);
        }
        get
        {
            return (double)GetValue(StrokeDashOffsetProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeLineJoinProperty =
        HelixProperty.Register<Border2D, PenLineJoin>("StrokeLineJoin",
            PenLineJoin.Miter,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: BorderNode2D node })
            {
                node.StrokeLineJoin = ((PenLineJoin)e.NewValue!).ToD2DLineJoin();
            }
        });

    public PenLineJoin StrokeLineJoin
    {
        set
        {
            SetValue(StrokeLineJoinProperty, value);
        }
        get
        {
            return (PenLineJoin)GetValue(StrokeLineJoinProperty)!;
        }
    }

    public static readonly DependencyProperty StrokeMiterLimitProperty =
        HelixProperty.Register<Border2D, double>("StrokeMiterLimit",
            1.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeMiterLimit = (float)(double)e.NewValue!;
                }
            });

    public double StrokeMiterLimit
    {
        set
        {
            SetValue(StrokeMiterLimitProperty, value);
        }
        get
        {
            return (double)GetValue(StrokeMiterLimitProperty)!;
        }
    }

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public static readonly DependencyProperty BorderThicknessProperty =
        HelixProperty.Register<Border2D, UIThickness>("BorderThickness",
            new UIThickness(0, 0, 0, 0), (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.BorderThickness = ((UIThickness)e.NewValue!).ToD2DThickness();
                }
            });

#if false
#elif WINUI
    new
#elif WPF
#elif AVALONIA
#else
#error Unknown framework
#endif
    public UIThickness BorderThickness
    {
        set
        {
            SetValue(BorderThicknessProperty, value);
        }
        get
        {
            return (UIThickness)GetValue(BorderThicknessProperty)!;
        }
    }
    #endregion

    private bool strokeChanged = true;

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new BorderNode2D();
    }

    protected override void OnAttached()
    {
        strokeChanged = true;
        base.OnAttached();
    }

    protected override void OnUpdate(RenderContext2D context)
    {
        base.OnUpdate(context);

        if (strokeChanged)
        {
            if (SceneNode is BorderNode2D node)
            {
                node.BorderBrush = BorderBrush.ToD2DBrush(context.DeviceContext);
            }

            strokeChanged = false;
        }
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
    {
        base.AssignDefaultValuesToSceneNode(node);

        if (node is not BorderNode2D c)
        {
            return;
        }

        c.CornerRadius = (float)CornerRadius;
        c.Padding = Padding.ToD2DThickness();
        c.StrokeDashCap = StrokeDashCap.ToD2DCapStyle();
        c.StrokeDashOffset = (float)StrokeDashOffset;
        c.StrokeDashStyle = StrokeDashStyle.ToD2DDashStyle();
        c.StrokeEndLineCap = StrokeEndLineCap.ToD2DCapStyle();
        c.StrokeLineJoin = StrokeLineJoin.ToD2DLineJoin();
        c.StrokeMiterLimit = (float)StrokeMiterLimit;
        c.StrokeStartLineCap = StrokeStartLineCap.ToD2DCapStyle();
        c.BorderThickness = BorderThickness.ToD2DThickness();
    }
}
