using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Extensions;
    using Model.Scene2D;
    using SharpDX;
    using Thickness = System.Windows.Thickness;

    public class Border2D : ContentElement2D
    {
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(Border2D), new PropertyMetadata(0.0,
                (d,e)=> {
                    ((d as Element2DCore).SceneNode as BorderNode2D).CornerRadius = (float)(double)e.NewValue;
                }));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border2D), new PropertyMetadata(new Thickness(0,0,0,0), 
                (d, e) => 
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).Padding = ((Thickness)e.NewValue).ToD2DThickness();
                }));

        #region Stroke properties
        public static DependencyProperty BorderBrushProperty
            = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
                (d, e) =>
                {
                    (d as Border2D).strokeChanged = true;
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

        public static DependencyProperty StrokeDashCapProperty
        = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as BorderNode2D).StrokeDashCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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

        public static DependencyProperty StrokeStartLineCapProperty
            = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).StrokeStartLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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

        public static DependencyProperty StrokeEndLineCapProperty
        = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as BorderNode2D).StrokeEndLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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

        public static DependencyProperty StrokeDashStyleProperty
            = DependencyProperty.Register("StrokeDashStyle", typeof(DashStyle), typeof(Border2D), new PropertyMetadata(DashStyles.Solid,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).StrokeDashStyle = ((DashStyle)e.NewValue).ToD2DDashStyle();
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

        public static DependencyProperty StrokeDashOffsetProperty
            = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(Border2D), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).StrokeDashOffset = (float)(double)e.NewValue;
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

        public static DependencyProperty StrokeLineJoinProperty
        = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(Border2D), new PropertyMetadata(PenLineJoin.Miter,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as BorderNode2D).StrokeLineJoin = ((PenLineJoin)e.NewValue).ToD2DLineJoin();
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

        public static DependencyProperty StrokeMiterLimitProperty
            = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(Border2D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).StrokeMiterLimit = (float)(double)e.NewValue;
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

        public static DependencyProperty BorderThicknessProperty
            = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border2D), 
                new PropertyMetadata(new Thickness(0,0,0,0), (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as BorderNode2D).BorderThickness = ((Thickness)e.NewValue).ToD2DThickness();
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
            base.OnAttached();
            strokeChanged = true;
        }

        protected override void OnUpdate(IRenderContext2D context)
        {
            base.OnUpdate(context);
            if (strokeChanged)
            {
                (SceneNode as BorderNode2D).BorderBrush = BorderBrush.ToD2DBrush(context.DeviceContext);
                strokeChanged = false;
            }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            var c = node as BorderNode2D;
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
}
