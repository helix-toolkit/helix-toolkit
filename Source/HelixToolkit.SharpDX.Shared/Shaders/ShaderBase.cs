/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;

    /// <summary>
    /// 
    /// </summary>
    public abstract class ShaderBase : DisposeObject
    {
        /// <summary>
        /// 
        /// </summary>
        public MappingProxy<ConstantBufferProxy> ConstantBufferMapping { get; } = new MappingProxy<ConstantBufferProxy>();
        /// <summary>
        /// 
        /// </summary>
        public MappingProxy<TextureMapping> ShaderResourceViewMapping { get; } = new MappingProxy<TextureMapping>();
        /// <summary>
        /// 
        /// </summary>
        public MappingProxy<UAVMapping> UnorderedAccessViewMapping { get; } = new MappingProxy<UAVMapping>();
        /// <summary>
        /// 
        /// </summary>
        public MappingProxy<SamplerMapping> SamplerMapping { get; } = new MappingProxy<SamplerMapping>();

        /// <summary>
        /// Gets the type of the shader.
        /// </summary>
        /// <value>
        /// The type of the shader.
        /// </value>
        public ShaderStage ShaderType { private set; get; }
        /// <summary>
        /// Gets the index of the shader stage.
        /// </summary>
        /// <value>
        /// The index of the shader stage.
        /// </value>
        public int ShaderStageIndex { private set; get; }
        /// <summary>
        /// If is null shader
        /// </summary>
        public bool IsNULL { protected set; get; }
        /// <summary>
        /// Shader Name
        /// </summary>
        public string Name { private set; get; }
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
    }
}