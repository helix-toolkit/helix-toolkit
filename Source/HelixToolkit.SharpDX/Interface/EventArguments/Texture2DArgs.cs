using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class Texture2DArgs : EventArgs
{
    /// <summary>
    /// The texture
    /// </summary>
    public readonly ShaderResourceViewProxy Texture;
    /// <summary>
    /// Initializes a new instance of the <see cref="Texture2DArgs"/> class.
    /// </summary>
    /// <param name="texture">The texture.</param>
    public Texture2DArgs(ShaderResourceViewProxy texture)
    {
        Texture = texture;
    }
    /// <summary>
    /// Performs an implicit conversion from <see cref="Texture2DArgs"/> to <see cref="ShaderResourceViewProxy"/>.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator ShaderResourceViewProxy(Texture2DArgs args)
    {
        return args.Texture;
    }
}
