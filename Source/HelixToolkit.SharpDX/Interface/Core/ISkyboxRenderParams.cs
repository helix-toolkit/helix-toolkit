namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface ISkyboxRenderParams
{
    /// <summary>
    /// Gets or sets the cube texture.
    /// </summary>
    /// <value>
    /// The cube texture.
    /// </value>
    TextureModel? CubeTexture
    {
        set; get;
    }
    /// <summary>
    /// Skip environment map rendering, but still keep it available for other object to use.
    /// </summary>
    bool SkipRendering
    {
        set; get;
    }
}
