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
    /// Use to store resources for <see cref="ComObject"/>. Each register will increase the reference counter for ComObject by calling <see cref="ComObject.QueryInterface{T}()"/>
    /// </summary>
    /// <typeparam name="TKEY"></typeparam>
    /// <typeparam name="TVALUE"></typeparam>
    /// <typeparam name="TDescription"></typeparam>
    public abstract class ComPoolBase<TKEY, TVALUE, TDescription> : IDisposable where TVALUE : ComObject where TKEY : struct
    {
        private readonly Dictionary<TKEY, StateProxy<TVALUE>> pool = new Dictionary<TKEY, StateProxy<TVALUE>>();
        /// <summary>
        /// 
        /// </summary>
        public Device Device { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public ComPoolBase(Device device)
        {
            this.Device = device;
        }
        /// <summary>
        /// Each register will increase the reference counter for ComObject by calling <see cref="ComObject.QueryInterface{T}()"/>
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
    public abstract class ResourcePoolBase<TKEY, TVALUE, TDescription> : DisposeObject where TVALUE : class
    {
        private readonly Dictionary<TKEY, TVALUE> pool = new Dictionary<TKEY, TVALUE>();
        /// <summary>
        /// 
        /// </summary>
        public Device Device { private set; get; }
        protected readonly LogWrapper logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="logger"></param>
        public ResourcePoolBase(Device device, LogWrapper logger)
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
                    value = Collect(Create(Device, ref description));
                    pool.Add(key, value);
                    return value;
                }
            }
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
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            pool.Clear();
            base.OnDispose(disposeManagedResources);
        }
    }
}
