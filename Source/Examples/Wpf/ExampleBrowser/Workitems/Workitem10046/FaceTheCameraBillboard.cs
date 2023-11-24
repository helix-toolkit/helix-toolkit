using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Workitem10046;

[DependencyProperty<double>("Height", DefaultValue = 10.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("Width", DefaultValue = 10.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<double>("OriginOffset", DefaultValue = 10.0, OnChanged = nameof(OnGeometryChanged))]
[DependencyProperty<Point3D>("Origin", OnChanged = nameof(OnGeometryChanged))]
public partial class FaceTheCameraBillboard : MeshElement3D
{
    // A quadrilateral defined by the four corner points.
    // tl              tr
    // +---------------+
    // |               |
    // |               |
    // +---------------+
    // bl              br

    // The texture coordinates are
    // (0,0)           (1,0)
    // +---------------+
    // |               |
    // |               |
    // +---------------+
    // (0,1)          (1,1)

    protected override void OnGeometryChanged()
    {
        base.OnGeometryChanged();
    }

    #region Backing and other properties ...

    private TimeSpan lastRenderTime = new();

    #endregion


    #region Rendering event manager stuff ...

    /// <summary>
    /// The is rendering flag.
    /// </summary>
    private bool _IsRendering;


    /// Lifted from HelixToolkit.Wpf.RenderingModelVisual3D

    /// <summary>
    /// The rendering event listener
    /// </summary>
    private readonly RenderingEventListener renderingEventListener;


    /// <summary>
    /// Subscribes to CompositionTarget.Rendering event.
    /// </summary>
    protected void SubscribeToRenderingEvent()
    {
        RenderingEventManager.AddListener(renderingEventListener);
    }

    /// <summary>
    /// Unsubscribes the CompositionTarget.Rendering event.
    /// </summary>
    protected void UnsubscribeRenderingEvent()
    {
        RenderingEventManager.RemoveListener(renderingEventListener);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is being rendered.
    /// When the visual is removed from the visual tree, this property should be set to false.
    /// </summary>
    public bool IsRendering
    {
        get
        {
            return this._IsRendering;
        }

        set
        {
            if (value != this._IsRendering)
            {
                this._IsRendering = value;
                if (this._IsRendering)
                {
                    this.SubscribeToRenderingEvent();
                }
                else
                {
                    this.UnsubscribeRenderingEvent();
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingModelVisual3D"/> class.
    /// </summary>
    public FaceTheCameraBillboard()
       : base()
    {
        renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
    }



    /// <summary>
    /// Handles the CompositionTarget.Rendering event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The <see cref="System.Windows.Media.RenderingEventArgs"/> instance containing the event data.</param>
    protected void OnCompositionTargetRendering(object? sender, RenderingEventArgs? eventArgs)
    {
        if (this._IsRendering)
        {
            if (!Visual3DHelper.IsAttachedToViewport3D(this))
            {
                return;
            }

            if (eventArgs is not null)
            {
                if (lastRenderTime == eventArgs.RenderingTime)
                {
                    return;
                }

                lastRenderTime = eventArgs.RenderingTime;
            }

            this.UpdateModel();
        }
    }

    /// <summary>
    /// Called when the parent of the 3-D visual object is changed.
    /// </summary>
    /// <param name="oldParent">
    /// A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the
    ///     <see
    ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
    /// object. If the
    ///     <see
    ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
    /// object did not have a previous parent, the value of the parameter is null.
    /// </param>
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
        base.OnVisualParentChanged(oldParent);
        var parent = VisualTreeHelper.GetParent(this);
        this.IsRendering = parent != null;
    }

    protected override MeshGeometry3D Tessellate()
    {
        // A quadrilateral defined by the four corner points.
        // tl              tr
        // +---------------+
        // |               |
        // |               |
        // +---------------+
        // bl              br

        // The texture coordinates are
        // (0,0)           (1,0)
        // +---------------+
        // |               |
        // |               |
        // +---------------+
        // (0,1)          (1,1)

        MeshBuilder builder = new(false, true);
        // Initially use defaults
        Vector3D upVector = new(0, 0, 1);
        Vector3D observerVector = new(0, 1, 0);

        // Get the value from the camera if a camera exists.
        Viewport3D? viewport = this.GetViewport3D();
        if (viewport != null)
        {
            if (viewport.Camera is ProjectionCamera camera)
            {
                Point3D cameraPosition = camera.Position;
                // Get direction to observer
                //upVector = camera.UpDirection;
                observerVector = Origin - camera.Position;
            }
        }
        observerVector.Normalize();
        upVector.Normalize();
        Vector3D widthVector = Vector3D.CrossProduct(upVector, observerVector);
        widthVector.Normalize();
        Vector3D heightVector = Vector3D.CrossProduct(widthVector, observerVector);
        heightVector.Normalize();


        Vector3D halfWidthVector = widthVector * Width * 0.5;
        Vector3D halfHeightVector = heightVector * Height * 0.5;
        // Centre of billboard
        Point3D centrePoint = Origin - (observerVector * OriginOffset);
        // Bottom-left corner to visual space
        Point3D bl = centrePoint + halfWidthVector + halfHeightVector;
        // Bottom-right corner to visual space
        Point3D br = centrePoint - halfWidthVector + halfHeightVector;
        // Top-right corner to visual space
        Point3D tr = centrePoint - halfWidthVector - halfHeightVector;
        // Top-left corner to visual space
        Point3D tl = centrePoint + halfWidthVector - halfHeightVector;
        builder.AddQuad(bl.ToVector(), br.ToVector(), tr.ToVector(), tl.ToVector(),
                 new Point(0, 1).ToVector(),
               new Point(1, 1).ToVector(),
               new Point(1, 0).ToVector(),
               new Point(0, 0).ToVector()
               );
        //  new Point(0, 1),
        //new Point(1, 1),
        //new Point(1, 0),
        //new Point(0, 0));
        return builder.ToMesh().ToMeshGeometry3D();
    }
}
