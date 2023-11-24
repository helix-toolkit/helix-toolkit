using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public sealed class SamplerStatePool : ReferenceCountedDictionaryPool<SamplerStateDescription, SamplerStateProxy, SamplerStateDescription>
{
    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="SamplerStatePool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public SamplerStatePool(Device device) : base(true)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref SamplerStateDescription key, ref SamplerStateDescription argument)
    {
        return !IsDisposed;
    }

    protected override SamplerStateProxy OnCreate(ref SamplerStateDescription key, ref SamplerStateDescription description)
    {
        return new SamplerStateProxy(new SamplerState(device, description));
    }
}
