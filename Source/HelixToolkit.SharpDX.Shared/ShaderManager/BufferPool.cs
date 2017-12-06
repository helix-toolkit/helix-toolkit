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
    public abstract class BufferPool<TKEY, TBUFFERDESC> : DisposeObject
    {
        private Dictionary<TKEY, IBufferProxy> pool = new Dictionary<TKEY, IBufferProxy>();


        public IBufferProxy Register(TBUFFERDESC description, Device device)
        {
            var key = GetKey(description);
            if (pool.ContainsKey(key))
            {
                return pool[key];
            }
            else
            {
                var buffer = Collect(CreateBuffer(description));
                buffer.CreateBuffer(device);
                pool.Add(key, buffer);
                return buffer;
            }
        }

        protected abstract TKEY GetKey(TBUFFERDESC description);
        protected abstract IBufferProxy CreateBuffer(TBUFFERDESC description);

        public IBufferProxy Get(TBUFFERDESC description)
        {
            return pool[GetKey(description)];
        }
    }
}
