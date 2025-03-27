﻿using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit.Wpf;

/// <summary>
/// A terrain texture base class.
/// </summary>
public abstract class TerrainTexture
{
    /// <summary>
    /// Initializes a new instance of the <see cref = "TerrainTexture" /> class.
    /// </summary>
    public TerrainTexture()
    {
        this.Material = Materials.Green;
    }

    /// <summary>
    /// Gets or sets the material.
    /// </summary>
    /// <value>The material.</value>
    public Material? Material { get; set; }

    /// <summary>
    /// Gets or sets the texture coordinates.
    /// </summary>
    /// <value>The texture coordinates.</value>
    public PointCollection? TextureCoordinates { get; set; }

    /// <summary>
    /// Calculates the texture of the specified model.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <param name="mesh">
    /// The mesh.
    /// </param>
    public virtual void Calculate(ITerrainModel model, MeshGeometry3D mesh)
    {
    }
}
