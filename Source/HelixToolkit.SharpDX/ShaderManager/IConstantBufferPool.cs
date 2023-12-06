using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// Pool to store and share constant buffers. Do not dispose constant buffer object externally.
/// </summary>
public interface IConstantBufferPool : IDisposable
{
    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    int Count
    {
        get;
    }
    /// <summary>
    /// Gets the device.
    /// </summary>
    /// <value>
    /// The device.
    /// </value>
    Device Device
    {
        get;
    }

    /// <summary>
    /// Registers the specified description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    ConstantBufferProxy? Register(ConstantBufferDescription description);

    /// <summary>
    /// Registers the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="structSize">Size of the structure.</param>
    /// <returns></returns>
    ConstantBufferProxy? Register(string name, int structSize);
}
