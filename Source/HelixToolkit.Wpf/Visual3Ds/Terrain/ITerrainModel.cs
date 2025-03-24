using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

public interface ITerrainModel
{
    /// <summary>
    /// Gets or sets the texture.
    /// </summary>
    /// <value>The texture.</value>
    TerrainTexture? Texture { get; set; }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset.</value>
    Point3D Offset { get; set; }

    /// <summary>
    /// Gets the lod.
    /// </summary>
    int Lod { get; }

    /// <summary>
    /// Creates the 3D model of the terrain.
    /// </summary>
    /// <param name="lod">
    /// The level of detail.
    /// </param>
    /// <returns>
    /// The Model3D.
    /// </returns>
    GeometryModel3D? CreateModel(int lod);

    /// <summary>
    /// Loads the specified file.
    /// </summary>
    /// <param name="source">
    /// The file name.
    /// </param>
    void Load(string source);
}
