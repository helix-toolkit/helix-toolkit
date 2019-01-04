/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using SharpDX.Direct3D11;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
#endif
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
    namespace Render
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
            /// <summary>
            /// Resets the draw calls.
            /// </summary>
            int ResetDrawCalls();
        }
        /// <summary>
        /// 
        /// </summary>
        public sealed class DeviceContextPool : DisposeObject, IDeviceContextPool
        {
            private readonly ConcurrentBag<DeviceContextProxy> contextPool = new ConcurrentBag<DeviceContextProxy>();

            private readonly Device device;
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public DeviceContextProxy Get()
            {
                if (contextPool.TryTake(out DeviceContextProxy context))
                {
                    return context;
                }
                else
                {
                    lock (this)
                    {
                        return Collect(new DeviceContextProxy(device));
                    }
                }
            }
            /// <summary>
            /// Puts the specified context back to the pool after use
            /// </summary>
            /// <param name="context">The context.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Put(DeviceContextProxy context)
            {
                context.ClearRenderTagetBindings();
                context.Reset();
                contextPool.Add(context);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int ResetDrawCalls()
            {
                int totalCalls = 0;
                foreach(var ctx in contextPool)
                {
                    totalCalls += ctx.NumberOfDrawCalls;
                    ctx.ResetDrawCalls();
                }
                return totalCalls;
            }
            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected override void OnDispose(bool disposeManagedResources)
            {
                while (!contextPool.IsEmpty)
                {
                    if(contextPool.TryTake(out DeviceContextProxy context))
                    {
                        RemoveAndDispose(ref context);
                    }
                }
                base.OnDispose(disposeManagedResources);
            }
        }
    }

}
