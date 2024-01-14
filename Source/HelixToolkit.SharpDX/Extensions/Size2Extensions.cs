using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class Size2Extensions
{
    /// <summary>
    /// To the vector2.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(this Size2F s)
    {
        return new Vector2(s.Width, s.Height);
    }
    /// <summary>
    /// To the vector2.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(this Size2 s)
    {
        return new Vector2(s.Width, s.Height);
    }
    /// <summary>
    /// To the size2 f.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2F ToSize2F(this Vector2 s)
    {
        return new Size2F(s.X, s.Y);
    }
    /// <summary>
    /// To the size2.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2 ToSize2(this Vector2 s)
    {
        return new Size2((int)s.X, (int)s.Y);
    }
    /// <summary>
    /// To the size f.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2F ToSizeF(this Size2 s)
    {
        return new Size2F(s.Width, s.Height);
    }
    /// <summary>
    /// To the size2.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Size2 ToSize2(this Size2F s)
    {
        return new Size2((int)s.Width, (int)s.Height);
    }
    
   
}
