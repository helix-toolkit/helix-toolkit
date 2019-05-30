// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Manipulator.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides an abstract base class for manipulators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///   Provides an abstract base class for manipulators.
    /// </summary>
    public abstract class Manipulator : UIElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(Manipulator), new UIPropertyMetadata((s, e) => ((Manipulator)s).ColorChanged()));

        /// <summary>
        /// Identifies the <see cref="Offset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset",
            typeof(Vector3D),
            typeof(Manipulator),
            new FrameworkPropertyMetadata(
                new Vector3D(0, 0, 0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).PositionChanged(e)));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(Point3D),
            typeof(Manipulator),
            new FrameworkPropertyMetadata(
                new Point3D(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).PositionChanged(e)));

        /// <summary>
        /// Identifies the <see cref="TargetTransform"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty =
            DependencyProperty.Register(
                "TargetTransform",
                typeof(Transform3D),
                typeof(Manipulator),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(Manipulator),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (s, e) => ((Manipulator)s).ValueChanged(e)));

        /// <summary>
        /// Identifies the <see cref="Material"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(Manipulator), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BackMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackMaterialProperty =
            DependencyProperty.Register("BackMaterial", typeof(Material), typeof(Manipulator), new PropertyMetadata(null));

        /// <summary>
        ///   Initializes a new instance of the <see cref="Manipulator" /> class.
        /// </summary>
        protected Manipulator()
        {
            this.Model = new GeometryModel3D();
            BindingOperations.SetBinding(this.Model, GeometryModel3D.MaterialProperty, new Binding("Material") { Source = this });
            BindingOperations.SetBinding(this.Model, GeometryModel3D.BackMaterialProperty, new Binding("BackMaterial") { Source = this });
            this.Visual3DModel = this.Model;
        }

        /// <summary>
        ///   Gets or sets the color of the manipulator.
        /// </summary>
        /// <value> The color. </value>
        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }

            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the material of the manipulator.
        /// </summary>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// Gets or sets the back material of the manipulator.
        /// </summary>
        public Material BackMaterial
        {
            get { return (Material)this.GetValue(BackMaterialProperty); }
            set { this.SetValue(BackMaterialProperty, value); }
        }

        /// <summary>
        ///   Gets or sets the offset of the visual (this vector is added to the Position point).
        /// </summary>
        /// <value> The offset. </value>
        public Vector3D Offset
        {
            get
            {
                return (Vector3D)this.GetValue(OffsetProperty);
            }

            set
            {
                this.SetValue(OffsetProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the position of the manipulator.
        /// </summary>
        /// <value> The position. </value>
        public Point3D Position
        {
            get
            {
                return (Point3D)this.GetValue(PositionProperty);
            }

            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the target transform.
        /// </summary>
        public Transform3D TargetTransform
        {
            get
            {
                return (Transform3D)this.GetValue(TargetTransformProperty);
            }

            set
            {
                this.SetValue(TargetTransformProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the manipulator value.
        /// </summary>
        /// <value> The value. </value>
        public double Value
        {
            get
            {
                return (double)this.GetValue(ValueProperty);
            }

            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the camera.
        /// </summary>
        protected ProjectionCamera Camera { get; set; }

        /// <summary>
        ///   Gets or sets the hit plane normal.
        /// </summary>
        protected Vector3D HitPlaneNormal { get; set; }

        /// <summary>
        ///   Gets or sets the model.
        /// </summary>
        protected GeometryModel3D Model { get; set; }

        /// <summary>
        ///   Gets or sets the parent viewport.
        /// </summary>
        protected Viewport3D ParentViewport { get; set; }

        /// <summary>
        /// Binds this manipulator to a given Visual3D.
        /// </summary>
        /// <param name="source">
        /// Source Visual3D which receives the manipulator transforms.
        /// </param>
        public virtual void Bind(ModelVisual3D source)
        {
            BindingOperations.SetBinding(this, TargetTransformProperty, new Binding("Transform") { Source = source });
            BindingOperations.SetBinding(this, Visual3D.TransformProperty, new Binding("Transform") { Source = source });
        }

        /// <summary>
        ///   Releases the binding of this manipulator.
        /// </summary>
        public virtual void UnBind()
        {
            BindingOperations.ClearBinding(this, TargetTransformProperty);
            BindingOperations.ClearBinding(this, Visual3D.TransformProperty);
        }

        /// <summary>
        /// Called when a property related to the geometry is changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected static void UpdateGeometry(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Manipulator)d).UpdateGeometry();
        }

        /// <summary>
        /// Projects the point on the hit plane.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="hitPlaneOrigin">
        /// The hit Plane Origin.
        /// </param>
        /// <param name="hitPlaneNormal">
        /// The hit plane normal (world coordinate system).
        /// </param>
        /// <returns>
        /// The point in world coordinates.
        /// </returns>
        protected virtual Point3D? GetHitPlanePoint(Point p, Point3D hitPlaneOrigin, Vector3D hitPlaneNormal)
        {
            return Viewport3DHelper.UnProject(this.ParentViewport, p, hitPlaneOrigin, hitPlaneNormal);
        }

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected abstract void UpdateGeometry();

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.ParentViewport = Visual3DHelper.GetViewport3D(this);
            this.Camera = this.ParentViewport.Camera as ProjectionCamera;
            var projectionCamera = this.Camera;
            if (projectionCamera != null)
            {
                this.HitPlaneNormal = projectionCamera.LookDirection;
            }

            this.CaptureMouse();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            this.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles changes in the Position property.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void PositionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Transform = new TranslateTransform3D(
                this.Position.X + this.Offset.X, this.Position.Y + this.Offset.Y, this.Position.Z + this.Offset.Z);
        }

        /// <summary>
        /// Handles changes in the Value property.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void ValueChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Transforms from world to local coordinates.
        /// </summary>
        /// <param name="worldPoint">
        /// The point (world coordinates).
        /// </param>
        /// <returns>
        /// Transformed vector (local coordinates).
        /// </returns>
        protected Point3D ToLocal(Point3D worldPoint)
        {
            var mat = Visual3DHelper.GetTransform(this);
            mat.Invert();
            var t = new MatrixTransform3D(mat);
            return t.Transform(worldPoint);
        }

        /// <summary>
        /// Transforms from local to world coordinates.
        /// </summary>
        /// <param name="point">
        /// The point (local coordinates).
        /// </param>
        /// <returns>
        /// Transformed point (world coordinates).
        /// </returns>
        protected Point3D ToWorld(Point3D point)
        {
            var mat = Visual3DHelper.GetTransform(this);
            var t = new MatrixTransform3D(mat);
            return t.Transform(point);
        }

        /// <summary>
        /// Transforms from local to world coordinates.
        /// </summary>
        /// <param name="vector">
        /// The vector (local coordinates).
        /// </param>
        /// <returns>
        /// Transformed vector (world coordinates).
        /// </returns>
        protected Vector3D ToWorld(Vector3D vector)
        {
            var mat = Visual3DHelper.GetTransform(this);
            var t = new MatrixTransform3D(mat);
            return t.Transform(vector);
        }

        /// <summary>
        ///   Handles changes in the Color property (this will override the materials).
        /// </summary>
        private void ColorChanged()
        {
            this.Material = MaterialHelper.CreateMaterial(this.Color);
            this.BackMaterial = this.Material;
        }
    }
}