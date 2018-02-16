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
        IEnumerable<IShader> Shaders { get; }
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
        void BindShader(IDeviceContext context);
        /// <summary>
        /// Binds the shader. Optionally bind its constant buffers
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        void BindShader(IDeviceContext context, bool bindConstantBuffer);
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IShader GetShader(ShaderStage type);
        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        void BindStates(IDeviceContext context, StateType type);
    }
}
