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
        public ulong Key;

        public OrderKey(ulong key)
        {
            Key = key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OrderKey Create(uint order, float cameraDistance)
        {
            uint distInt = (uint)Math.Min(uint.MaxValue, (cameraDistance * 1e4f));

            return new OrderKey(
                ((ulong)order << 32) +
                distInt);
        }

        public int CompareTo(OrderKey other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
