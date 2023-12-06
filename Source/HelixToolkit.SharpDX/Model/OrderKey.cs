using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Render order key
/// </summary>
public readonly struct OrderKey : IComparable<OrderKey>
{
    public uint Key
    {
        get;
    }

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
