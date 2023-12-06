using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public sealed class BlendStatePool : ReferenceCountedDictionaryPool<BlendStateDescription, BlendStateProxy, BlendStateDescription>
{
    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="BlendStatePool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public BlendStatePool(Device device) : base(true)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref BlendStateDescription key, ref BlendStateDescription argument)
    {
        return !IsDisposed;
    }

    protected override BlendStateProxy OnCreate(ref BlendStateDescription key, ref BlendStateDescription description)
    {
        if (device.FeatureLevel < global::SharpDX.Direct3D.FeatureLevel.Level_11_0 && description.IndependentBlendEnable)
        {
            description.IndependentBlendEnable = false;
        }
        return new BlendStateProxy(new BlendState(device, description));
    }
}
