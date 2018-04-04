#if DEBUG
//#define DEBUGMOUSEEVENT
#endif
using SharpDX;
using System.Windows;
using System.Windows.Input;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    using System;
    using System.Diagnostics;
    using Extensions;

    public abstract class Element2D : Element2DCore, ITransformable2D, IHitable2D
    {
        #region Dependency Properties
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element2D), new PropertyMetadata(Visibility.Visible, 
                (d, e) =>
                {
                    (d as Element2DCore).SceneNode.Visibility = ((Visibility)e.NewValue).ToD2DVisibility();
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
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element2D), new PropertyMetadata(true,
                (d,e)=> 
                {
                    (d as Element2DCore).SceneNode.IsHitTestVisible = (bool)e.NewValue;
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
        /// <summary>
        /// The is mouse over2 d property
        /// </summary>
        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Element2D),
                new PropertyMetadata(false, (d, e) =>
            {
                var model = d as Element2D;
                model.SceneNode.IsMouseOver = (bool)e.NewValue;
                model.OnMouseOverChanged((bool)e.NewValue, (bool)e.OldValue);
                model.InvalidateRender();
            }));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over2 d.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mouse over2 d; otherwise, <c>false</c>.
        /// </value>
        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Element2D),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).SceneNode.Width = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).SceneNode.Height = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(0.0, (d, e) => { (d as Element2DCore).SceneNode.MinimumWidth = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(0.0, (d, e) => { (d as Element2DCore).SceneNode.MinimumHeight = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).SceneNode.MaximumWidth = (float)(double)e.NewValue; }));

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
            new PropertyMetadata(double.PositiveInfinity, (d, e) => { (d as Element2DCore).SceneNode.MaximumHeight = (float)(double)e.NewValue; }));

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
            DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(Element2D), 
                new PropertyMetadata(HorizontalAlignment.Stretch, (d, e) => { (d as Element2DCore).SceneNode.HorizontalAlignment = ((HorizontalAlignment)e.NewValue).ToD2DHorizontalAlignment(); }));



        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }


        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(Element2D), 
                new PropertyMetadata(VerticalAlignment.Stretch, (d, e) => { (d as Element2DCore).SceneNode.VerticalAlignment = ((VerticalAlignment)e.NewValue).ToD2DVerticalAlignment(); }));




        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(Element2D), new PropertyMetadata(new Thickness(), 
                (d, e) => {
                    (d as Element2DCore).SceneNode.Margin = ((Thickness)e.NewValue).ToD2DThickness();
                }));

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Element2D), new PropertyMetadata(Media.Transform.Identity, (d, e) =>
            {
                (d as Element2DCore).SceneNode.ModelMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
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
                    (d as Element2DCore).SceneNode.RenderTransformOrigin = ((System.Windows.Point)e.NewValue).ToVector2();
                }));

        public bool EnableBitmapCache
        {
            get { return (bool)GetValue(EnableBitmapCacheProperty); }
            set { SetValue(EnableBitmapCacheProperty, value); }
        }

        public static readonly DependencyProperty EnableBitmapCacheProperty =
            DependencyProperty.Register("EnableBitmapCache", typeof(bool), typeof(Element2D),
                new PropertyMetadata(false, (d,e)=> { (d as Element2DCore).SceneNode.EnableBitmapCache = (bool)e.NewValue; }));


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

        public Element2D()
        {
            this.MouseEnter2D += Element2D_MouseEnter2D;
            this.MouseLeave2D += Element2D_MouseLeave2D;
        }


        protected virtual void Element2D_MouseLeave2D(object sender, RoutedEventArgs e)
        {
            if (!IsAttached) { return; }
            IsMouseOver = false;
#if DEBUGMOUSEEVENT
            Console.WriteLine("Element2D_MouseLeave2D");
#endif
        }

        protected virtual void Element2D_MouseEnter2D(object sender, RoutedEventArgs e)
        {
            if (!IsAttached) { return; }
            IsMouseOver = true;
#if DEBUGMOUSEEVENT
            Console.WriteLine("Element2D_MouseEnter2D");
#endif
        }

        protected virtual void OnMouseOverChanged(bool newValue, bool oldValue)
        {
#if DEBUGMOUSEEVENT
            Debug.WriteLine("OnMouseOverChanged:"+newValue);
#endif
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
