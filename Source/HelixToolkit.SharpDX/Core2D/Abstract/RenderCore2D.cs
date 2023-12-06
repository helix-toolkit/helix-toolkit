using SharpDX;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// 
/// </summary>
public abstract class RenderCore2D : DisposeObject
{
    /// <summary>
    /// Occurs when [on invalidate renderer].
    /// </summary>
    public event EventHandler<EventArgs>? InvalidateRender;

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty { get; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is rendering.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
    /// </value>
    public bool IsRendering
    {
        set; get;
    } = true;

    public IRenderHost? RenderHost
    {
        private set; get;
    } = null;

    private RectangleF rect = new();

    /// <summary>
    /// Absolute layout rectangle cooridnate for renderable
    /// </summary>
    public RectangleF LayoutBound
    {
        set
        {
            if (SetAffectsRender(ref rect, value))
            {
                OnLayoutBoundChanged(value);
            }
        }
        get
        {
            return rect;
        }
    }

    private RectangleF clippingBound = new();
    /// <summary>
    /// Gets or sets the layout clipping bound, includes border.
    /// </summary>
    /// <value>
    /// The layout clipping bound.
    /// </value>
    public RectangleF LayoutClippingBound
    {
        set
        {
            SetAffectsRender(ref clippingBound, value);
        }
        get
        {
            return clippingBound;
        }
    }

    private Matrix3x2 transform = Matrix3x2.Identity;

    /// <summary>
    /// Gets or sets the transform. <see cref="RenderCore2D.Transform"/>
    /// </summary>
    /// <value>
    /// The transform.
    /// </value>
    public Matrix3x2 Transform
    {
        set
        {
            SetAffectsRender(ref transform, value);
        }
        get
        {
            return transform;
        }
    }

    private Matrix3x2 localTransform = Matrix3x2.Identity;

    /// <summary>
    /// Gets or sets the local transform. This only transform local position. Same as RenderTransform
    /// </summary>
    /// <value>
    /// The local transform.
    /// </value>
    public Matrix3x2 LocalTransform
    {
        set
        {
            SetAffectsRender(ref localTransform, value);
        }
        get
        {
            return localTransform;
        }
    }

    private bool isMouseOver = false;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is mouse over.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
    /// </value>
    public bool IsMouseOver
    {
        set
        {
            SetAffectsRender(ref isMouseOver, value);
        }
        get
        {
            return isMouseOver;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is attached.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is attached; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttached { private set; get; } = false;
    /// <summary>
    /// Attaches the specified host.
    /// </summary>
    /// <param name="host">The host.</param>
    public void Attach(IRenderHost host)
    {
        if (IsAttached)
        {
            return;
        }
        if (host == null)
        {
            return;
        }
        RenderHost = host;
        IsAttached = OnAttach(host);
    }
    /// <summary>
    /// Called when [attach].
    /// </summary>
    /// <param name="host">The target.</param>
    /// <returns></returns>
    protected virtual bool OnAttach(IRenderHost host)
    {
        return true;
    }
    /// <summary>
    /// Detaches this instance.
    /// </summary>
    public void Detach()
    {
        if (!IsAttached)
        {
            return;
        }
        OnDetach();
        IsAttached = false;
    }
    /// <summary>
    /// Called when [detach].
    /// </summary>
    protected virtual void OnDetach()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layoutBound"></param>
    protected virtual void OnLayoutBoundChanged(RectangleF layoutBound)
    {
    }

    /// <summary>
    /// Renders the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public abstract void Render(RenderContext2D context);

    /// <summary>
    /// Invalidates the renderer.
    /// </summary>
    protected void InvalidateRenderer()
    {
        InvalidateRender?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="backingField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected bool SetAffectsRender<T>(ref T backingField, T value)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        InvalidateRenderer();
        return true;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        Detach();
        base.OnDispose(disposeManagedResources);
    }
}
