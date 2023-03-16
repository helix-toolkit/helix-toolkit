/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public static class BoundingSphereExtensions
    {
        /// <summary>
        /// Froms the points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static global::SharpDX.BoundingSphere FromPoints(IList<Vector3> points, int start, int count)
        {
            if (points == null || start < 0 || start >= points.Count || count < 0 || (start + count) > points.Count)
            {
                return new global::SharpDX.BoundingSphere();
            }

            var upperEnd = start + count;

            //Find the center of all points.
            var center = Vector3.Zero;
            for (var i = start; i < upperEnd; ++i)
            {
                var p = points[i];
                Vector3.Add(ref p, ref center, out center);
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
                Vector3.DistanceSquared(ref center, ref p, out var distance);

                if (distance > radius)
                    radius = distance;
            }

            //Find the real distance from the DistanceSquared.
            radius = (float)Math.Sqrt(radius);

            //Construct the sphere.
            return new global::SharpDX.BoundingSphere(center, radius);
        }

        /// <summary>
        /// Froms the points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static global::SharpDX.BoundingSphere FromPoints(IList<Vector3> points)
        {
            if (points == null)
            {
                return new global::SharpDX.BoundingSphere();
            }

            return FromPoints(points, 0, points.Count);
        }

        /// <summary>
        /// Transforms the bounding sphere.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static global::SharpDX.BoundingSphere TransformBoundingSphere(this global::SharpDX.BoundingSphere b, Matrix m)
        {
            var center = b.Center;
            var edgeX = b.Center + Vector3.UnitX * b.Radius;
            var edgeY = b.Center + Vector3.UnitY * b.Radius;
            var edgeZ = b.Center + Vector3.UnitZ * b.Radius;

            var worldCenter = Vector3.Transform(center, m);
            var worldEdgeX = Vector3.Transform(edgeX, m);
            var worldEdgeY = Vector3.Transform(edgeY, m);
            var worldEdgeZ = Vector3.Transform(edgeZ, m);

            var maxRadius = (float)Math.Sqrt(Math.Max(Math.Max((worldEdgeX - worldCenter).LengthSquared(),
                (worldEdgeY - worldCenter).LengthSquared()),
                (worldEdgeZ - worldCenter).LengthSquared()));

            return new global::SharpDX.BoundingSphere(worldCenter.ToXYZ(), maxRadius);
        }
    }
}
