/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUGRESOURCE
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    /// <summary>
    /// 
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
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public TVALUE Register(TDescription description)
        {
            TVALUE value;
            TKEY key = GetKey(ref description);
            if (pool.TryGetValue(key, out value))
            {
                return value.QueryInterface<TVALUE>();
            }
            else
            {
                value = Collect(Create(Device, ref description));
                pool.Add(key, value);
                value.Disposed += (s, e) => 
                {
                    pool.Remove(key);
                };
                return value;
            }
        }

        protected abstract TKEY GetKey(ref TDescription description);
        protected abstract TVALUE Create(Device device, ref TDescription description);

        protected override void Dispose(bool disposeManagedResources)
        {
            pool.Clear();
            base.Dispose(disposeManagedResources);
        }
    }


    /// <summary>
    /// 
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
        /// <param name="cbPool"></param>
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
            if (description == null) { return null; }
            TVALUE value;
            TKEY key = GetKey(ref description);
            if (pool.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                value = Collect(Create(Device, ref description));
                pool.Add(key, value);
                return value;
            }
        }

        protected abstract TKEY GetKey(ref TDescription description);
        protected abstract TVALUE Create(Device device, ref TDescription description);
        protected override void Dispose(bool disposeManagedResources)
        {
            pool.Clear();
            base.Dispose(disposeManagedResources);
        }
    }
}
