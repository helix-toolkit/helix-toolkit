namespace HelixToolkit.SharpDX;

/// <summary>
/// Custom Texture loading IO interface.
/// </summary>
public interface ITexturePathResolver
{
    /// <summary>
    /// Resolves the texture path.
    /// </summary>
    /// <param name="modelPath">The model path.</param>
    /// <param name="texturePath">The texture path.</param>
    /// <returns>Absolute file path for the texture</returns>
    string? Resolve(string modelPath, string texturePath);
}
