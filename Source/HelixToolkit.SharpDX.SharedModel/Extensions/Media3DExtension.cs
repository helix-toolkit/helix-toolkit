#if NETFX_CORE
using Media = Windows.UI;
#else
using Media = System.Windows.Media;
using System.Windows.Media.Media3D;
using Point = System.Windows.Point;
#endif
using SharpDX.Mathematics.Interop;
using System.Numerics;
using HelixToolkit.Mathematics;
using System.Runtime.CompilerServices;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public static class Media3DExtension
    {
#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D ToVector3D(this Vector3 vector)
        {
            return new Vector3D(vector.X, vector.Y, vector.Z);
        }

        public static Matrix3x3 ToMatrix3x3(this Media.Matrix m)
        {
            return new Matrix3x3((float)m.M11, (float)m.M12, 0, (float)m.M21, (float)m.M22, 0f, (float)m.OffsetX, (float)m.OffsetY, 1f);
        }
        public static Matrix3x2 ToMatrix3x2(this Media.Matrix m)
        {
            return new Matrix3x2((float)m.M11, (float)m.M12, (float)m.M21, (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
        }
#endif
#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D ToVector3D(this Transform3D trafo)
        {
            var matrix = trafo.Value;
            var w = 1.0 / matrix.M44;
            return new Vector3D(w * matrix.OffsetX, w * matrix.OffsetY, w * matrix.OffsetZ);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3D ToPoint3D(this Vector3 vector)
        {
            return new Point3D(vector.X, vector.Y, vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Size3D ToSize3D(this Vector3 vector)
        {
            return new Size3D(vector.X, vector.Y, vector.Z);
        }

#endif
#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D ToMatrix3D(this Matrix m)
        {
            return new Matrix3D(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.M41,
                (float)m.M42,
                (float)m.M43,
                (float)m.M44);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Point3D point)
        {
            return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Point3D point, float w = 1f)
        {
            return new Vector4((float)point.X, (float)point.Y, (float)point.Z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector3D vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3D vector, float w = 1f)
        {
            return new Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Transform3D trafo)
        {
            var matrix = trafo.Value;
            return new Vector4((float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Transform3D trafo)
        {
            var matrix = trafo.Value;
            return new Vector3((float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix ToMatrix(this Transform3D trafo)
        {
            var m = trafo.Value;
            return new Matrix(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.OffsetX,
                (float)m.OffsetY,
                (float)m.OffsetZ,
                (float)m.M44);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix ToMatrix(this Matrix3D m)
        {
            return new Matrix(
                (float)m.M11,
                (float)m.M12,
                (float)m.M13,
                (float)m.M14,
                (float)m.M21,
                (float)m.M22,
                (float)m.M23,
                (float)m.M24,
                (float)m.M31,
                (float)m.M32,
                (float)m.M33,
                (float)m.M34,
                (float)m.OffsetX,
                (float)m.OffsetY,
                (float)m.OffsetZ,
                (float)m.M44);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform3D AppendTransform(this Transform3D t1, Transform3D t2)
        {
            var g = new System.Windows.Media.Media3D.Transform3DGroup();
            g.Children.Add(t1);
            g.Children.Add(t2);
            return g;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform3D PrependTransform(this Transform3D t1, Transform3D t2)
        {
            var g = new System.Windows.Media.Media3D.Transform3DGroup();
            g.Children.Add(t2);
            g.Children.Add(t1);
            return g;
        }
#endif

#if NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Windows.Foundation.Point p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Windows.Foundation.Point ToPoint(this Vector2 p)
        {
            return new Windows.Foundation.Point(p.X, p.Y);
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Point vector)
        {
            return new Vector2((float)vector.X, (float)vector.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RawVector2 ToVector2Raw(this Point vector)
        {
            return new RawVector2((float)vector.X, (float)vector.Y);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color4 ToColor4(this Media.Color color)
        {
            return new Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Media.Color ToColor(this Color4 color)
        {
            return Media.Color.FromArgb((byte)(color.Alpha * 255), (byte)(color.Red * 255), (byte)(color.Green * 255), (byte)(color.Blue * 255));
            //return System.Windows.Media.Color.FromScRgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
    }
}
