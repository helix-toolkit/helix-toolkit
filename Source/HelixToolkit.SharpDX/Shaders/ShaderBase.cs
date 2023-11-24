using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public abstract class ShaderBase : DisposeObject
{
    /// <summary>
    /// 
    /// </summary>
    public MappingProxy<ConstantBufferProxy> ConstantBufferMapping { get; } = new();
    /// <summary>
    /// 
    /// </summary>
    public MappingProxy<TextureMapping> ShaderResourceViewMapping { get; } = new();
    /// <summary>
    /// 
    /// </summary>
    public MappingProxy<UAVMapping> UnorderedAccessViewMapping { get; } = new();
    /// <summary>
    /// 
    /// </summary>
    public MappingProxy<SamplerMapping> SamplerMapping { get; } = new();

    /// <summary>
    /// Gets the type of the shader.
    /// </summary>
    /// <value>
    /// The type of the shader.
    /// </value>
    public ShaderStage ShaderType
    {
        private set; get;
    }
    /// <summary>
    /// Gets the index of the shader stage.
    /// </summary>
    /// <value>
    /// The index of the shader stage.
    /// </value>
    public int ShaderStageIndex
    {
        private set; get;
    }
    /// <summary>
    /// If is null shader
    /// </summary>
    public bool IsNULL
    {
        protected set; get;
    }
    /// <summary>
    /// Shader Name
    /// </summary>
    public string Name
    {
        private set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="isNull"></param>
    public ShaderBase(string name, ShaderStage type, bool isNull = false)
    {
        ShaderType = type;
        ShaderStageIndex = type.ToIndex();
        Name = name;
        IsNULL = isNull;
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        ConstantBufferMapping.Dispose();
        ShaderResourceViewMapping.Dispose();
        UnorderedAccessViewMapping.Dispose();
        SamplerMapping.Dispose();
        base.OnDispose(disposeManagedResources);
    }
}
