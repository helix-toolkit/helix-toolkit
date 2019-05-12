// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtrudedVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that extrudes a section along a path.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that extrudes a section along a path.
    /// </summary>
    /// <remarks>
    /// The implementation will not work well if there are sharp bends in the path.
    /// </remarks>
    public class ExtrudedVisual3D : MeshElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Diameters"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiametersProperty = DependencyProperty.Register(
            "Diameters", typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="SectionXAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionXAxisProperty = DependencyProperty.Register(
            "SectionXAxis", typeof(Vector3D), typeof(ExtrudedVisual3D), new UIPropertyMetadata(GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Angles"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnglesProperty = DependencyProperty.Register(
            "Angles", typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="IsPathClosed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPathClosedProperty = DependencyProperty.Register(
            "IsPathClosed", typeof(bool), typeof(ExtrudedVisual3D), new UIPropertyMetadata(false, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="IsSectionClosed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSectionClosedProperty =
            DependencyProperty.Register(
                "IsSectionClosed", typeof(bool), typeof(ExtrudedVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", typeof(Point3DCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Section"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionProperty = DependencyProperty.Register(
            "Section",
            typeof(PointCollection),
            typeof(ExtrudedVisual3D),
            new UIPropertyMetadata(new PointCollection(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TextureCoordinates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextureCoordinatesProperty =
            DependencyProperty.Register(
                "TextureCoordinates", typeof(DoubleCollection), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtrudedVisual3D" /> class.
        /// </summary>
        public ExtrudedVisual3D()
        {
            this.Path = new Point3DCollection();
        }

        /// <summary>
        /// Gets or sets the diameters along the path.
        /// </summary>
        /// <value> The diameters. </value>
        public DoubleCollection Diameters
        {
            get
            {
                return (DoubleCollection)this.GetValue(DiametersProperty);
            }

            set
            {
                this.SetValue(DiametersProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diameters along the path.
        /// </summary>
        /// <value> The diameters. </value>
        public DoubleCollection Angles
        {
            get
            {
                return (DoubleCollection)this.GetValue(AnglesProperty);
            }

            set
            {
                this.SetValue(AnglesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the path is closed.
        /// </summary>
        public bool IsPathClosed
        {
            get
            {
                return (bool)this.GetValue(IsPathClosedProperty);
            }

            set
            {
                this.SetValue(IsPathClosedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the section is closed.
        /// </summary>
        public bool IsSectionClosed
        {
            get
            {
                return (bool)this.GetValue(IsSectionClosedProperty);
            }

            set
            {
                this.SetValue(IsSectionClosedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value> The path. </value>
        public Point3DCollection Path
        {
            get
            {
                return (Point3DCollection)this.GetValue(PathProperty);
            }

            set
            {
                this.SetValue(PathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        /// <value> The section. </value>
        public PointCollection Section
        {
            get
            {
                return (PointCollection)this.GetValue(SectionProperty);
            }

            set
            {
                this.SetValue(SectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the initial alignment of the x-axis of the section into the 3D viewport.
        /// </summary>
        /// <value> The section. </value>
        public Vector3D SectionXAxis
        {
            get
            {
                return (Vector3D)this.GetValue(SectionXAxisProperty);
            }

            set
            {
                this.SetValue(SectionXAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the texture coordinates along the path (X only).
        /// </summary>
        /// <value> The texture coordinates. </value>
        public DoubleCollection TextureCoordinates
        {
            get
            {
                return (DoubleCollection)this.GetValue(TextureCoordinatesProperty);
            }

            set
            {
                this.SetValue(TextureCoordinatesProperty, value);
            }
        }

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/> .
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry.
        /// </returns>
        protected override MeshGeometry3D Tessellate()
        {
            if (this.Path == null || this.Path.Count < 2)
            {
                return null;
            }

            // See also "The GLE Tubing and Extrusion Library":
            // http://linas.org/gle/
            // http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864
            var builder = new MeshBuilder(false, this.TextureCoordinates != null);

            var sectionXAxis = this.SectionXAxis;
            if (sectionXAxis.Length < 1e-6)
            {
                sectionXAxis = new Vector3D(1, 0, 0);
            }

            var forward = this.Path[1] - this.Path[0];
            var up = Vector3D.CrossProduct(forward, sectionXAxis);
            if (up.LengthSquared < 1e-6)
            {
                sectionXAxis = forward.FindAnyPerpendicular();
            }

            builder.AddTube(
                this.Path,
                this.Angles,
                this.TextureCoordinates,
                this.Diameters,
                this.Section,
                sectionXAxis,
                this.IsPathClosed,
                this.IsSectionClosed);
            return builder.ToMesh();
        }
    }
}
