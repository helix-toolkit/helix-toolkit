using HelixToolkit.SharpDX.Shaders;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public class ShaderPoolManager : DisposeObject, IShaderPoolManager
{
    private readonly ShaderPool?[] shaderPools = new ShaderPool?[Constants.NumShaderStages];
    private LayoutPool? layoutPool;
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderPoolManager"/> class.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="cbPool">The cb pool.</param>
    public ShaderPoolManager(Device device, IConstantBufferPool cbPool)
    {
        shaderPools[Constants.VertexIdx] = new ShaderPool(device, cbPool);
        shaderPools[Constants.DomainIdx] = new ShaderPool(device, cbPool);
        shaderPools[Constants.HullIdx] = new ShaderPool(device, cbPool);
        shaderPools[Constants.GeometryIdx] = new ShaderPool(device, cbPool);
        shaderPools[Constants.PixelIdx] = new ShaderPool(device, cbPool);
        shaderPools[Constants.ComputeIdx] = new ShaderPool(device, cbPool);
        layoutPool = new LayoutPool(device);
    }
    /// <summary>
    /// Registers the shader.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public ShaderBase? RegisterShader(ShaderDescription? description)
    {
        if (description == null || description.ByteCode is null)
        {
            return null;
        }

        var shaderPool = shaderPools[description.ShaderType.ToIndex()];

        if (shaderPool is null)
        {
            return null;
        }

        return shaderPool.TryCreateOrGet(description.ByteCode, description, out var shader)
            ? shader
            : null;
    }
    /// <summary>
    /// Registers the input layout.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public InputLayoutProxy? RegisterInputLayout(InputLayoutDescription? description)
    {
        if (description == null || description.ShaderByteCode is null || layoutPool is null)
        {
            return null;
        }

        return layoutPool.TryCreateOrGet(description.ShaderByteCode, description, out var inputLayout) ? inputLayout : null;
    }
    /// <summary>
    /// Called when [dispose].
    /// </summary>
    /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>
    protected override void OnDispose(bool disposeManagedResources)
    {
        for (var i = 0; i < shaderPools.Length; ++i)
        {
            RemoveAndDispose(ref shaderPools[i]);
        }
        RemoveAndDispose(ref layoutPool);
        base.OnDispose(disposeManagedResources);
    }
}
