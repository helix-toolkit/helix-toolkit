using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IBillboardBufferModel : IDisposable
{
    /// <summary>
    /// Gets the texture view.
    /// </summary>
    /// <value>
    /// The texture view.
    /// </value>
    ShaderResourceViewProxy? TextureView
    {
        get;
    }
    /// <summary>
    /// Gets the billboard type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    BillboardType Type
    {
        get;
    }
}
