// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextCreator.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Creates text label models or visuals.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Creates text label models or visuals.
    /// </summary>
    /// <remarks>
    /// See http://www.ericsink.com/wpf3d/4_Text.html
    /// </remarks>
    public static class TextCreator
    {
        /// <summary>
        /// Creates a Visual3D element containing a text label.
        /// </summary>
        /// <param name="text">
        /// The string
        /// </param>
        /// <param name="textColor">
        /// The color of the text.
        /// </param>
        /// <param name="isDoubleSided">
        /// Visible from both sides?
        /// </param>
        /// <param name="height">
        /// Height of the characters
        /// </param>
        /// <param name="center">
        /// The center of the label
        /// </param>
        /// <param name="textDirection">
        /// Horizontal direction of the label
        /// </param>
        /// <param name="updirection">
        /// Vertical direction of the label
        /// </param>
        /// <returns>
        /// Suitable for adding to your Viewport3D
        /// </returns>
        public static ModelVisual3D CreateTextLabel3D(
            string text, Brush textColor, bool isDoubleSided, double height, Point3D center, Vector3D textDirection, Vector3D updirection)
        {
            return new ModelVisual3D
                {
                    Content = CreateTextLabelModel3D(text, textColor, isDoubleSided, height, center, textDirection, updirection)
                };
        }

        /// <summary>
        /// Creates a model for the text label.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="isDoubleSided">double sided text if set to <c>true</c>.</param>
        /// <param name="height">The height.</param>
        /// <param name="center">The center.</param>
        /// <param name="textDirection">The textDirection.</param>
        /// <param name="updirection">The updirection.</param>
        /// <returns>A model.</returns>
        public static GeometryModel3D CreateTextLabelModel3D(
            string text, Brush textColor, bool isDoubleSided, double height, Point3D center, Vector3D textDirection, Vector3D updirection)
        {
            // First we need a textblock containing the text of our label
            var tb = new TextBlock(new Run(text)) { Foreground = textColor, FontFamily = new FontFamily("Arial") };

            // Now use that TextBlock as the brush for a material
            var mat = new DiffuseMaterial { Brush = new VisualBrush(tb) };

            // We just assume the characters are square
            double width = text.Length * height;

            // tb.Measure(new Size(text.Length * height * 2, height * 2));
            // width=tb.DesiredSize.Width;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            var p0 = center - width / 2 * textDirection - height / 2 * updirection;
            var p1 = p0 + updirection * 1 * height;
            var p2 = p0 + textDirection * width;
            var p3 = p0 + updirection * 1 * height + textDirection * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.
            var mg = new MeshGeometry3D { Positions = new Point3DCollection { p0, p1, p2, p3 } };

            if (isDoubleSided)
            {
                mg.Positions.Add(p0); // 4
                mg.Positions.Add(p1); // 5
                mg.Positions.Add(p2); // 6
                mg.Positions.Add(p3); // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (isDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.
            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (isDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            // And that's all.  Return the result.
            return new GeometryModel3D(mg, mat);
        }

    }
}