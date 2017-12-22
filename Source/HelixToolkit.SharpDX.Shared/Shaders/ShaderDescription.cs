/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Runtime.Serialization;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using ShaderManager;
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ShaderDescription
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType { set; get; }
        [DataMember]
        public FeatureLevel Level { set; get; }
        [DataMember]
        public byte[] ByteCode { set; get; }
        [DataMember]
        public ConstantBufferMapping[] ConstantBufferMappings { set; get; }
        [DataMember]
        public TextureMapping[] TextureMappings { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public ShaderDescription()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="featureLevel"></param>
        /// <param name="byteCode"></param>
        /// <param name="constantBuffers"></param>
        /// <param name="textures"></param>
        public ShaderDescription(string name, ShaderStage type, FeatureLevel featureLevel, byte[] byteCode,
            ConstantBufferMapping[] constantBuffers = null, TextureMapping[] textures = null)
        {
            Name = name;
            ShaderType = type;
            Level = featureLevel;
            ByteCode = byteCode;
            ConstantBufferMappings = constantBuffers;
            TextureMappings = textures;
        }
        /// <summary>
        /// Create Shader.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public IShader CreateShader(Device device, IConstantBufferPool pool)
        {
            if (ByteCode == null)
            {
                return new NullShader(ShaderType);
            }
            IShader shader = null;
            switch (ShaderType)
            {
                case ShaderStage.Vertex:
                    shader = new VertexShader(device, Name, ByteCode);
                    break;
                case ShaderStage.Pixel:
                    shader = new PixelShader(device, Name, ByteCode);
                    break;
                case ShaderStage.Compute:
                    break;
                case ShaderStage.Domain:
                    shader = new DomainShader(device, Name, ByteCode);
                    break;
                case ShaderStage.Hull:
                    shader = new HullShader(device, Name, ByteCode);
                    break;
                case ShaderStage.Geometry:
                    shader = new GeometryShader(device, Name, ByteCode);
                    break;
                default:
                    throw new ArgumentException("Shader Type does not supported.");
            }
            if (ConstantBufferMappings != null)
            {
                foreach (var mapping in ConstantBufferMappings)
                {
                    shader.AddConstantBuffer(mapping.Description.Name, mapping.Slot, pool.Register(mapping.Description));
                }
            }
            if (TextureMappings != null)
            {
                foreach (var mapping in TextureMappings)
                {
                    shader.AddTextureMapping(mapping.Description.Name, mapping.Slot, mapping);
                }
            }
            return shader;
        }

        public ShaderDescription Clone()
        {
            return new ShaderDescription(this.Name, this.ShaderType, this.Level, this.ByteCode,
                this.ConstantBufferMappings.Select(x => x.Clone()).ToArray(),
                this.TextureMappings.Select(x => x.Clone()).ToArray());
        }
    }
}
