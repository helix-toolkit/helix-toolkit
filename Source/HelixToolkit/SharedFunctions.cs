using System.Numerics;
using System.Runtime.CompilerServices;

namespace HelixToolkit;

/// <summary>
/// Functions for the Shared Projects to simplify the Code
/// </summary>
internal static class SharedFunctions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 CrossProduct(ref Vector3 first, ref Vector3 second)
    {
        return Vector3.Cross(first, second);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 CrossProduct(Vector3 first, Vector3 second)
    {
        return Vector3.Cross(first, second);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DotProduct(ref Vector3 first, ref Vector3 second)
    {
        return Vector3.Dot(first, second);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DotProduct(ref Vector2 first, ref Vector2 second)
    {
        return Vector2.Dot(first, second);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LengthSquared(ref Vector3 vector)
    {
        return vector.LengthSquared();
    }

    /// <summary>
    /// Lengthes the squared.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LengthSquared(ref Vector2 vector)
    {
        return vector.LengthSquared();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Length(ref Vector3 vector)
    {
        return vector.Length();
    }

    /// <summary>
    /// Finds the intersection between the plane and a line.
    /// </summary>
    /// <param name="plane">
    /// The plane.
    /// </param>
    /// <param name="la">
    /// The first point defining the line.
    /// </param>
    /// <param name="lb">
    /// The second point defining the line.
    /// </param>
    /// <returns>
    /// The intersection point.
    /// </returns>
    public static Vector3? LineIntersection(this Plane plane, Vector3 la, Vector3 lb)
    {
        // https://graphics.stanford.edu/~mdfisher/Code/Engine/Plane.cpp.html
        var diff = la - lb;
        var d = Vector3.Dot(diff, plane.Normal);
        if (d == 0)
        {
            return null;
        }

        var u = (Vector3.Dot(la, plane.Normal) + plane.D) / d;
        return (la + u * (lb - la));
    }
}
