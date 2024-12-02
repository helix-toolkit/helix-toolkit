﻿using HelixToolkit.Logger;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using Microsoft.Extensions.Logging;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Diagnostics.CodeAnalysis;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Shader and Technique manager
/// </summary>
public class EffectsManager : DisposeObject, IEffectsManager
{
    private static readonly ILogger logger = LogManager.Create<EffectsManager>();

    /// <summary>
    /// Occurs when [on dispose resources].
    /// </summary>
    public event EventHandler<EventArgs>? DisposingResources;
    /// <summary>
    /// Occurs when [device created].
    /// </summary>
    public event EventHandler<EventArgs>? Reinitialized;
    /// <summary>
    /// Occurs when [on invalidate renderer].
    /// </summary>
    public event EventHandler<EventArgs>? InvalidateRender;
    /// <summary>
    /// The minimum supported feature level.
    /// </summary>
    private const FeatureLevel MinimumFeatureLevel = FeatureLevel.Level_10_0;
    private readonly Dictionary<string, Lazy<IRenderTechnique?>> techniqueDict = new();
    private readonly Dictionary<string, TechniqueDescription> techniqueDescriptions = new();
    /// <summary>
    /// <see cref="IEffectsManager.RenderTechniques"/>
    /// </summary>
    public IEnumerable<string> RenderTechniques
    {
        get
        {
            return techniqueDict.Keys;
        }
    }

    private IConstantBufferPool? constantBufferPool;
    /// <summary>
    /// <see cref="IDevice3DResources.ConstantBufferPool"/>
    /// </summary>
    public IConstantBufferPool? ConstantBufferPool
    {
        get
        {
            return constantBufferPool;
        }
    }

    private IShaderPoolManager? shaderPoolManager;
    /// <summary>
    /// <see cref="IEffectsManager.ShaderManager"/>
    /// </summary>
    public IShaderPoolManager? ShaderManager
    {
        get
        {
            return shaderPoolManager;
        }
    }

    private IStatePoolManager? statePoolManager;

    /// <summary>
    /// <see cref="IDevice3DResources.StateManager"/> 
    /// </summary>
    public IStatePoolManager? StateManager
    {
        get
        {
            return statePoolManager;
        }
    }

    /// <summary>
    /// Gets the geometry buffer manager.
    /// </summary>
    /// <value>
    /// The geometry buffer manager.
    /// </value>
    public IGeometryBufferManager? GeometryBufferManager
    {
        get
        {
            return geometryBufferManager;
        }
    }

    private IGeometryBufferManager? geometryBufferManager;

    /// <summary>
    /// Gets the material texture manager.
    /// </summary>
    /// <value>
    /// The material texture manager.
    /// </value>
    public ITextureResourceManager? MaterialTextureManager
    {
        get
        {
            return materialTextureManager;
        }
    }
    private ITextureResourceManager? materialTextureManager;

    public IMaterialVariablePool? MaterialVariableManager
    {
        get
        {
            return materialVariableManager;
        }
    }
    private IMaterialVariablePool? materialVariableManager;

    public IStructArrayPool? StructArrayPool => structArrayPool;
    private StructArrayPool? structArrayPool;

    #region 3D Resoruces

    private global::SharpDX.Direct3D11.Device? device;

    private global::SharpDX.Direct3D11.Device1? device1;
    /// <summary>
    /// 
    /// </summary>
    public Device? Device { get { return device1; } }

    /// <summary>
    /// 
    /// </summary>
    public DriverType DriverType
    {
        private set; get;
    }

    private IDeviceContextPool? deviceContextPool;
    /// <summary>
    /// Gets the device context pool.
    /// </summary>
    /// <value>
    /// The device context pool.
    /// </value>
    public IDeviceContextPool? DeviceContextPool
    {
        get
        {
            return deviceContextPool;
        }
    }
    #endregion
    #region 2D Resources
    private global::SharpDX.Direct2D1.Device? device2D;
    /// <summary>
    /// Gets the device2d.
    /// </summary>
    /// <value>
    /// The device2 d.
    /// </value>
    public global::SharpDX.Direct2D1.Device? Device2D
    {
        get
        {
            return device2D;
        }
    }

    private global::SharpDX.Direct2D1.DeviceContext? deviceContext2D;
    /// <summary>
    /// Gets or sets the device2 d context.
    /// </summary>
    /// <value>
    /// The device2 d context.
    /// </value>
    public global::SharpDX.Direct2D1.DeviceContext? DeviceContext2D
    {
        get
        {
            return deviceContext2D;
        }
    }
    /// <summary>
    /// Gets the factory2 d.
    /// </summary>
    /// <value>
    /// The factory2 d.
    /// </value>
    public global::SharpDX.Direct2D1.Factory1? Factory2D
    {
        get
        {
            return factory2D;
        }
    }
    private global::SharpDX.Direct2D1.Factory1? factory2D;

    private global::SharpDX.WIC.ImagingFactory? wicImgFactory;
    /// <summary>
    /// Gets the wic img factory.
    /// </summary>
    /// <value>
    /// The wic img factory.
    /// </value>
    public global::SharpDX.WIC.ImagingFactory? WICImgFactory
    {
        get
        {
            return wicImgFactory;
        }
    }

    private global::SharpDX.DirectWrite.Factory? directWriteFactory;
    /// <summary>
    /// Gets the direct write factory.
    /// </summary>
    /// <value>
    /// The direct write factory.
    /// </value>
    public global::SharpDX.DirectWrite.Factory? DirectWriteFactory
    {
        get
        {
            return directWriteFactory;
        }
    }
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
    /// 
    /// </summary>
    public bool EnableSoftwareRendering
    {
        get;
    } = false;
    /// <summary>
    /// Initializes a new instance of the <see cref="EffectsManager"/> class.
    /// </summary>
    public EffectsManager()
        : this(new EffectsManagerConfiguration())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EffectsManager"/> class.
    /// </summary>
    /// <param name="adapterIndex">Index of the adapter.</param>
    public EffectsManager(int adapterIndex)
         : this(new EffectsManagerConfiguration()
         {
             AdapterIndex = adapterIndex
         })
    {
    }

    public EffectsManager(EffectsManagerConfiguration configuration)
    {
        EnableSoftwareRendering = configuration.EnableSoftwareRendering;
        Initialize(configuration.AdapterIndex);
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
            var adapterIndex = -1;
            GetBestAdapter(out adapterIndex);
            Initialize(adapterIndex);
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
        logger.LogInformation("Adapter Index = {0}", adapterIndex);
        var adapter = GetAdapter(ref adapterIndex);
        AdapterIndex = adapterIndex;
        if (AdapterIndex < 0 || adapter == null)
        {
            throw new PlatformNotSupportedException("Graphic adapter does not meet minimum requirement, must support DirectX 10 or above.");
        }

        if (adapter != null)
        {
            DriverType = EnableSoftwareRendering ? DriverType.Warp : DriverType.Hardware;
            if (adapter.Description.VendorId == 0x1414 && adapter.Description.DeviceId == 0x8c)
            {
                DriverType = DriverType.Warp;
                device = EnableSoftwareRendering ?
                    new global::SharpDX.Direct3D11.Device(DriverType, DeviceCreationFlags.BgraSupport)
                    : new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
            }
            else
            {
                if (DriverType == DriverType.Warp)
                {
#if DEBUGMEMORY
                    device = new global::SharpDX.Direct3D11.Device(DriverType, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug);
#else
                    device = new global::SharpDX.Direct3D11.Device(DriverType, DeviceCreationFlags.BgraSupport);
#endif
                }
                else
                {
#if DEBUGMEMORY
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug);
#else
                    device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
#endif
                    // DeviceCreationFlags.Debug should not be used in productive mode!
                    // See: http://sharpdx.org/forum/4-general/1774-how-to-debug-a-sharpdxexception
                    // See: http://stackoverflow.com/questions/19810462/launching-sharpdx-directx-app-with-devicecreationflags-debug                    
                }
            }

            device1 = device.QueryInterface<global::SharpDX.Direct3D11.Device1>();
        }

        RemoveAndDispose(ref adapter);

        logger.LogInformation("Direct3D device initilized. DriverType: {0}; FeatureLevel: {1}", DriverType, device!.FeatureLevel);

        #region Initial Internal Pools
        logger.LogInformation("Initializing resource pools");
        RemoveAndDispose(ref constantBufferPool);
        constantBufferPool = new ConstantBufferPool(Device!);

        RemoveAndDispose(ref shaderPoolManager);
        shaderPoolManager = new ShaderPoolManager(Device!, constantBufferPool);

        RemoveAndDispose(ref statePoolManager);
        statePoolManager = new StatePoolManager(Device!);

        RemoveAndDispose(ref geometryBufferManager);
        geometryBufferManager = new GeometryBufferManager(this);

        RemoveAndDispose(ref materialTextureManager);
        materialTextureManager = new TextureResourceManager(Device!);

        RemoveAndDispose(ref materialVariableManager);
        materialVariableManager = new MaterialVariablePool(this);

        RemoveAndDispose(ref deviceContextPool);
        deviceContextPool = new DeviceContextPool(Device!);

        RemoveAndDispose(ref structArrayPool);
        structArrayPool = new StructArrayPool();
        #endregion
        logger.LogInformation("Initializing Direct2D resources");
        factory2D = new global::SharpDX.Direct2D1.Factory1(global::SharpDX.Direct2D1.FactoryType.MultiThreaded);
        wicImgFactory = new global::SharpDX.WIC.ImagingFactory();
        directWriteFactory = new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Shared);
        using (var dxgiDevice2 = device.QueryInterface<global::SharpDX.DXGI.Device>())
        {
            device2D = new global::SharpDX.Direct2D1.Device(factory2D, dxgiDevice2);
            deviceContext2D = new global::SharpDX.Direct2D1.DeviceContext(device2D,
                global::SharpDX.Direct2D1.DeviceContextOptions.EnableMultithreadedOptimizations);
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
            throw new ArgumentException($"Technique {description.Name} already exists.");
        }
        techniqueDescriptions.Add(description.Name, description);
        techniqueDict.Add(description.Name, new Lazy<IRenderTechnique?>(() =>
        {
            return Initialized ? new Technique(description, Device!, this) : null;
        }, true));
    }

    /// <summary>
    /// Reinitializes all resources after calling <see cref="DisposeAllResources"/>.
    /// </summary>
    public void Reinitialize()
    {
        if (!Initialized)
        {
            Initialize();
            foreach (var tech in techniqueDescriptions.Values)
            {
                techniqueDict.Add(tech.Name, new Lazy<IRenderTechnique?>(() => { return Initialized ? new Technique(tech, Device, this) : null; }, true));
            }
            Reinitialized?.Invoke(this, EventArgs.Empty);
        }
    }
    /// <summary>
    /// Disposes all resources. This is used to handle such as DeviceLost or DeviceRemoved Error
    /// </summary>
    public void DisposeAllResources()
    {
        if (Initialized)
        {
            DisposeResources();
        }
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
        if (techniqueDict.TryGetValue(name, out var t))
        {
            if (t.IsValueCreated)
            {
                var v = t.Value;
                RemoveAndDispose(ref v);
            }
            techniqueDescriptions.Remove(name);
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
        foreach (var name in names)
        {
            RemoveTechnique(name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void GetBestAdapter(out int bestAdapterIndex)
    {
        using var f = new Factory1();
        bestAdapterIndex = -1;
        ulong bestVideoMemory = 0;
        ulong bestSystemMemory = 0;
        Adapter[] adapters = f.Adapters;
        logger.LogInformation("Trying to get best adapter. Number of adapters: {0}", adapters.Length);
        ulong MByte = 1024 * 1024;
        for (int adapterIndex = 0; adapterIndex < adapters.Length; adapterIndex++)
        {
            Adapter item = adapters[adapterIndex];

            Output[] outputs = item.Outputs;
            int outputsLength = outputs.Length;
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i]?.Dispose();
            }

            logger.LogInformation($"Adapter {adapterIndex}: Description: {item.Description.Description}; " +
                $"VendorId: {item.Description.VendorId}; " +
                $"Video Mem: {item.Description.DedicatedVideoMemory.ToUInt64() / MByte} MB; " +
                $"System Mem: {item.Description.DedicatedSystemMemory.ToUInt64() / MByte} MB; " +
                $"Shared Mem: {item.Description.SharedSystemMemory.ToUInt64() / MByte} MB; " +
                $"Num Outputs: {outputsLength}");
            // not skip the render only WARP device
            if (item.Description.VendorId != 0x1414 || item.Description.DeviceId != 0x8c)
            {
                // Windows 10 fix
                if (outputsLength == 0)
                {
                    continue;
                }
            }

            var level = global::SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(item);
            logger.LogInformation("Feature Level: {0}", level);
            if (level < MinimumFeatureLevel)
            {
                continue;
            }

            var videoMemory = item.Description.DedicatedVideoMemory.ToUInt64();
            var systemMemory = item.Description.DedicatedSystemMemory.ToUInt64();

            if ((bestAdapterIndex == -1) || (videoMemory > bestVideoMemory) || ((videoMemory == bestVideoMemory) && (systemMemory > bestSystemMemory)))
            {
                bestAdapterIndex = adapterIndex;
                bestVideoMemory = videoMemory;
                bestSystemMemory = systemMemory;
            }
        }

        for (int adapterIndex = 0; adapterIndex < adapters.Length; adapterIndex++)
        {
            adapters[adapterIndex]?.Dispose();
        }

        logger.LogInformation("Best Adapter: {0}", bestAdapterIndex);
    }

    private Adapter? GetAdapter(ref int index)
    {
        using var f = new Factory1();
        Adapter[] adapters = f.Adapters;
        if (adapters.Length <= index || index < 0)
        {
            GetBestAdapter(out index);
        }

        Adapter? adapter = index == -1 ? null : adapters[index];

        for (int adapterIndex = 0; adapterIndex < adapters.Length; adapterIndex++)
        {
            if (adapterIndex == index)
            {
                continue;
            }

            adapters[adapterIndex]?.Dispose();
        }

        return adapter;
    }

    /// <summary>
    /// Gets the technique.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    /// <exception cref="Exception">Manager has not been initialized.</exception>
    /// <exception cref="ArgumentException"></exception>
    public IRenderTechnique? GetTechnique(string name)
    {
        if (!techniqueDict.TryGetValue(name, out var t))
        {
            logger.LogWarning("Technique {0} does not exist. Return a null technique.", name);
            return new Technique(new TechniqueDescription() { Name = name, IsNull = true }, device1, this);
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
    public IRenderTechnique? this[string name]
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
        DisposeResources();
        Initialized = false;
        base.OnDispose(disposeManagedResources);
#if DEBUGMEMORY
        ReportResources();
#endif
    }

    private void DisposeResources()
    {
        DisposingResources?.Invoke(this, EventArgs.Empty);
        foreach (var technique in techniqueDict.Values.ToArray())
        {
            if (technique.IsValueCreated)
            {
                var t = technique.Value;
                RemoveAndDispose(ref t);
            }
        }
        techniqueDict.Clear();
        RemoveAndDispose(ref geometryBufferManager);
        RemoveAndDispose(ref materialTextureManager);
        RemoveAndDispose(ref materialVariableManager);
        RemoveAndDispose(ref directWriteFactory);
        RemoveAndDispose(ref shaderPoolManager);
        RemoveAndDispose(ref constantBufferPool);
        RemoveAndDispose(ref statePoolManager);
        RemoveAndDispose(ref deviceContextPool);
        RemoveAndDispose(ref deviceContext2D);
        RemoveAndDispose(ref device2D);
        RemoveAndDispose(ref factory2D);
        RemoveAndDispose(ref wicImgFactory);
        RemoveAndDispose(ref structArrayPool);
        Initialized = false;
        global::SharpDX.Toolkit.Graphics.WICHelper.Dispose();
        RemoveAndDispose(ref device1);
        RemoveAndDispose(ref device);
#if DEBUGMEMORY
        ReportResources();
#endif
    }

#if DEBUGMEMORY
        protected void ReportResources()
        {
            logger.LogDebug(global::SharpDX.Diagnostics.ObjectTracker.ReportActiveObjects());
            var liveObjects = global::SharpDX.Diagnostics.ObjectTracker.FindActiveObjects();
            logger.LogDebug("Live object count = {0}", liveObjects.Count);
            //if (liveObjects.Count != 0)
            //{
            //    foreach(var obj in liveObjects)
            //    {
            //        logger.LogDebug(obj.ToString());
            //    }
            //}
        }
#endif

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
        return $"ConstantBuffer Count: {constantBufferPool?.Count}\n" +
            $"BlendState Count: {statePoolManager?.BlendStatePool.Count}\n" +
            $"DepthStencilState Count: {statePoolManager?.DepthStencilStatePool.Count}\n" +
            $"RasterState Count: {statePoolManager?.RasterStatePool.Count}\n" +
            $"SamplerState Count: {statePoolManager?.SamplerStatePool.Count}\n" +
            $"GeometryBuffer Count:{geometryBufferManager?.Count}\n" +
            $"MaterialTexture Count:{materialTextureManager?.Count}\n" +
            $"MaterialVariable Count:{materialVariableManager?.Count}\n";
    }
}
