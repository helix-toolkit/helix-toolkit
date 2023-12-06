using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class AtomicHelper
{
    /// <summary>
    /// Only increment value by 1 if value is greater than comparand.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparand"></param>
    /// <returns>Success or not</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IncrementIfGreaterThan(ref int value, int comparand)
    {
        int next;
        do
        {
            next = Interlocked.CompareExchange(ref value, 0, 0);
            if (next <= comparand)
            {
                return false;
            }
        }
        while (Interlocked.CompareExchange(ref value, next + 1, next) != next);
        return true;
    }

    /// <summary>
    /// Only exchange value to target if value is greater than comparand.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="target"></param>
    /// <param name="comparand"></param>
    /// <returns>Success or not</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ExchangeIfGreaterThan(ref int value, int target, int comparand)
    {
        int next;
        do
        {
            next = Interlocked.CompareExchange(ref value, 0, 0);
            if (next <= comparand)
            {
                return false;
            }
        }
        while (Interlocked.CompareExchange(ref value, target, next) != next);
        return true;
    }

    /// <summary>
    /// Decrement value by 1 if value is larger than comparand
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparand"></param>
    /// <returns>Success or not</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DecrementIfGreaterThan(ref int value, int comparand)
    {
        int next;
        do
        {
            next = Interlocked.CompareExchange(ref value, 0, 0);
            if (next <= comparand)
            {
                return false;
            }
        }
        while (Interlocked.CompareExchange(ref value, next - 1, next) != next);
        return true;
    }

    /// <summary>
    /// Read the value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read(ref int value)
    {
        return Interlocked.CompareExchange(ref value, 0, 0);
    }
}
