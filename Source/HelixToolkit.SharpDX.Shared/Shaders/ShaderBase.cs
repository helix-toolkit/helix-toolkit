/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
        private readonly MappingCollection<int, string, UAVMapping> uavMapping = new MappingCollection<int, string, UAVMapping>();
        /// <summary>
        /// <see cref="IShader.CBufferMapping"/>
        /// </summary>
        public IEnumerable<Tuple<int, IBufferProxy>> CBufferMapping { get { return cbufferMapping.DataMapping; } }
        /// <summary>
        /// <see cref="IShader.TextureMapping"/>
        /// </summary>
        public IEnumerable<Tuple<int, TextureMapping>> TextureMapping { get { return texturesMapping.DataMapping; } }
        /// <summary>
        /// <see cref="IShader.UAVMapping"/>
        /// </summary>
        public IEnumerable<Tuple<int, UAVMapping>> UAVMapping { get { return uavMapping.DataMapping; } }
        /// <summary>
        /// <see cref="IShader.ConstantBufferCount"/>
        /// </summary>
        public int ConstantBufferCount { get { return cbufferMapping.Count; } }
        /// <summary>
        /// <see cref="IShader.TextureMappingCount"/>
        /// </summary>
        public int TextureMappingCount { get { return texturesMapping.Count; } } 
        /// <summary>
        /// <see cref="IShader.UAVMappingCount"/>
        /// </summary>
        public int UAVMappingCount { get { return uavMapping.Count; } }      
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
        /// <param name="slot">Buffer Register Index(bx) in Shader Code</param>
        /// <param name="buffer"></param>
        public void AddConstantBuffer(string name, int slot, IBufferProxy buffer)
        {
            cbufferMapping.Add(slot, name, buffer);
        }
        /// <summary>
        /// Add texture mapping. Only use to store the texture register information from shader code
        /// </summary>
        /// <param name="name"></param>
        /// <param name="slot">Texture register slot(tx) in Shader Code.</param>
        public void AddTextureMapping(string name, int slot, TextureMapping mapping)
        {
            texturesMapping.Add(slot, name, mapping);
        }
        /// <summary>
        /// <see cref="IShader.AddUAVMapping(string, int, Shaders.UAVMapping)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="slot"></param>
        /// <param name="mapping"></param>
        public void AddUAVMapping(string name, int slot, UAVMapping mapping)
        {
            uavMapping.Add(slot, name, mapping);
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
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool RemoveConstantBuffer(int slot)
        {
            return cbufferMapping.Remove(slot);
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
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool RemoveTextureMapping(int slot)
        {
            return texturesMapping.Remove(slot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveUAVMapping(string name)
        {
            return uavMapping.Remove(name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool RemoveUAVMapping(int slot)
        {
            return uavMapping.Remove(slot);
        }
        /// <summary>
        /// Try to get texture register slot in shader by its name, if failed, return -1;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int TryGetTextureIndex(string name)
        {
            int item;
            return texturesMapping.TryGetItem(name, out item) ? item : -1;
        }
        /// <summary>
        /// Try to get texture name by register slot. If failed, return empty string;
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string TryGetTextureName(int slot)
        {
            Tuple<string, TextureMapping> item;
            return texturesMapping.TryGetItem(slot, out item) ? item.Item1 : "";
        }

        /// <summary>
        /// Try to get uav register slot in shader by its name, if failed, return -1;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int TryGetUAVIndex(string name)
        {
            int item;
            return uavMapping.TryGetItem(name, out item) ? item : -1;
        }
        /// <summary>
        /// Try to get uav name by register slot. If failed, return empty string;
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string TryGetUAVName(int slot)
        {
            Tuple<string, UAVMapping> item;
            return uavMapping.TryGetItem(slot, out item) ? item.Item1 : "";
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
                return texturesMapping[name].Item2.Clone();                
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
                return texturesMapping[slot].Item2.Clone();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return a cloned uav mapping
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UAVMapping GetUAVMapping(string name)
        {
            if (uavMapping.HasItem(name))
            {
                return uavMapping[name].Item2.Clone();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Return a cloned uav mapping
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public UAVMapping GetUAVMapping(int slot)
        {
            if (uavMapping.HasItem(slot))
            {
                return uavMapping[slot].Item2.Clone();
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
        /// 
        /// </summary>
        public void ClearUAVMappings()
        {
            uavMapping.Clear();
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
        /// <param name="slot"></param>
        /// <param name="texture"></param>
        public abstract void BindTexture(DeviceContext context, int slot, ShaderResourceView texture);
        /// <summary>
        /// Bind a list of textures
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public abstract void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures);

        /// <summary>
        /// Bind specified uav resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public virtual void BindUAV(DeviceContext context, string name, UnorderedAccessView uav) { }
        /// <summary>
        ///  Bind specified uav resources.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slot"></param>
        /// <param name="texture"></param>
        public virtual void BindUAV(DeviceContext context, int slot, UnorderedAccessView uav) { }
        /// <summary>
        /// Bind a list of uav
        /// </summary>
        /// <param name="context"></param>
        /// <param name="textures"></param>
        public virtual void BindUAVs(DeviceContext context, IEnumerable<Tuple<int, UnorderedAccessView>> uavs) { }
    }
}
