/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public sealed class HullShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.HullShader Shader { private set; get; }
        public static readonly HullShader NullHullShader = new HullShader("NULL");
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public HullShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Hull)
        {
            Shader = Collect(new global::SharpDX.Direct3D11.HullShader(device, byteCode));
        }

        private HullShader(string name)
            :base(name, ShaderStage.Hull, true)
        {

        }
    }
}
