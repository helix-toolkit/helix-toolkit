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

            int upperEnd = start + count;

            //Find the center of all points.
            Vector3 center = Vector3.Zero;
            for (int i = start; i < upperEnd; ++i)
            {
                center += points[i];
            }

            //This is the center of our sphere.
            center /= (float)count;

            //Find the radius of the sphere
            float radius = 0f;
            for (int i = start; i < upperEnd; ++i)
            {
                //We are doing a relative distance comparison to find the maximum distance
                //from the center of our sphere.
                Vector3 p = points[i];
                float distance = Vector3.DistanceSquared(center, p);

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
            Vector3 center = b.Center;
            Vector3 edge = b.Center + Vector3.UnitX * b.Radius;

            Vector3 worldCenter = Vector3.Transform(center, m);
            Vector3 worldEdge = Vector3.Transform(edge, m);

            return new BoundingSphere(worldCenter, (worldEdge - worldCenter).Length());
        }

        /// <summary>
        /// Calculates the distance from a point to the nearest point on the sphere surface.
        /// </summary>
        /// <param name="b">The BoundingSphere</param>
        /// <param name="point">The point</param>
        /// <returns>The distance</returns>
        public static float DistanceTo(this BoundingSphere b, Vector3 point)
        {
            return Vector3.Distance(b.Center, point) - b.Radius;
        }

        /// <summary>
        /// Gets the intersection with the specified ray.
        /// </summary>
        /// <param name="box">The BoundingSphere</param>
        /// <param name="ray">The ray.</param>
        /// <param name="points">The intersection point(s).</param>
        /// <returns>The intersection points sorted by distance from the ray origin.</returns>
        public static bool Intersects(this BoundingSphere box, Ray ray, out Vector3[]? points)
        {
            float cx = box.Center.X;
            float cy = box.Center.Y;
            float cz = box.Center.Z;
            float r = box.Radius;

            float x1 = ray.Position.X;
            float y1 = ray.Position.Y;
            float z1 = ray.Position.Z;

            float dx = ray.Direction.X;
            float dy = ray.Direction.Y;
            float dz = ray.Direction.Z;

            // Quadratic solving
            float a = (dx * dx) + (dy * dy) + (dz * dz);
            float b = (2 * dx * (x1 - cx)) + (2 * dy * (y1 - cy)) + (2 * dz * (z1 - cz));
            float c = (x1 * x1) + (y1 * y1) + (z1 * z1) + (cx * cx) + (cz * cz) + (cy * cy) - (2 * ((cy * y1) + (cz * z1) + (cx * x1))) - (r * r);

            // Discriminant
            float q = (b * b) - (4 * a * c);

            // We have at least one possible intersection
            if (q >= 0)
            {
                float q2 = (float)Math.Sqrt((b * b) - (4 * a * c));

                // First root
                float t1 = (-b + q2) / (2 * a);

                // Second root
                float t2 = (-b - q2) / (2 * a);

                if (t1 >= 0 && t2 >= 0 && !t1.Equals(t2))
                {
                    Vector3 i1 = new Vector3(x1 + (dx * t1), y1 + (dy * t1), z1 + (dz * t1));
                    Vector3 i2 = new Vector3(x1 + (dx * t2), y1 + (dy * t2), z1 + (dz * t2));

                    points = t1 < t2 ? new[] { i1, i2 } : new[] { i2, i1 };
                    return true;
                }

                if (t1 >= 0)
                {
                    Vector3 i1 = new Vector3(x1 + (dx * t1), y1 + (dy * t1), z1 + (dz * t1));
                    points = new[] { i1 };
                    return true;
                }

                if (t2 >= 0)
                {
                    Vector3 i2 = new Vector3(x1 + (dx * t2), y1 + (dy * t2), z1 + (dz * t2));
                    points = new[] { i2 };
                    return true;
                }
            }

            points = null;
            return false;
        }
    }
}
