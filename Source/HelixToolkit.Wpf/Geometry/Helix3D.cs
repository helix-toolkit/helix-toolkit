// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helix3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Represents a 3D helix.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Helix
    /// </remarks>
    public class Helix3D
    {
        /// <summary>
        /// Gets or sets the turns.
        /// </summary>
        /// <value>The turns.</value>
        public double Turns { get; set; }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }
        /// <summary>
        /// Gets or sets the bottom radius.
        /// </summary>
        /// <value>The bottom radius.</value>
        public double BottomRadius { get; set; }
        /// <summary>
        /// Gets or sets the top radius.
        /// </summary>
        /// <value>The top radius.</value>
        public double TopRadius { get; set; }

        /// <summary>
        /// Gets the points.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        public Point3DCollection GetPoints(int n)
        {
            var points = new Point3DCollection();
            for (int i = 0; i < n; i++)
            {
                double u = (double)i / (n - 1);
                double b = Turns * 2 * Math.PI;
                double r = BottomRadius + (TopRadius - BottomRadius) * u;

                double x = r * Math.Cos(b * u);
                double y = r * Math.Sin(b * u);
                double z = u * Height;
                points.Add(new Point3D(x, y, z));
            }
            return points;
        }
    }
}