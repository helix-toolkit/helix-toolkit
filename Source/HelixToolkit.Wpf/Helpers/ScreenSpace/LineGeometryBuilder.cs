// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineGeometryBuilder.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Builds a mesh geometry for a collection of line segments.
    /// </summary>
    public class LineGeometryBuilder : ScreenGeometryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBuilder"/> class.
        /// </summary>
        /// <param name="visual">
        /// The visual parent of the geometry (the transform is calculated from this object).
        /// </param>
        public LineGeometryBuilder(Visual3D visual)
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
        /// Triangle indices.
        /// </returns>
        public Int32Collection CreateIndices(int n)
        {
            var indices = new Int32Collection(n * 3);

            for (int i = 0; i < n / 2; i++)
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
        /// Creates the positions for the specified line segments.
        /// </summary>
        /// <param name="points">
        /// The points of the line segments.
        /// </param>
        /// <param name="thickness">
        /// The thickness of the line.
        /// </param>
        /// <param name="depthOffset">
        /// The depth offset. A positive number (e.g. 0.0001) moves the point towards the camera.
        /// </param>
        /// <param name="clipping">
        /// The clipping.
        /// </param>
        /// <returns>
        /// The positions collection.
        /// </returns>
        public Point3DCollection CreatePositions(
            IList<Point3D> points,
            double thickness = 1.0,
            double depthOffset = 0.0,
            CohenSutherlandClipping clipping = null)
        {
            double halfThickness = thickness * 0.5;
            int segmentCount = points.Count / 2;

            var positions = new Point3DCollection(segmentCount * 4);

            for (int i = 0; i < segmentCount; i++)
            {
                int startIndex = i * 2;

                Point3D startPoint = points[startIndex];
                Point3D endPoint = points[startIndex + 1];

                var s0 = (Point4D)startPoint * this.visualToScreen;
                var s1 = (Point4D)endPoint * this.visualToScreen;

                if (clipping != null)
                {
                    double x0 = s0.X / s0.W;
                    double y0 = s0.Y / s0.W;
                    double x1 = s1.X / s1.W;
                    double y1 = s1.Y / s1.W;

                    if (!clipping.ClipLine(ref x0, ref y0, ref x1, ref y1))
                    {
                        continue;
                    }

                    s0.X = x0 * s0.W;
                    s0.Y = y0 * s0.W;
                    s1.X = x1 * s1.W;
                    s1.Y = y1 * s1.W;
                }

                double lx = s1.X / s1.W - s0.X / s0.W;
                double ly = s1.Y / s1.W - s0.Y / s0.W;
                double m = halfThickness / Math.Sqrt(lx * lx + ly * ly);

                double dx = -ly * m;
                double dy = lx * m;

                var p00 = s0;
                var p01 = s0;
                var p10 = s1;
                var p11 = s1;

                p00.X += dx * p00.W;
                p00.Y += dy * p00.W;
                p01.X -= dx * p01.W;
                p01.Y -= dy * p01.W;
                p10.X += dx * p10.W;
                p10.Y += dy * p10.W;
                p11.X -= dx * p11.W;
                p11.Y -= dy * p11.W;
                if (depthOffset != 0)
                {
                    p00.Z -= depthOffset;
                    p01.Z -= depthOffset;
                    p10.Z -= depthOffset;
                    p11.Z -= depthOffset;

                    p00 *= this.screenToVisual;
                    p01 *= this.screenToVisual;
                    p10 *= this.screenToVisual;
                    p11 *= this.screenToVisual;

                    positions.Add(new Point3D(p00.X / p00.W, p00.Y / p00.W, p00.Z / p00.W));
                    positions.Add(new Point3D(p01.X / p00.W, p01.Y / p01.W, p01.Z / p01.W));
                    positions.Add(new Point3D(p10.X / p00.W, p10.Y / p10.W, p10.Z / p10.W));
                    positions.Add(new Point3D(p11.X / p00.W, p11.Y / p11.W, p11.Z / p11.W));
                }
                else
                {
                    p00 *= this.screenToVisual;
                    p01 *= this.screenToVisual;
                    p10 *= this.screenToVisual;
                    p11 *= this.screenToVisual;

                    positions.Add(new Point3D(p00.X, p00.Y, p00.Z));
                    positions.Add(new Point3D(p01.X, p01.Y, p01.Z));
                    positions.Add(new Point3D(p10.X, p10.Y, p10.Z));
                    positions.Add(new Point3D(p11.X, p11.Y, p11.Z));
                }
            }

            positions.Freeze();
            return positions;
        }

    }
}