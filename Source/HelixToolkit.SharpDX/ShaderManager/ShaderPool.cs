using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// Pool to store and share shaders. Do not dispose shader object externally.
/// </summary>
public sealed class ShaderPool : ReferenceCountedDictionaryPool<byte[], ShaderBase, ShaderDescription>
{
    /// <summary>
    /// Gets or sets the constant buffer pool.
    /// </summary>
    /// <value>
    /// The constant buffer pool.
    /// </value>
    public IConstantBufferPool ConstantBufferPool
    {
        private set; get;
    }

    private readonly Device device;
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderPool"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="cbPool">The cb pool.</param>
    public ShaderPool(Device device, IConstantBufferPool cbPool)
        : base(false)
    {
        ConstantBufferPool = cbPool;
        this.device = device;
    }

    protected override bool CanCreate(ref byte[] key, ref ShaderDescription argument)
    {
        return key != null && key.Length > 0;
    }

    protected override ShaderBase? OnCreate(ref byte[] key, ref ShaderDescription description)
    {
        return description.ByteCode == null ?
            Constants.GetNullShader(description.ShaderType) : description.CreateShader(device, ConstantBufferPool);
    }
}
