using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public sealed class DepthStencilStatePool : ReferenceCountedDictionaryPool<DepthStencilStateDescription, DepthStencilStateProxy, DepthStencilStateDescription>
{
    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="DepthStencilStatePool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public DepthStencilStatePool(Device device) : base(true)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref DepthStencilStateDescription key, ref DepthStencilStateDescription argument)
    {
        return !IsDisposed;
    }

    protected override DepthStencilStateProxy OnCreate(ref DepthStencilStateDescription key, ref DepthStencilStateDescription description)
    {
        return new DepthStencilStateProxy(new DepthStencilState(device, description));
    }
}
