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
    /// 
    /// </summary>
    public sealed class HullShader : ShaderBase, IShader
    {
        private readonly global::SharpDX.Direct3D11.HullShader shader;

        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public HullShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Hull)
        {
            shader = Collect(new global::SharpDX.Direct3D11.HullShader(device, byteCode));
        }

        /// <summary>
        /// <see cref="IShader.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public void Bind(DeviceContext context)
        {
            context.HullShader.Set(shader);
        }
        /// <summary>
        /// <see cref="IShader.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.ConstantBufferMapping.Mappings)
            {
                context.HullShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
            }
        }
        /// <summary>
        /// Binds the sampler. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="sampler">The sampler.</param>
        public void BindSampler(DeviceContext context, string name, SamplerState sampler)
        {

        }
        /// <summary>
        /// Binds the sampler. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="sampler">The sampler.</param>
        public void BindSampler(DeviceContext context, int slot, SamplerState sampler)
        {

        }
        /// <summary>
        /// Binds the samplers. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="samplers">The samplers.</param>
        public void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers)
        {

        }

        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, string, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
        }
        /// <summary>
        /// <see cref="IShader.BindTextures(DeviceContext, IEnumerable{KeyValuePair{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {
        }
        /// <summary>
        /// Binds the uav. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, string name, UnorderedAccessView uav)
        {

        }
        /// <summary>
        /// Binds the uav. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav)
        {

        }
        /// <summary>
        /// Binds the ua vs. Not Valid for HullShader
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="uavs">The uavs.</param>
        public void BindUAVs(DeviceContext context, IEnumerable<KeyValuePair<int, UnorderedAccessView>> uavs)
        {

        }
    }
}
