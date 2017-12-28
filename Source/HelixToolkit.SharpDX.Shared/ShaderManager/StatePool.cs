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
    //public class BlendStatePool : ResourcePoolBase<BlendStateDescription, BlendState, BlendStateDescription>
    //{
    //    public BlendStatePool(Device device) : base(device) { }

    //    public override BlendState Register(BlendStateDescription description)
    //    {
    //        if (pool.ContainsKey(description))
    //        {
    //            return pool[description];
    //        }
    //        else
    //        {
    //            var state = Collect(new BlendState(Device, description));
    //            pool.Add(description, state);
    //            return state;
    //        }
    //    }
    //}

    //public class DepthStencilStatePool : ResourcePoolBase<DepthStencilStateDescription, DepthStencilState, DepthStencilStateDescription>
    //{
    //    public DepthStencilStatePool(Device device) : base(device) { }

    //    public override DepthStencilState Register(DepthStencilStateDescription description)
    //    {
    //        if (pool.ContainsKey(description))
    //        {
    //            return pool[description];
    //        }
    //        else
    //        {
    //            var state = Collect(new DepthStencilState(Device, description));
    //            pool.Add(description, state);
    //            return state;
    //        }
    //    }
    //}

    //public class RasterStatePool : ResourcePoolBase<RasterizerStateDescription, RasterizerState, RasterizerStateDescription>
    //{
    //    public RasterStatePool(Device device) : base(device) { }

    //    public override RasterizerState Register(RasterizerStateDescription description)
    //    {
    //        if (pool.ContainsKey(description))
    //        {
    //            return pool[description];
    //        }
    //        else
    //        {
    //            var state = Collect(new RasterizerState(Device, description));
    //            pool.Add(description, state);
    //            return state;
    //        }
    //    }
    //}

    //public class SamplerStatePool : ResourcePoolBase<SamplerStateDescription, SamplerState, SamplerStateDescription>
    //{
    //    public SamplerStatePool(Device device) : base(device)
    //    {
    //    }

    //    public override SamplerState Register(SamplerStateDescription description)
    //    {
    //        if (pool.ContainsKey(description))
    //        {
    //            return pool[description];
    //        }
    //        else
    //        {
    //            var state = Collect(new SamplerState(Device, description));
    //            pool.Add(description, state);
    //            return state;
    //        }
    //    }
    //}

    //public class StatePoolManager : DisposeObject, IStatePoolManager
    //{
    //    public BlendStatePool BlendStatePool { private set; get; }
    //    public RasterStatePool RasterStatePool { private set; get; }
    //    public DepthStencilStatePool DepthStencilStatePool { private set; get; }

    //    public SamplerStatePool SamplerStatePool { private set; get; }

    //    public StatePoolManager(Device device)
    //    {
    //        BlendStatePool = Collect(new BlendStatePool(device));
    //        RasterStatePool = Collect(new RasterStatePool(device));
    //        DepthStencilStatePool = Collect(new DepthStencilStatePool(device));
    //        SamplerStatePool = Collect(new SamplerStatePool(device));
    //    }

    //    public BlendState Register(BlendStateDescription desc)
    //    {
    //        return BlendStatePool.Register(desc);
    //    }

    //    public RasterizerState Register(RasterizerStateDescription desc)
    //    {
    //        return RasterStatePool.Register(desc);
    //    }

    //    public DepthStencilState Register(DepthStencilStateDescription desc)
    //    {
    //        return DepthStencilStatePool.Register(desc);
    //    }

    //    public SamplerState Register(SamplerStateDescription desc)
    //    {
    //        return SamplerStatePool.Register(desc);
    //    }
    //}
}
