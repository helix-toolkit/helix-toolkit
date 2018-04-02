using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using Model.Scene2D;
    using Extensions;
    public abstract class ShapeModel2D : Element2D
    {
        public static DependencyProperty FillProperty 
            = DependencyProperty.Register("Fill", typeof(Brush), typeof(ShapeModel2D), new PropertyMetadata(new SolidColorBrush(Colors.Black), 
                (d,e)=> 
                {
                    (d as ShapeModel2D).fillChanged = true;
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
        public static DependencyProperty StrokeProperty
            = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ShapeModel2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
                (d, e) =>
                {
                    (d as ShapeModel2D).strokeChanged = true;
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

        public static DependencyProperty StrokeDashCapProperty
        = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeDashCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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
            = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeStartLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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
        = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(ShapeModel2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeEndLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
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

        public static DependencyProperty StrokeDashArrayProperty
            = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ShapeModel2D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeDashArray = e.NewValue == null ? new float[0] : (e.NewValue as DoubleCollection).Select(x=>(float)x).ToArray();
                }));

        public DoubleCollection StrokeDashArray
        {
            set
            {
                SetValue(StrokeDashArrayProperty, value);
            }
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }
        }

        public static DependencyProperty StrokeDashOffsetProperty
            = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeDashOffset = (float)(double)e.NewValue;
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
        = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(ShapeModel2D), new PropertyMetadata(PenLineJoin.Bevel,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeLineJoin = ((PenLineJoin)e.NewValue).ToD2DLineJoin();
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
            = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeMiterLimit = (float)(double)e.NewValue;
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

        public static DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ShapeModel2D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeThickness = (float)(double)e.NewValue;
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
            get { return (DashStyle)GetValue(DashStyleProperty); }
            set { SetValue(DashStyleProperty, value); }
        }

        public static readonly DependencyProperty DashStyleProperty =
            DependencyProperty.Register("DashStyle", typeof(DashStyle), typeof(ShapeModel2D), new PropertyMetadata(DashStyles.Solid,
                (d,e)=> {
                    ((d as Element2DCore).SceneNode as ShapeNode2D).StrokeDashStyle = (e.NewValue as DashStyle).ToD2DDashStyle();
                }));


        #endregion

        private bool fillChanged = true;
        private bool strokeChanged = true;

        protected override void OnAttached()
        {
            fillChanged = true;
            strokeChanged = true;
        }

        protected override void OnUpdate(IRenderContext2D context)
        {
            base.OnUpdate(context);
            if (fillChanged)
            {
                (SceneNode as ShapeNode2D).Fill = Fill.ToD2DBrush(context.DeviceContext);
                fillChanged = false;
            }
            if (strokeChanged)
            {
                (SceneNode as ShapeNode2D).Stroke = Stroke.ToD2DBrush(context.DeviceContext);
                strokeChanged = false;
            }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            var c = node as ShapeNode2D;
            c.StrokeDashArray = StrokeDashArray == null ? new float[0] : StrokeDashArray.Select(x => (float)x).ToArray();
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
}
