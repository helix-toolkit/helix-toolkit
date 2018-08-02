using System;
using System.Runtime.CompilerServices;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model
#else
namespace HelixToolkit.Wpf.SharpDX.Model
#endif
{
    /// <summary>
    /// Render order key
    /// </summary>
    public struct OrderKey : IComparable<OrderKey>
    {
        public uint Key;

        public OrderKey(uint key)
        {
            Key = key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OrderKey Create(ushort order, ushort materialID)
        {
            return new OrderKey(((uint)order << 32) | materialID);
        }

        public int CompareTo(OrderKey other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
