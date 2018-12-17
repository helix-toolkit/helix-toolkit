using Assimp;

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
            /// To the sharp dx matrix.
            /// </summary>
            /// <param name="m">The m.</param>
            /// <returns></returns>
            public static global::SharpDX.Matrix ToSharpDXMatrix(this Matrix4x4 m)
            {
                var matrix = new global::SharpDX.Matrix(m.A1, m.A2, m.A3, m.A4, m.B1, m.B2, m.B3, m.B4, m.C1, m.C2, m.C3, m.C4, m.D1, m.D2, m.D3, m.D4);
                matrix.Transpose();
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
            /// To the sharp dx vector2.
            /// </summary>
            /// <param name="v">The v.</param>
            /// <returns></returns>
            public static global::SharpDX.Vector2 ToSharpDXVector2(this Vector2D v)
            {
                return new global::SharpDX.Vector2(v.X, v.Y);
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
            /// To the sharp dx quaternion.
            /// </summary>
            /// <param name="q">The q.</param>
            /// <returns></returns>
            public static global::SharpDX.Quaternion ToSharpDXQuaternion(this Quaternion q)
            {
                return new global::SharpDX.Quaternion(q.X, q.Y, q.Z, q.W);
            }
        }
    }

}
