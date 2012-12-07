// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContourHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Contour helper.
    /// </summary>
    /// <remarks>
    /// http://paulbourke.net/papers/conrec/
    /// </remarks>
    public class ContourHelper
    {
        /// <summary>
        /// The a.
        /// </summary>
        private readonly double A;

        /// <summary>
        /// The b.
        /// </summary>
        private readonly double B;

        /// <summary>
        /// The c.
        /// </summary>
        private readonly double C;

        /// <summary>
        /// The d.
        /// </summary>
        private readonly double D;

        /// <summary>
        /// The side.
        /// </summary>
        private readonly double[] side = new double[3];

        /// <summary>
        /// Initializes a new instance of the <see cref="ContourHelper"/> class.
        /// </summary>
        /// <param name="planeOrigin">
        /// The plane origin.
        /// </param>
        /// <param name="planeNormal">
        /// The plane normal.
        /// </param>
        public ContourHelper(Point3D planeOrigin, Vector3D planeNormal)
        {
            // Determine the equation of the plane as
            // Ax + By + Cz + D = 0
            double l =
                Math.Sqrt(planeNormal.X * planeNormal.X + planeNormal.Y * planeNormal.Y + planeNormal.Z * planeNormal.Z);
            this.A = planeNormal.X / l;
            this.B = planeNormal.Y / l;
            this.C = planeNormal.Z / l;
            this.D = -(planeNormal.X * planeOrigin.X + planeNormal.Y * planeOrigin.Y + planeNormal.Z * planeOrigin.Z);
        }

        /// <summary>
        /// Create a contour slice through a 3 vertex facet "p"
        /// </summary>
        /// <param name="p0">
        /// The facet p0.
        /// </param>
        /// <param name="p1">
        /// The facet p1.
        /// </param>
        /// <param name="p2">
        /// The facet p2.
        /// </param>
        /// <param name="s0">
        /// The first output point.
        /// </param>
        /// <param name="s1">
        /// The second output point.
        /// </param>
        /// <returns>
        /// -1 if the contour plane is above the facet
        /// -2 if the contour plane is below the facet
        /// 0 if it does cut the facet, and p0 is above the contour plane
        /// 10 if it does cut the facet, and p0 is below the contour plane
        /// -3 for an unexpected occurence
        /// </returns>
        public int ContourFacet(Point3D p0, Point3D p1, Point3D p2, out Point3D s0, out Point3D s1)
        {
            // Evaluate the equation of the plane for each vertex
            // If side < 0 then it is on the side to be retained
            // else it is to be clipped
            this.side[0] = this.A * p0.X + this.B * p0.Y + this.C * p0.Z + this.D;
            this.side[1] = this.A * p1.X + this.B * p1.Y + this.C * p1.Z + this.D;
            this.side[2] = this.A * p2.X + this.B * p2.Y + this.C * p2.Z + this.D;

            // Are all the vertices on the same side?
            if (this.side[0] >= 0 && this.side[1] >= 0 && this.side[2] >= 0)
            {
                s0 = new Point3D();
                s1 = new Point3D();
                return -1;
            }

            if (this.side[0] <= 0 && this.side[1] <= 0 && this.side[2] <= 0)
            {
                s0 = new Point3D();
                s1 = new Point3D();
                return -2;
            }

            // Is p0 the only point on a side by itself
            if ((this.side[0] * this.side[1] < 0) && (this.side[0] * this.side[2] < 0))
            {
                s0 = new Point3D(
                    p0.X - this.side[0] * (p2.X - p0.X) / (this.side[2] - this.side[0]),
                    p0.Y - this.side[0] * (p2.Y - p0.Y) / (this.side[2] - this.side[0]),
                    p0.Z - this.side[0] * (p2.Z - p0.Z) / (this.side[2] - this.side[0]));
                s1 = new Point3D(
                    p0.X - this.side[0] * (p1.X - p0.X) / (this.side[1] - this.side[0]),
                    p0.Y - this.side[0] * (p1.Y - p0.Y) / (this.side[1] - this.side[0]),
                    p0.Z - this.side[0] * (p1.Z - p0.Z) / (this.side[1] - this.side[0]));
                return this.side[0] > 0 ? 0 : 10;
            }

            // Is p1 the only point on a side by itself
            if ((this.side[1] * this.side[0] < 0) && (this.side[1] * this.side[2] < 0))
            {
                s0 = new Point3D(
                    p1.X - this.side[1] * (p2.X - p1.X) / (this.side[2] - this.side[1]),
                    p1.Y - this.side[1] * (p2.Y - p1.Y) / (this.side[2] - this.side[1]),
                    p1.Z - this.side[1] * (p2.Z - p1.Z) / (this.side[2] - this.side[1]));
                s1 = new Point3D(
                    p1.X - this.side[1] * (p0.X - p1.X) / (this.side[0] - this.side[1]),
                    p1.Y - this.side[1] * (p0.Y - p1.Y) / (this.side[0] - this.side[1]),
                    p1.Z - this.side[1] * (p0.Z - p1.Z) / (this.side[0] - this.side[1]));
                return this.side[1] > 0 ? 1 : 11;
            }

            /* Is p2 the only point on a side by itself */
            if ((this.side[2] * this.side[0] < 0) && (this.side[2] * this.side[1] < 0))
            {
                s0 = new Point3D(
                    p2.X - this.side[2] * (p0.X - p2.X) / (this.side[0] - this.side[2]),
                    p2.Y - this.side[2] * (p0.Y - p2.Y) / (this.side[0] - this.side[2]),
                    p2.Z - this.side[2] * (p0.Z - p2.Z) / (this.side[0] - this.side[2]));
                s1 = new Point3D(
                    p2.X - this.side[2] * (p1.X - p2.X) / (this.side[1] - this.side[2]),
                    p2.Y - this.side[2] * (p1.Y - p2.Y) / (this.side[1] - this.side[2]),
                    p2.Z - this.side[2] * (p1.Z - p2.Z) / (this.side[1] - this.side[2]));
                return this.side[2] > 0 ? 2 : 12;
            }

            s0 = new Point3D();
            s1 = new Point3D();
            return -10;

            // throw new InvalidOperationException("Shouldn't get here.");
        }

    }
}