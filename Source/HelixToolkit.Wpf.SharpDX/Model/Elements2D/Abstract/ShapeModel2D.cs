using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public abstract class ShapeModel2D : Element2D
{
    public static readonly DependencyProperty FillProperty
        = DependencyProperty.Register("Fill", typeof(Brush), typeof(ShapeModel2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
            (d, e) =>
            {
                if (d is ShapeModel2D node)
                {
                    node.fillChanged = true;
                }
            }));

    public Brush Fill
    {
        set
        {
            SetValue(FillProperty, value);
        }
        get
        {
            return (Brush)GetValue(FillProperty);
        }
    }

    #region Stroke properties
    public static readonly DependencyProperty StrokeProperty
        = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ShapeModel2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
            (d, e) =>
            {
                if (d is ShapeModel2D node)
                {
                    node.strokeChanged = true;
                }
            }));

    public Brush Stroke
    {
        set
        {
            SetValue(StrokeProperty, value);
        }
        get
        {
            return (Brush)GetValue(StrokeProperty);
        }
    }

    public static readonly DependencyProperty StrokeDashCapProperty
    = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: ShapeNode2D node })
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
        = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
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
    = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: ShapeNode2D node })
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

    public static readonly DependencyProperty StrokeDashArrayProperty
        = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ShapeModel2D), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
                {
                    node.StrokeDashArray = e.NewValue == null ? Array.Empty<float>() : (e.NewValue as DoubleCollection)?.ToFloatArray() ?? Array.Empty<float>();
                }
            }));

    public DoubleCollection? StrokeDashArray
    {
        set
        {
            SetValue(StrokeDashArrayProperty, value);
        }
        get
        {
            return (DoubleCollection?)GetValue(StrokeDashArrayProperty);
        }
    }

    public static readonly DependencyProperty StrokeDashOffsetProperty
        = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
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
    = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(ShapeModel2D), new PropertyMetadata(PenLineJoin.Bevel,
        (d, e) =>
        {
            if (d is Element2DCore { SceneNode: ShapeNode2D node })
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
        = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
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

    public static readonly DependencyProperty StrokeThicknessProperty
        = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
                {
                    node.StrokeThickness = (float)(double)e.NewValue;
                }
            }));

    public double StrokeThickness
    {
        set
        {
            SetValue(StrokeThicknessProperty, value);
        }
        get
        {
            return (double)GetValue(StrokeThicknessProperty);
        }
    }


    public DashStyle DashStyle
    {
        get
        {
            return (DashStyle)GetValue(DashStyleProperty);
        }
        set
        {
            SetValue(DashStyleProperty, value);
        }
    }

    public static readonly DependencyProperty DashStyleProperty =
        DependencyProperty.Register("DashStyle", typeof(DashStyle), typeof(ShapeModel2D), new PropertyMetadata(DashStyles.Solid,
            (d, e) =>
            {
                if (d is Element2DCore { SceneNode: ShapeNode2D node })
                {
                    node.StrokeDashStyle = (e.NewValue as DashStyle).ToD2DDashStyle();
                }
            }));

    #endregion

    private bool fillChanged = true;
    private bool strokeChanged = true;

    protected override void OnAttached()
    {
        fillChanged = true;
        strokeChanged = true;
    }

    protected override void OnUpdate(RenderContext2D context)
    {
        base.OnUpdate(context);
        if (fillChanged)
        {
            if (SceneNode is ShapeNode2D node)
            {
                node.Fill = Fill.ToD2DBrush(context.DeviceContext);
            }

            fillChanged = false;
        }
        if (strokeChanged)
        {
            if (SceneNode is ShapeNode2D node)
            {
                node.Stroke = Stroke.ToD2DBrush(context.DeviceContext);
            }

            strokeChanged = false;
        }
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
    {
        base.AssignDefaultValuesToSceneNode(node);

        if (node is not ShapeNode2D c)
        {
            return;
        }

        c.StrokeDashArray = StrokeDashArray == null ? Array.Empty<float>() : StrokeDashArray.ToFloatArray();
        c.StrokeDashCap = StrokeDashCap.ToD2DCapStyle();
        c.StrokeDashOffset = (float)StrokeDashOffset;
        c.StrokeEndLineCap = StrokeEndLineCap.ToD2DCapStyle();
        c.StrokeLineJoin = StrokeLineJoin.ToD2DLineJoin();
        c.StrokeMiterLimit = (float)StrokeMiterLimit;
        c.StrokeStartLineCap = StrokeStartLineCap.ToD2DCapStyle();
        c.StrokeThickness = (float)StrokeThickness;
        c.StrokeDashStyle = DashStyle.ToD2DDashStyle();
    }
}
