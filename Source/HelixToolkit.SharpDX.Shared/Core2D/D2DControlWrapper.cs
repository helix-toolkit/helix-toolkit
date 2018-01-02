/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public sealed class D2DControlWrapper : IDisposable
    {
        private RenderTarget d2DTarget;
        public RenderTarget D2DTarget { get { return d2DTarget; } }

        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref d2DTarget);
        }

        public void Initialize(SwapChain1 swapChain)
        {
            Disposer.RemoveAndDispose(ref d2DTarget);
            using (var surf = swapChain.GetBackBuffer<Surface>(0))
            {
                using (var factory = new global::SharpDX.Direct2D1.Factory())
                {
                    using (var dxgiDevice2 = swapChain.GetDevice<global::SharpDX.DXGI.Device>())
                    {
                        var properties = new RenderTargetProperties(new global::SharpDX.Direct2D1.PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
                        d2DTarget = new RenderTarget(factory, surf, properties);
                    }
                }
            }
        }
    }
}
