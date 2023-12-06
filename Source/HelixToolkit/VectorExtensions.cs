using System.Numerics;
using System.Runtime.CompilerServices;

namespace HelixToolkit;

public static class VectorExtensions
{
    /// <summary>
    /// Find a <see cref="Vector3"/> that is perpendicular to the given <see cref="Vector3"/>.
    /// </summary>
    /// <param name="n">
    /// The input vector.
    /// </param>
    /// <returns>
    /// A perpendicular vector.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 FindAnyPerpendicular(this Vector3 n)
    {
        n = Vector3.Normalize(n);
        var u = Vector3.Cross(Vector3.UnitY, n);
        if (u.LengthSquared() < 1e-3f)
        {
            u = Vector3.Cross(Vector3.UnitX, n);
        }

        u = Vector3.Normalize(u);

        return u;
    }

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
}
