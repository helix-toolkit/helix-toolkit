using global::SharpDX;
using System;
using System.Collections.Generic;

namespace HelixToolkit.Wpf.SharpDX
{
    public static class BoundingSphereExtensions
    {
        /// <summary>
        /// Constructs a <see cref="BoundingSphere" /> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the sphere.</param>
        /// <param name="start">The start index from points array to start compute the bounding sphere.</param>
        /// <param name="count">The count of points to process to compute the bounding sphere.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding sphere.</param>
        /// <exception cref="System.ArgumentNullException">points</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// start
        /// or
        /// count
        /// </exception>
        public static void FromPoints(IList<Vector3> points, int start, int count, out global::SharpDX.BoundingSphere result)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            // Check that start is in the correct range
            if (start < 0 || start >= points.Count)
            {
                throw new ArgumentOutOfRangeException("start", start, string.Format("Must be in the range [0, {0}]", points.Count - 1));
            }

            // Check that count is in the correct range
            if (count < 0 || (start + count) > points.Count)
            {
                throw new ArgumentOutOfRangeException("count", count, string.Format("Must be in the range <= {0}", points.Count));
            }

            var upperEnd = start + count;

            //Find the center of all points.
            Vector3 center = Vector3.Zero;
            for (int i = start; i < upperEnd; ++i)
            {
                var p = points[i];
                Vector3.Add(ref p, ref center, out center);
            }

            //This is the center of our sphere.
            center /= (float)count;

            //Find the radius of the sphere
            float radius = 0f;
            for (int i = start; i < upperEnd; ++i)
            {
                //We are doing a relative distance comparison to find the maximum distance
                //from the center of our sphere.
                float distance;
                var p = points[i];
                Vector3.DistanceSquared(ref center, ref p, out distance);

                if (distance > radius)
                    radius = distance;
            }

            //Find the real distance from the DistanceSquared.
            radius = (float)Math.Sqrt(radius);

            //Construct the sphere.
            result.Center = center;
            result.Radius = radius;
        }

        /// <summary>
        /// Constructs a <see cref="BoundingSphere"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the sphere.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding sphere.</param>
        public static void FromPoints(IList<Vector3> points, out global::SharpDX.BoundingSphere result)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            FromPoints(points, 0, points.Count, out result);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingSphere"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the sphere.</param>
        /// <returns>The newly constructed bounding sphere.</returns>
        public static global::SharpDX.BoundingSphere FromPoints(IList<Vector3> points)
        {
            global::SharpDX.BoundingSphere result;
            FromPoints(points, out result);
            return result;
        }

        public static global::SharpDX.BoundingSphere TransformBoundingSphere(this global::SharpDX.BoundingSphere b, Matrix m)
        {
            var center = b.Center;
            var edge = b.Center + Vector3.Right * b.Radius;

            var worldCenter = Vector3.Transform(center, m);
            var worldEdge = Vector3.Transform(edge, m);

            return new global::SharpDX.BoundingSphere(worldCenter.ToXYZ(), (worldEdge - worldCenter).Length());
        }

        public static void TransformBoundingSphere(this global::SharpDX.BoundingSphere b, ref Matrix m, out global::SharpDX.BoundingSphere boundSphere)
        {
            var center = b.Center;
            var edge = b.Center + Vector3.Right * b.Radius;

            var worldCenter = Vector3.Transform(center, m);
            var worldEdge = Vector3.Transform(edge, m);

            boundSphere = new global::SharpDX.BoundingSphere(worldCenter.ToXYZ(), (worldEdge - worldCenter).Length());
        }
    }
}
