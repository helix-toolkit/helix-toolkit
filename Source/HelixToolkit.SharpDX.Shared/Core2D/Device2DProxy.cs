/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Device2D = global::SharpDX.Direct2D1.Device;
using DeviceContext2D = global::SharpDX.Direct2D1.DeviceContext;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public interface ID2DTargetProxy : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        BitmapProxy D2DTarget { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swapChain"></param>
        void Initialize(SwapChain1 swapChain, DeviceContext2D deviceContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        void Initialize(Texture2D texture, DeviceContext2D deviceContext);
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class D2DTargetProxy : DisposeObject, ID2DTargetProxy 
    {
        private BitmapProxy d2DTarget;
        /// <summary>
        /// Gets the d2d target. Which is bind to the 3D back buffer/texture
        /// </summary>
        /// <value>
        /// The d2d target.
        /// </value>
        public BitmapProxy D2DTarget { get { return d2DTarget; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="swapChain"></param>
        public void Initialize(SwapChain1 swapChain, DeviceContext2D deviceContext)
        {
            RemoveAndDispose(ref d2DTarget);
            using (var surf = swapChain.GetBackBuffer<Surface>(0))
            {
                d2DTarget = Collect(BitmapProxy.Create("SwapChainTarget", deviceContext, surf));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void Initialize(Texture2D texture, DeviceContext2D deviceContext)
        {
            RemoveAndDispose(ref d2DTarget);        
            using (var surface = texture.QueryInterface<global::SharpDX.DXGI.Surface>())
            {
                d2DTarget = Collect(BitmapProxy.Create("TextureTarget", deviceContext, surface));
            }
        }

        public static implicit operator Bitmap1(D2DTargetProxy proxy)
        {
            return proxy.d2DTarget;
        }
    }
}
