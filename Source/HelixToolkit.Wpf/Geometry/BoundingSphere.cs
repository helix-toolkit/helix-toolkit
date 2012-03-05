// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BoundingSphere.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a bounding sphere.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Sphere
    ///   http://en.wikipedia.org/wiki/Bounding_sphere
    ///   http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.boundingsphere_members(v=xnagamestudio.10).aspx
    ///   http://www.ep.liu.se/ecp/034/009/ecp083409.pdf
    ///   http://blogs.agi.com/insight3d/index.php/2008/02/04/a-bounding/
    ///   http://realtimecollisiondetection.net/blog/?p=20
    ///   http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/welzl-r2484
    ///   http://softsurfer.com/Archive/algorithm_0107/algorithm_0107.htm#Bounding Ball
    /// </remarks>
    public class BoundingSphere
    {
        #region Constants and Fields

        /// <summary>
        ///   The center.
        /// </summary>
        private Point3D center;

        /// <summary>
        ///   The radius.
        /// </summary>
        private double radius;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BoundingSphere" /> class.
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

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the center of the sphere.
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
        ///   Gets or sets the radius of the sphere.
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

        #endregion

        #region Public Methods

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
                    Center = new Point3D(rect.X + rect.SizeX / 2, rect.Y + rect.SizeY / 2, rect.Z + rect.SizeZ / 2), 
                    Radius = 0.5 * Math.Sqrt(rect.SizeX * rect.SizeX + rect.SizeY * rect.SizeY + rect.SizeZ * rect.SizeZ)
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
            return new BoundingSphere { Center = original.Center + vector3 * (r2 + r1), Radius = r2 };
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
            return this.radius * this.radius + 2.0 * this.radius * sphere.radius + sphere.radius * sphere.radius > d2;
        }

        /// <summary>
        /// Gets the intersection with the specified ray.
        /// </summary>
        /// <param name="ray">
        /// The ray.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// True if intersections were found.
        /// </returns>
        public bool RayIntersection(Ray3D ray, out Point3D[] result)
        {
            throw new NotImplementedException();

            // http://www.devmaster.net/wiki/Ray-sphere_intersection
        }

        #endregion
    }
}