// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PieSliceVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a flat pie slice defined by center, normal, up vectors, inner and outer radius, start and end angles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a flat pie slice defined by center, normal, up vectors, inner and outer radius, start and end angles.
    /// </summary>
    public class PieSliceVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="EndAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
            "EndAngle", typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(90.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="InnerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.Register(
            "InnerRadius", typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(0.5, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Normal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal", typeof(Vector3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="OuterRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterRadiusProperty = DependencyProperty.Register(
            "OuterRadius", typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="StartAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            "StartAngle", typeof(double), typeof(PieSliceVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(PieSliceVisual3D), new UIPropertyMetadata(20, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="UpVector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpVectorProperty = DependencyProperty.Register(
            "UpVector", typeof(Vector3D), typeof(PieSliceVisual3D), new UIPropertyMetadata(new Vector3D(0, 1, 0), GeometryChanged));

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public Point3D Center
        {
            get
            {
                return (Point3D)this.GetValue(CenterProperty);
            }

            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end angle.
        /// </summary>
        /// <value>The end angle.</value>
        public double EndAngle
        {
            get
            {
                return (double)this.GetValue(EndAngleProperty);
            }

            set
            {
                this.SetValue(EndAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        /// <value>The inner radius.</value>
        public double InnerRadius
        {
            get
            {
                return (double)this.GetValue(InnerRadiusProperty);
            }

            set
            {
                this.SetValue(InnerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return (Vector3D)this.GetValue(NormalProperty);
            }

            set
            {
                this.SetValue(NormalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the outer radius.
        /// </summary>
        /// <value>The outer radius.</value>
        public double OuterRadius
        {
            get
            {
                return (double)this.GetValue(OuterRadiusProperty);
            }

            set
            {
                this.SetValue(OuterRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>The start angle.</value>
        public double StartAngle
        {
            get
            {
                return (double)this.GetValue(StartAngleProperty);
            }

            set
            {
                this.SetValue(StartAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of angular divisions of the slice.
        /// </summary>
        /// <value>The theta div.</value>
        public int ThetaDiv
        {
            get
            {
                return (int)this.GetValue(ThetaDivProperty);
            }

            set
            {
                this.SetValue(ThetaDivProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets up vector.
        /// </summary>
        /// <value>Up vector.</value>
        public Vector3D UpVector
        {
            get
            {
                return (Vector3D)this.GetValue(UpVectorProperty);
            }

            set
            {
                this.SetValue(UpVectorProperty, value);
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>A triangular mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            var pts = new List<Point3D>();
            var right = Vector3D.CrossProduct(this.UpVector, this.Normal);
            for (int i = 0; i < this.ThetaDiv; i++)
            {
                double angle = this.StartAngle + ((this.EndAngle - this.StartAngle) * i / (this.ThetaDiv - 1));
                double angleRad = angle / 180 * Math.PI;
                var dir = (right * Math.Cos(angleRad)) + (this.UpVector * Math.Sin(angleRad));
                pts.Add(this.Center + (dir * this.InnerRadius));
                pts.Add(this.Center + (dir * this.OuterRadius));
            }

            var b = new MeshBuilder(false, false);
            b.AddTriangleStrip(pts);
            return b.ToMesh();
        }
    }
}