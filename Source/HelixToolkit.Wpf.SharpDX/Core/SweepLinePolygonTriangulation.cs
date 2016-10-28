// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubchainSamplingPolygonTriangulation.cs" company="Helix Toolkit">
//   Copyright (c) 2016 Franz Spitaler
// </copyright>
// <summary>
//   A polygon triangulator for simple polygons with no holes. Expected runtime is O(n)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using Point = global::SharpDX.Vector2;
    using PointCollection = System.Collections.Generic.List<global::SharpDX.Vector2>;
    using Int32Collection = System.Collections.Generic.List<int>;
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on https://www.cs.princeton.edu/~chazelle/pubs/polygon-triang.pdf ???
    /// References
    /// http://www.cs.uu.nl/docs/vakken/ga/slides3.pdf
    /// </remarks>
    public class SweepLinePolygonTriangulation
    {
        public static Int32Collection Triangulate(IList<Point> polygon)
        {
            // Allocate and initialize List of Indices in Polygon
            var result = new Int32Collection();
            var points = new List<Point>(polygon);

            // Make the Polygon CounterClockWise
            if (!isCCW(polygon))
                points.Reverse();

            // Sort Points from highest y to lowest y
            // and if two or more Points have the same y Value from lowest x to highest x Value
            var events = points;
            events.Sort(delegate(Point first, Point second)
            {
                /*if (first == null && second == null) return 0;
                else if (first == null) return -1;
                else if (second == null) return 1;
                else */if (first.Y > second.Y || (first.Y == second.Y && first.X < second.X)) return 1;
                else if (first.Y == second.Y && first.X == second.X) return 0;
                else return -1;
            });

            // Construct Status, a List of Edges left of every Point of the Polygon
            // by shooting a Ray from the Vertex to the left.
            // The Helper Point of that Edge will be used to create Monotone Polygons
            // by adding Diagonals from/to Split- and Merge-Points



            // Sweep through the Polygon using the sorted Polygon Points


            // Update the Helpers for the left Edges of the Polygon Points


            // 


            return result;
        }


        // Compute the Orientation of the Polygon.
        /// <summary>
        /// Calculates the Orientation.
        /// </summary>
        /// <param name="polygon">The Polygon.</param>
        /// <returns>True if the Polygon is present in a CCW manner.</returns>
        private static bool isCCW(IList<Point> polygon)
        {
            int n = polygon.Count;
            double area = 0.0;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                area += polygon[p].X * polygon[q].Y - polygon[q].X * polygon[p].Y;
            }
            return area > 0.0f;
        }
    }
}
