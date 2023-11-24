using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public interface IStatePoolManager : IDisposable
{
    /// <summary>
    /// Gets the blend state pool.
    /// </summary>
    /// <value>
    /// The blend state pool.
    /// </value>
    BlendStatePool BlendStatePool
    {
        get;
    }
    /// <summary>
    /// Gets the raster state pool.
    /// </summary>
    /// <value>
    /// The raster state pool.
    /// </value>
    RasterStatePool RasterStatePool
    {
        get;
    }
    /// <summary>
    /// Gets the depth stencil state pool.
    /// </summary>
    /// <value>
    /// The depth stencil state pool.
    /// </value>
    DepthStencilStatePool DepthStencilStatePool
    {
        get;
    }

    /// <summary>
    /// Gets the sampler state pool.
    /// </summary>
    /// <value>
    /// The sampler state pool.
    /// </value>
    SamplerStatePool SamplerStatePool
    {
        get;
    }
    /// <summary>
    /// Registers the specified desc. This function increments state proxy internal reference counter. Must be disposed if not used.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    BlendStateProxy? Register(BlendStateDescription desc);

    /// <summary>
    /// Registers the specified desc. This function increments state proxy internal reference counter. Must be disposed if not used.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    RasterizerStateProxy? Register(RasterizerStateDescription desc);
    /// <summary>
    /// Registers the specified desc. This function increments state proxy internal reference counter. Must be disposed if not used.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    DepthStencilStateProxy? Register(DepthStencilStateDescription desc);
    /// <summary>
    /// Registers the specified desc. This function increments state proxy internal reference counter. Must be disposed if not used.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    SamplerStateProxy? Register(SamplerStateDescription desc);
}
