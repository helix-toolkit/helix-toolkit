namespace HelixToolkit.SharpDX;

/// <summary>
/// Used to cache texture models. Reuse existing texture model to avoid duplicate texture loading.
/// </summary>
public interface ITextureModelRepository
{
    /// <summary>
    /// Creates texture model from a specified stream such as memory stream or file stream.
    /// <para>This is used for implicit conversion from a Stream to TextureModel</para>
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns></returns>
    TextureModel? Create(Stream stream);
    /// <summary>
    /// Creates texture model from a specified texture path
    /// </summary>
    /// <param name="texturePath">The texture path.</param>
    /// <returns></returns>
    TextureModel? Create(string texturePath);
}
