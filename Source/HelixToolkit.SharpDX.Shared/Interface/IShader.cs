/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using SharpDX.Direct3D11;


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
    public interface IShader
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the type of the shader.
        /// </summary>
        /// <value>
        /// The type of the shader.
        /// </value>
        ShaderStage ShaderType { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </value>
        bool IsNULL { get; }
        /// <summary>
        /// Gets the constant buffer mapping.
        /// </summary>
        /// <value>
        /// The constant buffer mapping.
        /// </value>
        MappingProxy<IBufferProxy> ConstantBufferMapping { get; }
        /// <summary>
        /// Gets the shader resource view mapping.
        /// </summary>
        /// <value>
        /// The shader resource view mapping.
        /// </value>
        MappingProxy<TextureMapping> ShaderResourceViewMapping { get; }
        /// <summary>
        /// Gets the unordered access view mapping.
        /// </summary>
        /// <value>
        /// The unordered access view mapping.
        /// </value>
        MappingProxy<UAVMapping> UnorderedAccessViewMapping { get; }
        /// <summary>
        /// Gets the sampler mapping.
        /// </summary>
        /// <value>
        /// The sampler mapping.
        /// </value>
        MappingProxy<SamplerMapping> SamplerMapping { get; }
        /// <summary>
        /// Binds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Bind(DeviceContext context);
        /// <summary>
        /// Binds the constant buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        void BindConstantBuffers(DeviceContext context);
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="texture">The texture.</param>
        void BindTexture(DeviceContext context, int slot, ShaderResourceView texture);
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures);
        /// <summary>
        /// Bind specified uav resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="uav"></param>
        void BindUAV(DeviceContext context, string name, UnorderedAccessView uav);
        /// <summary>
        /// Bind specified uav resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="uav"></param>
        void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav);
        /// <summary>
        /// Bind a list of uavs
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uavs"></param>
        void BindUAVs(DeviceContext context, IEnumerable<KeyValuePair<int, UnorderedAccessView>> uavs);
        /// <summary>
        /// Bind specified sampler resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        void BindSampler(DeviceContext context, string name, SamplerState sampler);
        /// <summary>
        /// Bind specified sampler resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        void BindSampler(DeviceContext context, int slot, SamplerState sampler);
        /// <summary>
        /// Bind a list of samplers
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers);
    }
}