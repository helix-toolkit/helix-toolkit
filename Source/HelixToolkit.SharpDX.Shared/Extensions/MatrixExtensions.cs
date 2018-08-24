/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public static class MatrixExtensions
    {
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
