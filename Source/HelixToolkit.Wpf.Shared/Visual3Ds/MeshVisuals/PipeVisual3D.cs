// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipeVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a pipe between two points.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a pipe between two points.
    /// </summary>
    public class PipeVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(PipeVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="InnerDiameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register(
            "InnerDiameter", typeof(double), typeof(PipeVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Point1"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point1Property = DependencyProperty.Register(
            "Point1",
            typeof(Point3D),
            typeof(PipeVisual3D),
            new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Point2"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Point2Property = DependencyProperty.Register(
            "Point2",
            typeof(Point3D),
            typeof(PipeVisual3D),
            new UIPropertyMetadata(new Point3D(0, 0, 10), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(PipeVisual3D), new UIPropertyMetadata(36, GeometryChanged));

        /// <summary>
        /// Gets or sets the (outer) diameter.
        /// </summary>
        /// <value>The diameter. The default value is <c>1</c>.</value>
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
        /// <value>The inner diameter. The default value is <c>0</c>.</value>
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
        /// Gets or sets the start point.
        /// </summary>
        /// <value>The start point. The default value is <c>0,0,0</c>.</value>
        public Point3D Point1
        {
            get
            {
                return (Point3D)this.GetValue(Point1Property);
            }

            set
            {
                this.SetValue(Point1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point. The default value is <c>0,0,10</c>.</value>
        public Point3D Point2
        {
            get
            {
                return (Point3D)this.GetValue(Point2Property);
            }

            set
            {
                this.SetValue(Point2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the theta div.
        /// </summary>
        /// <value>The theta div. The default value is <c>36</c>.</value>
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
            builder.AddPipe(this.Point1, this.Point2, this.InnerDiameter, this.Diameter, this.ThetaDiv);
            return builder.ToMesh();
        }
    }
}