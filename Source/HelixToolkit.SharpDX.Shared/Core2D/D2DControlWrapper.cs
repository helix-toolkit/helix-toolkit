/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface ID2DTarget : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        RenderTarget D2DTarget { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swapChain"></param>
        void Initialize(SwapChain1 swapChain);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        void Initialize(Texture2D texture);
    }

    public sealed class D2DControlWrapper : DisposeObject, ID2DTarget 
    {
        private RenderTarget d2DTarget;
        public RenderTarget D2DTarget { get { return d2DTarget; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swapChain"></param>
        public void Initialize(SwapChain1 swapChain)
        {
            RemoveAndDispose(ref d2DTarget);
            using (var surf = swapChain.GetBackBuffer<Surface>(0))
            {
                using (var factory = new global::SharpDX.Direct2D1.Factory())
                {
                    //using (var dxgiDevice2 = swapChain.GetDevice<global::SharpDX.DXGI.Device>())
                    {
                        var properties = new RenderTargetProperties(new global::SharpDX.Direct2D1.PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
                        d2DTarget = Collect(new RenderTarget(factory, surf, properties));
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void Initialize(Texture2D texture)
        {
            RemoveAndDispose(ref d2DTarget);
            using (var surface = texture.QueryInterface<global::SharpDX.DXGI.Surface>())
            {
                using (var factory = new global::SharpDX.Direct2D1.Factory())
                {
                    var properties = new RenderTargetProperties(new PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
                    d2DTarget = Collect(new RenderTarget(factory, surface, properties));
                }
            }
        }
    }
}
