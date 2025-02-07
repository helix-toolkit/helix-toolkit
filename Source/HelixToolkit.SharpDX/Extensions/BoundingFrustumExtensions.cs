﻿using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX;

public static class BoundingFrustumExtensions
{
    /// <summary>
    /// Intersectses the specified frustum. Simplified from https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/BoundingFrustum.cs 
    /// </summary>
    /// <param name="frustum">The frustum.</param>
    /// <param name="box">The box.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(ref BoundingFrustum frustum, ref BoundingBox box)
    {
        for (var i = 0; i < 6; i++)
        {
            var plane = frustum.GetPlane(i);
            GetBoxToPlanePVertexNVertex(ref box, ref plane.Normal, out var p, out var n);
            if (Collision.PlaneIntersectsPoint(ref plane, ref p) == PlaneIntersectionType.Back)
                return false;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(ref BoundingFrustum frustum, ref BoundingSphere sphere)
    {
        for (var i = 0; i < 6; i++)
        {
            var plane = frustum.GetPlane(i);
            var result = plane.Intersects(ref sphere);
            if (result == PlaneIntersectionType.Back)
            {
                return false;
            }
            else if (result == PlaneIntersectionType.Intersecting)
            {
                return true;
            }
        }
        return true;
    }

    /// <summary>
    /// Intersectses the specified frustum. Return false if either box or sphere is not intersect with the frustum, otherwise return true
    /// </summary>
    /// <param name="frustum">The frustum.</param>
    /// <param name="box">The box.</param>
    /// <param name="sphere">The sphere.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInOrIntersectFrustum(ref BoundingFrustum frustum, ref BoundingBox box, ref BoundingSphere sphere)
    {
        for (var i = 0; i < 6; i++)
        {
            var plane = frustum.GetPlane(i);
            var sphereRet = plane.Intersects(ref sphere);
            if (sphereRet == PlaneIntersectionType.Back)
            {
                return false;
            }
            else if (sphereRet == PlaneIntersectionType.Intersecting)
            {
                return true;
            }
            GetBoxToPlanePVertexNVertex(ref box, ref plane.Normal, out var p, out var n);
            var boxRet = Collision.PlaneIntersectsPoint(ref plane, ref p);
            if (boxRet == PlaneIntersectionType.Back)
            {
                return false;
            }
            else if (boxRet == PlaneIntersectionType.Intersecting)
            {
                return true;
            }
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GetBoxToPlanePVertexNVertex(ref BoundingBox box, ref Vector3 planeNormal, out Vector3 p, out Vector3 n)
    {
        p = box.Minimum;
        if (planeNormal.X >= 0)
            p.X = box.Maximum.X;
        if (planeNormal.Y >= 0)
            p.Y = box.Maximum.Y;
        if (planeNormal.Z >= 0)
            p.Z = box.Maximum.Z;

        n = box.Maximum;
        if (planeNormal.X >= 0)
            n.X = box.Minimum.X;
        if (planeNormal.Y >= 0)
            n.Y = box.Minimum.Y;
        if (planeNormal.Z >= 0)
            n.Z = box.Minimum.Z;
    }
}
