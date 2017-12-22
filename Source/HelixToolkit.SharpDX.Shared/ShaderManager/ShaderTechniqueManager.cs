using System;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;
    using Shaders;
    using ShaderManager;
    /// <summary>
    /// Shader and Technique manager
    /// </summary>
    public abstract class ShaderTechniqueManager : DisposeObject, IEffectsManager
    {       
        /// <summary>
        /// The minimum supported feature level.
        /// </summary>
        private const FeatureLevel MinimumFeatureLevel = FeatureLevel.Level_10_0;
        private IDictionary<string, Lazy<IRenderTechnique>> techniqueDict { get; } = new Dictionary<string, Lazy<IRenderTechnique>>();
        /// <summary>
        /// <see cref="IEffectsManager.RenderTechniques"/>
        /// </summary>
        public IEnumerable<string> RenderTechniques { get { return techniqueDict.Keys; } }

        /// <summary>
        /// <see cref="IEffectsManager.ConstantBufferPool"/>
        /// </summary>
        public IConstantBufferPool ConstantBufferPool { get { return constantBufferPool; } }
        private IConstantBufferPool constantBufferPool;

        private IShaderPoolManager shaderPoolManager;
        /// <summary>
        /// <see cref="IEffectsManager.ShaderManager"/>
        /// </summary>
        public IShaderPoolManager ShaderManager { get { return shaderPoolManager; } }

        private IStatePoolManager statePoolManager;

        /// <summary>
        /// <see cref="IEffectsManager.StateManager"/> 
        /// </summary>
        public IStatePoolManager StateManager { get { return statePoolManager; } }
        /// <summary>
        /// 
        /// </summary>
        public global::SharpDX.Direct3D11.Device Device { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DriverType DriverType { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int AdapterIndex { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public bool Initialized { private set; get; } = false;

        public void Initialize()
        {
            if (Initialized)
            { return; }
            int adapterIndex;
#if DX11
            var adapter = GetBestAdapter(out adapterIndex);

            if (adapter != null)
            {
                if (adapter.Description.VendorId == 0x1414 && adapter.Description.DeviceId == 0x8c)
                {
                    DriverType = DriverType.Warp;
                    Device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
                }
                else
                {
                    DriverType = DriverType.Hardware;
                    Device = new global::SharpDX.Direct3D11.Device(adapter, DeviceCreationFlags.BgraSupport);
                    // DeviceCreationFlags.Debug should not be used in productive mode!
                    // See: http://sharpdx.org/forum/4-general/1774-how-to-debug-a-sharpdxexception
                    // See: http://stackoverflow.com/questions/19810462/launching-sharpdx-directx-app-with-devicecreationflags-debug
                }
            }
#else
            Device = new global::SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_1);
#endif
            AdapterIndex = adapterIndex;
            #region Initial Internal Pools
            RemoveAndDispose(ref constantBufferPool);
            constantBufferPool = Collect(new ConstantBufferPool(Device));

            RemoveAndDispose(ref shaderPoolManager);
            shaderPoolManager = Collect(new ShaderPoolManager(Device, constantBufferPool));

            RemoveAndDispose(ref statePoolManager);
            statePoolManager = Collect(new StatePoolManager(Device));
            #endregion
            #region Initial Techniques
            var techniqueDescs = LoadTechniqueDescriptions();
            foreach(var tech in techniqueDescs)
            {
                AddTechnique(tech);
            }
            #endregion
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
        /// <see cref="IEffectsManager.RemoveTechnique(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveTechnique(string name)
        {
            Lazy<IRenderTechnique> t;
            techniqueDict.TryGetValue(name, out t);
            if (t != null && t.IsValueCreated)
            {
                var v = t.Value;
                RemoveAndDispose(ref v);
            }
            return techniqueDict.Remove(name);
        }

        protected abstract IList<TechniqueDescription> LoadTechniqueDescriptions();

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

        public IRenderTechnique GetTechnique(string name)
        {
            if (!Initialized)
            {
                throw new Exception("Manager has not been initialized.");
            }
            Lazy<IRenderTechnique> t;
            techniqueDict.TryGetValue(name, out t);
            if (t == null)
            {
                throw new ArgumentException($"Technique {name} does not exist.");
            }
            return t.Value;
        }

        public IRenderTechnique this[string name]
        {
            get
            {
                return GetTechnique(name);
            }           
        }

        /// <summary>
        /// <see cref="DisposeObject.Dispose(bool)"/>
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            techniqueDict.Clear();
            base.Dispose(disposeManagedResources);
            Initialized = false;
            Device?.Dispose();
        }
    }
}
