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
        public sealed class BlendStatePool : StatePoolBase<BlendStateDescription, BlendState, BlendStateDescription>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BlendStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public BlendStatePool(Device device) : base(device) { }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override BlendState Create(Device device, ref BlendStateDescription description)
            {
                if(device.FeatureLevel < global::SharpDX.Direct3D.FeatureLevel.Level_11_0 && description.IndependentBlendEnable)
                {
                    description.IndependentBlendEnable = false;
                }
                return new BlendState(device, description);
            }

            protected override StateProxy<BlendState> CreateProxy(BlendState state)
            {
                return new BlendStateProxy(state);
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override BlendStateDescription GetKey(ref BlendStateDescription description)
            {
                return description;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class DepthStencilStatePool : StatePoolBase<DepthStencilStateDescription, DepthStencilState, DepthStencilStateDescription>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DepthStencilStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public DepthStencilStatePool(Device device) : base(device) { }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override DepthStencilState Create(Device device, ref DepthStencilStateDescription description)
            {
                return new DepthStencilState(device, description);
            }

            protected override StateProxy<DepthStencilState> CreateProxy(DepthStencilState state)
            {
                return new DepthStencilStateProxy(state);
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override DepthStencilStateDescription GetKey(ref DepthStencilStateDescription description)
            {
                return description;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class RasterStatePool : StatePoolBase<RasterizerStateDescription, RasterizerState, RasterizerStateDescription>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RasterStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public RasterStatePool(Device device) : base(device) { }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override RasterizerState Create(Device device, ref RasterizerStateDescription description)
            {
                return new RasterizerState(device, description);
            }

            protected override StateProxy<RasterizerState> CreateProxy(RasterizerState state)
            {
                return new RasterizerStateProxy(state);
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override RasterizerStateDescription GetKey(ref RasterizerStateDescription description)
            {
                return description;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class SamplerStatePool : StatePoolBase<SamplerStateDescription, SamplerState, SamplerStateDescription>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SamplerStatePool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public SamplerStatePool(Device device) : base(device)
            {
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override SamplerState Create(Device device, ref SamplerStateDescription description)
            {
                return new SamplerState(device, description);
            }

            protected override StateProxy<SamplerState> CreateProxy(SamplerState state)
            {
                return new SamplerStateProxy(state);
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override SamplerStateDescription GetKey(ref SamplerStateDescription description)
            {
                return description;
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
            public BlendStatePool BlendStatePool { private set; get; }
            /// <summary>
            /// Gets or sets the raster state pool.
            /// </summary>
            /// <value>
            /// The raster state pool.
            /// </value>
            public RasterStatePool RasterStatePool { private set; get; }
            /// <summary>
            /// Gets or sets the depth stencil state pool.
            /// </summary>
            /// <value>
            /// The depth stencil state pool.
            /// </value>
            public DepthStencilStatePool DepthStencilStatePool { private set; get; }

            /// <summary>
            /// Gets or sets the sampler state pool.
            /// </summary>
            /// <value>
            /// The sampler state pool.
            /// </value>
            public SamplerStatePool SamplerStatePool { private set; get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="StatePoolManager"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            public StatePoolManager(Device device)
            {
                BlendStatePool = Collect(new BlendStatePool(device));
                RasterStatePool = Collect(new RasterStatePool(device));
                DepthStencilStatePool = Collect(new DepthStencilStatePool(device));
                SamplerStatePool = Collect(new SamplerStatePool(device));
            }

            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public BlendStateProxy Register(BlendStateDescription desc)
            {
                return BlendStatePool.Register(desc) as BlendStateProxy;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public RasterizerStateProxy Register(RasterizerStateDescription desc)
            {
                return RasterStatePool.Register(desc) as RasterizerStateProxy;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public DepthStencilStateProxy Register(DepthStencilStateDescription desc)
            {
                return DepthStencilStatePool.Register(desc) as DepthStencilStateProxy;
            }
            /// <summary>
            /// Registers the specified desc.
            /// </summary>
            /// <param name="desc">The desc.</param>
            /// <returns></returns>
            public SamplerStateProxy Register(SamplerStateDescription desc)
            {
                return SamplerStatePool.Register(desc) as SamplerStateProxy;
            }
        }
    }

}
