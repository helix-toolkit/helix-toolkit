/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    ///Not Used
    ///
    public class BlendStatePool : ResourcePoolBase<BlendStateDescription, BlendState, BlendStateDescription>
    {
        public BlendStatePool(Device device) : base(device) { }

        protected override BlendState Create(Device device, ref BlendStateDescription description)
        {
            return new BlendState(device, description);
        }

        protected override BlendStateDescription GetKey(ref BlendStateDescription description)
        {
            return description;
        }
    }

    public class DepthStencilStatePool : ResourcePoolBase<DepthStencilStateDescription, DepthStencilState, DepthStencilStateDescription>
    {
        public DepthStencilStatePool(Device device) : base(device) { }

        protected override DepthStencilState Create(Device device, ref DepthStencilStateDescription description)
        {
            return new DepthStencilState(device, description);
        }

        protected override DepthStencilStateDescription GetKey(ref DepthStencilStateDescription description)
        {
            return description;
        }
    }

    public class RasterStatePool : ResourcePoolBase<RasterizerStateDescription, RasterizerState, RasterizerStateDescription>
    {
        public RasterStatePool(Device device) : base(device) { }

        protected override RasterizerState Create(Device device, ref RasterizerStateDescription description)
        {
            return new RasterizerState(device, description);
        }

        protected override RasterizerStateDescription GetKey(ref RasterizerStateDescription description)
        {
            return description;
        }
    }

    public class SamplerStatePool : ResourcePoolBase<SamplerStateDescription, SamplerState, SamplerStateDescription>
    {
        public SamplerStatePool(Device device) : base(device)
        {
        }

        protected override SamplerState Create(Device device, ref SamplerStateDescription description)
        {
            return new SamplerState(device, description);
        }

        protected override SamplerStateDescription GetKey(ref SamplerStateDescription description)
        {
            return description;
        }
    }

    public class StatePoolManager : DisposeObject, IStatePoolManager
    {
        public BlendStatePool BlendStatePool { private set; get; }
        public RasterStatePool RasterStatePool { private set; get; }
        public DepthStencilStatePool DepthStencilStatePool { private set; get; }

        public SamplerStatePool SamplerStatePool { private set; get; }

        public StatePoolManager(Device device)
        {
            BlendStatePool = Collect(new BlendStatePool(device));
            RasterStatePool = Collect(new RasterStatePool(device));
            DepthStencilStatePool = Collect(new DepthStencilStatePool(device));
            SamplerStatePool = Collect(new SamplerStatePool(device));
        }

        public BlendState Register(BlendStateDescription desc)
        {
            return BlendStatePool.Register(desc);
        }

        public RasterizerState Register(RasterizerStateDescription desc)
        {
            return RasterStatePool.Register(desc);
        }

        public DepthStencilState Register(DepthStencilStateDescription desc)
        {
            return DepthStencilStatePool.Register(desc);
        }

        public SamplerState Register(SamplerStateDescription desc)
        {
            return SamplerStatePool.Register(desc);
        }
    }
}
