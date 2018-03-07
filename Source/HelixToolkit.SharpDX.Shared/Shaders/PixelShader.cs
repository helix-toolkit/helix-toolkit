/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// Pixel Shader
    /// </summary>
    public sealed class PixelShader : ShaderBase, IShader
    {
        private readonly global::SharpDX.Direct3D11.PixelShader shader;

        /// <summary>
        /// Pixel Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public PixelShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Pixel)
        {
            shader = Collect(new global::SharpDX.Direct3D11.PixelShader(device, byteCode));
        }
        /// <summary>
        /// <see cref="IShader.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public void Bind(DeviceContext context)
        {
            context.PixelShader.Set(shader);
        }
        /// <summary>
        /// <see cref="IShader.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.ConstantBufferMapping.Mappings)
            {
                context.PixelShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
            }
        }

        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, string, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
            int slot = ShaderResourceViewMapping.TryGetBindSlot(name);
            if (slot < 0)
            { return; }
            context.PixelShader.SetShaderResource(slot, texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="texture"></param>
        public void BindTexture(DeviceContext context, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            context.PixelShader.SetShaderResource(slot, texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTextures(DeviceContext, IEnumerable{KeyValuePair{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {
            foreach (var texture in textures)
            {
                context.PixelShader.SetShaderResource(texture.Key, texture.Value);
            }
        }

        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, int, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        public void BindSampler(DeviceContext context, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            context.PixelShader.SetSampler(slot, sampler);
        }
        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, string, SamplerState)"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        public void BindSampler(DeviceContext context, string name, SamplerState sampler)
        {
            int slot = SamplerMapping.TryGetBindSlot(name);
            if (slot < 0) { return; }
            context.PixelShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// <see cref="IShader.BindSamplers(DeviceContext, IEnumerable{KeyValuePair{int, SamplerState}})"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        public void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.PixelShader.SetSampler(sampler.Key, sampler.Value);
            }
        }
        /// <summary>
        /// Binds the uav. Not Valid for PixelShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, string name, UnorderedAccessView uav)
        {
            
        }
        /// <summary>
        /// Binds the uav. Not Valid for PixelShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav)
        {
            
        }
        /// <summary>
        /// Binds the ua vs. Not Valid for PixelShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="uavs">The uavs.</param>
        public void BindUAVs(DeviceContext context, IEnumerable<KeyValuePair<int, UnorderedAccessView>> uavs)
        {
            
        }
    }
}
