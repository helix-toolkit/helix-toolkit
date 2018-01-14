/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using Render;
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
    public sealed class DeviceContextPool : DisposeObject
    {
        private readonly ConcurrentBag<DeviceContextProxy> contextPool = new ConcurrentBag<DeviceContextProxy>();

        private Device device;

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

        protected override void Dispose(bool disposeManagedResources)
        {
            DeviceContextProxy context;
            while (!contextPool.IsEmpty)
            {
                if(contextPool.TryTake(out context))
                {
                    context.Dispose();
                }
            }
            base.Dispose(disposeManagedResources);
        }
    }
}
