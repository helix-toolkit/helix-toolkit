// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// A visual element that contains a set of line segments. The thickness of the lines is defined in screen space.
    /// </summary>
    public class LinesVisual3D : ScreenSpaceVisual3D
    {
        #region Constants and Fields

        /// <summary>
        ///   The thickness property.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness", typeof(double), typeof(LinesVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        ///   The builder.
        /// </summary>
        private readonly LineGeometryBuilder builder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LinesVisual3D" /> class.
        /// </summary>
        public LinesVisual3D()
        {
            this.builder = new LineGeometryBuilder(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the thickness of the lines.
        /// </summary>
        /// <value>
        ///   The thickness.
        /// </value>
        public double Thickness
        {
            get
            {
                return (double)this.GetValue(ThicknessProperty);
            }

            set
            {
                this.SetValue(ThicknessProperty, value);
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
            if (this.Points != null)
            {
                int n = this.Points.Count;
                if (n > 0)
                {
                    if (this.Mesh.TriangleIndices.Count != n * 3)
                    {
                        this.Mesh.TriangleIndices = this.builder.CreateIndices(n);
                    }

                    this.Mesh.Positions = this.builder.CreatePositions(this.Points, this.Thickness, this.DepthOffset);
                }
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