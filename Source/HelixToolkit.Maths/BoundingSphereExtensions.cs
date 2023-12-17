using System;
using System.Collections.Generic;
using System.Numerics;

namespace HelixToolkit.Maths
{
    public static class BoundingSphereExtensions
    {
        /// <summary>
        /// Get bounding sphere from point list.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static BoundingSphere GetBoundingSphere(this IList<Vector3> points, int start, int count)
        {
            if (points == null || start < 0 || start >= points.Count || count < 0 || (start + count) > points.Count)
            {
                return BoundingSphere.Empty;
            }

            var upperEnd = start + count;

            //Find the center of all points.
            var center = Vector3.Zero;
            for (var i = start; i < upperEnd; ++i)
            {
                center += points[i];
            }

            //This is the center of our sphere.
            center /= (float)count;

            //Find the radius of the sphere
            var radius = 0f;
            for (var i = start; i < upperEnd; ++i)
            {
                //We are doing a relative distance comparison to find the maximum distance
                //from the center of our sphere.
                var p = points[i];
                var distance = Vector3.DistanceSquared(center, p);

                if (distance > radius)
                {
                    radius = distance;
                }
            }

            //Find the real distance from the DistanceSquared.
            radius = (float)Math.Sqrt(radius);

            //Construct the sphere.
            return new BoundingSphere(center, radius);
        }

        /// <summary>
        /// Froms the points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static BoundingSphere GetBoundingSphere(this IList<Vector3> points)
        {
            return points == null ? BoundingSphere.Empty : points.GetBoundingSphere(0, points.Count);
        }

        /// <summary>
        /// Transforms the bounding sphere.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static BoundingSphere TransformBoundingSphere(this BoundingSphere b, Matrix4x4 m)
        {
            var center = b.Center;
            var edge = b.Center + Vector3.UnitX * b.Radius;

            var worldCenter = Vector3.Transform(center, m);
            var worldEdge = Vector3.Transform(edge, m);

            return new BoundingSphere(worldCenter, (worldEdge - worldCenter).Length());
        }
    }
}
