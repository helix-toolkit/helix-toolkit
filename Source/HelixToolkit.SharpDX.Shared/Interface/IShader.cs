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
    public interface IShader : IDisposable
    {
        string Name { get; }
        ShaderStage ShaderType { get; }
        bool IsNULL { get; }
        MappingProxy<IBufferProxy> ConstantBufferMapping { get; }
        MappingProxy<TextureMapping> ShaderResourceViewMapping { get; }
        MappingProxy<UAVMapping> UnorderedAccessViewMapping { get; }
        MappingProxy<SamplerMapping> SamplerMapping { get; }
        void Bind(DeviceContext context);
        void BindConstantBuffers(DeviceContext context);
        void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        void BindTexture(DeviceContext context, int slot, ShaderResourceView texture);
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