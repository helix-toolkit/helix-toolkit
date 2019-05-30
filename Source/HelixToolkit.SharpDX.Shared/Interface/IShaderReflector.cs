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
    /// <summary>
    /// 
    /// </summary>
    public interface IShaderReflector
    {
        /// <summary>
        /// 
        /// </summary>
        FeatureLevel FeatureLevel { get; }
        /// <summary>
        /// Pass the byte code, reflect all shader buffer bindings
        /// </summary>
        /// <param name="byteCode"></param>
        /// <param name="stage"></param>
        void Parse(byte[] byteCode, ShaderStage stage);
        /// <summary>
        /// Get constant buffer mapping.
        /// </summary>
        Dictionary<string, ConstantBufferMapping> ConstantBufferMappings { get; }
        /// <summary>
        /// Get texture buffer mapping.
        /// </summary>
        Dictionary<string, TextureMapping> TextureMappings { get; }
        /// <summary>
        /// Get Unordered Access View buffer mapping.
        /// </summary>
        Dictionary<string, UAVMapping> UAVMappings { get; }
        /// <summary>
        /// Get Sampler mapping
        /// </summary>
        Dictionary<string, SamplerMapping> SamplerMappings { get; }
    }
}
