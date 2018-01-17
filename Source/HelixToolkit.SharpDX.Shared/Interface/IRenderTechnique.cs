/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using ShaderManager;
    using System;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderTechnique : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        Device Device { get; }
        /// <summary>
        /// Input layout for all passes
        /// </summary>
        InputLayout Layout { get; }

        /// <summary>
        /// All shader pass names
        /// </summary>
        IEnumerable<string> ShaderPassNames { get; }

        IConstantBufferPool ConstantBufferPool { get; }
        /// <summary>
        /// Get pass by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IShaderPass GetPass(string name);
        /// <summary>
        /// Get pass by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IShaderPass GetPass(int index);

        IEffectsManager EffectsManager { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IShaderPass this[int index] { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IShaderPass this[string name] { get; }
    }
}
