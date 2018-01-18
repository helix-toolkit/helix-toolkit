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
    /// <summary>
    /// 
    /// </summary>
    public interface IDevice2DProxy : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        RenderTarget D2DTarget { get; }
        /// <summary>
        /// Gets the d2d device.
        /// </summary>
        /// <value>
        /// The d2 d device.
        /// </value>
        Device2D D2DDevice
        {
            get;
        }
        /// <summary>
        /// Gets the d2d device context.
        /// </summary>
        /// <value>
        /// The d2d device context.
        /// </value>
        DeviceContext2D D2DDeviceContext
        {
            get;
        }
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

    /// <summary>
    /// 
    /// </summary>
    public sealed class Device2DProxy : DisposeObject, IDevice2DProxy 
    {
        private RenderTarget d2DTarget;
        /// <summary>
        /// Gets the d2d target. Which is bind to the 3D back buffer/texture
        /// </summary>
        /// <value>
        /// The d2d target.
        /// </value>
        public RenderTarget D2DTarget { get { return d2DTarget; } }

        private Device2D d2DDevice;
        /// <summary>
        /// Gets the d2d device.
        /// </summary>
        /// <value>
        /// The d2d device.
        /// </value>
        public Device2D D2DDevice { get { return d2DDevice; } }

        private DeviceContext2D d2DDeviceContext;
        /// <summary>
        /// Gets the d2d device context.
        /// </summary>
        /// <value>
        /// The d2d device context.
        /// </value>
        public DeviceContext2D D2DDeviceContext { get { return d2DDeviceContext; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swapChain"></param>
        public void Initialize(SwapChain1 swapChain)
        {
            RemoveAndDispose(ref d2DTarget);
            RemoveAndDispose(ref d2DDeviceContext);
            RemoveAndDispose(ref d2DDevice);
            using (var surf = swapChain.GetBackBuffer<Surface>(0))
            {
                using (var factory = new global::SharpDX.Direct2D1.Factory())
                {
                    using (var dxgiDevice2 = swapChain.GetDevice<global::SharpDX.DXGI.Device>())
                    {
                        var properties = new RenderTargetProperties(new PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
                        d2DTarget = Collect(new RenderTarget(factory, surf, properties));
                        d2DDevice = Collect(new Device2D(dxgiDevice2));
                        d2DDeviceContext = Collect(new DeviceContext2D(d2DDevice, DeviceContextOptions.EnableMultithreadedOptimizations));
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
            RemoveAndDispose(ref d2DDeviceContext);
            RemoveAndDispose(ref d2DDevice);          
            using (var surface = texture.QueryInterface<global::SharpDX.DXGI.Surface>())
            {
                using (var factory = new global::SharpDX.Direct2D1.Factory())
                {
                    using (var dxgiDevice2 = texture.Device.QueryInterface<global::SharpDX.DXGI.Device>())
                    {
                        var properties = new RenderTargetProperties(new PixelFormat(Format.Unknown, global::SharpDX.Direct2D1.AlphaMode.Premultiplied));
                        d2DTarget = Collect(new RenderTarget(factory, surface, properties));
                        d2DDevice = Collect(new Device2D(dxgiDevice2));
                        d2DDeviceContext = Collect(new DeviceContext2D(d2DDevice, DeviceContextOptions.EnableMultithreadedOptimizations));
                    }
                }
            }
        }
    }
}
