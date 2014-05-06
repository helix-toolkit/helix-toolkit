namespace Workitem10046
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    public class FaceTheCameraBillboard:MeshElement3D
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

      #region Dependency properties ...
      /// <summary>
      /// Identifies the <see cref="Height"/> dependency property.
      /// </summary>
      public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
          "Height", typeof(double), typeof(FaceTheCameraBillboard), new PropertyMetadata(10.0, GeometryChanged));


      /// <summary>
      /// Identifies the <see cref="Width"/> dependency property.
      /// </summary>
      public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
          "Width", typeof(double), typeof(FaceTheCameraBillboard), new PropertyMetadata(10.0, GeometryChanged));

      /// <summary>
      /// Identifies the <see cref="OriginOffset"/> dependency property.
      /// </summary>
      public static readonly DependencyProperty OriginOffsetProperty = DependencyProperty.Register(
          "OriginOffset", typeof(double), typeof(FaceTheCameraBillboard), new PropertyMetadata(10.0, GeometryChanged));

      /// <summary>
      /// Identifies the <see cref="Origin"/> dependency property.
      /// </summary>
      public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
          "Origin",
          typeof(Point3D),
          typeof(FaceTheCameraBillboard),
          new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));
      #endregion

      #region Backing and other properties ...


      private TimeSpan lastRenderTime = new TimeSpan();

      /// <summary>
      /// Gets or sets the Height.
      /// </summary>
      /// <value>The Height.</value>
      public double Height
      {
         get
         {
            return (double)this.GetValue(HeightProperty);
         }

         set
         {
            this.SetValue(HeightProperty, value);
         }
      }

      /// <summary>
      /// Gets or sets the width.
      /// </summary>
      /// <value>The width.</value>
      public double Width
      {
         get
         {
            return (double)this.GetValue(WidthProperty);
         }

         set
         {
            this.SetValue(WidthProperty, value);
         }
      }

      /// <summary>
      /// Gets or sets the center point of the plane.
      /// </summary>
      /// <value>The origin.</value>
      public Point3D Origin
      {
         get
         {
            return (Point3D)this.GetValue(OriginProperty);
         }

         set
         {
            this.SetValue(OriginProperty, value);
         }
      }

      /// <summary>
      /// Gets or sets the origin offset, how far in front of the origin the billboard should be.
      /// </summary>
      /// <value>The origin offset.</value>
      public double OriginOffset
      {
         get
         {
            return (double)this.GetValue(OriginOffsetProperty);
         }

         set
         {
            this.SetValue(OriginOffsetProperty, value);
         }
      }

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
            if (value != this._IsRendering) {
               this._IsRendering = value;
               if (this._IsRendering) {
                  this.SubscribeToRenderingEvent();
               }
               else {
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
      protected void OnCompositionTargetRendering(object sender, RenderingEventArgs eventArgs)
      {

         if (this._IsRendering) {
            if (!Visual3DHelper.IsAttachedToViewport3D(this)) {
               return;
            }

            if (lastRenderTime == eventArgs.RenderingTime) {
               return;
            }

            lastRenderTime = eventArgs.RenderingTime;

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

         MeshBuilder builder = new MeshBuilder(false, true);
         // Initially use defaults
         Vector3D upVector = new Vector3D(0, 0, 1);
         Vector3D observerVector = new Vector3D(0, 1, 0);

         // Get the value from the camera if a camera exists.
         Viewport3D viewport = this.GetViewport3D();
         if (viewport != null) {
            ProjectionCamera camera = viewport.Camera as ProjectionCamera;
            if (camera != null) {
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
         builder.AddQuad(bl, br, tr, tl,
                  new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0)
                );
                //  new Point(0, 1),
                //new Point(1, 1),
                //new Point(1, 0),
                //new Point(0, 0));
         return builder.ToMesh();

      }
   }
}
