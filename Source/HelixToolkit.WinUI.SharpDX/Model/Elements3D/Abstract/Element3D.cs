﻿using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.WinUI.SharpDX.Model;
using SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Element3DCore" />
[TemplatePart(Name = "PART_Container", Type = typeof(ContentPresenter))]
public abstract class Element3D : Element3DCore, IVisible
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
                if (d is Element3D element)
                {
                    element.SceneNode.Visible = (bool)e.NewValue && element.Visibility == Visibility.Visible;
                }
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
    public static readonly DependencyProperty HxTransform3DProperty =
        DependencyProperty.Register("HxTransform3D", typeof(Matrix), typeof(Element3D), new PropertyMetadata(Matrix.Identity,
            (d, e) =>
            {
                if (d is Element3DCore element)
                {
                    element.SceneNode.ModelMatrix = (Matrix)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public Matrix HxTransform3D
    {
        get { return (Matrix)this.GetValue(HxTransform3DProperty); }
        set { this.SetValue(HxTransform3DProperty, value); }
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
            if (d is Element3D element)
            {
                element.SceneNode.RenderOrder = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, (int)e.NewValue));
            }
        }));

    #endregion
    private static readonly Size oneSize = new(1, 1);

    private ContentPresenter? presenter;
    private FrameworkElement? child;
    private long visibilityToken;
    private long isHitTestVisibleToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Element3D"/> class.
    /// </summary>
    public Element3D()
    {
        this.DefaultStyleKey = typeof(Element3D);

        OnSceneNodeCreated += Element3D_OnSceneNodeCreated;
        Loaded += Element3D_Loaded;
        Unloaded += Element3D_Unloaded;
    }

    private void Element3D_Unloaded(object? sender, RoutedEventArgs e)
    {
        UnregisterPropertyChangedCallback(VisibilityProperty, visibilityToken);
        UnregisterPropertyChangedCallback(IsHitTestVisibleProperty, isHitTestVisibleToken);
    }

    private void Element3D_Loaded(object? sender, RoutedEventArgs e)
    {
        SceneNode.Visible = Visibility == Visibility.Visible && IsRendering;
        SceneNode.IsHitTestVisible = IsHitTestVisible;
        visibilityToken = RegisterPropertyChangedCallback(VisibilityProperty, (s, arg) =>
        {
            SceneNode.Visible = (Visibility)s.GetValue(arg) == Visibility.Visible && IsRendering;
        });

        isHitTestVisibleToken = RegisterPropertyChangedCallback(IsHitTestVisibleProperty, (s, arg) =>
        {
            SceneNode.IsHitTestVisible = (bool)s.GetValue(arg);
        });
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        presenter = GetTemplateChild("PART_Container") as ContentPresenter;
        if (presenter == null)
        {
            throw new Exception("Template must contain a ContentPresenter named as PART_Container.");
        }
        presenter.Content = child;
    }

    protected void AttachChild(FrameworkElement? child)
    {
        this.child = child;
        if (presenter != null)
        {
            presenter.Content = child;
        }
    }

    protected void DetachChild(FrameworkElement child)
    {
        this.child = null;
        if (presenter != null)
        {
            presenter.Content = null;
        }
    }

    private void Element3D_OnSceneNodeCreated(object? sender, SceneNodeCreatedEventArgs e)
    {
        e.Node.MouseDown += Node_MouseDown;
        e.Node.MouseUp += Node_MouseUp;
        e.Node.MouseMove += Node_MouseMove;
    }

    private void Node_MouseMove(object? sender, SceneNodeMouseMoveArgs e)
    {
        RaiseMouseMoveEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
    }

    private void Node_MouseUp(object? sender, SceneNodeMouseUpArgs e)
    {
        RaiseMouseUpEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
    }

    private void Node_MouseDown(object? sender, SceneNodeMouseDownArgs e)
    {
        RaiseMouseDownEvent(e.HitResult, e.Position.ToPoint(), e.Viewport as Viewport3DX, e.OriginalInputEventArgs as PointerRoutedEventArgs);
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
    public event EventHandler<MouseDown3DEventArgs>? Mouse3DDown;

    public event EventHandler<MouseUp3DEventArgs>? Mouse3DUp;

    public event EventHandler<MouseMove3DEventArgs>? Mouse3DMove;

    internal void RaiseMouseDownEvent(HitTestResult hitTestResult, Point p, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
    {
        Mouse3DDown?.Invoke(this, new MouseDown3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
    }

    internal void RaiseMouseUpEvent(HitTestResult hitTestResult, Point p, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
    {
        Mouse3DUp?.Invoke(this, new MouseUp3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
    }

    internal void RaiseMouseMoveEvent(HitTestResult hitTestResult, Point p, Viewport3DX? viewport = null, PointerRoutedEventArgs? originalInputEventArgs = null)
    {
        Mouse3DMove?.Invoke(this, new MouseMove3DEventArgs(hitTestResult, p, viewport, originalInputEventArgs));
    }
    #endregion
}
