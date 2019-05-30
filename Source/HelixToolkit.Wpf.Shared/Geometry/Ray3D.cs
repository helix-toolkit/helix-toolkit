// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ray3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a 3D ray.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a 3D ray.
    /// </summary>
    public class Ray3D
    {
        /// <summary>
        /// The direction
        /// </summary>
        private Vector3D direction;

        /// <summary>
        /// The origin
        /// </summary>
        private Point3D origin;

        /// <summary>
        /// Initializes a new instance of the <see cref = "Ray3D" /> class.
        /// </summary>
        public Ray3D()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray3D"/> class.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="d">
        /// The sender.
        /// </param>
        public Ray3D(Point3D o, Vector3D d)
        {
            this.Origin = o;
            this.Direction = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray3D"/> class.
        /// </summary>
        /// <param name="p0">
        /// The p0.
        /// </param>
        /// <param name="p1">
        /// The p1.
        /// </param>
        public Ray3D(Point3D p0, Point3D p1)
        {
            this.Origin = p0;
            this.Direction = p1 - p0;
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3D Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.direction = value;
            }
        }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point3D Origin
        {
            get
            {
                return this.origin;
            }

            set
            {
                this.origin = value;
            }
        }

        /// <summary>
        /// Gets the point on the ray that is nearest the specified point.
        /// </summary>
        /// <param name="p3">
        /// The point.
        /// </param>
        /// <returns>
        /// The nearest point on the ray.
        /// </returns>
        public Point3D GetNearest(Point3D p3)
        {
            return this.origin
                   + (Vector3D.DotProduct(p3 - this.origin, this.direction) / this.direction.LengthSquared
                      * this.direction);
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">
        /// A point on the plane.
        /// </param>
        /// <param name="normal">
        /// The normal of the plane.
        /// </param>
        /// <returns>
        /// The intersection point.
        /// </returns>
        public Point3D? PlaneIntersection(Point3D position, Vector3D normal)
        {
            Point3D intersection;
            if (this.PlaneIntersection(position, normal, out intersection))
            {
                return intersection;
            }

            return null;
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">A point on the plane.</param>
        /// <param name="normal">The normal of the plane.</param>
        /// <param name="intersection">The intersection point.</param>
        /// <returns>
        /// True if a intersection was found.
        /// </returns>
        public bool PlaneIntersection(Point3D position, Vector3D normal, out Point3D intersection)
        {
            // http://paulbourke.net/geometry/planeline/
            double dn = Vector3D.DotProduct(normal, this.Direction);
            if (dn.Equals(0))
            {
                intersection = default(Point3D);
                return false;
            }

            double u = Vector3D.DotProduct(normal, position - this.origin) / dn;
            intersection = this.Origin + (u * this.direction);
            return true;
        }
    }
}