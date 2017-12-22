/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    public abstract class BufferPool<TKEY, TBUFFERDESC> : ResourcePoolBase<TKEY, IBufferProxy, TBUFFERDESC>
    {
        public BufferPool(Device device) : base(device)
        {
        }

        public override IBufferProxy Register(TBUFFERDESC description)
        {
            var key = GetKey(description);
            if (pool.ContainsKey(key))
            {
                return pool[key];
            }
            else
            {
                var buffer = Collect(CreateBuffer(description));
                buffer.CreateBuffer(Device);
                pool.Add(key, buffer);
                return buffer;
            }
        }

        protected abstract TKEY GetKey(TBUFFERDESC description);
        protected abstract IBufferProxy CreateBuffer(TBUFFERDESC description);
    }
}
