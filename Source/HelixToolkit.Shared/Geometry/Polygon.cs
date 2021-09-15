// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a 2D polygon.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if SHARPDX
#if NETFX_CORE
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
#else
namespace HelixToolkit.Wpf
#endif
{
#if SHARPDX
    using Int32Collection = System.Collections.Generic.List<int>;
    using PointCollection = System.Collections.Generic.List<global::SharpDX.Vector2>;
#else
    using System.Windows.Media;
#endif

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
        /// Triangulate the polygon by using the sweep line algorithm
        /// </summary>
        /// <returns>An index collection.</returns>
        public Int32Collection Triangulate()
        {
            return SweepLinePolygonTriangulator.Triangulate(this.points);
        }
    }
}
