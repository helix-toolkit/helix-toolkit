using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    public abstract class ShaderBase : DisposeObject
    {
        private readonly MappingCollection<int, string, IBufferProxy> cbufferMapping = new MappingCollection<int, string, IBufferProxy>();
        private readonly MappingCollection<int, string, int> texturesMapping = new MappingCollection<int, string, int>();

        public IEnumerable<Tuple<int, IBufferProxy>> CBufferMapping { get { return cbufferMapping.DataMapping; } }

        public int ConstantBufferCount { get { return cbufferMapping.Count; } }
        public int TextureMappingCount { get { return texturesMapping.Count; } }       

        public ShaderStage ShaderType { private set; get; }

        public string Name { private set; get; }

        public ShaderBase(string name, ShaderStage type)
        {
            ShaderType = type;
            Name = name;
        }
        /// <summary>
        /// Add shader constant buffer
        /// </summary>
        /// <param name="name">Buffer Name in Shader Code</param>
        /// <param name="index">Buffer Register Index(bx) in Shader Code</param>
        /// <param name="buffer"></param>
        public void AddConstantBuffer(string name, int index, IBufferProxy buffer)
        {
            cbufferMapping.Add(index, name, buffer);
        }
        /// <summary>
        /// Add texture mapping. Only use to store the texture register information from shader code
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index">Texture register index(tx) in Shader Code.</param>
        public void AddTextureMapping(string name, int index)
        {
            texturesMapping.Add(index, name, index);
        }

        public bool RemoveConstantBuffer(string name)
        {
            return cbufferMapping.Remove(name);
        }

        public bool RemoveConstantBuffer(int index)
        {
            return cbufferMapping.Remove(index);
        }

        public bool RemoveTextureMapping(string name)
        {
            return texturesMapping.Remove(name);
        }

        public bool RemoveTextureMapping(int index)
        {
            return texturesMapping.Remove(index);
        }

        public int GetTextureIndex(string name)
        {
            return texturesMapping[name].Item2;
        }

        public string GetTextureName(int index)
        {
            return texturesMapping[index].Item1;
        }

        public void ClearConstantBuffer()
        {
            cbufferMapping.Clear();
        }

        public void ClearTextureMappings()
        {
            texturesMapping.Clear();
        }
        /// <summary>
        /// Bind this shader to pipeline
        /// </summary>
        /// <param name="context"></param>
        public abstract void Bind(DeviceContext context);
        /// <summary>
        /// Bind all constant buffers in this shader
        /// </summary>
        /// <param name="context"></param>
        public abstract void BindConstantBuffers(DeviceContext context);
        /// <summary>
        /// Bind specified texture resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public abstract void BindTexture(DeviceContext context, string name, ShaderResourceView texture);
        /// <summary>
        ///  Bind specified texture resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="texture"></param>
        public abstract void BindTexture(DeviceContext context, int index, ShaderResourceView texture);
        /// <summary>
        /// Bind a list of textures
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public abstract void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures);
    }
}
