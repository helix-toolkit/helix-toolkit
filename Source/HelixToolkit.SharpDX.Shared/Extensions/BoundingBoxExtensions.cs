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
            if (points.Count == 0)
            {
                result = new BoundingBox();
            }
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
            if (points.Count == 0)
            {
                return new BoundingBox();
            }
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

        /// <summary>
        /// Transform AABB with Affine Transformation matrix
        /// </summary>
        /// <param name="box"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static BoundingBox Transform(this BoundingBox box, Matrix transform)
        {
            /////////////////Row 4/////////////////
            var min = transform.TranslationVector;
            var max = transform.TranslationVector;
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
