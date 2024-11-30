using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Core2D;
using HelixToolkit.WinUI.SharpDX.Extensions;
using Microsoft.UI.Xaml.Media;
using Thickness = Microsoft.UI.Xaml.Thickness;
using DashStyle = SharpDX.Direct2D1.DashStyle;
using DashStyles = SharpDX.Direct2D1.DashStyle;
#else
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows.Media;
using Thickness = System.Windows.Thickness;
using DashStyle = System.Windows.Media.DashStyle;
using DashStyles = System.Windows.Media.DashStyles;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public class Border2D : ContentElement2D
{
    public double CornerRadius
    {
        get
        {
            return (double)GetValue(CornerRadiusProperty);
        }
        set
        {
            SetValue(CornerRadiusProperty, value);
        }
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(double), typeof(Border2D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.CornerRadius = (float)(double)e.NewValue;
                }
            }));

    public Thickness Padding
    {
        get
        {
            return (Thickness)GetValue(PaddingProperty);
        }
        set
        {
            SetValue(PaddingProperty, value);
        }
    }

    public static readonly DependencyProperty PaddingProperty =
        DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border2D), new PropertyMetadata(new Thickness(0, 0, 0, 0),
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.Padding = ((Thickness)e.NewValue).ToD2DThickness();
                }
            }));

    #region Stroke properties
    public static readonly DependencyProperty BorderBrushProperty
        = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border2D), new PropertyMetadata(new SolidColorBrush(UIColors.Black),
            (d, e) =>
            {
                if (d is Border2D node)
                {
                    node.strokeChanged = true;
                }
            }));

    public Brush BorderBrush
    {
        set
        {
            SetValue(BorderBrushProperty, value);
        }
        get
        {
            return (Brush)GetValue(BorderBrushProperty);
        }
    }

    public static readonly DependencyProperty StrokeDashCapProperty
    = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: BorderNode2D node })
            {
                node.StrokeDashCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
            }
        }));

    public PenLineCap StrokeDashCap
    {
        set
        {
            SetValue(StrokeDashCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeDashCapProperty);
        }
    }

    public static readonly DependencyProperty StrokeStartLineCapProperty
        = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeStartLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
                }
            }));

    public PenLineCap StrokeStartLineCap
    {
        set
        {
            SetValue(StrokeStartLineCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeStartLineCapProperty);
        }
    }

    public static readonly DependencyProperty StrokeEndLineCapProperty
    = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: BorderNode2D node })
            {
                node.StrokeEndLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
            }
        }));

    public PenLineCap StrokeEndLineCap
    {
        set
        {
            SetValue(StrokeEndLineCapProperty, value);
        }
        get
        {
            return (PenLineCap)GetValue(StrokeEndLineCapProperty);
        }
    }

    public static readonly DependencyProperty StrokeDashStyleProperty
        = DependencyProperty.Register("StrokeDashStyle", typeof(DashStyle), typeof(Border2D), new PropertyMetadata(DashStyles.Solid,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeDashStyle = ((DashStyle)e.NewValue).ToD2DDashStyle();
                }
            }));

    public DashStyle StrokeDashStyle
    {
        set
        {
            SetValue(StrokeDashStyleProperty, value);
        }
        get
        {
            return (DashStyle)GetValue(StrokeDashStyleProperty);
        }
    }

    public static readonly DependencyProperty StrokeDashOffsetProperty
        = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(Border2D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeDashOffset = (float)(double)e.NewValue;
                }
            }));

    public double StrokeDashOffset
    {
        set
        {
            SetValue(StrokeDashOffsetProperty, value);
        }
        get
        {
            return (double)GetValue(StrokeDashOffsetProperty);
        }
    }

    public static readonly DependencyProperty StrokeLineJoinProperty
    = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(Border2D), new PropertyMetadata(PenLineJoin.Miter,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: BorderNode2D node })
            {
                node.StrokeLineJoin = ((PenLineJoin)e.NewValue).ToD2DLineJoin();
            }
        }));

    public PenLineJoin StrokeLineJoin
    {
        set
        {
            SetValue(StrokeLineJoinProperty, value);
        }
        get
        {
            return (PenLineJoin)GetValue(StrokeLineJoinProperty);
        }
    }

    public static readonly DependencyProperty StrokeMiterLimitProperty
        = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(Border2D), new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.StrokeMiterLimit = (float)(double)e.NewValue;
                }
            }));

    public double StrokeMiterLimit
    {
        set
        {
            SetValue(StrokeMiterLimitProperty, value);
        }
        get
        {
            return (double)GetValue(StrokeMiterLimitProperty);
        }
    }

    public static readonly DependencyProperty BorderThicknessProperty
        = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border2D),
            new PropertyMetadata(new Thickness(0, 0, 0, 0), (d, e) =>
            {
                if (d is Element2DCore { SceneNode: BorderNode2D node })
                {
                    node.BorderThickness = ((Thickness)e.NewValue).ToD2DThickness();
                }
            }));

    public Thickness BorderThickness
    {
        set
        {
            SetValue(BorderThicknessProperty, value);
        }
        get
        {
            return (Thickness)GetValue(BorderThicknessProperty);
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
