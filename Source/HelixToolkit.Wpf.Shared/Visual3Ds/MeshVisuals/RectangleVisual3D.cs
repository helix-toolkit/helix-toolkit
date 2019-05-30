// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a 3D rectangle defined by origin, normal, length and width.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a 3D rectangle defined by origin, normal, length and width.
    /// </summary>
    public class RectangleVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="DivLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DivLengthProperty = DependencyProperty.Register(
            "DivLength", typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

        /// <summary>
        /// Identifies the <see cref="DivWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DivWidthProperty = DependencyProperty.Register(
            "DivWidth", typeof(int), typeof(RectangleVisual3D), new UIPropertyMetadata(10, GeometryChanged, CoerceDivValue));

        /// <summary>
        /// Identifies the <see cref="LengthDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register(
                "LengthDirection",
                typeof(Vector3D),
                typeof(RectangleVisual3D),
                new PropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Normal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal",
            typeof(Vector3D),
            typeof(RectangleVisual3D),
            new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Origin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin",
            typeof(Point3D),
            typeof(RectangleVisual3D),
            new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(RectangleVisual3D), new PropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// Gets or sets the number of divisions in the 'length' direction.
        /// </summary>
        /// <value>The number of divisions.</value>
        public int DivLength
        {
            get
            {
                return (int)this.GetValue(DivLengthProperty);
            }

            set
            {
                this.SetValue(DivLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of divisions in the 'width' direction.
        /// </summary>
        /// <value>The number of divisions.</value>
        public int DivWidth
        {
            get
            {
                return (int)this.GetValue(DivWidthProperty);
            }

            set
            {
                this.SetValue(DivWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length.
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
        /// Gets or sets the length direction.
        /// </summary>
        /// <value>The length direction.</value>
        public Vector3D LengthDirection
        {
            get
            {
                return (Vector3D)this.GetValue(LengthDirectionProperty);
            }

            set
            {
                this.SetValue(LengthDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the normal vector of the plane.
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
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>A triangular mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            Vector3D u = this.LengthDirection;
            Vector3D w = this.Normal;
            Vector3D v = Vector3D.CrossProduct(w, u);
            u = Vector3D.CrossProduct(v, w);

            u.Normalize();
            v.Normalize();
            w.Normalize();

            double le = this.Length;
            double wi = this.Width;

            var pts = new List<Point3D>();
            for (int i = 0; i < this.DivLength; i++)
            {
                double fi = -0.5 + ((double)i / (this.DivLength - 1));
                for (int j = 0; j < this.DivWidth; j++)
                {
                    double fj = -0.5 + ((double)j / (this.DivWidth - 1));
                    pts.Add(this.Origin + (u * le * fi) + (v * wi * fj));
                }
            }

            var builder = new MeshBuilder(false, true);
            builder.AddRectangularMesh(pts, this.DivWidth);

            return builder.ToMesh();
        }

        /// <summary>
        /// Coerces the division value.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>A value not less than 2.</returns>
        private static object CoerceDivValue(DependencyObject d, object baseValue)
        {
            return Math.Max(2, (int)baseValue);
        }
    }
}