// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapTexture.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Terrain texture using a bitmap. Set the Left,Right,Bottom and Top coordinates to get the right alignment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Terrain texture using a bitmap. Set the Left,Right,Bottom and Top coordinates to get the right alignment.
    /// </summary>
    public class MapTexture : TerrainTexture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapTexture"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public MapTexture(string source)
        {
            this.Material = MaterialHelper.CreateImageMaterial(source, 1);
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public double Bottom { get; set; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public double Left { get; set; }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public double Right { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public double Top { get; set; }

        /// <summary>
        /// Calculates the texture of the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var texcoords = new PointCollection();
            foreach (var p in mesh.Positions)
            {
                double x = p.X + model.Offset.X;
                double y = p.Y + model.Offset.Y;
                double u = (x - this.Left) / (this.Right - this.Left);
                double v = (y - this.Top) / (this.Bottom - this.Top);
                texcoords.Add(new Point(u, v));
            }

            this.TextureCoordinates = texcoords;
        }

    }
}