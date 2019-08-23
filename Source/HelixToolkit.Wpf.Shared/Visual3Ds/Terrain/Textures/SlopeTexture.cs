// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlopeTexture.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Texture by the slope angle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Texture by the slope angle.
    /// </summary>
    public class SlopeTexture : TerrainTexture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlopeTexture"/> class.
        /// </summary>
        /// <param name="gradientSteps">
        /// The gradient steps.
        /// </param>
        public SlopeTexture(int gradientSteps)
        {
            if (gradientSteps > 0)
            {
                this.Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.BlueWhiteRed, gradientSteps);
            }
            else
            {
                this.Brush = GradientBrushes.BlueWhiteRed;
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        /// <value>The brush.</value>
        public Brush Brush { get; set; }

        /// <summary>
        /// Calculates the texture for the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="mesh">
        /// The mesh.
        /// </param>
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var normals = MeshGeometryHelper.CalculateNormals(mesh);
            var texcoords = new PointCollection();
            var up = new Vector3D(0, 0, 1);
            for (int i = 0; i < normals.Count; i++)
            {
                double slope = Math.Acos(Vector3D.DotProduct(normals[i], up)) * 180 / Math.PI;
                double u = slope / 40;
                if (u > 1)
                {
                    u = 1;
                }

                if (u < 0)
                {
                    u = 0;
                }

                texcoords.Add(new Point(u, u));
            }

            this.TextureCoordinates = texcoords;
            this.Material = MaterialHelper.CreateMaterial(this.Brush);
        }

    }
}