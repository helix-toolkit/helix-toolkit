using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public sealed class RasterStatePool : ReferenceCountedDictionaryPool<RasterizerStateDescription, RasterizerStateProxy, RasterizerStateDescription>
{
    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="RasterStatePool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public RasterStatePool(Device device) : base(true)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref RasterizerStateDescription key, ref RasterizerStateDescription argument)
    {
        return !IsDisposed;
    }

    protected override RasterizerStateProxy OnCreate(ref RasterizerStateDescription key, ref RasterizerStateDescription description)
    {
        return new RasterizerStateProxy(new RasterizerState(device, description));
    }
}
