/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NullShader : ShaderBase, IShader
    {
        /// <summary>
        /// The null
        /// </summary>
        public const string NULL = "NULL";
        /// <summary>
        /// The compute null
        /// </summary>
        public static readonly NullShader ComputeNull = new NullShader(ShaderStage.Compute);
        /// <summary>
        /// The domain null
        /// </summary>
        public static readonly NullShader DomainNull = new NullShader(ShaderStage.Domain);
        /// <summary>
        /// The geometry null
        /// </summary>
        public static readonly NullShader GeometryNull = new NullShader(ShaderStage.Geometry);
        /// <summary>
        /// The hull null
        /// </summary>
        public static readonly NullShader HullNull = new NullShader(ShaderStage.Hull);
        /// <summary>
        /// The pixel null
        /// </summary>
        public static readonly NullShader PixelNull = new NullShader(ShaderStage.Pixel);
        /// <summary>
        /// The vertex null
        /// </summary>
        public static readonly NullShader VertexNull = new NullShader(ShaderStage.Vertex);
        /// <summary>
        /// Initializes a new instance of the <see cref="NullShader"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public NullShader(ShaderStage type) : base(NULL, type, true)
        {
        }
        /// <summary>
        /// Binds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Bind(DeviceContext context)
        {
            switch (ShaderType)
            {
                case ShaderStage.Compute:
                    context.ComputeShader.Set(null);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.Set(null);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.Set(null);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.Set(null);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.Set(null);
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.Set(null);
                    break;
            }
        }
        /// <summary>
        /// Binds the constant buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        public void BindConstantBuffers(DeviceContext context)
        {

        }

        public void BindSampler(DeviceContext context, string name, SamplerState sampler)
        {
            throw new NotImplementedException();
        }

        public void BindSampler(DeviceContext context, int slot, SamplerState sampler)
        {
            throw new NotImplementedException();
        }

        public void BindSamplers(DeviceContext context, IEnumerable<KeyValuePair<int, SamplerState>> samplers)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="index">The index.</param>
        /// <param name="texture">The texture.</param>
        public void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {

        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        public void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {

        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        public void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {

        }
        /// <summary>
        /// Binds the uav.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, string name, UnorderedAccessView uav)
        {

        }
        /// <summary>
        /// Binds the uav.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="uav">The uav.</param>
        public void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav)
        {

        }
        /// <summary>
        /// Binds the ua vs.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="uavs">The uavs.</param>
        public void BindUAVs(DeviceContext context, IEnumerable<KeyValuePair<int, UnorderedAccessView>> uavs)
        {

        }
    }
}
