/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGRESOURCE
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using HelixToolkit.Logger;
    using Utilities;
    /// <summary>
    /// Use to store state resources for dynamic creation/destory. Each register will increase the reference counter/>
    /// <para>User must call Dispose() to dispose the resources if not used.</para>
    /// </summary>
    /// <typeparam name="TKEY"></typeparam>
    /// <typeparam name="TVALUE"></typeparam>
    /// <typeparam name="TDescription"></typeparam>
    public abstract class StatePoolBase<TKEY, TVALUE, TDescription> : IDisposable where TVALUE : ComObject where TKEY : struct
    {
        private readonly Dictionary<TKEY, StateProxy<TVALUE>> pool = new Dictionary<TKEY, StateProxy<TVALUE>>();
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count { get { return pool.Count; } }
        /// <summary>
        /// 
        /// </summary>
        public Device Device { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public StatePoolBase(Device device)
        {
            this.Device = device;
        }
        /// <summary>
        /// Each register will increase the reference counter
        /// Calling owner is responsible for dispose the obtained resource. Resource will be disposed automatically once reference counter = 0.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public StateProxy<TVALUE> Register(TDescription description)
        {
            TKEY key = GetKey(ref description);
            lock (pool)
            {
                if (pool.TryGetValue(key, out StateProxy<TVALUE> value))
                {
                    value.IncRef();
                    return value;
                }
                else
                {
                    var newValue = CreateProxy(Create(Device, ref description));
                    pool.Add(key, newValue);
                    newValue.Disposed += (s, e) => 
                    {
                        lock (pool)
                        {
                            pool.Remove(key);
                        }
                    };
                    return newValue;
                }
            }
        }
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected abstract TKEY GetKey(ref TDescription description);
        /// <summary>
        /// Creates the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected abstract TVALUE Create(Device device, ref TDescription description);
        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        protected abstract StateProxy<TVALUE> CreateProxy(TVALUE state);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    lock (pool)
                    {
                        var arr = pool.Values.ToArray();
                        foreach (var item in arr)
                        {
                            item.ForceDispose();
                        }
                        pool.Clear();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ComPoolBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


    /// <summary>
    /// Use to store resources which have long life time with limited numbers such as Constant buffers, Shaders, etc. 
    /// <para>Do not dispose the resource externally.</para>
    /// Resource life time is the same as the pool's life time. 
    /// <para>Do not use for dynamic allocated resources.</para>
    /// </summary>
    /// <typeparam name="TKEY"></typeparam>
    /// <typeparam name="TVALUE"></typeparam>
    /// <typeparam name="TDescription"></typeparam>
    public abstract class LongLivedResourcePoolBase<TKEY, TVALUE, TDescription> : IDisposable where TVALUE : class, IDisposable
    {
        private readonly Dictionary<TKEY, TVALUE> pool = new Dictionary<TKEY, TVALUE>();
        /// <summary>
        /// 
        /// </summary>
        public Device Device { private set; get; }
        public int Count { get { return pool.Count; } }
        protected readonly LogWrapper logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="logger"></param>
        public LongLivedResourcePoolBase(Device device, LogWrapper logger)
        {
            this.Device = device;
            this.logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public TVALUE Register(TDescription description)
        {
            if (description == null)
            { return null; }
            TKEY key = GetKey(ref description);
            if (key == null)
            { return null; }
            lock (pool)
            {
                if (pool.TryGetValue(key, out TVALUE value))
                {
                    ErrorCheck(value, ref description);
                    return value;
                }
                else
                {
                    value = Create(Device, ref description);
                    pool.Add(key, value);
                    return value;
                }
            }
        }
        /// <summary>
        /// Destory the value by its description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public bool Destory(TDescription description)
        {
            if (description == null)
            {
                return false;
            }
            TKEY key = GetKey(ref description);
            if(key == null)
            {
                return false;
            }
            lock (pool)
            {
                if(pool.TryGetValue(key, out TVALUE value))
                {
                    value.Dispose();
                    pool.Remove(key);
                    return true;
                }
            }
            return false;
        }

        protected virtual void ErrorCheck(TVALUE value, ref TDescription description)
        {
        }
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected abstract TKEY GetKey(ref TDescription description);
        /// <summary>
        /// Creates the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected abstract TVALUE Create(Device device, ref TDescription description);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    // TODO: dispose managed state (managed objects).
                    lock (pool)
                    {
                        var arr = pool.Values.ToArray();
                        foreach (var item in arr)
                        {
                            item.Dispose();
                        }
                        pool.Clear();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ResourcePoolBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
