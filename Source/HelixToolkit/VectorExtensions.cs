using System.Numerics;
using System.Runtime.CompilerServices;

namespace HelixToolkit;

public static class VectorExtensions
{
    /// <summary>
    /// Determines whether the specified vector is undefined (NaN,NaN,NaN).
    /// </summary>
    /// <param name="v">The vector.</param>
    /// <returns>
    /// <c>true</c> if the specified vector is undefined; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUndefined(this Vector3 v)
    {
        return float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z);
    }

    /// <summary>
    /// Calculates the distance from a point to a plane.
    /// </summary>
    /// <param name="point">The point used to calculate distance</param>
    /// <param name="planePosition">The position of plane</param>
    /// <param name="planeNormal">The normal vector of plane</param>
    /// <returns>
    /// The distance from given point to the given plane<br/>
    /// Equal zero: Point on the plane<br/>
    /// Greater than zero: The point is on the same side of the plane's normal vector<br/>
    /// Less than zero: The point is on the opposite side of the plane's normal vector<br/>
    /// </returns>
    public static float DistanceToPlane(this Vector3 point, Vector3 planePosition, Vector3 planeNormal)
    {
        Vector3 planeToPoint = point - planePosition;
        planeNormal = Vector3.Normalize(planeNormal);
        return Vector3.Dot(planeToPoint, planeNormal);
    }

    /// <summary>
    /// Calculates the projection of a point onto a plane.
    /// </summary>
    /// <param name="point">The point used to calculate projection</param>
    /// <param name="planePosition">The position of plane</param>
    /// <param name="planeNormal">The normal vector of plane</param>
    /// <returns>The projection of a given point on a given plane.</returns>
    public static Vector3 ProjectOnPlane(this Vector3 point, Vector3 planePosition, Vector3 planeNormal)
    {
        float distance = DistanceToPlane(point, planePosition, planeNormal);
        planeNormal = Vector3.Normalize(planeNormal);
        return point - distance * planeNormal;
    }

    /// <summary>
    /// Angles the between two vectors. Return Radians;
    /// </summary>
    /// <param name="vector1">The vector1.</param>
    /// <param name="vector2">The vector2.</param>
    /// <returns></returns>
    public static float AngleBetween(this Vector3 vector1, Vector3 vector2)
    {
        vector1 = Vector3.Normalize(vector1);
        vector2 = Vector3.Normalize(vector2);
        var ratio = Vector3.Dot(vector1, vector2);
        float theta;

        if (ratio < 0)
        {
            theta = (float)(Math.PI - 2.0 * Math.Asin((-vector1 - vector2).Length() / 2.0));
        }
        else
        {
            theta = (float)(2.0 * Math.Asin((vector1 - vector2).Length() / 2.0));
        }
        return theta;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Normalized(this Vector3 vector)
    {
        return Vector3.Normalize(vector);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 Normalized(this Vector4 vector)
    {
        return Vector4.Normalize(vector);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Matrix4x4 Inverted(this Matrix4x4 m)
    {
        return !Matrix4x4.Invert(m, out var inv) ? new() : inv;
    }
}
