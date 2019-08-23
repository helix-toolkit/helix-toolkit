// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelixVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a helix.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a helix.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Helix
    /// A helix (pl: helixes or helices) is a type of space curve, i.e. a smooth curve in three-dimensional space.
    /// It is characterised by the fact that the tangent line at any point makes a constant angle with a fixed line
    /// called the axis. Examples of helixes are coil springs and the handrails of spiral staircases. A "filled-in"
    /// helix – for example, a spiral ramp – is called a helicoid. Helices are important in biology, as the DNA
    /// molecule is formed as two intertwined helices, and many proteins have helical substructures, known as alpha
    /// helices. The word helix comes from the Greek word ἕλιξ.
    /// </remarks>
    public class HelixVisual3D : ParametricSurface3D
    {
        /// <summary>
        /// Gets or sets the origin.
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
        /// Identifies the <see cref="Origin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin",
            typeof(Point3D),
            typeof(HelixVisual3D),
            new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(0.5, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Phase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(
            "Phase", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Radius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Turns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TurnsProperty = DependencyProperty.Register(
            "Turns", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

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
        /// Gets or sets the phase.
        /// </summary>
        /// <value>The phase.</value>
        public double Phase
        {
            get
            {
                return (double)this.GetValue(PhaseProperty);
            }

            set
            {
                this.SetValue(PhaseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius
        {
            get
            {
                return (double)this.GetValue(RadiusProperty);
            }

            set
            {
                this.SetValue(RadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of turns.
        /// </summary>
        /// <value>The turns.</value>
        public double Turns
        {
            get
            {
                return (double)this.GetValue(TurnsProperty);
            }

            set
            {
                this.SetValue(TurnsProperty, value);
            }
        }

        /// <summary>
        /// Evaluates the surface.
        /// </summary>
        /// <param name="u">
        /// The u parameter.
        /// </param>
        /// <param name="v">
        /// The v parameter.
        /// </param>
        /// <param name="texCoord">
        /// The texture coordinate.
        /// </param>
        /// <returns>
        /// The evaluated <see cref="Point3D"/>.
        /// </returns>
        protected override Point3D Evaluate(double u, double v, out Point texCoord)
        {
            double color = u;
            v *= 2 * Math.PI;

            double b = this.Turns * 2 * Math.PI;
            double r = this.Radius / 2;
            double d = this.Diameter;
            double dr = this.Diameter / r;
            double p = this.Phase / 180 * Math.PI;

            double x = r * Math.Cos((b * u) + p) * (2 + (dr * Math.Cos(v)));
            double y = r * Math.Sin((b * u) + p) * (2 + (dr * Math.Cos(v)));
            double z = (u * this.Length) + (d * Math.Sin(v));

            texCoord = new Point(color, 0);
            return this.Origin + new Vector3D(x, y, z);
        }
    }
}