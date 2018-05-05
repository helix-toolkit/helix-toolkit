/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    public sealed class NullShader : ShaderBase
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
        private NullShader(ShaderStage type) : base(NULL, type, true)
        {
        }

        public static NullShader GetNullShader(ShaderStage type)
        {
            switch (type)
            {
                case ShaderStage.Compute:
                    return ComputeNull;
                case ShaderStage.Domain:
                    return DomainNull;
                case ShaderStage.Geometry:
                    return GeometryNull;
                case ShaderStage.Hull:
                    return HullNull;
                case ShaderStage.Pixel:
                    return PixelNull;
                case ShaderStage.Vertex:
                    return VertexNull;
                default:
                    return new NullShader(type);
            }
        }
        /// <summary>
        /// Binds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Bind(DeviceContext context)
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
        public override void BindConstantBuffers(DeviceContext context)
        {

        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="index">The index.</param>
        /// <param name="texture">The texture.</param>
        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {

        }
        /// <summary>
        /// Binds the texture.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="texture">The texture.</param>
        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {

        }
        /// <summary>
        /// Binds the textures.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="textures">The textures.</param>
        public override void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {

        }
    }
}
