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
        public ConstantBufferDescription[] ConstantBufferDescriptions { set; get; }
        [DataMember]
        public TextureDescription[] TextureDescriptions { set; get; }
        public ShaderDescription()
        {

        }
        public ShaderDescription(string name, ShaderStage type, FeatureLevel featureLevel, byte[] byteCode, 
            ConstantBufferDescription[] constantBuffers = null, TextureDescription[] textures = null)
        {
            Name = name;
            ShaderType = type;
            Level = featureLevel;
            ByteCode = byteCode;
            ConstantBufferDescriptions = constantBuffers;
            TextureDescriptions = textures;
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
                    break;
                default:
                    throw new ArgumentException("Shader Type does not supported.");
            }
            if (ConstantBufferDescriptions != null)
            {
                foreach(var bufDesc in ConstantBufferDescriptions)
                {
                    shader.AddConstantBuffer(bufDesc.Name, bufDesc.Slot, pool.Register(bufDesc));
                }
            }
            if(TextureDescriptions != null)
            {
                foreach(var texDesc in TextureDescriptions)
                {
                    shader.AddTextureMapping(texDesc.Name, texDesc.Slot);
                }
            }
            return shader;
        }
    }
}
