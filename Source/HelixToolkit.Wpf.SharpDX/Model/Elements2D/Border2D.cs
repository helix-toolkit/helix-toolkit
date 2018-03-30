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
    using Core2D;
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
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border2D), new PropertyMetadata(new Thickness(0,0,0,0), 
                (d, e) => { (d as Element2DCore).InvalidateMeasure(); }));

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

        public static DependencyProperty StrokeDashStyleProperty
            = DependencyProperty.Register("StrokeDashStyle", typeof(DashStyle), typeof(Border2D), new PropertyMetadata(DashStyles.Solid,
                (d, e) =>
                {
                    (d as Border2D).strokeStyleChanged = true;
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
        = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(Border2D), new PropertyMetadata(PenLineJoin.Miter,
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

        public static DependencyProperty BorderThicknessProperty
            = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border2D), 
                new PropertyMetadata(new Thickness(0,0,0,0), (d, e) =>
                {
                    var m = d as Border2D;
                    if (m.borderCore == null)
                    { return; }
                    var thick = (Thickness)e.NewValue;
                    m.borderCore.BorderThickness = new Vector4((float)thick.Left, (float)thick.Top, (float)thick.Right, (float)thick.Bottom);
                    m.InvalidateMeasure();
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
            borderCore.BorderThickness = new Vector4((float)BorderThickness.Left, (float)BorderThickness.Top, (float)BorderThickness.Right, (float)BorderThickness.Bottom);
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (strokeChanged)
            {
                borderCore.StrokeBrush = BorderBrush.ToD2DBrush(context.DeviceContext);
                strokeChanged = false;
            }
            if (strokeStyleChanged)
            {
                borderCore.StrokeStyle = new D2D.StrokeStyle(context.DeviceResources.Factory2D,
                    new D2D.StrokeStyleProperties()
                    {
                        DashCap = this.StrokeDashCap.ToD2DCapStyle(),
                        StartCap = StrokeStartLineCap.ToD2DCapStyle(),
                        EndCap = StrokeEndLineCap.ToD2DCapStyle(),
                        DashOffset = (float)StrokeDashOffset,
                        LineJoin = StrokeLineJoin.ToD2DLineJoin(),
                        MiterLimit = Math.Max(1, (float)StrokeMiterLimit),
                        DashStyle = StrokeDashStyle.ToD2DDashStyle()
                    });
                strokeStyleChanged = false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if (contentInternal != null)
            {
                var margin = new Size2F((float)(BorderThickness.Left/2 + Padding.Left + BorderThickness.Right/2 + Padding.Right), 
                    (float)(BorderThickness.Top/2 + Padding.Top + BorderThickness.Bottom/2 + Padding.Bottom));
                var childAvail = new Size2F(Math.Max(0, availableSize.Width - margin.Width), Math.Max(0, availableSize.Height - margin.Height));
                
                var size = base.MeasureOverride(childAvail);
                if(WidthInternal != float.PositiveInfinity && HeightInternal != float.PositiveInfinity)
                {
                    return availableSize;
                }
                else
                {
                    if(WidthInternal != float.PositiveInfinity)
                    {
                        size.Width = WidthInternal;
                    }
                    if(HeightInternal != float.PositiveInfinity)
                    {
                        size.Height = HeightInternal;
                    }
                    return size;
                }
            }
            else
            {
                return new Size2F((float)(BorderThickness.Left/2 + Padding.Left + BorderThickness.Right/2 + Padding.Right + MarginWidthHeight.X + WidthInternal == float.PositiveInfinity ? 0 : WidthInternal),
                    (float)(BorderThickness.Top/2 + Padding.Top + BorderThickness.Bottom/2 + Padding.Bottom + MarginWidthHeight.Y + HeightInternal == float.PositiveInfinity ? 0 : HeightInternal));
            }
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            var contentRect = new RectangleF(finalSize.Left, finalSize.Top, finalSize.Width, finalSize.Height);
            contentRect.Left += (float)(BorderThickness.Left/2 + Padding.Left);
            contentRect.Right -= (float)(BorderThickness.Right/2 + Padding.Right);
            contentRect.Top += (float)(BorderThickness.Top/2 + Padding.Top);
            contentRect.Bottom -= (float)(BorderThickness.Bottom/2 + Padding.Bottom);
            base.ArrangeOverride(contentRect);
            return finalSize;
        }
    }
}
