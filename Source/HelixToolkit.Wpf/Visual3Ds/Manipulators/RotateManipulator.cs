// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RotateManipulator.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element containing a manipulator that can rotate around an axis.
    /// </summary>
    public class RotateManipulator : Manipulator
    {
        /// <summary>
        /// Identifies the <see cref="Axis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register(
            "Axis",
            typeof(Vector3D),
            typeof(RotateManipulator),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(3.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="InnerDiameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register(
            "InnerDiameter", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(2.5, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(RotateManipulator), new UIPropertyMetadata(0.1, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Pivot"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PivotProperty = DependencyProperty.Register(
            "Pivot", typeof(Point3D), typeof(Manipulator), new PropertyMetadata(new Point3D()));

        /// <summary>
        /// The last point.
        /// </summary>
        private Point3D lastPoint;

        /// <summary>
        /// Initializes a new instance of the <see cref = "RotateManipulator" /> class.
        /// </summary>
        public RotateManipulator()
        {
            this.Model = new GeometryModel3D();
            this.Visual3DModel = this.Model;
            this.OnGeometryChanged();
        }

        /// <summary>
        /// Gets or sets the rotation axis.
        /// </summary>
        /// <value>The axis.</value>
        public Vector3D Axis
        {
            get
            {
                return (Vector3D)this.GetValue(AxisProperty);
            }

            set
            {
                this.SetValue(AxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diameter.
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
        /// Gets or sets the inner diameter.
        /// </summary>
        /// <value>The inner diameter.</value>
        public double InnerDiameter
        {
            get
            {
                return (double)this.GetValue(InnerDiameterProperty);
            }

            set
            {
                this.SetValue(InnerDiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the cylinder.
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

        /// <summary>
        /// Gets or sets the pivot point of the manipulator.
        /// </summary>
        /// <value> The position. </value>
        public Point3D Pivot
        {
            get
            {
                return (Point3D)this.GetValue(PivotProperty);
            }

            set
            {
                this.SetValue(PivotProperty, value);
            }
        }

        /// <summary>
        /// The on geometry changed.
        /// </summary>
        protected override void OnGeometryChanged()
        {
            var mb = new MeshBuilder(false, false);
            var p0 = new Point3D(0, 0, 0);
            var d = this.Axis;
            d.Normalize();
            var p1 = p0 - (d * this.Length * 0.5);
            var p2 = p0 + (d * this.Length * 0.5);
            mb.AddPipe(p1, p2, this.InnerDiameter, this.Diameter, 60);
            this.Model.Geometry = mb.ToMesh();
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
            var hitPlaneOrigin = this.ToWorld(this.Position);
            var hitPlaneNormal = this.ToWorld(this.Axis);
            var p = e.GetPosition(this.ParentViewport);

            var hitPoint = this.GetHitPlanePoint(p, hitPlaneOrigin, hitPlaneNormal);
            if (hitPoint != null)
            {
                this.lastPoint = this.ToLocal(hitPoint.Value);
            }
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
                var hitPlaneOrigin = this.ToWorld(this.Position);
                var hitPlaneNormal = this.ToWorld(this.Axis);

                var position = e.GetPosition(this.ParentViewport);
                var hitPoint = this.GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
                if (hitPoint == null)
                {
                    return;
                }

                var currentPoint = this.ToLocal(hitPoint.Value);

                var v = this.lastPoint - this.Position;
                var u = currentPoint - this.Position;
                v.Normalize();
                u.Normalize();

                var currentAxis = Vector3D.CrossProduct(u, v);
                double sign = -Vector3D.DotProduct(this.Axis, currentAxis);
                double theta = Math.Sign(sign) * Math.Asin(currentAxis.Length) / Math.PI * 180;
                this.Value += theta;

                if (this.TargetTransform != null)
                {
                    var rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(this.Axis, theta), Pivot);
                    this.TargetTransform = Transform3DHelper.CombineTransform(rotateTransform, this.TargetTransform);
                }

                hitPoint = this.GetHitPlanePoint(position, hitPlaneOrigin, hitPlaneNormal);
                if (hitPoint != null)
                {
                    this.lastPoint = this.ToLocal(hitPoint.Value);
                }
            }
        }

    }
}