/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX.Direct3D11;
using System.Collections.Concurrent;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
#endif
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Render
#else
namespace HelixToolkit.UWP.Render
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceContextPool
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        DeviceContextProxy Get();
        /// <summary>
        /// Puts the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Put(DeviceContextProxy context);
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class DeviceContextPool : DisposeObject, IDeviceContextPool
    {
        private readonly ConcurrentBag<DeviceContextProxy> contextPool = new ConcurrentBag<DeviceContextProxy>();

        private Device device;
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContextPool"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceContextPool(Device device)
        {
            this.device = device;
        }
        /// <summary>
        /// Gets this instance from pool
        /// </summary>
        /// <returns></returns>
        public DeviceContextProxy Get()
        {
            DeviceContextProxy context;
            if (contextPool.TryTake(out context))
            {
                return context;
            }
            else
            {
                return new DeviceContextProxy(device);
            }
        }
        /// <summary>
        /// Puts the specified context back to the pool after use
        /// </summary>
        /// <param name="context">The context.</param>
        public void Put(DeviceContextProxy context)
        {
            contextPool.Add(context);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            DeviceContextProxy context;
            while (!contextPool.IsEmpty)
            {
                if(contextPool.TryTake(out context))
                {
                    context.Dispose();
                }
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
