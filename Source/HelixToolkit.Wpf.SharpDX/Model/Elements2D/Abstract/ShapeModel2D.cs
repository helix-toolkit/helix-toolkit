
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using D2D = global::SharpDX.Direct2D1;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
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
                (d as ShapeModel2D).strokeStyleChanged = true;
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
                    (d as ShapeModel2D).strokeStyleChanged = true;
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
                (d as ShapeModel2D).strokeStyleChanged = true;
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
                    (d as ShapeModel2D).strokeStyleChanged = true;
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
                    (d as ShapeModel2D).strokeStyleChanged = true;
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
                (d as ShapeModel2D).strokeStyleChanged = true;
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
                    (d as ShapeModel2D).strokeStyleChanged = true;
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
                    if((d as ShapeModel2D).shapeRenderable == null)
                    { return; }   
                    (d as ShapeModel2D).shapeRenderable.StrokeWidth = (float)Math.Max(0, (double)e.NewValue);
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
                    (d as ShapeModel2D).strokeStyleChanged = true;
                }));


        #endregion

        private bool fillChanged = true;
        private bool strokeChanged = true;
        private bool strokeStyleChanged = true;

        protected ShapeRenderCore2DBase shapeRenderable;


        protected override RenderCore2D CreateRenderCore()
        {
            shapeRenderable = CreateShapeRenderCore();
            AssignProperties();
            return shapeRenderable;
        }

        protected abstract ShapeRenderCore2DBase CreateShapeRenderCore();

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                fillChanged = true;
                strokeChanged = true;
                strokeStyleChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void AssignProperties()
        {
            shapeRenderable.StrokeWidth = (float)StrokeThickness;
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (fillChanged)
            {
                shapeRenderable.FillBrush = Fill.ToD2DBrush(context.DeviceContext);
                fillChanged = false;
            }
            if (strokeChanged)
            {
                shapeRenderable.StrokeBrush = Stroke.ToD2DBrush(context.DeviceContext);
                strokeChanged = false;
            }
            if (strokeStyleChanged)
            {
                shapeRenderable.StrokeStyle = new D2D.StrokeStyle(context.DeviceContext.Factory,
                    new D2D.StrokeStyleProperties()
                    {
                        DashCap = this.StrokeDashCap.ToD2DCapStyle(),
                        StartCap = StrokeStartLineCap.ToD2DCapStyle(),
                        EndCap = StrokeEndLineCap.ToD2DCapStyle(),
                        DashOffset = (float)StrokeDashOffset,
                        LineJoin = StrokeLineJoin.ToD2DLineJoin(),
                        MiterLimit = Math.Max(1, (float)StrokeMiterLimit),
                        DashStyle = DashStyle.ToD2DDashStyle()
                    },
                    StrokeDashArray == null ? new float[0] : StrokeDashArray.Select(x=>(float)x).ToArray());
                strokeStyleChanged = false;
            }
        }
    }
}
