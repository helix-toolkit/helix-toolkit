using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public abstract class ShaderBase : DisposeObject, IShader
    {
        private readonly MappingCollection<int, string, IBufferProxy> cbufferMapping = new MappingCollection<int, string, IBufferProxy>();
        private readonly MappingCollection<int, string, TextureMapping> texturesMapping = new MappingCollection<int, string, TextureMapping>();
        /// <summary>
        /// Constant buffer mapping
        /// </summary>
        public IEnumerable<Tuple<int, IBufferProxy>> CBufferMapping { get { return cbufferMapping.DataMapping; } }

        public IEnumerable<Tuple<int, TextureMapping>> TextureMapping { get { return texturesMapping.DataMapping; } }
        /// <summary>
        /// 
        /// </summary>
        public int ConstantBufferCount { get { return cbufferMapping.Count; } }
        /// <summary>
        /// 
        /// </summary>
        public int TextureMappingCount { get { return texturesMapping.Count; } }       
        /// <summary>
        /// Bind Stage
        /// </summary>
        public ShaderStage ShaderType { private set; get; }
        /// <summary>
        /// Shader Name
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
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
        public void AddTextureMapping(string name, int index, TextureMapping mapping)
        {
            texturesMapping.Add(index, name, mapping);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveConstantBuffer(string name)
        {
            return cbufferMapping.Remove(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveConstantBuffer(int index)
        {
            return cbufferMapping.Remove(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveTextureMapping(string name)
        {
            return texturesMapping.Remove(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveTextureMapping(int index)
        {
            return texturesMapping.Remove(index);
        }
        /// <summary>
        /// Try to get texture register index in shader by its name, if failed, return -1;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int TryGetTextureIndex(string name)
        {
            int item;
            return texturesMapping.TryGetItem(name, out item) ? item : -1;
        }
        /// <summary>
        /// Try to get texture name by register index. If failed, return empty string;
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string TryGetTextureName(int index)
        {
            Tuple<string, TextureMapping> item;
            return texturesMapping.TryGetItem(index, out item) ? item.Item1 : "";
        }
        /// <summary>
        /// Return a cloned texture mapping
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TextureMapping GetTextureMapping(string name)
        {
            if (texturesMapping.HasItem(name))
            {
                return (TextureMapping)texturesMapping[name].Item2.Clone();                
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Return a cloned texture mapping
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public TextureMapping GetTextureMapping(int slot)
        {
            if (texturesMapping.HasItem(slot))
            {
                return (TextureMapping)texturesMapping[slot].Item2.Clone();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearConstantBuffer()
        {
            cbufferMapping.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
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
