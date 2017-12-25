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
    public abstract class ShaderBase : DisposeObject, IShader
    {
        public MappingProxy<IBufferProxy> ConstantBufferMapping { get; } = new MappingProxy<IBufferProxy>();
        public MappingProxy<TextureMapping> ShaderResourceViewMapping { get; } = new MappingProxy<TextureMapping>();
        public MappingProxy<UAVMapping> UnorderedAccessViewMapping { get; } = new MappingProxy<UAVMapping>();
        public MappingProxy<SamplerMapping> SamplerMapping { get; } = new MappingProxy<SamplerMapping>();

        /// <summary>
        /// Bind Stage
        /// </summary>
        public ShaderStage ShaderType { private set; get; }
        /// <summary>
        /// Shader Name
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public ShaderBase(string name, ShaderStage type)
        {
            ShaderType = type;
            Name = name;
        }

        /// <summary>
        /// Bind this shader to pipeline
        /// </summary>
        /// <param name="context"></param>
        public abstract void Bind(DeviceContext context);
        /// <summary>
        /// Bind all constant buffers in this shader
        /// </summary>
        /// <param name="context"></param>
        public abstract void BindConstantBuffers(DeviceContext context);
        /// <summary>
        /// Bind specified texture resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public abstract void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        /// <summary>
        ///  Bind specified texture resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="texture"></param>
        public abstract void BindTexture(DeviceContext context, int slot, ShaderResourceView texture);
        /// <summary>
        /// Bind a list of textures
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public abstract void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures);

        /// <summary>
        /// <see cref="IShader.BindUAV(DeviceContext, string, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="uav"></param>
        public virtual void BindUAV(DeviceContext context, string name, UnorderedAccessView uav) { throw new NotImplementedException(); }
        /// <summary>
        /// <see cref="IShader.BindUAV(DeviceContext, int, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="uav"></param>
        public virtual void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav) { throw new NotImplementedException(); }
        /// <summary>
        /// <see cref="IShader.BindUAVs(DeviceContext, IEnumerable{Tuple{int, UnorderedAccessView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uavs"></param>
        public virtual void BindUAVs(DeviceContext context, IEnumerable<Tuple<int, UnorderedAccessView>> uavs) { throw new NotImplementedException(); }

        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, string, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        public virtual void BindSampler(DeviceContext context, string name, SamplerState sampler) { }
        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, int, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        public virtual void BindSampler(DeviceContext context, int slot, SamplerState sampler) { }
        /// <summary>
        /// <see cref="IShader.BindSamplers(DeviceContext, IEnumerable{Tuple{int, SamplerState}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        public virtual void BindSamplers(DeviceContext context, IEnumerable<Tuple<int, SamplerState>> samplers) { }
    }
}
