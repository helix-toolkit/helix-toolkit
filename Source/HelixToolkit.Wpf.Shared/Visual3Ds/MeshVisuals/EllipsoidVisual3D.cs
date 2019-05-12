// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EllipsoidVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows an axis aligned ellipsoid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows an axis aligned ellipsoid.
    /// </summary>
    public class EllipsoidVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center",
            typeof(Point3D),
            typeof(EllipsoidVisual3D),
            new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="PhiDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhiDivProperty = DependencyProperty.Register(
            "PhiDiv", typeof(int), typeof(EllipsoidVisual3D), new PropertyMetadata(30, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="RadiusX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register(
            "RadiusX", typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="RadiusY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register(
            "RadiusY", typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="RadiusZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusZProperty = DependencyProperty.Register(
            "RadiusZ", typeof(double), typeof(EllipsoidVisual3D), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(EllipsoidVisual3D), new PropertyMetadata(60, GeometryChanged));

        /// <summary>
        /// Gets or sets the center of the ellipsoid (this will set the transform of the element).
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
        /// Gets or sets the number of divisions in the phi direction (from "top" to "bottom").
        /// </summary>
        /// <value>The number of divisions.</value>
        public int PhiDiv
        {
            get
            {
                return (int)this.GetValue(PhiDivProperty);
            }

            set
            {
                this.SetValue(PhiDivProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the X equatorial radius of the ellipsoid.
        /// </summary>
        /// <value>The radius.</value>
        public double RadiusX
        {
            get
            {
                return (double)this.GetValue(RadiusXProperty);
            }

            set
            {
                this.SetValue(RadiusXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y equatorial radius of the ellipsoid.
        /// </summary>
        /// <value>The radius.</value>
        public double RadiusY
        {
            get
            {
                return (double)this.GetValue(RadiusYProperty);
            }

            set
            {
                this.SetValue(RadiusYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the polar radius of the ellipsoid.
        /// </summary>
        /// <value>The radius.</value>
        public double RadiusZ
        {
            get
            {
                return (double)this.GetValue(RadiusZProperty);
            }

            set
            {
                this.SetValue(RadiusZProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of divisions in the theta direction (around the sphere).
        /// </summary>
        /// <value>The number of divisions.</value>
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
        /// Do the tessellation and return the <see cref="MeshGeometry3D" />.
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry.
        /// </returns>
        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false, true);
            builder.AddEllipsoid(this.Center, this.RadiusX, this.RadiusY, this.RadiusZ, this.ThetaDiv, this.PhiDiv);
            return builder.ToMesh();
        }
    }
}