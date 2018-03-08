/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP.Shaders
#else
namespace HelixToolkit.Wpf.SharpDX.Shaders
#endif
{
    using System;
    using Utilities;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public interface IShaderPass
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsNULL { get; }
        /// <summary>
        /// Gets the shaders.
        /// </summary>
        /// <value>
        /// The shaders.
        /// </value>
        IReadOnlyList<ShaderBase> Shaders { get; }
        /// <summary>
        /// Gets the state of the blend.
        /// </summary>
        /// <value>
        /// The state of the blend.
        /// </value>
        BlendStateProxy BlendState { get; }
        /// <summary>
        /// 
        /// </summary>
        DepthStencilStateProxy DepthStencilState { get; }
        /// <summary>
        /// 
        /// </summary>
        RasterizerStateProxy RasterState { get; }
        /// <summary>
        /// Binds the shader and its constant buffers
        /// </summary>
        /// <param name="context">The context.</param>
        void BindShader(DeviceContextProxy context);
        /// <summary>
        /// Binds the shader. Optionally bind its constant buffers
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        void BindShader(DeviceContextProxy context, bool bindConstantBuffer);
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        ShaderBase GetShader(ShaderStage type);
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        void SetShader(ShaderBase shader);
        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        void BindStates(DeviceContextProxy context, StateType type);
    }
}
