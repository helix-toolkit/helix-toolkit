// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillboardGeometryBuilder.cs" company="Helix 3D Toolkit">
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
                indices.Add(i * 4 + 0);
                indices.Add(i * 4 + 1);
                indices.Add(i * 4 + 2);

                // top left triangle
                indices.Add(i * 4 + 2);
                indices.Add(i * 4 + 3);
                indices.Add(i * 4 + 0);
            }

            indices.Freeze();
            return indices;
        }

        /// <summary>
        /// Gets the billboard positions with the current screen transform.
        /// </summary>
        /// <returns>The positions.</returns>
        public Point3DCollection GetPositions(IList<Billboard> billboards)
        {
            var positions = new Point3DCollection(billboards.Count * 4);
            foreach (var bb in billboards)
            {
                var screenPoint = (Point4D)bb.Position * this.visualToScreen;

                double spx = screenPoint.X;
                double spy = screenPoint.Y;
                double spz = screenPoint.Z;
                double spw = screenPoint.W;

                if (!bb.DepthOffset.Equals(0))
                {
                    spz -= bb.DepthOffset * spw;
                }

                var p0 = new Point4D(spx, spy, spz, spw) * this.screenToVisual;
                double pwinverse = 1 / p0.W;

                var p1 = new Point4D(spx + bb.Left * spw, spy + bb.Bottom * spw, spz, spw) * this.screenToVisual;
                positions.Add(new Point3D(p1.X * pwinverse, p1.Y * pwinverse, p1.Z * pwinverse));

                var p2 = new Point4D(spx + bb.Right * spw, spy + bb.Bottom * spw, spz, spw) * this.screenToVisual;
                positions.Add(new Point3D(p2.X * pwinverse, p2.Y * pwinverse, p2.Z * pwinverse));

                var p3 = new Point4D(spx + bb.Right * spw, spy + bb.Top * spw, spz, spw) * this.screenToVisual;
                positions.Add(new Point3D(p3.X * pwinverse, p3.Y * pwinverse, p3.Z * pwinverse));

                var p4 = new Point4D(spx + bb.Left * spw, spy + bb.Top * spw, spz, spw) * this.screenToVisual;
                positions.Add(new Point3D(p4.X * pwinverse, p4.Y * pwinverse, p4.Z * pwinverse));
            }

            positions.Freeze();
            return positions;
        }
    }

    /// <summary>
    /// Represents a billboard.
    /// </summary>
    public class Billboard
    {
        /// <summary>
        /// The position
        /// </summary>
        internal Point3D Position;

        /// <summary>
        /// The relative left position (screen coordinates).
        /// </summary>
        internal double Left;

        /// <summary>
        /// The relative right position (screen coordinates).
        /// </summary>
        internal double Right;

        /// <summary>
        /// The relative top position (screen coordinates).
        /// </summary>
        internal double Top;

        /// <summary>
        /// The relative bottom position (screen coordinates).
        /// </summary>
        internal double Bottom;

        /// <summary>
        /// The depth offset
        /// </summary>
        internal double DepthOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        /// <param name="depthOffset">The depth offset.</param>
        public Billboard(Point3D position, double size, double depthOffset)
        {
            double halfSize = size / 2.0;
            Position = position;
            this.Left = -halfSize;
            this.Right = halfSize;
            this.Top = -halfSize;
            this.Bottom = halfSize;
            DepthOffset = depthOffset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        /// <param name="depthOffset">The depth offset.</param>
        public Billboard(Point3D position, double width = 1.0, double height = 1.0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center, double depthOffset = 0.0)
        {
            // Set horizontal alignment factor
            var xa = -0.5;
            if (horizontalAlignment == HorizontalAlignment.Left) xa = 0;
            if (horizontalAlignment == HorizontalAlignment.Right) xa = -1;

            // Set vertical alignment factor
            var ya = -0.5;
            if (verticalAlignment == VerticalAlignment.Top) ya = 0;
            if (verticalAlignment == VerticalAlignment.Bottom) ya = -1;

            double left = xa * width;
            double top = ya * height;

            Position = position;
            this.Left = left;
            this.Right = left + width;
            this.Top = top;
            this.Bottom = top + height;
            DepthOffset = depthOffset;
        }
    }
}