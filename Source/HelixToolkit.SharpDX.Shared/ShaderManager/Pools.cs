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
    public abstract class GeneralPool<TKEY, TVALUE, TDescription> : DisposeObject
    {
        protected readonly Dictionary<TKEY, TVALUE> pool = new Dictionary<TKEY, TVALUE>();

        public Device Device { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="cbPool"></param>
        public GeneralPool(Device device)
        {
            this.Device = device;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public abstract TVALUE Register(TDescription description);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKEY key)
        {
            return pool.Remove(key);
        }
    }
}
