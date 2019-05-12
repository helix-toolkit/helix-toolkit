// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineSegment.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a line segment in two-dimensional space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// Represents a line segment in two-dimensional space.
    /// </summary>
    public class LineSegment
    {
        /// <summary>
        /// The first point of the line segment.
        /// </summary>
        private readonly Point p1;

        /// <summary>
        /// The second point of the line segment.
        /// </summary>
        private readonly Point p2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> class.
        /// </summary>
        /// <param name="p1">The first point of the line segment.</param>
        /// <param name="p2">The second point of the line segment.</param>
        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        /// <summary>
        /// Gets the first point of the line segment.
        /// </summary>
        /// <value>The point.</value>
        public Point P1
        {
            get
            {
                return this.p1;
            }
        }

        /// <summary>
        /// Gets the second point of the line segment.
        /// </summary>
        /// <value>The point.</value>
        public Point P2
        {
            get
            {
                return this.p2;
            }
        }

        /// <summary>
        /// Checks if there are any intersections of two line segments.
        /// </summary>
        /// <param name="a1">One vertex of line a.</param>
        /// <param name="a2">The other vertex of the line a.</param>
        /// <param name="b1">One vertex of line b.</param>
        /// <param name="b2">The other vertex of the line b.</param>
        /// <returns>
        /// <c>true</c>, if the two lines are crossed. Otherwise, it returns <c>false</c>.
        /// </returns>
        public static bool AreLineSegmentsIntersecting(Point a1, Point a2, Point b1, Point b2)
        {
            if (b1 == b2 || a1 == a2)
            {
                return false;
            }

            if ((((a2.X - a1.X) * (b1.Y - a1.Y)) - ((b1.X - a1.X) * (a2.Y - a1.Y)))
                * (((a2.X - a1.X) * (b2.Y - a1.Y)) - ((b2.X - a1.X) * (a2.Y - a1.Y))) > 0)
            {
                return false;
            }

            if ((((b2.X - b1.X) * (a1.Y - b1.Y)) - ((a1.X - b1.X) * (b2.Y - b1.Y)))
                * (((b2.X - b1.X) * (a2.Y - b1.Y)) - ((a2.X - b1.X) * (b2.Y - b1.Y))) > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates whether the specified line segment intersects with the current line segment.
        /// </summary>
        /// <param name="other">The line segment to check.</param>
        /// <returns>
        /// <c>true</c> if the specified line segment intersects with the current line segment; otherwise <c>false</c>.
        /// </returns>
        public bool IntersectsWith(LineSegment other)
        {
            return AreLineSegmentsIntersecting(this.p1, this.p2, other.p1, other.p2);
        }
    }
}