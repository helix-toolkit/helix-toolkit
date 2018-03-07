/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;

    /// <summary>
    /// 
    /// </summary>
    public abstract class ShaderBase : DisposeObject
    {
        /// <summary>
        /// 
        /// </summary>
        public MappingProxy<IBufferProxy> ConstantBufferMapping { get; } = new MappingProxy<IBufferProxy>();
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
        /// <see cref="IShader.ShaderType"/>
        /// </summary>
        public ShaderStage ShaderType { private set; get; }
        /// <summary>
        /// <see cref="IShader.IsNULL"/>
        /// </summary>
        public bool IsNULL { private set; get; }
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
            Name = name;
            IsNULL = isNull;
        }
    }
}
