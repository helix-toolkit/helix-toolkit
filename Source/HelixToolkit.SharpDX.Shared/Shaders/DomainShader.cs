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
    public class DomainShader : ShaderBase
    {
        private readonly global::SharpDX.Direct3D11.DomainShader shader;

        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public DomainShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Domain)
        {
            shader = Collect(new global::SharpDX.Direct3D11.DomainShader(device, byteCode));
        }

        /// <summary>
        /// <see cref="IShader.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(DeviceContext context)
        {
            context.DomainShader.Set(shader);
        }
        /// <summary>
        /// <see cref="IShader.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.ConstantBufferMapping.Mappings)
            {
                context.DomainShader.SetConstantBuffer(buff.Item1, buff.Item2.Buffer);
            }
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, string, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
            int slot = ShaderResourceViewMapping.TryGetBindSlot(name);
            if (slot < 0)
            { return; }
            context.DomainShader.SetShaderResource(slot, texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
            context.DomainShader.SetShaderResource(index, texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTextures(DeviceContext, IEnumerable{Tuple{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public override void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures)
        {
            foreach (var texture in textures)
            {
                context.DomainShader.SetShaderResource(texture.Item1, texture.Item2);
            }
        }

        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, int, SamplerState)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="sampler"></param>
        public override void BindSampler(DeviceContext context, int slot, SamplerState sampler)
        {
            context.DomainShader.SetSampler(slot, sampler);
        }
        /// <summary>
        /// <see cref="IShader.BindSampler(DeviceContext, string, SamplerState)"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="sampler"></param>
        public override void BindSampler(DeviceContext context, string name, SamplerState sampler)
        {
            int slot = SamplerMapping.TryGetBindSlot(name);
            if (slot < 0)
            { return; }
            context.DomainShader.SetSampler(slot, sampler);
        }

        /// <summary>
        /// <see cref="IShader.BindSamplers(DeviceContext, IEnumerable{Tuple{int, SamplerState}})"/> 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="samplers"></param>
        public override void BindSamplers(DeviceContext context, IEnumerable<Tuple<int, SamplerState>> samplers)
        {
            foreach (var sampler in samplers)
            {
                context.DomainShader.SetSampler(sampler.Item1, sampler.Item2);
            }
        }
    }
}
