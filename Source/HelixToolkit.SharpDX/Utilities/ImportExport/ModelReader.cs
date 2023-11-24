using HelixToolkit.SharpDX.Model;
using SharpDX;
using Mesh3DGroup = System.Collections.Generic.List<HelixToolkit.SharpDX.Object3D>;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Class ModelReader.
/// </summary>
public abstract class ModelReader : IModelReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelReader"/> class.
    /// </summary>
    protected ModelReader()
    {
        this.DefaultMaterial = new PhongMaterialCore
        {
            Name = "Gold",
            AmbientColor = new Color4(0.24725f, 0.1995f, 0.0745f, 1.0f),
            DiffuseColor = new Color4(0.75164f, 0.60648f, 0.22648f, 1.0f),
            SpecularColor = new Color4(0.628281f, 0.555802f, 0.366065f, 1.0f),
            EmissiveColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f),
            SpecularShininess = 51.2f,
        };
    }
    /// <summary>
    /// Gets or sets the default material.
    /// </summary>
    /// <value>
    /// The default material.
    /// </value>
    public MaterialCore DefaultMaterial
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the directory.
    /// </summary>
    /// <value>The directory.</value>
    public string Directory
    {
        get; set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the texture path.
    /// </summary>
    /// <value>The texture path.</value>
    public string TexturePath
    {
        get
        {
            return this.Directory;
        }

        set
        {
            this.Directory = value;
        }
    }

    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="info"></param>
    /// <returns>The model.</returns>
    public virtual Mesh3DGroup? Read(string path, ModelInfo info = default)
    {
        this.Directory = Path.GetDirectoryName(path) ?? string.Empty;
        using var s = File.OpenRead(path);
        return this.Read(s, info);
    }

    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">The stream.</param>
    /// <param name="info"></param>
    /// <returns>The model.</returns>
    public abstract Mesh3DGroup? Read(Stream s, ModelInfo info = default);
}
