/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;
#if !CORE
#if NETFX_CORE
using Media = Windows.UI.Xaml.Media;
#else
using Media = System.Windows.Media;
#endif
#endif
using global::SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public static class MatrixExtensions
    {
#if !CORE
        public static Matrix3x3 ToMatrix3x3(this Media.Matrix m)
        {
            return new Matrix3x3((float)m.M11, (float)m.M12, 0, (float)m.M21, (float)m.M22, 0f, (float)m.OffsetX, (float)m.OffsetY, 1f);
        }
        public static Matrix3x2 ToMatrix3x2(this Media.Matrix m)
        {
            return new Matrix3x2((float)m.M11, (float)m.M12, (float)m.M21, (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
        }
#endif
        /// <summary>
        /// Pseudo inversion
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <returns></returns>
        public static Matrix PsudoInvert(ref Matrix viewMatrix)
        {
            //var v33Transpose = new Matrix3x3(
            //    viewMatrix.M11, viewMatrix.M21, viewMatrix.M31,
            //    viewMatrix.M12, viewMatrix.M22, viewMatrix.M32,
            //    viewMatrix.M13, viewMatrix.M23, viewMatrix.M33);

            //var vpos = viewMatrix.Row4.ToVector3();

            //     vpos = Vector3.Transform(vpos, v33Transpose) * -1;

            var x = viewMatrix.M41 * viewMatrix.M11 + viewMatrix.M42 * viewMatrix.M12 + viewMatrix.M43 * viewMatrix.M13;
            var y = viewMatrix.M41 * viewMatrix.M21 + viewMatrix.M42 * viewMatrix.M22 + viewMatrix.M43 * viewMatrix.M23;
            var z = viewMatrix.M41 * viewMatrix.M31 + viewMatrix.M42 * viewMatrix.M32 + viewMatrix.M43 * viewMatrix.M33;

            return new Matrix(
                viewMatrix.M11, viewMatrix.M21, viewMatrix.M31, 0,
                viewMatrix.M12, viewMatrix.M22, viewMatrix.M32, 0,
                viewMatrix.M13, viewMatrix.M23, viewMatrix.M33, 0, -x, -y, -z, 1);
        }
        /// <summary>
        /// Pseudo  inversion
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <returns></returns>
        public static Matrix PsudoInvert(this Matrix viewMatrix)
        {
            //var v33Transpose = new Matrix3x3(
            //    viewMatrix.M11, viewMatrix.M21, viewMatrix.M31,
            //    viewMatrix.M12, viewMatrix.M22, viewMatrix.M32,
            //    viewMatrix.M13, viewMatrix.M23, viewMatrix.M33);

            //var vpos = viewMatrix.Row4.ToVector3();

            //     vpos = Vector3.Transform(vpos, v33Transpose) * -1;

            var x = viewMatrix.M41 * viewMatrix.M11 + viewMatrix.M42 * viewMatrix.M12 + viewMatrix.M43 * viewMatrix.M13;
            var y = viewMatrix.M41 * viewMatrix.M21 + viewMatrix.M42 * viewMatrix.M22 + viewMatrix.M43 * viewMatrix.M23;
            var z = viewMatrix.M41 * viewMatrix.M31 + viewMatrix.M42 * viewMatrix.M32 + viewMatrix.M43 * viewMatrix.M33;

            return new Matrix(
                viewMatrix.M11, viewMatrix.M21, viewMatrix.M31, 0,
                viewMatrix.M12, viewMatrix.M22, viewMatrix.M32, 0,
                viewMatrix.M13, viewMatrix.M23, viewMatrix.M33, 0, -x, -y, -z, 1);
        }
    }
}
