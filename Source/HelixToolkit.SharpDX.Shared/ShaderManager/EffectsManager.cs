/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGMEMORY
using System;
using System.Collections.Generic;
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
using System.Linq;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Render;
    using Shaders;
    using ShaderManager;
    using Core;
    using HelixToolkit.Logger;
    using System.Runtime.CompilerServices;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics;

    /// <summary>
    /// Shader and Technique manager
    /// </summary>
    public class EffectsManager : DisposeObject, IEffectsManager
    {
        private readonly LogWrapper logger;
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public LogWrapper Logger { get { return logger; } }
        /// <summary>
        /// Occurs when [on dispose resources].
        /// </summary>
        public event EventHandler<EventArgs> DisposingResources;
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        public event EventHandler<EventArgs> InvalidateRender;
        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        private const FeatureLevel MinimumFeatureLevel = FeatureLevel.Level_10_0;
        private IDictionary<string, Lazy<IRenderTechnique>> techniqueDict { get; } = new Dictionary<string, Lazy<IRenderTechnique>>();
        /// <summary>
        /// <see cref="IEffectsManager.RenderTechniques"/>
        /// </summary>
        public IEnumerable<string> RenderTechniques { get { return techniqueDict.Keys; } }

        private IConstantBufferPool constantBufferPool;
        /// <summary>
        /// <see cref="IDevice3DResources.ConstantBufferPool"/>
        /// </summary>
        public IConstantBufferPool ConstantBufferPool { get { return constantBufferPool; } }

        private IShaderPoolManager shaderPoolManager;
        /// <summary>
        /// <see cref="IEffectsManager.ShaderManager"/>
        /// </summary>
        public IShaderPoolManager ShaderManager { get { return shaderPoolManager; } }

        private IStatePoolManager statePoolManager;

        /// <summary>
        /// <see cref="IDevice3DResources.StateManager"/> 
        /// </summary>
        public IStatePoolManager StateManager { get { return statePoolManager; } }

        /// <summary>
        /// Gets the geometry buffer manager.
        /// </summary>
        /// <value>
        /// The geometry buffer manager.
        /// </value>
        public IGeometryBufferManager GeometryBufferManager { get { return geometryBufferManager; } }

        private IGeometryBufferManager geometryBufferManager;

        /// <summary>
        /// Gets the material texture manager.
        /// </summary>
        /// <value>
        /// The material texture manager.
        /// </value>
        public ITextureResourceManager MaterialTextureManager { get { return materialTextureManager; } }
        private ITextureResourceManager materialTextureManager;

        public IMaterialVariablePool MaterialVariableManager { get { return materialVariableManager; } }
        private IMaterialVariablePool materialVariableManager;
        #region 3D Resoruces

        private global::SharpDX.Direct3D11.Device device;

#if DX11_1
        private global::SharpDX.Direct3D11.Device1 device1;
        /// <summary>
        /// 
        /// </summary>
        public Device Device { get { return device1; } }
#else
        /// <summary>
        /// 
        /// </summary>
        public Device Device { get { return device; } }
#endif
        /// <summary>
        /// 
        /// </summary>
        public DriverType DriverType { private set; get; }

        private IDeviceContextPool deviceContextPool;
        /// <summary>
        /// Gets the device context pool.
        /// </summary>
        /// <value>
        /// The device context pool.
        /// </value>
        public IDeviceContextPool DeviceContextPool { get { return deviceContextPool; } }
        #endregion
        #region 2D Resources
        private global::SharpDX.Direct2D1.Device device2D;
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2 d.
        /// </value>
        public global::SharpDX.Direct2D1.Device Device2D { get { return device2D; } }


        private global::SharpDX.Direct2D1.DeviceContext deviceContext2D;
        /// <summary>
        /// Gets or sets the device2 d context.
        /// </summary>
        /// <value>
        /// The device2 d context.
        /// </value>
        public global::SharpDX.Direct2D1.DeviceContext DeviceContext2D { get { return deviceContext2D; } }
        /// <summary>
        /// Gets the factory2 d.
        /// </summary>
        /// <value>
        /// The factory2 d.
        /// </value>
        public global::SharpDX.Direct2D1.Factory1 Factory2D { get { return factory2D; } }
        private global::SharpDX.Direct2D1.Factory1 factory2D;

        private global::SharpDX.WIC.ImagingFactory wicImgFactory;
        /// <summary>
        /// Gets the wic img factory.
        /// </summary>
        /// <value>
        /// The wic img factory.
        /// </value>
        public global::SharpDX.WIC.ImagingFactory WICImgFactory { get { return wicImgFactory; } }

        private global::SharpDX.DirectWrite.Factory directWriteFactory;
        /// <summary>
        /// Gets the direct write factory.
        /// </summary>
        /// <value>
        /// The direct write factory.
        /// </value>
        public global::SharpDX.DirectWrite.Factory DirectWriteFactory { get { return directWriteFactory; } }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public int AdapterIndex { private set; get; } = -1;
        /// <summary>
        /// 
        /// </summary>
        public bool Initialized { private set; get; } = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsManager"/> class.
        /// </summary>
        public EffectsManager()
        {
#if DEBUG
            logger = new LogWrapper(new DebugLogger());
#else
            logger = new LogWrapper(new NullLogger());
#endif
            Initialize();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsManager"/> class.
        /// </summary>
        /// <param name="externallogger">The logger.</param>
        public EffectsManager(ILogger externallogger)
        {
            this.logger = externallogger == null ? new LogWrapper(new NullLogger()) : new LogWrapper(externallogger);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsManager"/> class.
        /// </summary>
        /// <param name="adapterIndex">Index of the adapter.</param>
        public EffectsManager(int adapterIndex)
        {
#if DEBUG
            logger = new LogWrapper(new DebugLogger());
#else
            logger = new LogWrapper(new NullLogger());
#endif
            Initialize(adapterIndex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectsManager"/> class.
        /// </summary>
        /// <param name="adapterIndex">Index of the adapter.</param>
        /// <param name="externallogger">The logger.</param>
        public EffectsManager(int adapterIndex, ILogger externallogger)
        {
            this.logger = externallogger == null ? new LogWrapper(new NullLogger()) : new LogWrapper(externallogger);
            Initialize(adapterIndex);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
#if DEBUGMEMORY
            global::SharpDX.Configuration.EnableObjectTracking = true;
#endif
            if (AdapterIndex == -1)
            {
                int adapterIndex = -1;
                using (var adapter = GetBestAdapter(out adapterIndex))
                {
                    Initialize(adapterIndex);
                }
            }
            else
            {
                Initialize(AdapterIndex);
            }
        }
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize(int adapterIndex)
        {
            Log(LogLevel.Information, $"Adapter Index = {adapterIndex}");
            var adapter = GetAdapter(ref adapterIndex);
            AdapterIndex = adapterIndex;
#if DX11
            if (adapter != null)
            {
                if (adapter.Description.VendorId == 0x1414 && adapter.Description.DeviceId == 0x8c)
                {
                    DriverType = DriverType.Warp;
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
                }
                else
                {
                    DriverType = DriverType.Hardware;
#if DEBUGMEMORY
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug);
#else
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
#if DX11_1
                    device1 = device.QueryInterface<global::SharpDX.Direct3D11.Device1>();
#endif
#endif
                    // DeviceCreationFlags.Debug should not be used in productive mode!
                    // See: http://sharpdx.org/forum/4-general/1774-how-to-debug-a-sharpdxexception
                    // See: http://stackoverflow.com/questions/19810462/launching-sharpdx-directx-app-with-devicecreationflags-debug
                }
            }
#else
            device = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1);
#endif
            Log(LogLevel.Information, $"Direct3D device initilized. DriverType: {DriverType}; FeatureLevel: {device.FeatureLevel}");

            #region Initial Internal Pools
            Log(LogLevel.Information, "Initializing resource pools");
            RemoveAndDispose(ref constantBufferPool);
            constantBufferPool = Collect(new ConstantBufferPool(Device, Logger));

            RemoveAndDispose(ref shaderPoolManager);
            shaderPoolManager = Collect(new ShaderPoolManager(Device, constantBufferPool, Logger));

            RemoveAndDispose(ref statePoolManager);
            statePoolManager = Collect(new StatePoolManager(Device));

            RemoveAndDispose(ref geometryBufferManager);
            geometryBufferManager = Collect(new GeometryBufferManager(this));

            RemoveAndDispose(ref materialTextureManager);
            materialTextureManager = Collect(new TextureResourceManager(Device));

            RemoveAndDispose(ref materialVariableManager);
            materialVariableManager = Collect(new MaterialVariablePool(this));

            RemoveAndDispose(ref deviceContextPool);
            deviceContextPool = Collect(new DeviceContextPool(Device));
#endregion
            Log(LogLevel.Information, "Initializing Direct2D resources");
            factory2D = Collect(new global::SharpDX.Direct2D1.Factory1(global::SharpDX.Direct2D1.FactoryType.MultiThreaded));
            wicImgFactory = Collect(new global::SharpDX.WIC.ImagingFactory());
            directWriteFactory = Collect(new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Shared));
            using (var dxgiDevice2 = device.QueryInterface<global::SharpDX.DXGI.Device>())
            {
                device2D = Collect(new global::SharpDX.Direct2D1.Device(factory2D, dxgiDevice2));
                deviceContext2D = Collect(new global::SharpDX.Direct2D1.DeviceContext(device2D, global::SharpDX.Direct2D1.DeviceContextOptions.EnableMultithreadedOptimizations));
            }
            Initialized = true;
        }

        /// <summary>
        /// <see cref="IEffectsManager.AddTechnique(TechniqueDescription)"/>
        /// </summary>
        /// <param name="description"></param>
        public void AddTechnique(TechniqueDescription description)
        {
            if (techniqueDict.ContainsKey(description.Name))
            {
                throw new ArgumentException($"Technique { description.Name } already exists.");
            }
            techniqueDict.Add(description.Name, new Lazy<IRenderTechnique>(() => { return Initialized ? Collect(new Technique(description, Device, this)) : null; }, true));
        }

        /// <summary>
        /// Determines whether the specified name has technique.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if the specified name has technique; otherwise, <c>false</c>.
        /// </returns>
        public bool HasTechnique(string name)
        {
            return techniqueDict.ContainsKey(name);
        }
        /// <summary>
        /// <see cref="IEffectsManager.RemoveTechnique(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveTechnique(string name)
        {
            if(techniqueDict.TryGetValue(name, out Lazy<IRenderTechnique> t))
            {
                if (t.IsValueCreated)
                {
                    var v = t.Value;
                    RemoveAndDispose(ref v);
                }
                return techniqueDict.Remove(name);
            }
            return false;
        }

        /// <summary>
        /// Removes all technique.
        /// </summary>
        public void RemoveAllTechniques()
        {
            var names = techniqueDict.Keys.ToArray();
            foreach(var name in names)
            {
                RemoveTechnique(name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static Adapter GetBestAdapter(out int bestAdapterIndex)
        {
            using (var f = new Factory1())
            {
                Adapter bestAdapter = null;
                bestAdapterIndex = -1;
                int adapterIndex = -1;
                ulong bestVideoMemory = 0;
                ulong bestSystemMemory = 0;

                foreach (var item in f.Adapters)
                {
                    adapterIndex++;

                    // not skip the render only WARP device
                    if (item.Description.VendorId != 0x1414 || item.Description.DeviceId != 0x8c)
                    {
                        // Windows 10 fix
                        if (item.Outputs == null || item.Outputs.Length == 0)
                        {
                            continue;
                        }
                    }

                    var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(item);

                    if (level < MinimumFeatureLevel)
                    {
                        continue;
                    }

                    ulong videoMemory = item.Description.DedicatedVideoMemory.ToUInt64();
                    ulong systemMemory = item.Description.DedicatedSystemMemory.ToUInt64();

                    if ((bestAdapter == null) || (videoMemory > bestVideoMemory) || ((videoMemory == bestVideoMemory) && (systemMemory > bestSystemMemory)))
                    {
                        bestAdapter = item;
                        bestAdapterIndex = adapterIndex;
                        bestVideoMemory = videoMemory;
                        bestSystemMemory = systemMemory;
                    }
                }

                return bestAdapter;
            }
        }

        private static Adapter GetAdapter(ref int index)
        {
            using (var f = new Factory1())
            {
                if (f.Adapters.Length <= index)
                {
                    return GetBestAdapter(out index);
                }
                else
                {
                    return f.Adapters[index];
                }
            }
        }
        /// <summary>
        /// Gets the technique.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Manager has not been initialized.</exception>
        /// <exception cref="ArgumentException"></exception>
        public IRenderTechnique GetTechnique(string name)
        {            
            Lazy<IRenderTechnique> t;
            techniqueDict.TryGetValue(name, out t);
            if (t == null)
            {
                Log(LogLevel.Warning, $"Technique {name} does not exist. Return a null technique.");
                Debug.WriteLine($"Technique {name} does not exist. Return a null technique.");
#if DX11_1
                return new Technique(new TechniqueDescription() { Name = name, IsNull = true }, device1, this);
#else
                return new Technique(new TechniqueDescription() { Name = name, IsNull = true }, device, this);
#endif
            }
            return t.Value;
        }
        /// <summary>
        /// Gets the <see cref="IRenderTechnique"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="IRenderTechnique"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IRenderTechnique this[string name]
        {
            get
            {
                return GetTechnique(name);
            }           
        }

        /// <summary>
        /// <see cref="DisposeObject.OnDispose(bool)"/>
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        [SuppressMessage("Microsoft.Usage", "CA2213", Justification = "False positive.")]
        protected override void OnDispose(bool disposeManagedResources)
        {
            DisposingResources?.Invoke(this, EventArgs.Empty);
            foreach(var technique in techniqueDict.Values.ToArray())
            {
                if (technique.IsValueCreated)
                {
                    var t = technique.Value;
                    RemoveAndDispose(ref t);
                }
            }
            techniqueDict.Clear();
            RemoveAndDispose(ref shaderPoolManager);           
            base.OnDispose(disposeManagedResources);
            Initialized = false;
            global::SharpDX.Toolkit.Graphics.WICHelper.Dispose();
#if DX11_1
            Disposer.RemoveAndDispose(ref device1);
#endif
            Disposer.RemoveAndDispose(ref device);
#if DEBUGMEMORY
            ReportResources();
#endif
        }

#region Handle Device Error        
        /// <summary>
        /// Called when [device error].
        /// </summary>
        public void OnDeviceError()
        {
            techniqueDict.Clear();
            DisposingResources?.Invoke(this, EventArgs.Empty);
            this.DisposeAndClear();
            Disposer.RemoveAndDispose(ref device);
            Initialized = false;
#if DEBUGMEMORY
            ReportResources();
#endif
            Initialize(AdapterIndex);
        }
#endregion

#if DEBUGMEMORY
        protected void ReportResources()
        {
            Console.WriteLine(global::SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
            var liveObjects = global::SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
            Console.WriteLine($"Live object count = {liveObjects.Count}");
            //if (liveObjects.Count != 0)
            //{
            //    foreach(var obj in liveObjects)
            //    {
            //        Console.WriteLine(obj.ToString());
            //    }
            //}
        }
#endif
        private void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(level, msg, nameof(EffectsManager), caller, sourceLineNumber);
        }

        public void RaiseInvalidateRender()
        {
            InvalidateRender?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Outputs the resource cout summary.
        /// </summary>
        /// <returns></returns>
        public string GetResourceCountSummary()
        {
            return $"ConstantBuffer Count: {constantBufferPool.Count}\n" +
                $"BlendState Count: {statePoolManager.BlendStatePool.Count}\n" +
                $"DepthStencilState Count: {statePoolManager.DepthStencilStatePool.Count}\n" +
                $"RasterState Count: {statePoolManager.RasterStatePool.Count}\n" +
                $"SamplerState Count: {statePoolManager.SamplerStatePool.Count}\n" +
                $"GeometryBuffer Count:{geometryBufferManager.Count}\n" +
                $"MaterialTexture Count:{materialTextureManager.Count}\n" +
                $"MaterialVariable Count:{materialVariableManager.Count}\n";
        }
    }
}
