using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// Pool to store and share shader layouts. Do not dispose layout object externally.
/// </summary>
public sealed class LayoutPool : ReferenceCountedDictionaryPool<byte[], InputLayoutProxy, InputLayoutDescription>
{
    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutPool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public LayoutPool(Device device)
        : base(false)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref byte[] key, ref InputLayoutDescription argument)
    {
        return key != null && key.Length > 0;
    }

    protected override InputLayoutProxy? OnCreate(ref byte[] key, ref InputLayoutDescription description)
    {
        if (description.ShaderByteCode is null)
        {
            return null;
        }

        return new InputLayoutProxy(device, description.ShaderByteCode, description.InputElements);
    }
}
