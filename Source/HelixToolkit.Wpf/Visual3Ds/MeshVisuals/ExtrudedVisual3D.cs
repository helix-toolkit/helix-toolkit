// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtrudedVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that extrudes a section along a path.
    /// </summary>
    /// <remarks>
    /// The implementation will not work well if there are sharp bends in the path.
    /// </remarks>
    public class ExtrudedVisual3D : MeshElement3D
    {
        // See also "The GLE Tubing and Extrusion Library":
        // http://linas.org/gle/
        // http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864
        #region Constants and Fields

        /// <summary>
        ///   The diameters property.
        /// </summary>
        public static readonly DependencyProperty DiametersProperty = DependencyProperty.Register(
            "Diameters", typeof(IList<double>), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The is path closed property.
        /// </summary>
        public static readonly DependencyProperty IsPathClosedProperty = DependencyProperty.Register(
            "IsPathClosed", typeof(bool), typeof(ExtrudedVisual3D), new UIPropertyMetadata(false, GeometryChanged));

        /// <summary>
        ///   The is section closed property.
        /// </summary>
        public static readonly DependencyProperty IsSectionClosedProperty =
            DependencyProperty.Register(
                "IsSectionClosed", typeof(bool), typeof(PipeVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        ///   The path property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", typeof(IList<Point3D>), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        ///   The section property.
        /// </summary>
        public static readonly DependencyProperty SectionProperty = DependencyProperty.Register(
            "Section", 
            typeof(IList<Point>), 
            typeof(ExtrudedVisual3D), 
            new UIPropertyMetadata(new List<Point>(), GeometryChanged));

        /// <summary>
        ///   The texture coordinates property.
        /// </summary>
        public static readonly DependencyProperty TextureCoordinatesProperty =
            DependencyProperty.Register(
                "TextureCoordinates", typeof(IList<double>), typeof(ExtrudedVisual3D), new UIPropertyMetadata(null));

        /// <summary>
        ///   The up vector property.
        /// </summary>
        public static readonly DependencyProperty UpVectorProperty = DependencyProperty.Register(
            "UpVector", 
            typeof(Vector3D), 
            typeof(PipeVisual3D), 
            new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExtrudedVisual3D" /> class.
        /// </summary>
        public ExtrudedVisual3D()
        {
            this.Path = new List<Point3D>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the diameters along the path.
        /// </summary>
        /// <value> The diameters. </value>
        public IList<double> Diameters
        {
            get
            {
                return (IList<double>)this.GetValue(DiametersProperty);
            }

            set
            {
                this.SetValue(DiametersProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the path is closed.
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
        ///   Gets or sets a value indicating whether the section is closed.
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
        ///   Gets or sets the path.
        /// </summary>
        /// <value> The path. </value>
        public IList<Point3D> Path
        {
            get
            {
                return (IList<Point3D>)this.GetValue(PathProperty);
            }

            set
            {
                this.SetValue(PathProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the section.
        /// </summary>
        /// <value> The section. </value>
        public IList<Point> Section
        {
            get
            {
                return (IList<Point>)this.GetValue(SectionProperty);
            }

            set
            {
                this.SetValue(SectionProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the texture coordinates along the path (X only).
        /// </summary>
        /// <value> The texture coordinates. </value>
        public IList<double> TextureCoordinates
        {
            get
            {
                return (IList<double>)this.GetValue(TextureCoordinatesProperty);
            }

            set
            {
                this.SetValue(TextureCoordinatesProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the up vector.
        /// </summary>
        /// <value> The up vector. </value>
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

        #endregion

        #region Methods

        /// <summary>
        /// Do the tesselation and return the <see cref="MeshGeometry3D"/> .
        /// </summary>
        /// <returns>
        /// A triangular mesh geometry. 
        /// </returns>
        protected override MeshGeometry3D Tessellate()
        {
            if (this.Path == null)
            {
                return null;
            }

            var builder = new MeshBuilder(false, this.TextureCoordinates != null);
            builder.AddTube(
                this.Path, 
                this.TextureCoordinates, 
                this.Diameters, 
                this.Section, 
                this.IsPathClosed, 
                this.IsSectionClosed);
            return builder.ToMesh();
        }

        #endregion
    }
}