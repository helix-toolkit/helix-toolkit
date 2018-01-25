/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using ShaderManager;
    using Shaders;
    using System;
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceResources : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        int AdapterIndex { get; }
        /// <summary>
        /// 
        /// </summary>
        Device Device { get; }
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2d.
        /// </value>
        global::SharpDX.Direct2D1.Device Device2D { get; }
        /// <summary>
        /// Gets the device context2d.
        /// </summary>
        /// <value>
        /// The device context2d.
        /// </value>
        global::SharpDX.Direct2D1.DeviceContext DeviceContext2D { get; }
        /// <summary>
        /// 
        /// </summary>
            DriverType DriverType { get; }

        /// <summary>
        /// Called when [device error].
        /// </summary>
        void OnDeviceError();
        /// <summary>
        /// Occurs when [on dispose resources].
        /// </summary>
        event EventHandler<bool> OnDisposeResources;
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IEffectsManager : IDeviceResources
    {

        /// <summary>
        /// 
        /// </summary>
        IConstantBufferPool ConstantBufferPool { get; }

        /// <summary>
        /// 
        /// </summary>
        IShaderPoolManager ShaderManager { get; }
        /// <summary>
        /// Get list of existing technique names
        /// </summary>
        IEnumerable<string> RenderTechniques { get; }
        /// <summary>
        /// 
        /// </summary>
        IStatePoolManager StateManager { get; }

        /// <summary>
        /// Gets the geometry buffer manager.
        /// </summary>
        /// <value>
        /// The geometry buffer manager.
        /// </value>
        IGeometryBufferManager GeometryBufferManager { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IRenderTechnique GetTechnique(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IRenderTechnique this[string name] { get; }

        /// <summary>
        /// Add a technique by description
        /// </summary>
        /// <param name="description"></param>
        void AddTechnique(TechniqueDescription description);
        /// <summary>
        /// Remove a technique by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RemoveTechnique(string name);
    }
}
