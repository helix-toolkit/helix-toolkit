/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
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
            for (int i = 0; i < 6; i++)
            {
                var plane = frustum.GetPlane(i);
                GetBoxToPlanePVertexNVertex(ref box, ref plane.Normal, out Vector3 p, out Vector3 n);
                if (Collision.PlaneIntersectsPoint(ref plane, ref p) == PlaneIntersectionType.Back)
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(ref BoundingFrustum frustum, ref BoundingSphere sphere)
        {
            for (int i = 0; i < 6; i++)
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
        public static bool Intersects(ref BoundingFrustum frustum, ref BoundingBox box, ref BoundingSphere sphere)
        {
            for (int i = 0; i < 6; i++)
            {
                var plane = frustum.GetPlane(i);         
                if (plane.Intersects(ref sphere) == PlaneIntersectionType.Back)
                {
                    return false;
                }
                GetBoxToPlanePVertexNVertex(ref box, ref plane.Normal, out Vector3 p, out Vector3 n);
                if (Collision.PlaneIntersectsPoint(ref plane, ref p) == PlaneIntersectionType.Back)
                {
                    return false;
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
}
