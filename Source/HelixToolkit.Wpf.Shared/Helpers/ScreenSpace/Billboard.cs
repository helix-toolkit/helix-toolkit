// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Billboard.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a billboard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

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
        /// The depth offset in normalized units
        /// </summary>
        internal double DepthOffset;

        /// <summary>
        /// The depth offset in model (world) units
        /// </summary>
        internal double WorldDepthOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Billboard" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="size">The size.</param>
        /// <param name="depthOffset">The depth offset.</param>
        /// <param name="worldDepthOffset">The depth offset in world coordinates.</param>
        public Billboard(Point3D position, double size, double depthOffset, double worldDepthOffset = 0)
        {
            double halfSize = size / 2.0;
            this.Position = position;
            this.Left = -halfSize;
            this.Right = halfSize;
            this.Top = -halfSize;
            this.Bottom = halfSize;
            this.DepthOffset = depthOffset;
            this.WorldDepthOffset = worldDepthOffset;
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
        /// <param name="worldDepthOffset">The depth offset in world coordinates.</param>
        public Billboard(Point3D position, double width = 1.0, double height = 1.0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, VerticalAlignment verticalAlignment = VerticalAlignment.Center, double depthOffset = 0.0, double worldDepthOffset = 0)
        {
            // Set horizontal alignment factor
            var xa = -0.5;
            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                xa = 0;
            }

            if (horizontalAlignment == HorizontalAlignment.Right)
            {
                xa = -1;
            }

            // Set vertical alignment factor
            var ya = -0.5;
            if (verticalAlignment == VerticalAlignment.Top)
            {
                ya = 0;
            }

            if (verticalAlignment == VerticalAlignment.Bottom)
            {
                ya = -1;
            }

            double left = xa * width;
            double top = ya * height;

            this.Position = position;
            this.Left = left;
            this.Right = left + width;
            this.Top = top;
            this.Bottom = top + height;
            this.DepthOffset = depthOffset;
            this.WorldDepthOffset = worldDepthOffset;
        }
    }
}