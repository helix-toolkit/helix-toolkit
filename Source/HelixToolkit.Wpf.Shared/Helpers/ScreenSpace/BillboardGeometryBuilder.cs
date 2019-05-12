// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillboardGeometryBuilder.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Builds a mesh geometry for a collection of billboards.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Builds a mesh geometry for a collection of billboards.
    /// </summary>
    public class BillboardGeometryBuilder : ScreenGeometryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardGeometryBuilder"/> class.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        public BillboardGeometryBuilder(Visual3D visual)
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
        public static Int32Collection CreateIndices(int n)
        {
            var indices = new Int32Collection(n * 6);
            for (int i = 0; i < n; i++)
            {
                // bottom right triangle
                indices.Add((i * 4) + 0);
                indices.Add((i * 4) + 1);
                indices.Add((i * 4) + 2);

                // top left triangle
                indices.Add((i * 4) + 2);
                indices.Add((i * 4) + 3);
                indices.Add((i * 4) + 0);
            }

            indices.Freeze();
            return indices;
        }

        /// <summary>
        /// Gets the billboard positions with the current screen transform.
        /// </summary>
        /// <param name="billboards">The billboards.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The positions.</returns>
        public Point3DCollection GetPositions(IList<Billboard> billboards, Vector offset)
        {
            var positions = new Point3DCollection(billboards.Count * 4);

            foreach (var bb in billboards)
            {
                Point4D screenPoint;
                if (!bb.WorldDepthOffset.Equals(0))
                {
                    var viewPoint = (Point4D)bb.Position * this.visualToProjection;
                    screenPoint = new Point4D(viewPoint.X, viewPoint.Y, viewPoint.Z + bb.WorldDepthOffset, viewPoint.W) * this.projectionToScreen;
                }
                else
                {
                    screenPoint = (Point4D)bb.Position * this.visualToScreen;
                }

                double spw = screenPoint.W;
                double spx = screenPoint.X;
                double spy = screenPoint.Y;
                double spz = screenPoint.Z - (bb.DepthOffset * spw);

                // Convert bottom-left corner to visual space
                var p = new Point4D(spx + ((bb.Left + offset.X) * spw), spy + ((bb.Bottom + offset.Y) * spw), spz, spw) * this.screenToVisual;
                double wi = 1 / p.W;
                positions.Add(new Point3D(p.X * wi, p.Y * wi, p.Z * wi));

                // Convert bottom-right corner to visual space
                p = new Point4D(spx + ((bb.Right + offset.X) * spw), spy + ((bb.Bottom + offset.Y) * spw), spz, spw) * this.screenToVisual;
                wi = 1 / p.W;
                positions.Add(new Point3D(p.X * wi, p.Y * wi, p.Z * wi));

                // Convert top-right corner to visual space
                p = new Point4D(spx + ((bb.Right + offset.X) * spw), spy + ((bb.Top + offset.Y) * spw), spz, spw) * this.screenToVisual;
                wi = 1 / p.W;
                positions.Add(new Point3D(p.X * wi, p.Y * wi, p.Z * wi));

                // Convert top-left corner to visual space
                p = new Point4D(spx + ((bb.Left + offset.X) * spw), spy + ((bb.Top + offset.Y) * spw), spz, spw) * this.screenToVisual;
                wi = 1 / p.W;
                positions.Add(new Point3D(p.X * wi, p.Y * wi, p.Z * wi));
            }

            positions.Freeze();
            return positions;
        }

        /// <summary>
        /// Gets the billboard positions with the current screen transform.
        /// </summary>
        /// <param name="billboards">The billboards.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="pinWidth">Width of the pins.</param>
        /// <returns>The mesh vertices.</returns>
        public Point3DCollection GetPinPositions(IList<Billboard> billboards, Vector offset, double pinWidth)
        {
            var pinStart = new Point(0, 0);
            var pinEnd = pinStart + (offset * (1 + (2 * pinWidth / offset.Length)));
            var pinNormal = new Vector(pinEnd.Y, -pinEnd.X);
            pinNormal.Normalize();
            pinNormal *= pinWidth * 0.5;

            var pinPoints = new Point[4];
            pinPoints[0] = new Point(0, 0) + (pinNormal * 0.5);
            pinPoints[1] = new Point(0, 0) - (pinNormal * 0.5);
            pinPoints[2] = pinEnd - pinNormal;
            pinPoints[3] = pinEnd + pinNormal;

            var positions = new Point3DCollection(billboards.Count * 4);
            foreach (var bb in billboards)
            {
                Point4D screenPoint;
                if (!bb.WorldDepthOffset.Equals(0))
                {
                    var viewPoint = (Point4D)bb.Position * this.visualToProjection;
                    screenPoint = new Point4D(viewPoint.X, viewPoint.Y, viewPoint.Z + bb.WorldDepthOffset, viewPoint.W) * this.projectionToScreen;
                }
                else
                {
                    screenPoint = (Point4D)bb.Position * this.visualToScreen;
                }

                double spw = screenPoint.W;
                double spx = screenPoint.X;
                double spy = screenPoint.Y;
                double spz = screenPoint.Z - ((bb.DepthOffset - 1e-5) * spw);

                foreach (var pinPoint in pinPoints)
                {
                    var p = new Point4D(spx + (pinPoint.X * spw), spy + (pinPoint.Y * spw), spz, spw) * this.screenToVisual;
                    double wi = 1 / p.W;
                    positions.Add(new Point3D(p.X * wi, p.Y * wi, p.Z * wi));
                }
            }

            positions.Freeze();
            return positions;
        }
    }
}