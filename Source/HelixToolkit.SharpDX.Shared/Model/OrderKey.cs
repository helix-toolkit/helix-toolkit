using System;
using System.Runtime.CompilerServices;

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
    namespace Model
    {
        /// <summary>
        /// Render order key
        /// </summary>
        public struct OrderKey : IComparable<OrderKey>
        {
            public uint Key { get; }

            public OrderKey(uint key)
            {
                Key = key;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static OrderKey Create(ushort order, ushort materialID)
            {
                //return new OrderKey(((uint)order << 16) | materialID);
                return new OrderKey(order);
            }

            public int CompareTo(OrderKey other)
            {
                return Key.CompareTo(other.Key);
            }
        }
    }

}
