using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public struct EnumHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(ShaderStage option, ShaderStage flag)
    {
        return (option & flag) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(StateType option, StateType flag)
    {
        return (option & flag) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(RenderDetail option, RenderDetail flag)
    {
        return (option & flag) != 0;
    }
}
