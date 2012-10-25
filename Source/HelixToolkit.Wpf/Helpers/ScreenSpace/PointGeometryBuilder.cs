// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointGeometryBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
                foreach (var v in outline)
                {
                    var p = screenPoint;
                    p.X += v.X * p.W;
                    p.Y += v.Y * p.W;
                    if (depthOffset != 0)
                    {
                        p.Z -= depthOffset * p.W;
                        p *= this.screenToVisual;
                        positions.Add(new Point3D(p.X / p.W, p.Y / p.W, p.Z / p.W));
                    }
                    else
                    {
                        p *= this.screenToVisual;
                        Debug.Assert(Math.Abs(p.W - 1) < 1e-6, "Something wrong with the homogeneous coordinates.");
                        positions.Add(new Point3D(p.X, p.Y, p.Z));
                    }
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
        /// <param name="depthOffset">The depth offset.</param>
        /// <returns>The points of the billboard.</returns>
        public Point3DCollection CreateBillboard(Point3D position, double width = 1.0, double height = 1.0, double depthOffset = 0.0)
        {
            double halfWidth = width / 2.0;
            double halfHeight = height / 2.0;

            var outline = new[]
                {
                    new Vector(-halfWidth, halfHeight), new Vector(-halfWidth, -halfHeight), new Vector(halfWidth, halfHeight),
                    new Vector(halfWidth, -halfHeight)
                };

            var positions = new Point3DCollection(4);

                var screenPoint = (Point4D)position * this.visualToScreen;
                foreach (var v in outline)
                {
                    var p = screenPoint;
                    p.X += v.X * p.W;
                    p.Y += v.Y * p.W;
                    if (depthOffset != 0)
                    {
                        p.Z -= depthOffset * p.W;
                        p *= this.screenToVisual;
                        positions.Add(new Point3D(p.X / p.W, p.Y / p.W, p.Z / p.W));
                    }
                    else
                    {
                        p *= this.screenToVisual;
                        Debug.Assert(Math.Abs(p.W - 1) < 1e-6, "Something wrong with the homogeneous coordinates.");
                        positions.Add(new Point3D(p.X, p.Y, p.Z));
                    }
                }

            positions.Freeze();
            return positions;
        }
    }
}