/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// Vertex Shader
    /// </summary>
    public sealed class VertexShader : ShaderBase
    {
        private readonly global::SharpDX.Direct3D11.VertexShader shader;
        
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public VertexShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Vertex)
        {
            shader = Collect(new global::SharpDX.Direct3D11.VertexShader(device, byteCode));        
        }

        /// <summary>
        /// <see cref="ShaderBase.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(DeviceContext context)
        {
            context.VertexShader.Set(shader);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach(var buff in this.ConstantBufferMapping.Mappings)
            {
                context.VertexShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
            }
        }
        /// <summary>
        /// <see cref="ShaderBase.BindTexture(DeviceContext, string, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
            int slot = ShaderResourceViewMapping.TryGetBindSlot(name);
            if (slot < 0)
            { return; }
            context.VertexShader.SetShaderResource(slot, texture);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, int slot, ShaderResourceView texture)
        {
            if (slot < 0)
            { return; }
            context.VertexShader.SetShaderResource(slot, texture);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindTextures(DeviceContext, IEnumerable{KeyValuePair{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public override void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {
            foreach(var texture in textures)
            {
                context.VertexShader.SetShaderResource(texture.Key, texture.Value);
            }
        }
        /// <summary>
        /// <see cref="ShaderBase.BindSampler(DeviceContext, int, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        public override void BindSampler(DeviceContext context, int slot, SamplerState sampler)
        {
            if (slot < 0)
            { return; }
            context.VertexShader.SetSampler(slot, sampler);
        }
        /// <summary>
        /// <see cref="ShaderBase.BindSampler(DeviceContext, string, SamplerState)"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        public override void BindSampler(DeviceContext context, string name, SamplerState sampler)
        {
            int slot = SamplerMapping.TryGetBindSlot(name);
            if (slot < 0) { return; }
            context.VertexShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// <see cref="ShaderBase.BindSamplers(DeviceContext, IEnumerable{KeyValuePair{int, SamplerState}})"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        public override void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers)
        {
            foreach(var sampler in samplers)
            {
                context.VertexShader.SetSampler(sampler.Key, sampler.Value);
            }
        }
    }
}
