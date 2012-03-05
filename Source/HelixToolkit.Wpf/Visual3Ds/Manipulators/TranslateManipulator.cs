// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateManipulator.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a manipulator that can translate along an axis.
    /// </summary>
    public class TranslateManipulator : Manipulator
    {
        #region Constants and Fields

        /// <summary>
        ///   The diameter property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(TranslateManipulator), new UIPropertyMetadata(0.2, GeometryChanged));

        /// <summary>
        ///   The direction property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", 
            typeof(Vector3D), 
            typeof(TranslateManipulator), 
            new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        ///   The length property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(TranslateManipulator), new UIPropertyMetadata(2.0, GeometryChanged));

        /// <summary>
        ///   The last point.
        /// </summary>
        private Point3D lastPoint;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the diameter of the manipulator arrow.
        /// </summary>
        /// <value>The diameter.</value>
        public double Diameter
        {
            get
            {
                return (double)this.GetValue(DiameterProperty);
            }

            set
            {
                this.SetValue(DiameterProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the direction of the translation.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3D Direction
        {
            get
            {
                return (Vector3D)this.GetValue(DirectionProperty);
            }

            set
            {
                this.SetValue(DirectionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the length of the manipulator arrow.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get
            {
                return (double)this.GetValue(LengthProperty);
            }

            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when geometry has been changed.
        /// </summary>
        protected override void OnGeometryChanged()
        {
            var mb = new MeshBuilder(false,false);
            var p0 = new Point3D(0, 0, 0);
            Vector3D d = this.Direction;
            d.Normalize();
            Point3D p1 = p0 + d * this.Length;
            mb.AddArrow(p0, p1, this.Diameter);
            this.model.Geometry = mb.ToMesh();
        }

        /// <summary>
        /// The on mouse down.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Vector3D wDirection = this.ToWorld(this.Direction);

            Vector3D up = Vector3D.CrossProduct(this.Camera.LookDirection, wDirection);
            Point3D HitPlaneOrigin = this.ToWorld(this.Position);
            this.HitPlaneNormal = Vector3D.CrossProduct(up, wDirection);
            Point p = e.GetPosition(this.ParentViewport);

            Point3D? np = this.GetNearestPoint(p, HitPlaneOrigin, this.HitPlaneNormal);
            if (np == null)
            {
                return;
            }

            Point3D lp = this.ToLocal(np.Value);

            this.lastPoint = lp;
            this.CaptureMouse();
        }

        /// <summary>
        /// The on mouse move.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.IsMouseCaptured)
            {
                Point3D HitPlaneOrigin = this.ToWorld(this.Position);
                Point p = e.GetPosition(this.ParentViewport);
                Point3D? nearestPoint = this.GetNearestPoint(p, HitPlaneOrigin, this.HitPlaneNormal);
                if (nearestPoint == null)
                {
                    return;
                }

                Vector3D delta = this.ToLocal(nearestPoint.Value) - this.lastPoint;
                this.Value += Vector3D.DotProduct(delta, this.Direction);

                if (this.TargetTransform != null)
                {
                    var translateTransform = new TranslateTransform3D(delta);
                    this.TargetTransform = Transform3DHelper.CombineTransform(translateTransform, this.TargetTransform);

                    // this.Transform = Transform3DHelper.CombineTransform(translateTransform, this.Transform);
                }
                else
                {
                    this.Position += delta;
                }

                nearestPoint = this.GetNearestPoint(p, HitPlaneOrigin, this.HitPlaneNormal);
                if (nearestPoint != null)
                {
                    this.lastPoint = this.ToLocal(nearestPoint.Value);
                }
            }
        }

        /// <summary>
        /// Gets the nearest point on the translation axis.
        /// </summary>
        /// <param name="position">
        /// The position (in screen coordinates).
        /// </param>
        /// <param name="hitPlaneOrigin">
        /// The hit plane origin (world coordinate system).
        /// </param>
        /// <param name="hitPlaneNormal">
        /// The hit plane normal (world coordinate system).
        /// </param>
        /// <returns>
        /// The nearest point (world coordinates) or null if no point could be found.
        /// </returns>
        private Point3D? GetNearestPoint(Point position, Point3D hitPlaneOrigin, Vector3D hitPlaneNormal)
        {
            Point3D? hpp = this.GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
            if (hpp == null)
            {
                return null;
            }

            var ray = new Ray3D(this.ToWorld(this.Position), this.ToWorld(this.Direction));
            return ray.GetNearest(hpp.Value);
        }

        #endregion
    }
}