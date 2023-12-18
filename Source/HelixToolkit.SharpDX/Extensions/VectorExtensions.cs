using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class VectorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(this Vector2 vector, float z = 1.0f)
    {
        return new Vector3(vector.X, vector.Y, z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(this Vector4 vector)
    {
        return new Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToXYZ(this Vector4 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(this Vector3 vector, float w = 1f)
    {
        return new Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color4 ToColor4(this Vector4 vector, float w = 1f)
    {
        return new Color4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color4 ToColor4(this Vector3 vector, float w = 1f)
    {
        return new Color4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color4 ToColor4(this Color3 vector, float alpha = 1f)
    {
        return new Color4(vector.Red, vector.Green, vector.Blue, alpha);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color4 ToColor4(this Vector2 vector, float z = 1f, float w = 1f)
    {
        return new Color4((float)vector.X, (float)vector.Y, z, w);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color4 Normalized(this Color4 vector)
    {
        var v = vector.ToVector3();
        return v.Normalized().ToColor4();
    }
}

public static class VectorComparisonExtensions
{
    /// <summary>
    /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v.
    /// ATTENTION: For example (a.AllSmaller(b)) is not the same as !(a.AllGreaterOrEqual(b)) but !(a.AnyGreaterOrEqual(b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmallerOrEqual(this Vector2 v1, Vector2 v2)
    {
        return (v1.X <= v2.X && v1.Y <= v2.Y);
    }

    /// <summary>
    /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v.
    /// ATTENTION: For example (a.AllSmaller(b)) is not the same as !(a.AllGreaterOrEqual(b)) but !(a.AnyGreaterOrEqual(b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmallerOrEqual(this Vector3 v1, Vector3 v2)
    {
        return (v1.X <= v2.X && v1.Y <= v2.Y && v1.Z <= v2.Z);
    }

    /// <summary>
    /// Returns whether ALL elements of v are SmallerOrEqual s.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmallerOrEqual(this Vector3 v, float s)
    {
        return (v.X <= s && v.Y <= s && v.Z <= s);
    }

    /// <summary>
    /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v.
    /// ATTENTION: For example (a.AllSmaller(b)) is not the same as !(a.AllGreaterOrEqual(b)) but !(a.AnyGreaterOrEqual(b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmaller(this Vector2 v1, Vector2 v2)
    {
        return (v1.X < v2.X && v1.Y < v2.Y);
    }

    /// <summary>
    /// Returns whether ALL elements of this are SmallerOrEqual the corresponding element of v.
    /// ATTENTION: For example (a.AllSmaller(b)) is not the same as !(a.AllGreaterOrEqual(b)) but !(a.AnyGreaterOrEqual(b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmaller(this Vector3 v1, Vector3 v2)
    {
        return (v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z);
    }

    /// <summary>
    /// Returns whether ALL elements of v are SmallerOrEqual s.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllSmaller(this Vector3 v, float s)
    {
        return (v.X < s && v.Y < s && v.Z < s);
    }

    /// <summary>
    /// Returns whether AT LEAST ONE element of a is SmallerOrEqual the corresponding element of b.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnySmallerOrEqual(this Vector3 a, Vector3 b)
    {
        return (a.X <= b.X || a.Y <= b.Y || a.Z <= b.Z);
    }

    /// <summary>
    /// Returns whether AT LEAST ONE element of v is SmallerOrEqual s.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnySmallerOrEqual(this Vector3 v, float s)
    {
        return (v.X <= s || v.Y <= s || v.Z <= s);
    }

    /// <summary>
    /// Returns whether ALL elements of a are GreaterOrEqual the corresponding element of b.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllGreaterOrEqual(this Vector3 a, Vector3 b)
    {
        return (a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z);
    }

    /// <summary>
    /// Returns whether ALL elements of v are GreaterOrEqual s.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllGreaterOrEqual(this Vector3 v, float s)
    {
        return (v.X >= s && v.Y >= s && v.Z >= s);
    }

    /// <summary>
    /// Returns whether AT LEAST ONE element of a is GreaterOrEqual the corresponding element of b.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyGreaterOrEqual(this Vector3 a, Vector3 b)
    {
        return (a.X >= b.X || a.Y >= b.Y || a.Z >= b.Z);
    }

    /// <summary>
    /// Returns whether AT LEAST ONE element of v is GreaterOrEqual s.
    /// ATTENTION: For example (AllSmaller(a,b)) is not the same as !(AllGreaterOrEqual(a,b)) but !(AnyGreaterOrEqual(a,b)).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AnyGreaterOrEqual(this Vector3 v, float s)
    {
        return (v.X >= s || v.Y >= s || v.Z >= s);
    }

    /// <summary>
    /// Component-wise min vec
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ComponentMin(this Vector3 a, Vector3 b)
    {
        return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    }

    /// <summary>
    /// Component-wise max vec
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ComponentMax(this Vector3 a, Vector3 b)
    {
        return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    }
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
    /// To the rectangle f.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF ToRectangleF(this Vector2 v)
    {
        return new RectangleF(0, 0, v.X, v.Y);
    }
    /// <summary>
    /// To the rectangle.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle ToRectangle(this Vector2 v)
    {
        return new Rectangle(0, 0, (int)v.X, (int)v.Y);
    }

    /// <summary>
    /// To the rectangle f.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(this RectangleF v)
    {
        return new Vector2(v.Width, v.Height);
    }
    /// <summary>
    /// To the rectangle.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToRectangle(this Rectangle v)
    {
        return new Vector2(v.Width, v.Height);
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

    /// <summary>
    /// Point to plane position. Front/Back/Intersecting.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="plane">The plane.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlaneIntersectionType PointToPlanePosition(this Vector3 point, ref Plane plane)
    {
        var normal = plane.Normal * (plane.D >= 0 ? 1 : -1);
        var v1 = new Vector4(normal, Math.Abs(plane.D));
        var v2 = new Vector4(point, 1);
        var ret = Vector4.Dot(v1, v2);
        return ret > 0 ? PlaneIntersectionType.Front : ret == 0 ? PlaneIntersectionType.Intersecting : PlaneIntersectionType.Back;
    }

    /// <summary>
    /// Point to plane position. Front/Back/Intersecting.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="plane">The plane.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlaneIntersectionType PointToPlanePosition(this Vector3 point, Plane plane)
    {
        var v1 = new Vector4((plane.Normal * (plane.D > 0 ? 1 : -1)), Math.Abs(plane.D));
        var v2 = new Vector4(point, 1);
        var ret = Vector4.Dot(v1, v2);
        return ret > 0 ? PlaneIntersectionType.Front : ret == 0 ? PlaneIntersectionType.Intersecting : PlaneIntersectionType.Back;
    }
}
