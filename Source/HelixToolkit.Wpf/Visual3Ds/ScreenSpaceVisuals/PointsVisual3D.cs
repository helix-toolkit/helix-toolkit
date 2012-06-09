// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointsVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// A visual element that contains a set of points. The size of the points is defined in screen space.
    /// </summary>
    public class PointsVisual3D : ScreenSpaceVisual3D
    {
        #region Constants and Fields

        /// <summary>
        /// The width property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(PointsVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// The builder.
        /// </summary>
        private readonly PointGeometryBuilder builder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PointsVisual3D" /> class.
        /// </summary>
        public PointsVisual3D()
        {
            this.builder = new PointGeometryBuilder(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the size of the points.
        /// </summary>
        /// <value>
        ///   The size.
        /// </value>
        public double Size
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

        #endregion

        #region Methods

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected override void UpdateGeometry()
        {
            this.Mesh.Positions = null;
            if (this.Points == null)
            {
                return;
            }

            int n = this.Points.Count;
            if (n > 0)
            {
                if (this.Mesh.TriangleIndices.Count != n * 6)
                {
                    this.Mesh.TriangleIndices = this.builder.CreateIndices(n);
                }

                this.Mesh.Positions = this.builder.CreatePositions(this.Points, this.Size, this.DepthOffset);
            }
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform is updated.
        /// </returns>
        protected override bool UpdateTransforms()
        {
            return this.builder.UpdateTransforms();
        }

        #endregion
    }
}