// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ray3.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using global::SharpDX;

    /// <summary>
    /// Represents a 3D ray.
    /// </summary>    
    [Obsolete("We do not need own structures, since SharpDX does it all for us.")]
    public class Ray3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "Ray3" /> class.
        /// </summary>
        public Ray3()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray3"/> class.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="direction">
        /// The sender.
        /// </param>
        public Ray3(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public Vector3 Origin { get; set; }

        /// <summary>
        /// Gets the point on the ray that is nearest the specified point.
        /// </summary>
        /// <param name="p3">
        /// The point.
        /// </param>
        /// <returns>
        /// The nearest point on the ray.
        /// </returns>
        public Vector3 GetNearest(Vector3 p3)
        {
            var l = this.Direction.LengthSquared();
            var u = (((p3.X - this.Origin.X) * this.Direction.X) +
                ((p3.Y - this.Origin.Y) * this.Direction.Y)
                        + ((p3.Z - this.Origin.Z) * this.Direction.Z)) / l;
            return this.Origin + (u * this.Direction);
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <returns>The intersection point.</returns>
        public Vector3? PlaneIntersection(Vector3 position, Vector3 normal)
        {
            // http://paulbourke.net/geometry/planeline/
            var dn = Vector3.Dot(normal, this.Direction);
            if (dn.Equals(0))
            {
                return null;
            }

            var u = Vector3.Dot(normal, position - this.Origin) / dn;
            return this.Origin + (u * this.Direction);
        }
    }
}