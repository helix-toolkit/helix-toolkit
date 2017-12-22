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
    public class VertexShader : ShaderBase
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
        /// <see cref="IShader.Bind(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void Bind(DeviceContext context)
        {
            context.VertexShader.Set(shader);
        }
        /// <summary>
        /// <see cref="IShader.BindConstantBuffers(DeviceContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach(var buff in this.CBufferMapping)
            {
                context.VertexShader.SetConstantBuffer(buff.Item1, buff.Item2.Buffer);
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
            context.VertexShader.SetShaderResource(TryGetTextureIndex(name), texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTexture(DeviceContext, int, ShaderResourceView)"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
            context.VertexShader.SetShaderResource(index, texture);
        }
        /// <summary>
        /// <see cref="IShader.BindTextures(DeviceContext, IEnumerable{Tuple{int, ShaderResourceView}})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public override void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures)
        {
            foreach(var texture in textures)
            {
                context.VertexShader.SetShaderResource(texture.Item1, texture.Item2);
            }
        }
    }
}
