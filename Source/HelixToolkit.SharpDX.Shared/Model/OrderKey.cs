using System;
using System.Runtime.CompilerServices;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
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
