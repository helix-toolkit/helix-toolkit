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
    public class ComputeShader : ShaderBase
    {
        private readonly global::SharpDX.Direct3D11.ComputeShader shader;

        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public ComputeShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Compute)
        {
            shader = Collect(new global::SharpDX.Direct3D11.ComputeShader(device, byteCode));
        }

        /// <summary>
        /// <see cref="IShader.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(DeviceContext context)
        {
            context.ComputeShader.Set(shader);
        }
        /// <summary>
        /// <see cref="IShader.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.CBufferMapping)
            {
                context.ComputeShader.SetConstantBuffer(buff.Item1, buff.Item2.Buffer);
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
            context.ComputeShader.SetShaderResource(TryGetTextureIndex(name), texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
            context.ComputeShader.SetShaderResource(index, texture);
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
                context.ComputeShader.SetShaderResource(texture.Item1, texture.Item2);
            }
        }
        /// <summary>
        /// <see cref="IShader.BindUAV(DeviceContext, int, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="uav"></param>
        public override void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav)
        {
            context.ComputeShader.SetUnorderedAccessView(slot, uav);
        }
        /// <summary>
        /// <see cref="IShader.BindUAV(DeviceContext, string, UnorderedAccessView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="uav"></param>
        public override void BindUAV(DeviceContext context, string name, UnorderedAccessView uav)
        {
            context.ComputeShader.SetUnorderedAccessView(TryGetTextureIndex(name), uav);
        }
        /// <summary>
        /// <see cref="IShader.BindUAVs(DeviceContext, IEnumerable{Tuple{int, UnorderedAccessView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uavs"></param>
        public override void BindUAVs(DeviceContext context, IEnumerable<Tuple<int, UnorderedAccessView>> uavs)
        {
            foreach(var uav in uavs)
            {
                context.ComputeShader.SetUnorderedAccessView(uav.Item1, uav.Item2);
            }
        }
    }
}
