/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Matrix = System.Numerics.Matrix4x4;
using Point = Windows.Foundation.Point;

namespace HelixToolkit.UWP
{
    using Model;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3DCore" />
    [TemplatePart(Name = "PART_ItemsContainer", Type = typeof(ItemsControl))]
    public abstract class Element3D : Element3DCore
    {
        #region Dependency Properties
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3D).SceneNode.Visible = (bool)e.NewValue && (d as Element3D).Visibility == Visibility.Visible;
                }));

        /// <summary>
        /// Indicates, if this element should be rendered.
        /// Use this also to make the model visible/unvisible
        /// default is true
        /// </summary>
        public bool IsRendering
        {
            get { return (bool)GetValue(IsRenderingProperty); }
            set { SetValue(IsRenderingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty Transform3DProperty =
            DependencyProperty.Register("Transform3D", typeof(Matrix), typeof(Element3D), new PropertyMetadata(Matrix.Identity,
                (d, e) =>
                {
                    (d as Element3DCore).SceneNode.ModelMatrix = (Matrix)e.NewValue;
                }));

        /// <summary>
        /// 
        /// </summary>
        public new Matrix Transform3D
        {
            get { return (Matrix)this.GetValue(Transform3DProperty); }
            set { this.SetValue(Transform3DProperty, value); }
        }

        /// <summary>
        /// Gets or sets the manual render order.
        /// </summary>
        /// <value>
        /// The render order.
        /// </value>
        public int RenderOrder
        {
            get { return (int)GetValue(RenderOrderProperty); }
            set { SetValue(RenderOrderProperty, value); }
        }

        /// <summary>
        /// The render order property
        /// </summary>
        public static readonly DependencyProperty RenderOrderProperty =
            DependencyProperty.Register("RenderOrder", typeof(int), typeof(Element3D), new PropertyMetadata(0, (d, e) =>
            {
                (d as Element3D).SceneNode.RenderOrder = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, (int)e.NewValue));
            }));
        #endregion
        private static readonly Size oneSize = new Size(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="Element3D"/> class.
        /// </summary>
        public Element3D()
        {
            this.DefaultStyleKey = typeof(Element3D);
            RegisterPropertyChangedCallback(VisibilityProperty, (s, e) =>
            {
                SceneNode.Visible = (Visibility)s.GetValue(e) == Visibility.Visible && IsRendering;
            });

            RegisterPropertyChangedCallback(IsHitTestVisibleProperty, (s, e) =>
            {
                SceneNode.IsHitTestVisible = (bool)s.GetValue(e);
            });            
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return oneSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return oneSize;
        }

        ///// <summary>
        ///// Invoked whenever application code or internal processes(such as a rebuilding layout pass) call ApplyTemplate.In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        ///// </summary>
        //protected override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    itemsContainer = GetTemplateChild("PART_ItemsContainer") as ItemsControl;
        //    itemsContainer?.Items.Clear();
        //}

        #region Events
        public event EventHandler<MouseDown3DEventArgs> OnMouse3DDown;

        public event EventHandler<MouseUp3DEventArgs> OnMouse3DUp;

        public event EventHandler<MouseMove3DEventArgs> OnMouse3DMove;

        internal void RaiseMouseDownEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null)
        {
            OnMouse3DDown?.Invoke(this, new MouseDown3DEventArgs(hitTestResult, p, viewport));
        }

        internal void RaiseMouseUpEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null)
        {
            OnMouse3DUp?.Invoke(this, new MouseUp3DEventArgs(hitTestResult, p, viewport));
        }

        internal void RaiseMouseMoveEvent(HitTestResult hitTestResult, Point p, Viewport3DX viewport = null)
        {
            OnMouse3DMove?.Invoke(this, new MouseMove3DEventArgs(hitTestResult, p, viewport));
        }
        #endregion
    }

    public abstract class Mouse3DEventArgs
    {
        public HitTestResult HitTestResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public Point Position { get; private set; }

        public Mouse3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
        {
            this.HitTestResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
        }
    }

    public sealed class MouseDown3DEventArgs : Mouse3DEventArgs
    {
        public MouseDown3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(hitTestResult, position, viewport)
        { }
    }

    public sealed class MouseUp3DEventArgs : Mouse3DEventArgs
    {
        public MouseUp3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(hitTestResult, position, viewport)
        { }
    }

    public sealed class MouseMove3DEventArgs : Mouse3DEventArgs
    {
        public MouseMove3DEventArgs(HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(hitTestResult, position, viewport)
        { }
    }
}
