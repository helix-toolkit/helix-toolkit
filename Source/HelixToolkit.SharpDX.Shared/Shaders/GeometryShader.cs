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
    public sealed class GeometryShader : ShaderBase
    {
        private global::SharpDX.Direct3D11.GeometryShader shader;
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryShader"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="name">The name.</param>
        /// <param name="byteCode">The byte code.</param>
        public GeometryShader(Device device, string name, byte[] byteCode) : base(name, ShaderStage.Geometry)
        {
            shader = Collect(new global::SharpDX.Direct3D11.GeometryShader(device, byteCode));
        }
        /// <summary>
        /// Binds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Bind(DeviceContext context)
        {
            context.GeometryShader.Set(shader);
        }
        /// <summary>
        /// Binds the constant buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.ConstantBufferMapping.Mappings)
            {
                context.GeometryShader.SetConstantBuffer(buff.Key, buff.Value.Buffer);
            }
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="index">The index.</param>
        /// <param name="texture">The texture.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {
        }
    }
}
