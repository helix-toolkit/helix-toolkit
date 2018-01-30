using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using D2D = global::SharpDX.Direct2D1;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Extensions;
    using SharpDX;

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
                    if((d as Border2D).borderCore == null)
                    {
                        return;
                    }
                    (d as Border2D).borderCore.CornerRadius = (float)(double)e.NewValue;
                }));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border2D), new FrameworkPropertyMetadata(new Thickness(0,0,0,0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        #region Stroke properties
        public static DependencyProperty StrokeProperty
            = DependencyProperty.Register("Stroke", typeof(Brush), typeof(Border2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
                (d, e) =>
                {
                    (d as Border2D).strokeChanged = true;
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
        = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(Border2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                (d as Border2D).strokeStyleChanged = true;
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
                    (d as Border2D).strokeStyleChanged = true;
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
                (d as Border2D).strokeStyleChanged = true;
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
            = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(Border2D), new PropertyMetadata(new DoubleCollection(),
                (d, e) =>
                {
                    (d as Border2D).strokeStyleChanged = true;
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
            = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(Border2D), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    (d as Border2D).strokeStyleChanged = true;
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
        = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(Border2D), new PropertyMetadata(PenLineJoin.Bevel,
            (d, e) =>
            {
                (d as Border2D).strokeStyleChanged = true;
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
                    (d as Border2D).strokeStyleChanged = true;
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
            = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Border2D), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) =>
                {
                    if ((d as Border2D).borderCore == null)
                    { return; }
                    (d as Border2D).borderCore.StrokeThickness = (float)Math.Max(0, (double)e.NewValue);
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
        #endregion

        private bool strokeChanged = true;
        private bool strokeStyleChanged = true;

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                strokeChanged = true;
                strokeStyleChanged = true;
                OnAssignVariables();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void OnAssignVariables()
        {
            borderCore.CornerRadius = (float)this.CornerRadius;
            borderCore.StrokeThickness = (float)this.StrokeThickness;
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (strokeChanged)
            {
                borderCore.StrokeBrush = Stroke.ToD2DBrush(context.DeviceContext);
                strokeChanged = false;
            }
            if (strokeStyleChanged)
            {
                borderCore.StrokeStyle = new D2D.StrokeStyle(context.DeviceContext.Factory,
                    new D2D.StrokeStyleProperties()
                    {
                        DashCap = this.StrokeDashCap.ToD2DCapStyle(),
                        StartCap = StrokeStartLineCap.ToD2DCapStyle(),
                        EndCap = StrokeEndLineCap.ToD2DCapStyle(),
                        DashOffset = (float)StrokeDashOffset,
                        LineJoin = StrokeLineJoin.ToD2DLineJoin(),
                        MiterLimit = Math.Max(1, (float)StrokeMiterLimit),
                        DashStyle = D2D.DashStyle.Dash
                    },
                    StrokeDashArray.Select(x => (float)x).ToArray());
                strokeStyleChanged = false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if(contentInternal != null)
            {
                var margin = new Size2F((float)(StrokeThickness + Padding.Left + Padding.Right + MarginWidthHeight.X), (float)(StrokeThickness + Padding.Top + Padding.Bottom + MarginWidthHeight.Y));
                var childAvail = new Size2F(Math.Max(0, availableSize.Width - margin.Width), Math.Max(0, availableSize.Height - margin.Height));
                
                return base.MeasureOverride(childAvail);
            }
            else
            {
                return new Size2F((float)(StrokeThickness + Padding.Left + Padding.Right + MarginWidthHeight.X), (float)(StrokeThickness + Padding.Top + Padding.Bottom + MarginWidthHeight.Y));
            }
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            if (contentInternal != null)
            {
                var contentRect = new RectangleF(finalSize.Left, finalSize.Top, finalSize.Width, finalSize.Height);
                contentRect.Left += (float)(StrokeThickness + Padding.Left);
                contentRect.Right -= (float)(StrokeThickness + Padding.Right);
                contentRect.Top += (float)(StrokeThickness + Padding.Top);
                contentRect.Bottom -= (float)(StrokeThickness + Padding.Bottom);
                base.ArrangeOverride(contentRect);
                return finalSize;
            }
            else
            {
                return new RectangleF();
            }
        }
    }
}
