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
        public IConstantBufferPool ConstantBufferPool { private set; get; }

        public IDictionary<string, Technique> Techniques { get; } = new Dictionary<string, Technique>();

        public global::SharpDX.Direct3D11.Device Device { private set; get; }

        public DriverType DriverType { private set; get; }

        public int AdapterIndex { private set; get; }

        public ShaderTechniqueManager()
        {
            ConstantBufferPool = Collect(new ConstantBufferPool());
        }

        public void Initialize()
        {
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
            var techniques = LoadTechniques(Device, ConstantBufferPool);
            foreach(var tech in techniques)
            {
                Techniques.Add(tech.Name, Collect(tech));
            }
        }

        protected abstract IList<Technique> LoadTechniques(global::SharpDX.Direct3D11.Device device, IConstantBufferPool bufferPool);

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
        /// <summary>
        /// <see cref="DisposeObject.Dispose(bool)"/>
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected override void Dispose(bool disposeManagedResources)
        {
            Techniques.Clear();
            base.Dispose(disposeManagedResources);
        }
    }

    public class DefaultShaderTechniqueManager : ShaderTechniqueManager
    {
        protected override IList<Technique> LoadTechniques(global::SharpDX.Direct3D11.Device device, IConstantBufferPool bufferPool)
        {
            var renderBlinn = new Technique(DefaultRenderTechniqueNames.Blinn, device, DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput,
                new[]
                {
                    DefaultVSShaderDescriptions.VSMeshDefault,
                    DefaultPSShaderDescriptions.PSMeshBlinnPhong
                }, bufferPool);

            var renderBlinnInstancing = new Technique(DefaultRenderTechniqueNames.InstancingBlinn, device, DefaultVSShaderByteCodes.VSMeshInstancing, DefaultInputLayout.VSInputInstancing,
                new[]
                {
                    DefaultVSShaderDescriptions.VSMeshInstancing,
                    DefaultPSShaderDescriptions.PSMeshBlinnPhong
                }, bufferPool);

            return new[] { renderBlinn };
        }
    }
}
