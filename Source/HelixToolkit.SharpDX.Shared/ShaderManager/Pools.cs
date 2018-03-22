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
    using Utilities;
    /// <summary>
    /// Use to store resources for <see cref="ComObject"/>. Each register will increase the reference counter for ComObject by calling <see cref="ComObject.QueryInterface{T}()"/>
    /// </summary>
    /// <typeparam name="TKEY"></typeparam>
    /// <typeparam name="TVALUE"></typeparam>
    /// <typeparam name="TDescription"></typeparam>
    public abstract class ComPoolBase<TKEY, TVALUE, TDescription> : DisposeObject where TVALUE : ComObject
    {
        private readonly Dictionary<TKEY, TVALUE> pool = new Dictionary<TKEY, TVALUE>();
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
            TVALUE value;
            TKEY key = GetKey(ref description);
            lock (pool)
            {
                if (pool.TryGetValue(key, out value))
                {
                    return CreateProxy(value.QueryInterface<TVALUE>());
                }
                else
                {
                    value = Collect(Create(Device, ref description));
                    pool.Add(key, value);
                    value.Disposed += (s, e) => 
                    {
                        lock (pool)
                        {
                            pool.Remove(key);
                        }
                    };
                    return CreateProxy(value);
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
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (pool)
                {
                    foreach (var item in pool.Values.ToArray())
                    {
                        item.Dispose();
                    }
                    pool.Clear();
                }
            }
            base.OnDispose(disposeManagedResources);
        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public ResourcePoolBase(Device device)
        {
            this.Device = device;
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
            TVALUE value;
            TKEY key = GetKey(ref description);
            if (key == null)
            { return null; }
            lock (pool)
            {
                if (pool.TryGetValue(key, out value))
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
