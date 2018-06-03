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
    /// Pixel Shader
    /// </summary>
    public sealed class PixelShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.PixelShader Shader { private set; get; } = null;
        public static readonly PixelShader NullPixelShader = new PixelShader("NULL");
        /// <summary>
        /// Pixel Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public PixelShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Pixel)
        {
            Shader = Collect(new global::SharpDX.Direct3D11.PixelShader(device, byteCode));
        }

        private PixelShader(string name)
            :base(name, ShaderStage.Pixel, true)
        {

        }
    }
}
