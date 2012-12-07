// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media;

    /// <summary>
    /// Represents a 2D polygon.
    /// </summary>
    public class Polygon
    {
        // http://softsurfer.com/Archive/algorithm_0101/algorithm_0101.htm
        /// <summary>
        /// The points.
        /// </summary>
        internal PointCollection points;

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get
            {
                return this.points ?? (this.points = new PointCollection());
            }

            set
            {
                this.points = value;
            }
        }

        /// <summary>
        /// Triangulate the polygon by cutting ears
        /// </summary>
        /// <returns>An index collection.</returns>
        public Int32Collection Triangulate()
        {
            return CuttingEarsTriangulator.Triangulate(this.points);
        }

    }
}