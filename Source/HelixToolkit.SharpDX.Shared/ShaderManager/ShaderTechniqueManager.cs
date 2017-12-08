using System;
using System.Collections.Generic;
using System.Text;

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
        private IDictionary<string, Lazy<Technique>> techniqueDict { get; } = new Dictionary<string, Lazy<Technique>>();

        public IConstantBufferPool ConstantBufferPool { get { return constantBufferPool; } }
        private IConstantBufferPool constantBufferPool;

        private IShaderPoolManager shaderPoolManager;
        public IShaderPoolManager ShaderManager { get { return shaderPoolManager; } }

        private IStatePoolManager statePoolManager;
        public IStatePoolManager StateManager { get { return statePoolManager; } }

        public global::SharpDX.Direct3D11.Device Device { private set; get; }

        public DriverType DriverType { private set; get; }

        public int AdapterIndex { private set; get; }

        public bool Initialized { private set; get; } = false;

        public ShaderTechniqueManager()
        {
        }

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
            var techniques = LoadTechniques(Device);
            foreach(var tech in techniques)
            {
                techniqueDict.Add(tech.Item1, tech.Item2);
            }
            #endregion
            Initialized = true;
        }

        protected IList<Tuple<string, Lazy<Technique>>> LoadTechniques(global::SharpDX.Direct3D11.Device device)
        {
            var techniqueDescs = LoadTechniqueDescriptions();
            if(techniqueDescs == null)
            {
                return new Tuple<string, Lazy<Technique>>[0];
            }
            else
            {
                var list = new List<Tuple<string, Lazy<Technique>>>(techniqueDescs.Count);
                foreach(var desc in techniqueDescs)
                {
                    list.Add(Tuple.Create(desc.Name, new Lazy<Technique>(()=> 
                    {
                        return Initialized ? Collect(new Technique(desc, device, this)) : null;
                    }, 
                    true)));
                }
                return list;
            }
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

        public Technique GetTechnique(string name)
        {
            return techniqueDict[name].Value;
        }

        public Technique this[string name]
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
        }
    }

    public class DefaultShaderTechniqueManager : ShaderTechniqueManager
    {
        protected override IList<TechniqueDescription> LoadTechniqueDescriptions()
        {
            var renderBlinn = new TechniqueDescription()
            {
                Name = DefaultRenderTechniqueNames.Blinn,
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                ShaderList = new[]
                {
                    DefaultVSShaderDescriptions.VSMeshDefault,
                    DefaultPSShaderDescriptions.PSMeshBlinnPhong
                },
                BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
            };

            var renderBlinnInstancing = new TechniqueDescription()
            {
                Name = DefaultRenderTechniqueNames.InstancingBlinn,
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshInstancing, DefaultInputLayout.VSInputInstancing),
                ShaderList = new[]
                {
                    DefaultVSShaderDescriptions.VSMeshInstancing,
                    DefaultPSShaderDescriptions.PSMeshBlinnPhong
                },
                BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
            };

            var renderBoneSkinning = new TechniqueDescription()
            {
                Name = DefaultRenderTechniqueNames.BoneSkinBlinn,
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBoneSkinning, DefaultInputLayout.VSInputBoneSkinning),
                ShaderList = new[]
                {
                    DefaultVSShaderDescriptions.VSMeshBoneSkinning,
                    DefaultPSShaderDescriptions.PSMeshBlinnPhong
                },
                BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
            };

            return new List<TechniqueDescription>{ renderBlinn, renderBlinnInstancing, renderBoneSkinning };
        }
    }
}
