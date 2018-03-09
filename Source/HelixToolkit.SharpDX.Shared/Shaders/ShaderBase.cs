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
        /// <see cref="ShaderBase.ShaderType"/>
        /// </summary>
        public ShaderStage ShaderType { private set; get; }
        /// <summary>
        /// <see cref="ShaderBase.IsNULL"/>
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
        public abstract void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures);

        /// <summary>
        /// <see cref="ShaderBase.BindUAV(DeviceContext, string, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="uav"></param>
        public virtual void BindUAV(DeviceContext context, string name, UnorderedAccessView uav) {  }
        /// <summary>
        /// <see cref="ShaderBase.BindUAV(DeviceContext, int, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="uav"></param>
        public virtual void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav) { }
        /// <summary>
        /// <see cref="ShaderBase.BindUAVs(DeviceContext, IEnumerable{KeyValuePair{int, UnorderedAccessView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uavs"></param>
        public virtual void BindUAVs(DeviceContext context, IEnumerable<KeyValuePair<int, UnorderedAccessView>> uavs) { }

        /// <summary>
        /// <see cref="ShaderBase.BindSampler(DeviceContext, string, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        public virtual void BindSampler(DeviceContext context, string name, SamplerState sampler) { }
        /// <summary>
        /// <see cref="ShaderBase.BindSampler(DeviceContext, int, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        public virtual void BindSampler(DeviceContext context, int slot, SamplerState sampler) { }
        /// <summary>
        /// <see cref="ShaderBase.BindSamplers(DeviceContext, IEnumerable{KeyValuePair{int, SamplerState}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        public virtual void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers) { }
    }
}
