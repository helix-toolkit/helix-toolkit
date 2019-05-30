// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoundingSphere.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a bounding sphere.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a bounding sphere.
    /// </summary>
    public class BoundingSphere
    {
        //// http://en.wikipedia.org/wiki/Sphere
        //// http://en.wikipedia.org/wiki/Bounding_sphere
        //// http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.boundingsphere_members(v=xnagamestudio.10).aspx
        //// http://www.ep.liu.se/ecp/034/009/ecp083409.pdf
        //// http://blogs.agi.com/insight3d/index.php/2008/02/04/a-bounding/
        //// http://realtimecollisiondetection.net/blog/?p=20
        //// http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/welzl-r2484
        //// http://softsurfer.com/Archive/algorithm_0107/algorithm_0107.htm#Bounding Ball

        /// <summary>
        /// The center.
        /// </summary>
        private Point3D center;

        /// <summary>
        /// The radius.
        /// </summary>
        private double radius;

        /// <summary>
        /// Initializes a new instance of the <see cref = "BoundingSphere" /> class.
        /// </summary>
        public BoundingSphere()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        /// <param name="center">
        /// The center.
        /// </param>
        /// <param name="diameter">
        /// The diameter.
        /// </param>
        public BoundingSphere(Point3D center, double diameter)
        {
            this.center = center;
            this.radius = diameter / 2;
        }

        /// <summary>
        /// Gets or sets the center of the sphere.
        /// </summary>
        /// <value>The center.</value>
        public Point3D Center
        {
            get
            {
                return this.center;
            }

            set
            {
                this.center = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius of the sphere.
        /// </summary>
        /// <value>The diameter.</value>
        public double Radius
        {
            get
            {
                return this.radius;
            }

            set
            {
                this.radius = value;
            }
        }

        /// <summary>
        /// Creates a bounding sphere from a collection of points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <returns>
        /// The bounding sphere.
        /// </returns>
        public static BoundingSphere CreateFromPoints(IEnumerable<Point3D> points)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <see cref="BoundingSphere"/> from a <see cref="Rect3D"/>.
        /// </summary>
        /// <param name="rect">
        /// The 3D rectangle.
        /// </param>
        /// <returns>
        /// A sphere.
        /// </returns>
        public static BoundingSphere CreateFromRect3D(Rect3D rect)
        {
            return new BoundingSphere
                {
                    Center = new Point3D(rect.X + (rect.SizeX * 0.5), rect.Y + (rect.SizeY * 0.5), rect.Z + (rect.SizeZ * 0.5)),
                    Radius = 0.5 * Math.Sqrt((rect.SizeX * rect.SizeX) + (rect.SizeY * rect.SizeY) + (rect.SizeZ * rect.SizeZ))
                };
        }

        /// <summary>
        /// Creates a merged bounding sphere.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <param name="additional">
        /// The additional.
        /// </param>
        /// <returns>
        /// The merged bounding sphere.
        /// </returns>
        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            var vector = additional.center - original.center;
            var distance = vector.Length;
            if (original.radius + additional.radius >= distance)
            {
                if (original.radius - additional.radius >= distance)
                {
                    return original;
                }

                if (additional.radius - original.radius >= distance)
                {
                    return additional;
                }
            }

            var vector3 = vector * (1 / distance);
            var r1 = Math.Min(-original.radius, distance - additional.radius);
            var r2 = (Math.Max(original.radius, distance + additional.radius) - r1) * 0.5;
            return new BoundingSphere { Center = original.Center + (vector3 * (r2 + r1)), Radius = r2 };
        }

        /// <summary>
        /// Determines if the specified point is inside the sphere.
        /// </summary>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <returns>
        /// True if the point is inside.
        /// </returns>
        public bool Contains(Point3D point)
        {
            return point.DistanceToSquared(this.center) < this.radius * this.radius;
        }

        /// <summary>
        /// Calculates the distance from a point to the nearest point on the sphere surface.
        /// </summary>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <returns>
        /// The distance.
        /// </returns>
        public double DistanceFrom(Point3D point)
        {
            return point.DistanceTo(this.center) - this.radius;
        }

        /// <summary>
        /// Determines if the sphere intersects with the specified sphere.
        /// </summary>
        /// <param name="sphere">
        /// The sphere to check against.
        /// </param>
        /// <returns>
        /// True if the spheres intersect.
        /// </returns>
        public bool Intersects(BoundingSphere sphere)
        {
            double d2 = this.center.DistanceToSquared(sphere.center);
            return (this.radius * this.radius) + (2.0 * this.radius * sphere.radius) + (sphere.radius * sphere.radius) > d2;
        }

        /// <summary>
        /// Gets the intersection with the specified ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <param name="result">The intersection point(s).</param>
        /// <returns>The intersection points sorted by distance from the ray origin.</returns>
        public bool RayIntersection(Ray3D ray, out Point3D[] result)
        {
            double cx = this.center.X;
            double cy = this.center.Y;
            double cz = this.center.Z;
            double r = this.radius;

            double x1 = ray.Origin.X;
            double y1 = ray.Origin.Y;
            double z1 = ray.Origin.Z;

            double dx = ray.Direction.X;
            double dy = ray.Direction.Y;
            double dz = ray.Direction.Z;

            // Quadratic solving
            double a = (dx * dx) + (dy * dy) + (dz * dz);
            double b = (2 * dx * (x1 - cx)) + (2 * dy * (y1 - cy)) + (2 * dz * (z1 - cz));
            double c = (x1 * x1) + (y1 * y1) + (z1 * z1) + (cx * cx) + (cz * cz) + (cy * cy) - (2 * ((cy * y1) + (cz * z1) + (cx * x1))) - (r * r);

            // Discriminant
            double q = (b * b) - (4 * a * c);

            // We have at least one possible intersection
            if (q >= 0)
            {
                double q2 = Math.Sqrt((b * b) - (4 * a * c));

                // First root
                double t1 = (-b + q2) / (2 * a);

                // Second root
                double t2 = (-b - q2) / (2 * a);

                if (t1 >= 0 && t2 >= 0 && !t1.Equals(t2))
                {
                    var i1 = new Point3D(x1 + (dx * t1), y1 + (dy * t1), z1 + (dz * t1));
                    var i2 = new Point3D(x1 + (dx * t2), y1 + (dy * t2), z1 + (dz * t2));

                    result = t1 < t2 ? new[] { i1, i2 } : new[] { i2, i1 };
                    return true;
                }

                if (t1 >= 0)
                {
                    var i1 = new Point3D(x1 + (dx * t1), y1 + (dy * t1), z1 + (dz * t1));
                    result = new[] { i1 };
                    return true;
                }

                if (t2 >= 0)
                {
                    var i2 = new Point3D(x1 + (dx * t2), y1 + (dy * t2), z1 + (dz * t2));
                    result = new[] { i2 };
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}