// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plane3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a plane in three-dimensional space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a plane in three-dimensional space.
    /// </summary>
    public class Plane3D
    {
        /// <summary>
        /// The normal.
        /// </summary>
        private Vector3D normal;

        /// <summary>
        /// The position.
        /// </summary>
        private Point3D position;

        /// <summary>
        /// Initializes a new instance of the <see cref = "Plane3D" /> class.
        /// </summary>
        public Plane3D()
        {
            // Width=50;
            // Height=50;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane3D"/> class.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="normal">
        /// The normal.
        /// </param>
        public Plane3D(Point3D position, Vector3D normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return normal;
            }

            set
            {
                normal = value;
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Finds the intersection between the plane and a line.
        /// </summary>
        /// <param name="la">
        /// The first point defining the line.
        /// </param>
        /// <param name="lb">
        /// The second point defining the line.
        /// </param>
        /// <returns>
        /// The intersection point.
        /// </returns>
        public Point3D? LineIntersection(Point3D la, Point3D lb)
        {
            // http://en.wikipedia.org/wiki/Line-plane_intersection
            var l = lb - la;
            double a = Vector3D.DotProduct(position - la, normal);
            double b = Vector3D.DotProduct(l, normal);
            if (a.Equals(0) && b.Equals(0))
            {
                return null;
            }

            if (b.Equals(0))
            {
                return null;
            }

            return la + ((a / b) * l);
        }

        /// <summary>
        /// Calculates the distance from a point to a plane.
        /// </summary>
        /// <param name="point">The point used to calculate distance</param>
        /// <returns>
        /// The distance from given point to the given plane<br/>
        /// Equal zero: Point on the plane<br/>
        /// Greater than zero: The point is on the same side of the plane's normal vector<br/>
        /// Less than zero: The point is on the opposite side of the plane's normal vector<br/>
        /// </returns>
        public double DistanceTo(Point3D point)
        {
            return point.DistanceToPlane(position, normal);
        }

        /// <summary>
        /// Calculates the projection of a point onto a plane.
        /// </summary>
        /// <param name="point">The point used to calculate projection</param>
        /// <returns>
        /// The projection of a given point on a given plane.
        /// </returns>
        public Point3D Project(Point3D point)
        {
            return point.ProjectOnPlane(position, normal);
        }

        /// <summary>
        /// Check whether a plane intersects with a given <see cref="Rect3D"/> box.
        /// </summary>
        /// <param name="rect">The Rect3D bounding box</param>
        /// <returns>
        /// Whether the two objects intersected.
        /// </returns>
        public PlaneIntersectionType Intersects(Rect3D rect)
        {
            return rect.Intersects(position, normal);
        }

        // public void SetYZ(double x, int dir)
        // {
        // Position = new Point3D(x, 0, Height / 2);
        // Normal = new Vector3D(dir, 0, 0);
        // Up = new Vector3D(0, 0, 1);
        // }

        // public void SetXY(double z, int dir)
        // {
        // Position = new Point3D(0, 0, z);
        // Normal = new Vector3D(0, 0, dir);
        // Up = new Vector3D(1, 0, 0);
        // }

        // public void SetXZ(double y, int dir)
        // {
        // Position = new Point3D(0, y, 0);
        // Normal = new Vector3D(0, dir, 0);
        // Up = new Vector3D(1, 0, 0);
        // }

        // public Point3D[] GetCornerPoints()
        // {
        // var pts = new Point3D[4];
        // Vector3D right = Vector3D.CrossProduct(Normal, Up);
        // pts[0] = Position + (-right * 0.5 * Width - Up * 0.5 * Height);
        // pts[1] = Position + (right * 0.5 * Width - Up * 0.5 * Height);
        // pts[2] = Position + (right * 0.5 * Width + Up * 0.5 * Height);
        // pts[3] = Position + (-right * 0.5 * Width + Up * 0.5 * Height);
        // return pts;
        // }
    }
}