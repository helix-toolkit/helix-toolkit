using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public sealed class StatePoolManager : DisposeObject, IStatePoolManager
{
    /// <summary>
    /// Gets or sets the blend state pool.
    /// </summary>
    /// <value>
    /// The blend state pool.
    /// </value>
    public BlendStatePool BlendStatePool
    {
        get;
    }
    /// <summary>
    /// Gets or sets the raster state pool.
    /// </summary>
    /// <value>
    /// The raster state pool.
    /// </value>
    public RasterStatePool RasterStatePool
    {
        get;
    }
    /// <summary>
    /// Gets or sets the depth stencil state pool.
    /// </summary>
    /// <value>
    /// The depth stencil state pool.
    /// </value>
    public DepthStencilStatePool DepthStencilStatePool
    {
        get;
    }

    /// <summary>
    /// Gets or sets the sampler state pool.
    /// </summary>
    /// <value>
    /// The sampler state pool.
    /// </value>
    public SamplerStatePool SamplerStatePool
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatePoolManager"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public StatePoolManager(Device device)
    {
        BlendStatePool = new BlendStatePool(device);
        RasterStatePool = new RasterStatePool(device);
        DepthStencilStatePool = new DepthStencilStatePool(device);
        SamplerStatePool = new SamplerStatePool(device);
    }

    /// <summary>
    /// Registers the specified desc.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    public BlendStateProxy? Register(BlendStateDescription desc)
    {
        return BlendStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
    }
    /// <summary>
    /// Registers the specified desc.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    public RasterizerStateProxy? Register(RasterizerStateDescription desc)
    {
        return RasterStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
    }
    /// <summary>
    /// Registers the specified desc.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    public DepthStencilStateProxy? Register(DepthStencilStateDescription desc)
    {
        return DepthStencilStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
    }
    /// <summary>
    /// Registers the specified desc.
    /// </summary>
    /// <param name="desc">The desc.</param>
    /// <returns></returns>
    public SamplerStateProxy? Register(SamplerStateDescription desc)
    {
        return SamplerStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        BlendStatePool.Dispose();
        RasterStatePool.Dispose();
        DepthStencilStatePool.Dispose();
        SamplerStatePool.Dispose();
        base.OnDispose(disposeManagedResources);
    }
}
