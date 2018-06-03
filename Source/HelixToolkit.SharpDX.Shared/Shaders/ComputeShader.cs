/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ComputeShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.ComputeShader Shader { private set; get; }
        public static readonly ComputeShader NullComputeShader = new ComputeShader("NULL");
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public ComputeShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Compute)
        {
            Shader = Collect(new global::SharpDX.Direct3D11.ComputeShader(device, byteCode));
        }

        private ComputeShader(string name)
            :base(name, ShaderStage.Compute, true)
        {

        }
    }
}
