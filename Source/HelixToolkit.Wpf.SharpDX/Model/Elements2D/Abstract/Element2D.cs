using SharpDX;
using System.Windows;
using System.Windows.Input;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using System.Diagnostics;

    public abstract class Element2D : Element2DCore, ITransformable2D, IHitable2D
    {
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element2D), new PropertyMetadata(Visibility.Visible, (d, e) =>
            {
                (d as Element2D).VisibilityInternal = (Visibility)e.NewValue;
            }));

        /// <summary>
        /// 
        /// </summary>
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

        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element2D), new PropertyMetadata(true, (d,e)=> 
            {
                (d as Element2DCore).IsHitTestVisibleInternal = (bool)e.NewValue;
            }));

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

        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Element2D), new PropertyMetadata(false, (d, e) =>
            {
                var model = d as Element2D;
                model.RenderCore.IsMouseOver = (bool)e.NewValue;
            }));

        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Element2D),
            new PropertyMetadata(0.0, (d, e) => { (d as Element2D).WidthInternal = (float)(double)e.NewValue; }));

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

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(Element2D),
            new PropertyMetadata(0.0, (d, e) => { (d as Element2D).HeightInternal = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(0.0, (d, e) => { (d as Element2D).MinimumWidthInternal = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(0.0, (d, e) => { (d as Element2D).MinimumHeightInternal = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(double.MaxValue, (d, e) => { (d as Element2D).MaximumWidthInternal = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(double.MaxValue, (d, e) => { (d as Element2D).MaximumHeightInternal = (float)(double)e.NewValue; }));

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



        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }


        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Element2D), new PropertyMetadata(HorizontalAlignment.Center, (d, e) => { (d as Element2D).HorizontalAlignmentInternal = (HorizontalAlignment)e.NewValue; }));



        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }


        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(Element2D), new PropertyMetadata(VerticalAlignment.Center, (d, e) => { (d as Element2D).VerticalAlignmentInternal = (VerticalAlignment)e.NewValue; }));




        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(Element2D), new PropertyMetadata(new Thickness(), 
                (d, e) => {
                    var t = (Thickness)e.NewValue;
                    (d as Element2D).MarginInternal = t;
                }));




        //public System.Windows.Point Position
        //{
        //    get { return (System.Windows.Point)GetValue(PositionProperty); }
        //    set { SetValue(PositionProperty, value); }
        //}

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty PositionProperty =
        //    DependencyProperty.Register("Position", typeof(System.Windows.Point), typeof(Element2D), new PropertyMetadata(new System.Windows.Point(0, 0),
        //        (d, e) => {
        //            var p = (System.Windows.Point)e.NewValue;
        //            (d as Element2D).PositionInternal = new Vector2((float)p.X, (float)p.Y);
        //        }));


        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Element2D), new PropertyMetadata(Media.Transform.Identity, (d, e) =>
            {
                (d as Element2D).ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
            }));

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



        public System.Windows.Point RenderTransformOrigin
        {
            get { return (System.Windows.Point)GetValue(RenderTransformOriginProperty); }
            set { SetValue(RenderTransformOriginProperty, value); }
        }

        public static readonly DependencyProperty RenderTransformOriginProperty =
            DependencyProperty.Register("RenderTransformOrigin", typeof(System.Windows.Point), typeof(Element2D), new PropertyMetadata(new System.Windows.Point(0.5,0.5),
                (d,e)=> {
                    (d as Element2D).RenderTransformOriginInternal = ((System.Windows.Point)e.NewValue).ToVector2();
                }));

        public bool EnableBitmapCache
        {
            get { return (bool)GetValue(EnableBitmapCacheProperty); }
            set { SetValue(EnableBitmapCacheProperty, value); }
        }

        public static readonly DependencyProperty EnableBitmapCacheProperty =
            DependencyProperty.Register("EnableBitmapCache", typeof(bool), typeof(Element2D),
                new PropertyMetadata(true, (d,e)=> { (d as Element2D).EnableBitmapCacheInternal = (bool)e.NewValue; }));


        #endregion


        #region Events
        public delegate void Mouse2DRoutedEventHandler(object sender, Mouse2DEventArgs e);
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

        public event Mouse2DRoutedEventHandler MouseDown2D
        {
            add { AddHandler(MouseDown2DEvent, value); }
            remove { RemoveHandler(MouseDown2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseUp2D
        {
            add { AddHandler(MouseUp2DEvent, value); }
            remove { RemoveHandler(MouseUp2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseMove2D
        {
            add { AddHandler(MouseMove2DEvent, value); }
            remove { RemoveHandler(MouseMove2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseEnter2D
        {
            add { AddHandler(MouseEnter2DEvent, value); }
            remove { RemoveHandler(MouseEnter2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseLeave2D
        {
            add { AddHandler(MouseLeave2DEvent, value); }
            remove { RemoveHandler(MouseLeave2DEvent, value); }
        }
        #endregion



        protected virtual void Element2D_MouseLeave2D(object sender, RoutedEventArgs e)
        {
            if (!IsAttached) { return; }
            IsMouseOver = false;
#if DEBUG
            Debug.WriteLine("Element2D_MouseLeave2D");
#endif
        }

        protected virtual void Element2D_MouseEnter2D(object sender, RoutedEventArgs e)
        {
            if (!IsAttached) { return; }
            IsMouseOver = true;
#if DEBUG
            Debug.WriteLine("Element2D_MouseEnter2D");
#endif
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                this.MouseEnter2D += Element2D_MouseEnter2D;
                this.MouseLeave2D += Element2D_MouseLeave2D;
                return true;
            }
            else { return false; }
        }


        protected override void OnDetach()
        {
            this.MouseEnter2D -= Element2D_MouseEnter2D;
            this.MouseLeave2D -= Element2D_MouseLeave2D;
            OnDetach();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            var pm = e.Property.DefaultMetadata;
            if(pm is FrameworkPropertyMetadata fm)
            {
                if (fm.AffectsMeasure)
                {
                    InvalidateMeasure();
                }
                else if (fm.AffectsArrange)
                {
                    InvalidateArrange();
                }
                if (fm.AffectsRender)
                {
                    InvalidateVisual();
                }
            }
        }
    }

    public class Mouse2DEventArgs : RoutedEventArgs
    {
        public HitTest2DResult HitTest2DResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public System.Windows.Point Position { get; private set; }

        public InputEventArgs InputArgs { get; private set; }
        public Mouse2DEventArgs(RoutedEvent routedEvent, object source, HitTest2DResult hitTestResult, System.Windows.Point position, Viewport3DX viewport = null, InputEventArgs inputArgs = null)
            : base(routedEvent, source)
        {
            this.HitTest2DResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
            InputArgs = inputArgs;
        }
        public Mouse2DEventArgs(RoutedEvent routedEvent, object source, Viewport3DX viewport = null)
            : base(routedEvent, source)
        {
            this.Viewport = viewport;
        }
    }
}
