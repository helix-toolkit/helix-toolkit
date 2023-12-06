using HelixToolkit.Logger;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// Pool to store and share constant buffers. Do not dispose constant buffer object externally.
/// </summary>
public sealed class ConstantBufferPool : ReferenceCountedDictionaryPool<string, ConstantBufferProxy, ConstantBufferDescription>, IConstantBufferPool
{
    private static readonly ILogger logger = LogManager.Create<ConstantBufferPool>();
    private readonly Device device;
    public Device Device => device;
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantBufferPool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    public ConstantBufferPool(Device device)
        : base(false)
    {
        this.device = device;
    }

    protected override bool CanCreate(ref string key, ref ConstantBufferDescription argument)
    {
        return !string.IsNullOrEmpty(key);
    }

    /// <summary>
    /// Creates the specified constant buffer.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    protected override ConstantBufferProxy OnCreate(ref string key, ref ConstantBufferDescription description)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Creating constant buffer. Key: {0}; Size: {1}", key, description.StructSize);
        }
        var buffer = description.CreateBuffer();
        buffer.CreateBuffer(device);
        ErrorCheck(buffer, ref description);
        return buffer;
    }

    private void ErrorCheck(ConstantBufferProxy value, ref ConstantBufferDescription description)
    {
        if (value.StructureSize != description.StructSize)
        {
            throw new ArgumentException($"Constant buffer with same name is found but their size does not match.\n" +
                $"Name: {description.Name}. Existing Size:{value.StructureSize}; New Size:{description.StructSize}.\n" +
                $"Potential Causes: Different Constant buffer header has been used.\n" +
                $"Please refer and update to latest HelixToolkit.SharpDX.ShaderBuilder.CommonBuffers.hlsl. Link: https://github.com/helix-toolkit/helix-toolkit");
        }
        if (description.Variables.Count > 0)
        {
            foreach (var variable in description.Variables)
            {
                value.AddVariable(variable);
            }
        }
    }

    /// <summary>
    /// Registers the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="structSize">Size of the structure.</param>
    /// <returns></returns>
    public ConstantBufferProxy? Register(string name, int structSize)
    {
        return Register(new ConstantBufferDescription(name, structSize));
    }

    public ConstantBufferProxy? Register(ConstantBufferDescription description)
    {
        if (TryCreateOrGet(description.Name, description, out var buffer))
        {
            foreach (var var in description.Variables)
            {
                buffer.AddVariable(var);
            }
            return buffer;
        }
        return null;
    }
}
