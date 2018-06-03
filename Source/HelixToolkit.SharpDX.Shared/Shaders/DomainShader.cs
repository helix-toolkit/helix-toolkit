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
    public sealed class DomainShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.DomainShader Shader { private set; get; }
        public static readonly DomainShader NullDomainShader = new DomainShader("NULL");
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public DomainShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Domain)
        {
            Shader = Collect(new global::SharpDX.Direct3D11.DomainShader(device, byteCode));
        }

        private DomainShader(string name)
            :base(name, ShaderStage.Domain, true)
        {

        }
    }
}
