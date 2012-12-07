// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGeometryBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Builds a mesh geometry for a collection of points.
    /// </summary>
    public class PointGeometryBuilder : ScreenGeometryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointGeometryBuilder"/> class.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        public PointGeometryBuilder(Visual3D visual)
            : base(visual)
        {
        }

        /// <summary>
        /// Creates the triangle indices.
        /// </summary>
        /// <param name="n">
        /// The number of points.
        /// </param>
        /// <returns>
        /// The triangle indices.
        /// </returns>
        public Int32Collection CreateIndices(int n)
        {
            var indices = new Int32Collection(n * 6);

            for (int i = 0; i < n; i++)
            {
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 1);
                indices.Add(i * 4 + 0);

                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 1);
            }

            indices.Freeze();
            return indices;
        }

        /// <summary>
        /// Creates the positions for the specified points.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="size">
        /// The size of the points.
        /// </param>
        /// <param name="depthOffset">
        /// The depth offset. A positive number (e.g. 0.0001) moves the point towards the camera.
        /// </param>
        /// <returns>
        /// The positions collection.
        /// </returns>
        public Point3DCollection CreatePositions(IList<Point3D> points, double size = 1.0, double depthOffset = 0.0)
        {
            double halfSize = size / 2.0;
            int numPoints = points.Count;

            var outline = new[]
                {
                    new Vector(-halfSize, halfSize), new Vector(-halfSize, -halfSize), new Vector(halfSize, halfSize),
                    new Vector(halfSize, -halfSize)
                };

            var positions = new Point3DCollection(numPoints * 4);

            for (int i = 0; i < numPoints; i++)
            {
                var screenPoint = (Point4D)points[i] * this.visualToScreen;

                double spx = screenPoint.X;
                double spy = screenPoint.Y;
                double spz = screenPoint.Z;
                double spw = screenPoint.W;

                if (!depthOffset.Equals(0))
                {
                    spz -= depthOffset * spw;
                }

                var p0 = new Point4D(spx, spy, spz, spw) * this.screenToVisual;
                double pwinverse = 1 / p0.W;

                foreach (var v in outline)
                {
                    var p = new Point4D(spx + v.X * spw, spy + v.Y * spw, spz, spw) * this.screenToVisual;
                    positions.Add(new Point3D(p.X * pwinverse, p.Y * pwinverse, p.Z * pwinverse));
                }
            }

            positions.Freeze();
            return positions;
        }

        /// <summary>
        /// Creates a billboard.
        /// </summary>
        /// <param name="position">The position (centre).</param>
        /// <param name="width">The width of the billboard.</param>
        /// <param name="height">The height of the billboard.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <param name="depthOffset">The depth offset.</param>
        /// <returns>The points of the billboard.</returns>
        public Point3DCollection CreateBillboard(Point3D position, double width = 1.0, double height = 1.0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center, double depthOffset = 0.0)
        {
            // Set horizontal alignment factor
            var xa = -0.5;
            if (horizontalAlignment == HorizontalAlignment.Left) xa = 0;
            if (horizontalAlignment == HorizontalAlignment.Right) xa = -1;

            // Set vertical alignment factor
            var ya = -0.5;
            if (verticalAlignment == VerticalAlignment.Top) ya = 0;
            if (verticalAlignment == VerticalAlignment.Bottom) ya = -1;

            double x0 = xa * width;
            double x1 = x0 + width;
            double y0 = ya * height;
            double y1 = y0 + height;

            var outline = new[]
                {
                    new Vector(x0, y1), new Vector(x0, y0), new Vector(x1, y1), new Vector(x1, y0)
                };

            var positions = new Point3DCollection(4);

            var screenPoint = (Point4D)position * this.visualToScreen;
            double spx = screenPoint.X;
            double spy = screenPoint.Y;
            double spz = screenPoint.Z;
            double spw = screenPoint.W;

            if (!depthOffset.Equals(0))
            {
                spz -= depthOffset * spw;
            }

            var p0 = new Point4D(spx, spy, spz, spw) * this.screenToVisual;
            double pwinverse = 1 / p0.W;

            foreach (var v in outline)
            {
                var p = new Point4D(spx + v.X * spw, spy + v.Y * spw, spz, spw) * this.screenToVisual;
                // Debug.Assert(Math.Abs(p.W - 1) < 1e-6, "Something wrong with the homogeneous coordinates.");
                positions.Add(new Point3D(p.X * pwinverse, p.Y * pwinverse, p.Z * pwinverse));
            }

            positions.Freeze();
            return positions;
        }
    }
}