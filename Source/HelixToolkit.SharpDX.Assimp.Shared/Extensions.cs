using Assimp;
using SharpDX.IO;
using System;
using System.IO;

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
    namespace Assimp
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Extensions
        {
            /// <summary>
            /// To the sharp dx matrix. Already transposed after this function
            /// </summary>
            /// <param name="m">The m.</param>
            /// <param name="isColumnMajor"></param>
            /// <returns></returns>
            public static global::SharpDX.Matrix ToSharpDXMatrix(this Matrix4x4 m, bool isColumnMajor)
            {
                var matrix = new global::SharpDX.Matrix(m.A1, m.A2, m.A3, m.A4, m.B1, m.B2, m.B3, m.B4, m.C1, m.C2, m.C3, m.C4, m.D1, m.D2, m.D3, m.D4);
                if (isColumnMajor)
                {
                    matrix.Transpose();
                }
                return matrix;
            }
            /// <summary>
            /// To the assimp matrix. Already transposed after this function
            /// </summary>
            /// <param name="m">The m.</param>
            /// <param name="toColumnMajor"></param>
            /// <returns></returns>
            public static Matrix4x4 ToAssimpMatrix(this global::SharpDX.Matrix m, bool toColumnMajor)
            {
                var matrix = new Matrix4x4(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
                if (toColumnMajor)
                {
                    matrix.Transpose();
                }
                return matrix;
            }
            /// <summary>
            /// To the sharp dx vector3.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static global::SharpDX.Vector3 ToSharpDXVector3(this Vector3D v)
            {
                return new global::SharpDX.Vector3(v.X, v.Y, v.Z);
            }

            /// <summary>
            /// To the assimp vector3d.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static Vector3D ToAssimpVector3D(this global::SharpDX.Vector3 v)
            {
                return new Vector3D(v.X, v.Y, v.Z);
            }
            /// <summary>
            /// To the sharp dx vector2.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static global::SharpDX.Vector2 ToSharpDXVector2(this Vector2D v)
            {
                return new global::SharpDX.Vector2(v.X, v.Y);
            }

            /// <summary>
            /// To the assimp vector2d.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static Vector2D ToAssimpVector2D(this global::SharpDX.Vector2 v)
            {
                return new Vector2D(v.X, v.Y);
            }
            /// <summary>
            /// To the assimp vector3d.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static Vector3D ToAssimpVector3D(this global::SharpDX.Vector2 v)
            {
                return new Vector3D(v.X, v.Y, 0);
            }
            /// <summary>
            /// To the sharp dx vector2.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static global::SharpDX.Vector2 ToSharpDXVector2(this Vector3D v)
            {
                return new global::SharpDX.Vector2(v.X, v.Y);
            }
            /// <summary>
            /// To the sharp dx color4.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static global::SharpDX.Color4 ToSharpDXColor4(this Color4D v)
            {
                return new global::SharpDX.Color4(v.R, v.G, v.B, v.A);
            }

            /// <summary>
            /// To the assimp color4d.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <param name="alpha"></param>
            /// <returns></returns>
            public static Color4D ToAssimpColor4D(this global::SharpDX.Color4 v, float alpha = 1f)
            {
                return new Color4D(v.Red, v.Green, v.Blue, 1f);
            }

            /// <summary>
            /// To the sharp dx quaternion.
            /// </summary>
            /// <param name="q">The q.</param>
            /// <returns></returns>
            public static global::SharpDX.Quaternion ToSharpDXQuaternion(this Quaternion q)
            {
                return new global::SharpDX.Quaternion(q.X, q.Y, q.Z, q.W);
            }

            /// <summary>
            /// To the assimp quaternion.
            /// </summary>
            /// <param name="q">The q.</param>
            /// <returns></returns>
            public static Quaternion ToAssimpQuaternion(this global::SharpDX.Quaternion q)
            {
                return new Quaternion(q.W, q.X, q.Y, q.Z);
            }

            /// <summary>
            /// To the Helix UVTransform.
            /// </summary>
            /// <param name="transform">The transform.</param>
            /// <returns></returns>
            public static UVTransform ToHelixUVTransform(this global::Assimp.UVTransform transform)
            {
                return new UVTransform(transform.Rotation, transform.Scaling.ToSharpDXVector2(), transform.Translation.ToSharpDXVector2());
            }
        }
    }
}
