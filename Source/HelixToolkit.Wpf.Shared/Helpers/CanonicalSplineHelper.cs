// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanonicalSplineHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interpolates a list of points using a canonical spline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interpolates a list of points using a canonical spline.
    /// </summary>
    public static class CanonicalSplineHelper
    {
        // CanonicalSplineHelper.cs (c) 2009 by Charles Petzold (WPF and Silverlight)
        // www.charlespetzold.com/blog/2009/01/Canonical-Splines-in-WPF-and-Silverlight.html
        /// <summary>
        /// Creates a spline of points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="tension">
        /// The tension.
        /// </param>
        /// <param name="tensions">
        /// The tensions.
        /// </param>
        /// <param name="isClosed">
        /// True if the spline is closed.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        /// <returns>
        /// A list of screen points.
        /// </returns>
        public static List<Point3D> CreateSpline(IList<Point3D> points, double tension = 0.5, IList<double> tensions = null, bool isClosed = false, double tolerance = 0.25)
        {
            var result = new List<Point3D>();
            if (points == null)
            {
                return result;
            }

            int n = points.Count;
            if (n < 1)
            {
                return result;
            }

            if (n < 2)
            {
                result.AddRange(points);
                return result;
            }

            if (n == 2)
            {
                if (!isClosed)
                {
                    Segment(result, points[0], points[0], points[1], points[1], tension, tension, tolerance);
                }
                else
                {
                    Segment(result, points[1], points[0], points[1], points[0], tension, tension, tolerance);
                    Segment(result, points[0], points[1], points[0], points[1], tension, tension, tolerance);
                }
            }
            else
            {
                bool useTensionCollection = tensions != null && tensions.Count > 0;

                for (int i = 0; i < n; i++)
                {
                    double t1 = useTensionCollection ? tensions[i % tensions.Count] : tension;
                    double t2 = useTensionCollection ? tensions[(i + 1) % tensions.Count] : tension;

                    if (i == 0)
                    {
                        Segment(
                            result,
                            isClosed ? points[n - 1] : points[0],
                            points[0],
                            points[1],
                            points[2],
                            t1,
                            t2,
                            tolerance);
                    }
                    else if (i == n - 2)
                    {
                        Segment(
                            result,
                            points[i - 1],
                            points[i],
                            points[i + 1],
                            isClosed ? points[0] : points[i + 1],
                            t1,
                            t2,
                            tolerance);
                    }
                    else if (i == n - 1)
                    {
                        if (isClosed)
                        {
                            Segment(result, points[i - 1], points[i], points[0], points[1], t1, t2, tolerance);
                        }
                    }
                    else
                    {
                        Segment(result, points[i - 1], points[i], points[i + 1], points[i + 2], t1, t2, tolerance);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The segment.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="pt0">
        /// The pt 0.
        /// </param>
        /// <param name="pt1">
        /// The pt 1.
        /// </param>
        /// <param name="pt2">
        /// The pt 2.
        /// </param>
        /// <param name="pt3">
        /// The pt 3.
        /// </param>
        /// <param name="t1">
        /// The t 1.
        /// </param>
        /// <param name="t2">
        /// The t 2.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        private static void Segment(
            IList<Point3D> points,
            Point3D pt0,
            Point3D pt1,
            Point3D pt2,
            Point3D pt3,
            double t1,
            double t2,
            double tolerance)
        {
            // See Petzold, "Programming Microsoft Windows with C#", pages 645-646 or
            // Petzold, "Programming Microsoft Windows with Microsoft Visual Basic .NET", pages 638-639
            // for derivation of the following formulas:
            double sx1 = t1 * (pt2.X - pt0.X);
            double sy1 = t1 * (pt2.Y - pt0.Y);
            double sz1 = t1 * (pt2.Z - pt0.Z);
            double sx2 = t2 * (pt3.X - pt1.X);
            double sy2 = t2 * (pt3.Y - pt1.Y);
            double sz2 = t2 * (pt3.Z - pt1.Z);

            double ax = sx1 + sx2 + 2 * pt1.X - 2 * pt2.X;
            double ay = sy1 + sy2 + 2 * pt1.Y - 2 * pt2.Y;
            double az = sz1 + sz2 + 2 * pt1.Z - 2 * pt2.Z;
            double bx = -2 * sx1 - sx2 - 3 * pt1.X + 3 * pt2.X;
            double by = -2 * sy1 - sy2 - 3 * pt1.Y + 3 * pt2.Y;
            double bz = -2 * sz1 - sz2 - 3 * pt1.Z + 3 * pt2.Z;

            double cx = sx1;
            double cy = sy1;
            double cz = sz1;
            double dx = pt1.X;
            double dy = pt1.Y;
            double dz = pt1.Z;

            var num = (int)((Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y) + Math.Abs(pt1.Z - pt2.Z)) / tolerance);

            // Notice begins at 1 so excludes the first point (which is just pt1)
            for (int i = 1; i < num; i++)
            {
                double t = (double)i / (num - 1);
                var pt = new Point3D(
                    ax * t * t * t + bx * t * t + cx * t + dx,
                    ay * t * t * t + by * t * t + cy * t + dy,
                    az * t * t * t + bz * t * t + cz * t + dz);
                points.Add(pt);
            }
        }

    }
}