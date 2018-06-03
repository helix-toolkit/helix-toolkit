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
    using Utilities;

    /// <summary>
    /// Vertex Shader
    /// </summary>
    public sealed class VertexShader : ShaderBase
    {
        internal global::SharpDX.Direct3D11.VertexShader Shader { private set; get; } = null;
        public static readonly VertexShader NullVertexShader = new VertexShader("NULL");
        /// <summary>
        /// Vertex Shader
        /// </summary>
        /// <param name="device"></param>
        /// <param name="name"></param>
        /// <param name="byteCode"></param>
        public VertexShader(Device device, string name, byte[] byteCode)
            :base(name, ShaderStage.Vertex)
        {
             Shader = Collect(new global::SharpDX.Direct3D11.VertexShader(device, byteCode));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexShader"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private VertexShader(string name)
            : base(name, ShaderStage.Vertex, true)
        {
        }
    }
}
