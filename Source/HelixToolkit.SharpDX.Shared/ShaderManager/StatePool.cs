/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace ShaderManager
    {
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public sealed class BlendStatePool : ReferenceCountedDictionaryPool<BlendStateDescription, BlendStateProxy, BlendStateDescription>
        {
            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="BlendStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public BlendStatePool(Device device) : base(true)
            {
                this.device = device;
            }

            protected override bool CanCreate(ref BlendStateDescription key, ref BlendStateDescription argument)
            {
                return !IsDisposed;
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override BlendStateProxy OnCreate(ref BlendStateDescription key, ref BlendStateDescription description)
            {
                if (device.FeatureLevel < global::SharpDX.Direct3D.FeatureLevel.Level_11_0 && description.IndependentBlendEnable)
                {
                    description.IndependentBlendEnable = false;
                }
                return new BlendStateProxy(new BlendState(device, description));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class DepthStencilStatePool : ReferenceCountedDictionaryPool<DepthStencilStateDescription, DepthStencilStateProxy, DepthStencilStateDescription>
        {
            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="DepthStencilStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public DepthStencilStatePool(Device device) : base(true)
            {
                this.device = device;
            }

            protected override bool CanCreate(ref DepthStencilStateDescription key, ref DepthStencilStateDescription argument)
            {
                return !IsDisposed;
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override DepthStencilStateProxy OnCreate(ref DepthStencilStateDescription key, ref DepthStencilStateDescription description)
            {
                return new DepthStencilStateProxy(new DepthStencilState(device, description));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class RasterStatePool : ReferenceCountedDictionaryPool<RasterizerStateDescription, RasterizerStateProxy, RasterizerStateDescription>
        {
            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="RasterStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public RasterStatePool(Device device) : base(true)
            {
                this.device = device;
            }

            protected override bool CanCreate(ref RasterizerStateDescription key, ref RasterizerStateDescription argument)
            {
                return !IsDisposed;
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override RasterizerStateProxy OnCreate(ref RasterizerStateDescription key, ref RasterizerStateDescription description)
            {
                return new RasterizerStateProxy(new RasterizerState(device, description));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class SamplerStatePool : ReferenceCountedDictionaryPool<SamplerStateDescription, SamplerStateProxy, SamplerStateDescription>
        {
            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="SamplerStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public SamplerStatePool(Device device) : base(true)
            {
                this.device = device;
            }

            protected override bool CanCreate(ref SamplerStateDescription key, ref SamplerStateDescription argument)
            {
                return !IsDisposed;
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override SamplerStateProxy OnCreate(ref SamplerStateDescription key, ref SamplerStateDescription description)
            {
                return new SamplerStateProxy(new SamplerState(device, description));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class StatePoolManager : DisposeObject, IStatePoolManager
        {
            /// <summary>
            /// Gets or sets the blend state pool.
            /// </summary>
            /// <value>
            /// The blend state pool.
            /// </value>
            public BlendStatePool BlendStatePool
            {
                get;
            }
            /// <summary>
            /// Gets or sets the raster state pool.
            /// </summary>
            /// <value>
            /// The raster state pool.
            /// </value>
            public RasterStatePool RasterStatePool
            {
                get;
            }
            /// <summary>
            /// Gets or sets the depth stencil state pool.
            /// </summary>
            /// <value>
            /// The depth stencil state pool.
            /// </value>
            public DepthStencilStatePool DepthStencilStatePool
            {
                get;
            }

            /// <summary>
            /// Gets or sets the sampler state pool.
            /// </summary>
            /// <value>
            /// The sampler state pool.
            /// </value>
            public SamplerStatePool SamplerStatePool
            {
                get;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="StatePoolManager"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public StatePoolManager(Device device)
            {
                BlendStatePool = new BlendStatePool(device);
                RasterStatePool = new RasterStatePool(device);
                DepthStencilStatePool = new DepthStencilStatePool(device);
                SamplerStatePool = new SamplerStatePool(device);
            }

            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public BlendStateProxy Register(BlendStateDescription desc)
            {
                return BlendStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public RasterizerStateProxy Register(RasterizerStateDescription desc)
            {
                return RasterStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public DepthStencilStateProxy Register(DepthStencilStateDescription desc)
            {
                return DepthStencilStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public SamplerStateProxy Register(SamplerStateDescription desc)
            {
                return SamplerStatePool.TryCreateOrGet(desc, desc, out var state) ? state : null;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                BlendStatePool.Dispose();
                RasterStatePool.Dispose();
                DepthStencilStatePool.Dispose();
                SamplerStatePool.Dispose();
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
