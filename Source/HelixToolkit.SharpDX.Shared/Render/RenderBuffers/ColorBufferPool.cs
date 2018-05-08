using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using SharpDX.Direct3D11;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{   
    using Utilities;

    public sealed class ColorBufferPool : DisposeObject
    {
        private readonly ConcurrentBag<ShaderResourceViewProxy> pool = new ConcurrentBag<ShaderResourceViewProxy>();
        private readonly IDevice3DResources deviceResourse;
        private readonly Texture2DDescription description;

        public ColorBufferPool(IDevice3DResources deviceResourse, Texture2DDescription desc)
        {
            this.deviceResourse = deviceResourse;
            description = desc;
        }

        public ShaderResourceViewProxy Get()
        {
            if (IsDisposed)
            {
                return null;
            }
            ShaderResourceViewProxy proxy;
            if(pool.TryTake(out proxy))
            {
                return proxy;
            }
            else
            {
                var texture = Collect(new ShaderResourceViewProxy(deviceResourse.Device, description));
                texture.CreateRenderTargetView();
                texture.CreateTextureView();
                return texture;
            }
        }

        public void Put(ShaderResourceViewProxy proxy)
        {
            if (IsDisposed)
            {
                return;
            }
            pool.Add(proxy);
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            ShaderResourceViewProxy proxy;
            while (pool.TryTake(out proxy))
            {
                continue;
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
