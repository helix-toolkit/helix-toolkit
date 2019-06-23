// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Triangle.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a triangle in two-dimensional space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// Represents a triangle in two-dimensional space.
    /// </summary>
    public class Triangle
    {
        /// <summary>
        /// The first point of the triangle.
        /// </summary>
        private readonly Point p1;

        /// <summary>
        /// The second point of the triangle.
        /// </summary>
        private readonly Point p2;

        /// <summary>
        /// The third point of the triangle.
        /// </summary>
        private readonly Point p3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        public Triangle(Point a, Point b, Point c)
        {
            this.p1 = a;
            this.p2 = b;
            this.p3 = c;
        }

        /// <summary>
        /// Gets the first point of the triangle.
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
        /// Gets the second point of the triangle.
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
        /// Gets the third point of the triangle.
        /// </summary>
        /// <value>The point.</value>
        public Point P3
        {
            get
            {
                return this.p3;
            }
        }

        /// <summary>
        /// Checks whether the specified rectangle is completely inside the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsCompletelyInside(Rect rect)
        {
            return rect.Contains(this.p2) && rect.Contains(this.p3) && rect.Contains(this.P3);
        }

        /// <summary>
        /// Checks whether the specified rectangle is completely inside the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsRectCompletelyInside(Rect rect)
        {
            return this.IsPointInside(rect.TopLeft) && this.IsPointInside(rect.TopRight)
                   && this.IsPointInside(rect.BottomLeft) && this.IsPointInside(rect.BottomRight);
        }

        /// <summary>
        /// Checks whether the specified point is inside the triangle. 
        /// </summary>
        /// <param name="p">The point to be checked.</param>
        /// <returns>
        /// <c>true</c> if the specified point is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsPointInside(Point p)
        {
            // http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
            var s = (this.p1.Y * this.p3.X) - (this.p1.X * this.p3.Y) + ((this.p3.Y - this.p1.Y) * p.X) + ((this.p1.X - this.p3.X) * p.Y);
            var t = (this.p1.X * this.p2.Y) - (this.p1.Y * this.p2.X) + ((this.p1.Y - this.p2.Y) * p.X) + ((this.p2.X - this.p1.X) * p.Y);

            if ((s < 0) != (t < 0))
            {
                return false;
            }

            var a = (-this.p2.Y * this.p3.X) + (this.p1.Y * (this.p3.X - this.p2.X)) + (this.p1.X * (this.p2.Y - this.p3.Y)) + (this.p2.X * this.p3.Y);
            if (a < 0.0)
            {
                s = -s;
                t = -t;
                a = -a;
            }

            return s > 0 && t > 0 && (s + t) < a;
        }

        /// <summary>
        /// Indicates whether the specified rectangle intersects with the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle intersects with the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IntersectsWith(Rect rect)
        {
            return LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.TopRight, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.TopRight, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.TopRight, rect.BottomRight);
        }
    }
}