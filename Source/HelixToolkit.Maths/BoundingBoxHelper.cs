namespace HelixToolkit.Maths
{
    /// <summary>
    /// 
    /// </summary>
    public static class BoundingBoxHelper
    {
        /// <summary>
        /// Get bounding box from list of points
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static BoundingBox FromPoints(IList<Vector3>? points)
        {
            if (points == null || points.Count == 0)
            {
                return new BoundingBox();
            }
            points.MinMax(out var min, out var max);
            Vector3 diff = max - min;
            return diff.AnySmallerOrEqual(0.0001f) // Avoid bound too small on one dimension.
               ? new BoundingBox(min - new Vector3(0.1f), max + new Vector3(0.1f))
               : new BoundingBox(min, max);
        }
        /// <summary>
        /// Transform AABB with Affine Transformation matrix
        /// </summary>
        /// <param name="box"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static BoundingBox Transform(this BoundingBox box, Matrix transform)
        {
            /////////////////Row 4/////////////////
            Vector3 min = transform.Translation;
            Vector3 max = transform.Translation;
            /////////////////Row 1/////////////////
            if (transform.M11 > 0f)
            {
                min.X += transform.M11 * box.Minimum.X;
                max.X += transform.M11 * box.Maximum.X;
            }
            else
            {
                min.X += transform.M11 * box.Maximum.X;
                max.X += transform.M11 * box.Minimum.X;
            }

            if (transform.M12 > 0f)
            {
                min.Y += transform.M12 * box.Minimum.X;
                max.Y += transform.M12 * box.Maximum.X;
            }
            else
            {
                min.Y += transform.M12 * box.Maximum.X;
                max.Y += transform.M12 * box.Minimum.X;
            }

            if (transform.M13 > 0f)
            {
                min.Z += transform.M13 * box.Minimum.X;
                max.Z += transform.M13 * box.Maximum.X;
            }
            else
            {
                min.Z += transform.M13 * box.Maximum.X;
                max.Z += transform.M13 * box.Minimum.X;
            }
            /////////////////Row 2/////////////////
            if (transform.M21 > 0f)
            {
                min.X += transform.M21 * box.Minimum.Y;
                max.X += transform.M21 * box.Maximum.Y;
            }
            else
            {
                min.X += transform.M21 * box.Maximum.Y;
                max.X += transform.M21 * box.Minimum.Y;
            }

            if (transform.M22 > 0f)
            {
                min.Y += transform.M22 * box.Minimum.Y;
                max.Y += transform.M22 * box.Maximum.Y;
            }
            else
            {
                min.Y += transform.M22 * box.Maximum.Y;
                max.Y += transform.M22 * box.Minimum.Y;
            }

            if (transform.M23 > 0f)
            {
                min.Z += transform.M23 * box.Minimum.Y;
                max.Z += transform.M23 * box.Maximum.Y;
            }
            else
            {
                min.Z += transform.M23 * box.Maximum.Y;
                max.Z += transform.M23 * box.Minimum.Y;
            }
            ///////////////Row 3///////////////////////
            if (transform.M31 > 0f)
            {
                min.X += transform.M31 * box.Minimum.Z;
                max.X += transform.M31 * box.Maximum.Z;
            }
            else
            {
                min.X += transform.M31 * box.Maximum.Z;
                max.X += transform.M31 * box.Minimum.Z;
            }

            if (transform.M32 > 0f)
            {
                min.Y += transform.M32 * box.Minimum.Z;
                max.Y += transform.M32 * box.Maximum.Z;
            }
            else
            {
                min.Y += transform.M32 * box.Maximum.Z;
                max.Y += transform.M32 * box.Minimum.Z;
            }

            if (transform.M33 > 0f)
            {
                min.Z += transform.M33 * box.Minimum.Z;
                max.Z += transform.M33 * box.Maximum.Z;
            }
            else
            {
                min.Z += transform.M33 * box.Maximum.Z;
                max.Z += transform.M33 * box.Minimum.Z;
            }

            return new BoundingBox(min, max);
        }
    }
}
