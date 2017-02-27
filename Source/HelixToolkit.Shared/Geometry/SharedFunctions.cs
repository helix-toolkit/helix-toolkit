using System;
using System.Collections.Generic;
using System.Text;

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
#if SHARPDX
    using global::SharpDX;
    using Vector3D = global::SharpDX.Vector3;
    using Point3D = global::SharpDX.Vector3;
    using DoubleOrSingle = System.Single;
    using System.Linq;

#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using DoubleOrSingle = System.Double;
#endif
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
        public static Vector3D CrossProduct(ref Vector3D first, ref Vector3D second)
        {
#if SHARPDX
            return Vector3.Cross(first, second);
#else
            return Vector3D.CrossProduct(first, second);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleOrSingle DotProduct(ref Vector3D first, ref Vector3D second)
        {
            return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleOrSingle LengthSquared(ref Vector3D vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DoubleOrSingle Length(ref Vector3D vector)
        {
            return (DoubleOrSingle)Math.Sqrt(LengthSquared(ref vector));
        }

#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Media3D.Point3D ToPoint3D(ref Vector3D vector)
        {
            return new System.Windows.Media.Media3D.Point3D(vector.X, vector.Y, vector.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Media3D.Vector3D ToVector3D(ref Vector3D vector)
        {
            return new System.Windows.Media.Media3D.Vector3D(vector.X, vector.Y, vector.Z);
        }
#endif
#if SHARPDX
#if !NETFX_CORE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D ToPoint3D(ref System.Windows.Media.Media3D.Vector3D vector)
        {
            return new Point3D((DoubleOrSingle)vector.X, (DoubleOrSingle)vector.Y, (DoubleOrSingle)vector.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D ToVector3D(ref System.Windows.Media.Media3D.Vector3D vector)
        {
            return new Vector3D((DoubleOrSingle)vector.X, (DoubleOrSingle)vector.Y, (DoubleOrSingle)vector.Z);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Media3D.Vector3DCollection ToVector3DCollection(SharpDX.Core.Vector3Collection collection)
        {
            return new System.Windows.Media.Media3D.Vector3DCollection(collection.Select(v => ToVector3D(ref v)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Media3D.Point3DCollection ToPoint3DCollection(SharpDX.Core.Vector3Collection collection)
        {
            return new System.Windows.Media.Media3D.Point3DCollection(collection.Select(v => ToPoint3D(ref v)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.PointCollection ToPointCollection(SharpDX.Core.Vector2Collection collection)
        {
            return new System.Windows.Media.PointCollection(collection.Select(v => new System.Windows.Point(v.X, v.Y)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Int32Collection ToInt32Collection(SharpDX.Core.IntCollection collection)
        {
            return new System.Windows.Media.Int32Collection(collection);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static System.Windows.Media.Media3D.MeshGeometry3D ToMeshGeometry3D(SharpDX.MeshGeometry3D mesh)
        {
            return new System.Windows.Media.Media3D.MeshGeometry3D()
            {
                Normals = ToVector3DCollection(mesh.Normals),
                Positions = ToPoint3DCollection(mesh.Positions),
                TextureCoordinates = ToPointCollection(mesh.TextureCoordinates),
                TriangleIndices = ToInt32Collection(mesh.TriangleIndices)
            };
        }
#endif
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
        public static Point3D? LineIntersection(this Plane plane, Point3D la, Point3D lb)
        {
            // http://en.wikipedia.org/wiki/Line-plane_intersection
            var l = lb - la;
            var pos = plane.Normal * plane.D;
            var k = pos - la;
            var a = DotProduct(ref k, ref plane.Normal);
            var b = DotProduct(ref l, ref plane.Normal);
            if (a.Equals(0) && b.Equals(0))
            {
                return null;
            }

            if (b.Equals(0))
            {
                return null;
            }

            return la + ((a / b) * l);
        }
#endif
    }
}
