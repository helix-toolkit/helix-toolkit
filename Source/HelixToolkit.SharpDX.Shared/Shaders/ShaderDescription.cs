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
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ShaderDescription
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { set; get; }
        /// <summary>
        /// Gets or sets the type of the shader.
        /// </summary>
        /// <value>
        /// The type of the shader.
        /// </value>
        [DataMember]
        public ShaderStage ShaderType { set; get; }
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        [DataMember]
        public FeatureLevel Level { set; get; }
        /// <summary>
        /// Gets or sets the byte code.
        /// </summary>
        /// <value>
        /// The byte code.
        /// </value>
        [DataMember]
        public byte[] ByteCode { set; get; }
        /// <summary>
        /// Gets or sets the constant buffer mappings.
        /// </summary>
        /// <value>
        /// The constant buffer mappings.
        /// </value>
        [DataMember]
        public ConstantBufferMapping[] ConstantBufferMappings { set; get; }
        /// <summary>
        /// Gets or sets the texture mappings.
        /// </summary>
        /// <value>
        /// The texture mappings.
        /// </value>
        [DataMember]
        public TextureMapping[] TextureMappings { set; get; }
        /// <summary>
        /// Gets or sets the uav mappings.
        /// </summary>
        /// <value>
        /// The uav mappings.
        /// </value>
        [DataMember]
        public UAVMapping[] UAVMappings { get; set; }
        /// <summary>
        /// Gets or sets the sampler mappings.
        /// </summary>
        /// <value>
        /// The sampler mappings.
        /// </value>
        [DataMember]
        public SamplerMapping[] SamplerMappings { set; get; }

        /// <summary>
        /// Gets or sets the shader reflector.
        /// </summary>
        /// <value>
        /// The shader reflector.
        /// </value>
        protected IShaderReflector shaderReflector { private set; get; }
        /// <summary>
        /// Create a empty description
        /// </summary>
        public ShaderDescription()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="byteCode"></param>
        protected ShaderDescription(string name, ShaderStage type, byte[] byteCode)
        {
            Name = name;
            ShaderType = type;
            ByteCode = byteCode;
        }
        /// <summary>
        /// Manually specifiy buffer mappings.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="featureLevel"></param>
        /// <param name="byteCode"></param>
        /// <param name="constantBuffers"></param>
        /// <param name="textures"></param>
        /// <param name="samplers"></param>
        public ShaderDescription(string name, ShaderStage type, FeatureLevel featureLevel, byte[] byteCode,
            ConstantBufferMapping[] constantBuffers = null, TextureMapping[] textures = null, SamplerMapping[] samplers = null)
            : this(name, type, byteCode)
        {
            Level = featureLevel;
            ConstantBufferMappings = constantBuffers;
            TextureMappings = textures;
            SamplerMappings = samplers;
        }

        /// <summary>
        /// Create shader using reflector to get buffer mapping directly from shader codes.
        /// <para>Actual creation happened when calling <see cref="CreateShader(Device, IConstantBufferPool)"/></para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="reflector"></param>
        /// <param name="byteCode"></param>
        public ShaderDescription(string name, ShaderStage type, IShaderReflector reflector, byte[] byteCode)
            : this(name, type, byteCode)
        {
            shaderReflector = reflector;
        }

        /// <summary>
        /// Create Shader.
        /// <para>All constant buffers for all shaders are created here./></para>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="pool"></param>
        /// <returns></returns>
        public ShaderBase CreateShader(Device device, IConstantBufferPool pool)
        {
            if (ByteCode == null)
            {
                return null;
            }
            if(shaderReflector != null)
            {
                shaderReflector.Parse(ByteCode, ShaderType);
                Level = shaderReflector.FeatureLevel;
                if (Level > device.FeatureLevel)
                {
                    throw new Exception($"Shader {this.Name} requires FeatureLevel {Level}. Current device only supports FeatureLevel {device.FeatureLevel} and below.");
                }
                this.ConstantBufferMappings = shaderReflector.ConstantBufferMappings.Values.ToArray();
                this.TextureMappings = shaderReflector.TextureMappings.Values.ToArray();
                this.UAVMappings = shaderReflector.UAVMappings.Values.ToArray();
                this.SamplerMappings = shaderReflector.SamplerMappings.Values.ToArray();
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
                    shader = new ComputeShader(device, Name, ByteCode);
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
                    shader = NullShader.GetNullShader(ShaderType);
                    break;
            }
            if (ConstantBufferMappings != null)
            {
                foreach (var mapping in ConstantBufferMappings)
                {
                    shader.ConstantBufferMapping.AddMapping(mapping.Description.Name, mapping.Slot, pool.Register(mapping.Description));
                }
            }
            if (TextureMappings != null)
            {
                foreach (var mapping in TextureMappings)
                {
                    shader.ShaderResourceViewMapping.AddMapping(mapping.Description.Name, mapping.Slot, mapping);
                }
            }
            if (UAVMappings != null)
            {
                foreach(var mapping in UAVMappings)
                {
                    shader.UnorderedAccessViewMapping.AddMapping(mapping.Description.Name, mapping.Slot, mapping);
                }
            }
            if (SamplerMappings != null)
            {
                foreach(var mapping in SamplerMappings)
                {
                    shader.SamplerMapping.AddMapping(mapping.Name, mapping.Slot, mapping);
                }
            }
            return shader;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public ShaderDescription Clone()
        {
            return new ShaderDescription(this.Name, this.ShaderType, this.Level, this.ByteCode,
                this.ConstantBufferMappings.Select(x => x.Clone()).ToArray(),
                this.TextureMappings.Select(x => x.Clone()).ToArray());
        }
    }
}
