using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public sealed class BufferMappingDesc
    {
        public string Name { private set; get; }
        public int Slot { private set; get; }
        public int Size { private set; get; } = 0;
        public BufferMappingDesc(string name, int slot, int size = 0)
        {
            Name = name;
            Slot = slot;
            Size = size;
        }
    }
    public interface IShaderReflector
    {
        FeatureLevel FeatureLevel { get; }

        void Parse(byte[] byteCode, ShaderStage stage);
        /// <summary>
        /// Get constant buffer mapping. <para>Key: Name; Item1: Slot; Item2: Size</para>
        /// </summary>
        IDictionary<string, ConstantBufferMapping> ConstantBufferMappings { get; }
        /// <summary>
        /// Get constant buffer mapping. <para>Key: Name; Value: Slot;</para>
        /// </summary>
        IDictionary<string, TextureMapping> TextureMappings { get; }      
    }
}
