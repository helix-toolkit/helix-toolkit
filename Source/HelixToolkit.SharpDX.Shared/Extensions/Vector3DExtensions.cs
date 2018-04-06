/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System.Runtime.CompilerServices;
#if CORE
    
#else
#if NETFX_CORE   
    using Vector3D = System.Numerics.Vector3;
    using Matrix3D = System.Numerics.Matrix4x4;
    using Media = Windows.UI;
#else
    using System.Windows.Media.Media3D;
    using Point = System.Windows.Point;
    using Media = System.Windows.Media;
#endif
#endif
    public static class VectorExtensions
    {
#if !CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D ToVector3D(this Vector3 vector)
        {
            return new Vector3D(vector.X, vector.Y, vector.Z);
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
#if !CORE
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
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToVector3(this Vector2 vector, float z = 1.0f)
        {
            return new global::SharpDX.Vector3(vector.X, vector.Y, z);
        }
#if !CORE
#if NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector2 ToVector2(this Windows.Foundation.Point p)
        {
            return new global::SharpDX.Vector2((float)p.X, (float)p.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Windows.Foundation.Point ToPoint(this global::SharpDX.Vector2 p)
        {
            return new Windows.Foundation.Point(p.X, p.Y);
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector2 ToVector2(this Point vector)
        {
            return new global::SharpDX.Vector2((float)vector.X, (float)vector.Y);
        }
#endif
#endif

#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToVector3(this Point3D point)
        {
            return new global::SharpDX.Vector3((float)point.X, (float)point.Y, (float)point.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector4 ToVector4(this Point3D point, float w = 1f)
        {
            return new global::SharpDX.Vector4((float)point.X, (float)point.Y, (float)point.Z, w);
        }
#endif
#if !CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToVector3(this Vector3D vector)
        {
            return new global::SharpDX.Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToVector3(this Vector4 vector)
        {
            return new global::SharpDX.Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToXYZ(this Vector4 vector)
        {
            return new global::SharpDX.Vector3(vector.X, vector.Y, vector.Z);
        }

#if !CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector4 ToVector4(this Vector3D vector, float w = 1f)
        {
            return new global::SharpDX.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
        }
#endif
        /// <summary>
        /// Angles the between two vectors. Return Radians;
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns></returns>
        public static float AngleBetween(this Vector3 vector1, Vector3 vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
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
#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector4 ToVector4(this Transform3D trafo)
        {
            var matrix = trafo.Value;
            return new global::SharpDX.Vector4((float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 ToVector3(this Transform3D trafo)
        {
            var matrix = trafo.Value;
            return new global::SharpDX.Vector3((float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Matrix ToMatrix(this Transform3D trafo)
        {
            var m = trafo.Value;
            return new global::SharpDX.Matrix(
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
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector4 ToVector4(this global::SharpDX.Vector3 vector, float w = 1f)
        {
            return new global::SharpDX.Vector4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Color4 ToColor4(this global::SharpDX.Vector4 vector, float w = 1f)
        {
            return new global::SharpDX.Color4((float)vector.X, (float)vector.Y, (float)vector.Z, (float)vector.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Color4 ToColor4(this global::SharpDX.Vector3 vector, float w = 1f)
        {
            return new global::SharpDX.Color4((float)vector.X, (float)vector.Y, (float)vector.Z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Color4 ToColor4(this global::SharpDX.Color3 vector, float alpha = 1f)
        {
            return new global::SharpDX.Color4(vector.Red, vector.Green, vector.Blue, alpha);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Color4 ToColor4(this global::SharpDX.Vector2 vector, float z = 1f, float w = 1f)
        {
            return new global::SharpDX.Color4((float)vector.X, (float)vector.Y, z, w);
        }

#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Matrix ToMatrix(this Matrix3D m)
        {
            return new global::SharpDX.Matrix(
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
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector3 Normalized(this global::SharpDX.Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector4 Normalized(this global::SharpDX.Vector4 vector)
        {
            vector.Normalize();
            return vector;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Color4 Normalized(this global::SharpDX.Color4 vector)
        {
            var v = vector.ToVector3();
            v.Normalize();
            return v.ToColor4();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Matrix Inverted(this global::SharpDX.Matrix m)
        {
            m.Invert();
            return m;
        }

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
        public static global::SharpDX.Vector3 FindAnyPerpendicular(this Vector3 n)
        {
            n.Normalize();
            Vector3 u = Vector3.Cross(new Vector3(0, 1, 0), n);
            if (u.LengthSquared() < 1e-3)
            {
                u = Vector3.Cross(new Vector3(1, 0, 0), n);
            }

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
            return float.IsNaN(v.X) && float.IsNaN(v.Y) && float.IsNaN(v.Z);
        }

#if !NETFX_CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color4 ToColor4(this Media.Color color)
        {
            color.Clamp();
            return new global::SharpDX.Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Color ToColor(this Color4 color)
        {
            //return System.Windows.Media.Color.FromArgb((byte)(color.Alpha * 256), (byte)(color.Red * 256), (byte)(color.Green * 256), (byte)(color.Blue * 256));
            return System.Windows.Media.Color.FromScRgb(color.Alpha, color.Red, color.Green, color.Blue);
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
#else
#if !CORE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color4 ToColor4(this Media.Color color)
        {
            return new global::SharpDX.Color4((float)color.R / 255, (float)color.G / 255, (float)color.B / 255, (float)color.A / 255);
        }
#endif
#endif
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
    }
}
