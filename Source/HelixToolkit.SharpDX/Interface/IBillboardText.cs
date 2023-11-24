namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IBillboardText
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    BillboardType Type
    {
        get;
    }
    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <value>
    /// The texture.
    /// </value>
    TextureModel? Texture
    {
        get;
    }

    /// <summary>
    /// Draws the texture.
    /// </summary>
    /// <param name="devices">The devices.</param>
    void DrawTexture(IDeviceResources devices);
    /// <summary>
    /// Gets the billboard vertices.
    /// </summary>
    /// <value>
    /// The billboard vertices.
    /// </value>
    IList<BillboardVertex> BillboardVertices
    {
        get;
    }
    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    float Width
    {
        get;
    }
    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    float Height
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether this billboard internal is initialized.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    bool IsInitialized
    {
        get;
    }
}
