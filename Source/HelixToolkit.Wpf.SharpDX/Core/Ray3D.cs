// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ray3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Diagnostics;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a 3D ray.
    /// </summary>
    public class Ray3D
    {
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
        public Vector3D Direction { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Point3D Origin { get; set; }

        /// <summary>
        /// Gets the point on the ray that is nearest the specified point.
        /// </summary>
        /// <param name="p3">
        /// The point.
        /// </param>
        /// <returns>
        /// </returns>
        public Point3D GetNearest(Point3D p3)
        {
            double l = this.Direction.LengthSquared;
            Debug.Assert(l != 0);
            double u = ((p3.X - this.Origin.X) * this.Direction.X + (p3.Y - this.Origin.Y) * this.Direction.Y
                        + (p3.Z - this.Origin.Z) * this.Direction.Z) / l;
            return this.Origin + u * this.Direction;
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <returns>The intersection point.</returns>
        public Point3D? PlaneIntersection(Point3D position, Vector3D normal)
        {
            // http://paulbourke.net/geometry/planeline/
            double dn = Vector3D.DotProduct(normal, this.Direction);
            if (dn == 0)
            {
                return null;
            }

            double u = Vector3D.DotProduct(normal, position - this.Origin) / dn;
            return this.Origin + u * this.Direction;
        }

    }
}