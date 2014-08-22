// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeometryHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Defines the GeometryHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// The geometry helper.
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Check whether the triangle is in the rectangle.
        /// </summary>
        /// <param name="a">The vertex a of triangle</param>
        /// <param name="b">The vertex b of triangle</param>
        /// <param name="c">The vertex c of triangle</param>
        /// <param name="rect">The rectangle</param>
        /// <returns>
        /// True if the triangle is inside the rectangle.
        /// </returns>
        public static bool TriangleInsideRect(Point a, Point b, Point c, Rect rect)
        {
            return rect.Contains(a) && rect.Contains(b) && rect.Contains(c);
        }

        /// <summary>
        /// Check whether the rectangle is inside the triangle.
        /// </summary>
        /// <param name="a">The vertex a of triangle</param>
        /// <param name="b">The vertex b of triangle</param>
        /// <param name="c">The vertex c of triangle</param>
        /// <param name="rect">The rectangle.</param>
        /// <returns>
        /// True if the rectangle is inside triangle. Otherwise, it returns false.
        /// </returns>
        public static bool RectInsideTriangle(Point a, Point b, Point c, Rect rect)
        {
            return PointInTriangle(rect.TopLeft, a, b, c) && PointInTriangle(rect.TopRight, a, b, c)
                   && PointInTriangle(rect.BottomLeft, a, b, c) && PointInTriangle(rect.BottomRight, a, b, c);
        }

        /// <summary>
        /// Check whether the point is in the triangle. See: http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        /// </summary>
        /// <param name="p">The point to be checked.</param>
        /// <param name="p0">The vertex p0 of the triangle.</param>
        /// <param name="p1">The vertex p1 of the triangle.</param>
        /// <param name="p2">The vertex p2 of the triangle.</param>
        /// <returns>
        /// True if the point is in the triangle. Otherwise, it returns false.
        /// </returns>
        public static bool PointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
            var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

            if ((s < 0) != (t < 0))
                return false;

            var a = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
            if (a < 0.0)
            {
                s = -s;
                t = -t;
                a = -a;
            }

            return s > 0 && t > 0 && (s + t) < a;
        }

        /// <summary>
        /// Intersect of two lines.
        /// </summary>
        /// <param name="a1">One vertex of line a.</param>
        /// <param name="a2">The other vertex of the line a.</param>
        /// <param name="b1">One vertex of line b.</param>
        /// <param name="b2">The other vertex of the line b.</param>
        /// <returns>
        /// True, if the two lines are crossed. Otherwise, it returns false.
        /// </returns>
        public static bool Intersect(Point a1, Point a2, Point b1, Point b2)
        {
            if (b1 == b2 || a1 == a2)
            {
                return false;
            }

            if (((a2.X - a1.X) * (b1.Y - a1.Y) - (b1.X - a1.X) * (a2.Y - a1.Y))
                * ((a2.X - a1.X) * (b2.Y - a1.Y) - (b2.X - a1.X) * (a2.Y - a1.Y)) > 0)
            {
                return false;
            }

            if (((b2.X - b1.X) * (a1.Y - b1.Y) - (a1.X - b1.X) * (b2.Y - b1.Y))
                * ((b2.X - b1.X) * (a2.Y - b1.Y) - (a2.X - b1.X) * (b2.Y - b1.Y)) > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Intersect of the point with the rectangle.
        /// </summary>
        /// <param name="a">The vertex a of the triangle.</param>
        /// <param name="b">The vertex b of the triangle.</param>
        /// <param name="c">The vertex c of the triangle.</param>
        /// <param name="rect">The rectangle to be checked.</param>
        /// <returns>
        /// True, if the triangle intersect with the rectangle. Otherwise, it returns false.
        /// </returns>
        public static bool Intersect(Point a, Point b, Point c, Rect rect)
        {
            return Intersect(a, b, rect.BottomLeft, rect.BottomRight) || Intersect(a, b, rect.BottomLeft, rect.TopLeft)
                   || Intersect(a, b, rect.TopLeft, rect.TopRight) || Intersect(a, b, rect.TopRight, rect.BottomRight)
                   || Intersect(b, c, rect.BottomLeft, rect.BottomRight)
                   || Intersect(b, c, rect.BottomLeft, rect.TopLeft) || Intersect(b, c, rect.TopLeft, rect.TopRight)
                   || Intersect(b, c, rect.TopRight, rect.BottomRight)
                   || Intersect(c, a, rect.BottomLeft, rect.BottomRight)
                   || Intersect(c, a, rect.BottomLeft, rect.TopLeft) || Intersect(c, a, rect.TopLeft, rect.TopRight)
                   || Intersect(c, a, rect.TopRight, rect.BottomRight);
        }
    }
}
