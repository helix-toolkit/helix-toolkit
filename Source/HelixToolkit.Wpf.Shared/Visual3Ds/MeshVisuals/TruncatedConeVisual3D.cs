// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TruncatedConeVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a truncated cone defined by origin, height, normal, base- and top radius.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a truncated cone defined by origin, height, normal, base- and top radius.
    /// </summary>
    public class TruncatedConeVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="BaseCap"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseCapProperty = DependencyProperty.Register(
            "BaseCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="BaseRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseRadiusProperty = DependencyProperty.Register(
            "BaseRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(2.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Normal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal",
            typeof(Vector3D),
            typeof(TruncatedConeVisual3D),
            new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Origin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin",
            typeof(Point3D),
            typeof(TruncatedConeVisual3D),
            new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="ThetaDiv"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThetaDivProperty = DependencyProperty.Register(
            "ThetaDiv", typeof(int), typeof(TruncatedConeVisual3D), new PropertyMetadata(35, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TopCap"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopCapProperty = DependencyProperty.Register(
            "TopCap", typeof(bool), typeof(TruncatedConeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TopRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopRadiusProperty = DependencyProperty.Register(
            "TopRadius", typeof(double), typeof(TruncatedConeVisual3D), new PropertyMetadata(0.0, GeometryChanged));

        /// <summary>
        /// Gets or sets a value indicating whether to include a base cap.
        /// </summary>
        public bool BaseCap
        {
            get
            {
                return (bool)this.GetValue(BaseCapProperty);
            }

            set
            {
                this.SetValue(BaseCapProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the base radius.
        /// </summary>
        /// <value>The base radius.</value>
        public double BaseRadius
        {
            get
            {
                return (double)this.GetValue(BaseRadiusProperty);
            }

            set
            {
                this.SetValue(BaseRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
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
        /// Gets or sets the theta div.
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
        /// Gets or sets a value indicating whether to include a top cap.
        /// </summary>
        public bool TopCap
        {
            get
            {
                return (bool)this.GetValue(TopCapProperty);
            }

            set
            {
                this.SetValue(TopCapProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top radius.
        /// </summary>
        /// <value>The top radius.</value>
        public double TopRadius
        {
            get
            {
                return (double)this.GetValue(TopRadiusProperty);
            }

            set
            {
                this.SetValue(TopRadiusProperty, value);
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>A triangular mesh geometry.</returns>
        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false, true);
            builder.AddCone(
                this.Origin,
                this.Normal,
                this.BaseRadius,
                this.TopRadius,
                this.Height,
                this.BaseCap,
                this.TopCap,
                this.ThetaDiv);
            return builder.ToMesh();
        }
    }
}