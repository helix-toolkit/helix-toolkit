using System.Runtime.CompilerServices;

namespace HelixToolkit;

public static class NumericHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt32(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
#if NET8_0_OR_GREATER
        return int.Parse(s, provider);
#else
        return int.Parse(s.ToString(), provider);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseSingle(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
#if NET8_0_OR_GREATER
        return float.Parse(s, provider);
#else
        return float.Parse(s.ToString(), provider);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
#if NET8_0_OR_GREATER
        return double.Parse(s, provider);
#else
        return double.Parse(s.ToString(), provider);
#endif
    }
}
