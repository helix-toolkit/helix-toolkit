/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP.Shaders
#else
namespace HelixToolkit.Wpf.SharpDX.Shaders
#endif
{
    using System;
    using Utilities;
    public interface IShaderPass : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsNULL { get; }
        IEnumerable<IShader> Shaders { get; }
        BlendStateProxy BlendState { get; }
        /// <summary>
        /// 
        /// </summary>
        DepthStencilStateProxy DepthStencilState { get; }
        /// <summary>
        /// 
        /// </summary>
        RasterizerStateProxy RasterState { get; }

        void BindShader(DeviceContext context);

        IShader GetShader(ShaderStage type);

        void BindStates(DeviceContext context, StateType type);
    }
}
