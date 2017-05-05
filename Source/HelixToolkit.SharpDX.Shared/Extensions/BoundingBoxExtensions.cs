using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelixToolkit.Wpf.SharpDX
{
    public static class BoundingBoxExtensions
    {
        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the box.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding box.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public static void FromPoints(IList<Vector3> points, out BoundingBox result)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var p in points)
            {
                var point = p;
                Vector3.Min(ref min, ref point, out min);
                Vector3.Max(ref max, ref point, out max);
            }

            result = new BoundingBox(min, max);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the box.</param>
        /// <returns>The newly constructed bounding box.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public static BoundingBox FromPoints(IList<Vector3> points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach(var p in points)
            {
                var point = p;
                Vector3.Min(ref min, ref point, out min);
                Vector3.Max(ref max, ref point, out max);
            }

            return new BoundingBox(min, max);
        }
    }
}
