/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using HelixToolkit.Logger;
    using ShaderManager;
    using Shaders;
    using System;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public interface IDevice2DResources
    {
        /// <summary>
        /// Gets the factory2 d.
        /// </summary>
        /// <value>
        /// The factory2 d.
        /// </value>
        global::SharpDX.Direct2D1.Factory1 Factory2D { get; }
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
        /// Gets the wic img factory.
        /// </summary>
        /// <value>
        /// The wic img factory.
        /// </value>
        global::SharpDX.WIC.ImagingFactory WICImgFactory { get; }
        /// <summary>
        /// Gets the direct write factory.
        /// </summary>
        /// <value>
        /// The direct write factory.
        /// </value>
        global::SharpDX.DirectWrite.Factory DirectWriteFactory { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IDevice3DResources
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
        /// 
        /// </summary>
        DriverType DriverType { get; }
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
        /// Gets the material texture manager.
        /// </summary>
        /// <value>
        /// The material texture manager.
        /// </value>
        ITextureResourceManager MaterialTextureManager { get; }
        /// <summary>
        /// Gets the material variable manager.
        /// </summary>
        /// <value>
        /// The material variable manager.
        /// </value>
        IMaterialVariablePool MaterialVariableManager { get; }
        /// <summary>
        /// 
        /// </summary>
        IConstantBufferPool ConstantBufferPool { get; }
        /// <summary>
        /// Gets the device context pool.
        /// </summary>
        /// <value>
        /// The device context pool.
        /// </value>
        IDeviceContextPool DeviceContextPool { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceResources : IDevice3DResources, IDevice2DResources, IDisposable
    {
        /// <summary>
        /// Called when [device error].
        /// </summary>
        void OnDeviceError();
        /// <summary>
        /// Occurs when [on dispose resources].
        /// </summary>
        event EventHandler<EventArgs> DisposingResources;
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IEffectsManager : IDeviceResources
    {
        /// <summary>
        /// 
        /// </summary>
        LogWrapper Logger { get; }
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
        /// <summary>
        /// Removes all techniques.
        /// </summary>
        void RemoveAllTechniques();
        /// <summary>
        /// Determines whether the specified name has technique.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if the specified name has technique; otherwise, <c>false</c>.
        /// </returns>
        bool HasTechnique(string name);
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        event EventHandler<EventArgs> InvalidateRender;
        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
        void RaiseInvalidateRender();
        /// <summary>
        /// Outputs the resource count summary.
        /// </summary>
        /// <returns></returns>
        string GetResourceCountSummary();
    }
}
