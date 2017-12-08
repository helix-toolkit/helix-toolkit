using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using ShaderManager;
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
        public ShaderDescription()
        {

        }
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
        public ShaderBase CreateShader(Device device, IConstantBufferPool pool)
        {
            if(ByteCode == null)
            {
                return new NullShader(ShaderType);
            }
            ShaderBase shader = null;
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
                    break;
                case ShaderStage.Hull:
                    break;
                case ShaderStage.Geometry:
                    shader = new GeometryShader(device, Name, ByteCode);
                    break;
                default:
                    throw new ArgumentException("Shader Type does not supported.");
            }
            if (ConstantBufferMappings != null)
            {
                foreach(var mapping in ConstantBufferMappings)
                {
                    shader.AddConstantBuffer(mapping.Description.Name, mapping.Slot, pool.Register(mapping.Description));
                }
            }
            if(TextureMappings != null)
            {
                foreach(var mapping in TextureMappings)
                {
                    shader.AddTextureMapping(mapping.Description.Name, mapping.Slot);
                }
            }
            return shader;
        }
    }
}
