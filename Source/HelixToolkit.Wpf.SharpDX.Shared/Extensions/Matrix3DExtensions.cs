// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Matrix3DExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Media3D;

    public static class Matrix3DExtensions
    {
        //private static MathNet.Numerics.Algorithms.LinearAlgebra.Mkl.MklLinearAlgebraProvider mklSolver = new MathNet.Numerics.Algorithms.LinearAlgebra.Mkl.MklLinearAlgebraProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D AddMatrix(this Matrix3D m1, Matrix3D m2)
        {
            return new Matrix3D(
                (m1.M11 + m2.M11),
                (m1.M12 + m2.M12),
                (m1.M13 + m2.M13),
                (m1.M14 + m2.M14),
                (m1.M21 + m2.M21),
                (m1.M22 + m2.M22),
                (m1.M23 + m2.M23),
                (m1.M24 + m2.M24),
                (m1.M31 + m2.M31),
                (m1.M32 + m2.M32),
                (m1.M33 + m2.M33),
                (m1.M34 + m2.M34),
                (m1.M44 + m2.M44),
                (m1.OffsetX + m2.OffsetX),
                (m1.OffsetY + m2.OffsetY),
                (m1.OffsetZ + m2.OffsetZ)
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D MultiplyMatrix(this Matrix3D m, double scalar)
        {
            return new Matrix3D(
                scalar * (m.M11),
                scalar * (m.M12),
                scalar * (m.M13),
                scalar * (m.M14),
                scalar * (m.M21),
                scalar * (m.M22),
                scalar * (m.M23),
                scalar * (m.M24),
                scalar * (m.M31),
                scalar * (m.M32),
                scalar * (m.M33),
                scalar * (m.M34),
                scalar * (m.M44),
                scalar * (m.OffsetX),
                scalar * (m.OffsetY),
                scalar * (m.OffsetZ)
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Translate3D(global::SharpDX.Vector3 v)
        {
            var m = Matrix3D.Identity;
            m.OffsetX = v.X;
            m.OffsetY = v.Y;
            m.OffsetZ = v.Z;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Translate3D(Vector3D v)
        {
            var m = Matrix3D.Identity;
            m.OffsetX = v.X;
            m.OffsetY = v.Y;
            m.OffsetZ = v.Z;
            return m;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Translate3D(double x, double y, double z)
        {
            var m = Matrix3D.Identity;
            m.OffsetX = x;
            m.OffsetY = y;
            m.OffsetZ = z;
            return m;
        }

        /// <summary>
        /// Computes an optimal similarity transform for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A similarity transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Similarity2D(IList<Point3D> p, IList<Point3D> q)
        {
            double a = 0;
            double b = 0;
            double c = 0;
            var n = p.Count;
            for (var i = 0; i < n; i++)
            {
                a += (p[i].X * p[i].X + p[i].Y * p[i].Y);
                b += (p[i].X * q[i].X + p[i].Y * q[i].Y);
                c += (p[i].Y * q[i].X - p[i].X * q[i].Y);
            }

            var r1 = b / a;
            var r2 = c / a;
            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an optimal rigid transform for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A rigid transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Rigid2D(IList<Point3D> p, IList<Point3D> q)
        {
            var n = p.Count;
            double b = 0;
            double c = 0;

            for (var i = 0; i < n; i++)
            {
                b += (p[i].X * q[i].X + p[i].Y * q[i].Y);
                c += (p[i].Y * q[i].X - p[i].X * q[i].Y);
            }
            var d = Math.Sqrt(b * b + c * c);
            var r1 = b / d;
            var r2 = c / d;


            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an optimal rigid transform for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A rigid transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Rigid2D(IList<Vector3D> p, IList<Vector3D> q)
        {
            var n = p.Count;
            double b = 0;
            double c = 0;

            for (var i = 0; i < n; i++)
            {
                b += (p[i].X * q[i].X + p[i].Y * q[i].Y);
                c += (p[i].Y * q[i].X - p[i].X * q[i].Y);
            }
            var d = Math.Sqrt(b * b + c * c);
            var r1 = b / d;
            var r2 = c / d;


            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an optimal similarity transform for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="w">weight: importance of each point </param>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A similarity transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Similarity2D(IList<double> w, IList<Point3D> p, IList<Point3D> q)
        {
            var n = w.Count;
            double a = 0;
            double b = 0;
            double c = 0;

            for (var i = 0; i < n; i++)
            {
                a += w[i] * (p[i].X * p[i].X + p[i].Y * p[i].Y);
                b += w[i] * (p[i].X * q[i].X + p[i].Y * q[i].Y);
                c += w[i] * (p[i].Y * q[i].X - p[i].X * q[i].Y);
            }

            var r1 = b / a;
            var r2 = c / a;
            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an optimal rigid local rotation for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="w">weight: importance of each point </param>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A rigid transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Rigid2D(IList<double> w, IList<Point3D> p, IList<Point3D> q)
        {
            var n = w.Count;
            double b = 0;
            double c = 0;

            for (var i = 0; i < n; i++)
            {
                b += w[i] * (p[i].X * q[i].X + p[i].Y * q[i].Y);
                c += w[i] * (p[i].Y * q[i].X - p[i].X * q[i].Y);
            }
            var d = Math.Sqrt(b * b + c * c);
            var r1 = b / d;
            var r2 = c / d;

            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an optimal affine transform for two points sets.
        /// This function assums that the translational component has been already removed, i.e. 
        /// that both point sets have the same centriod. 
        /// </summary>
        /// <param name="w">weight: importance of each point </param>
        /// <param name="p">original points</param>
        /// <param name="q">transformed points</param>
        /// <returns>A affine transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D Affine2D(IList<double> w, IList<Point3D> p, IList<Point3D> q)
        {
            var n = w.Count;
            double b1, b2, b3, b4;
            b1 = b2 = b3 = b4 = 0;
            double m11 = 0, m12 = 0, m22 = 0;

            for (var i = 0; i < n; i++)
            {
                m11 += w[i] * p[i].X * p[i].X;
                m12 += w[i] * p[i].X * p[i].Y;
                m22 += w[i] * p[i].Y * p[i].Y;

                b1 += w[i] * p[i].X * q[i].X;
                b2 += w[i] * p[i].Y * q[i].X;
                b3 += w[i] * p[i].X * q[i].Y;
                b4 += w[i] * p[i].Y * q[i].Y;
            }

            // M = [m11, m12    Mi = [m22, -m12
            //      m12, m22];        -m12, m11]

            //var A = new Matrix3D();
            //for (int i = 0; i < 4; i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        A[i, j] = 0;
            //    }
            //}
            //A[0,0] = m11; A[0,1] = m12;
            //A[1,0] = m12; A[1,1] = m22;
            //A[2,2] = m11; A[2,3] = m12;
            //A[3,2] = m12; A[3,3] = m22;

            //var det = m11 * m22 - m12 * m12;

            //A[0, 0] = det * m22; A[0, 1] = -det * m12;
            //A[1, 0] = -det * m12; A[1, 1] = det * m11;
            //A[2, 2] = det * m22; A[2, 3] = -det * m12;
            //A[3, 2] = -det * m12; A[3, 3] = det * m11;

            //var B = new Vector3D(b);

            //var T = A * B;


            var t1 = m22 * b1 - m12 * b2;
            var t2 = -m12 * b1 + m11 * b2;
            var t3 = m22 * b3 - m12 * b4;
            var t4 = -m12 * b3 + m11 * b4;

            return new Matrix3D(
                t1, t2, 0, 0,
                t3, t1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        /// <summary>
        /// Computes an ansiotropic similarty for an object, which is only scaled along the X-axis, 
        /// but maintains its Y-scale constantly.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns>A ansiotropic similarty transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D AnisotropicSimilarityX2D(Point3D p1, Point3D p2, Point3D q1, Point3D q2)
        {
            // rotation
            var b = (p1.X * q1.X + p1.Y * q1.Y) + (p2.X * q2.X + p2.Y * q2.Y);
            var c = (p1.Y * q1.X - p1.X * q1.Y) + (p2.Y * q2.X - p2.X * q2.Y);
            var d = Math.Sqrt(b * b + c * c);
            var r1 = b / d;
            var r2 = c / d;

            // anisortropic scale
            var s = Math.Sqrt(q1.X * q1.X + q1.Y * q1.Y) / Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y);

            var m = new Matrix3D(
                +s * r1, -s * r2, 0, 0,
                +r2, +r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }

        /// <summary>
        /// Computes an ansiotropic similarty for an object, which is only scaled along the Y-axis, 
        /// but maintains its X-scale constantly.
        /// </summary>                    
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns>A ansiotropic similarty transform T: p = Tq</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D AnisotropicSimilarityY2D(Point3D p1, Point3D p2, Point3D q1, Point3D q2)
        {
            // rotation
            var b = (p1.X * q1.X + p1.Y * q1.Y) + (p2.X * q2.X + p2.Y * q2.Y);
            var c = (p1.Y * q1.X - p1.X * q1.Y) + (p2.Y * q2.X - p2.X * q2.Y);
            var d = Math.Sqrt(b * b + c * c);
            var r1 = b / d;
            var r2 = c / d;

            // anisortropic scale
            var s = Math.Sqrt(q1.X * q1.X + q1.Y * q1.Y) / Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y);

            var m = new Matrix3D(
                +r1, -r2, 0, 0,
                +s * r2, +s * r1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            return m;
        }


#if REQUIRES_SVD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Matrix3D Rigid3D(IList<Vector3D> p, IList<Vector3D> q)
        {
            int n = p.Count;
            double[] m = new double[9];
            for (int i = 0; i < n; i++)
            {
                m[0] += p[i].X * q[i].X;
                m[1] += p[i].X * q[i].Y;
                m[2] += p[i].X * q[i].Z;
                m[3] += p[i].Y * q[i].X;
                m[4] += p[i].Y * q[i].Y;
                m[5] += p[i].Y * q[i].Z;
                m[6] += p[i].Z * q[i].X;
                m[7] += p[i].Z * q[i].Y;
                m[8] += p[i].Z * q[i].Z;
            }

            var M = new MathNet.Numerics.LinearAlgebra.Double.DenseMatrix(3, 3, m);
            //double[] S = new double[3];
            //double[] U = new double[9];
            //double[] VT = new double[9];
            //mklSolver.SingularValueDecomposition(true, M.Data, 3, 3, S, U, VT);
            //var RR = new Matrix3D()
            //{
            //    M11 = U[0] * VT[0] + U[1] * VT[3] * U[2] * VT[6],
            //    M12 = U[0] * VT[1] + U[1] * VT[4] * U[2] * VT[7],
            //    M13 = U[0] * VT[2] + U[1] * VT[5] * U[2] * VT[8],

            //    M21 = U[3] * VT[0] + U[4] * VT[3] * U[5] * VT[6],
            //    M22 = U[3] * VT[1] + U[4] * VT[4] * U[5] * VT[7],
            //    M23 = U[3] * VT[2] + U[4] * VT[5] * U[5] * VT[8],

            //    M31 = U[6] * VT[0] + U[7] * VT[3] * U[8] * VT[6],
            //    M32 = U[6] * VT[1] + U[7] * VT[4] * U[8] * VT[7],
            //    M33 = U[6] * VT[2] + U[7] * VT[5] * U[8] * VT[8],
            //};
            //return RR;

            // R = V*UT <==> RT = VT*U
            var svd = M.Svd(true);
            var RT = svd.U().Multiply(svd.VT());
            var R = new Matrix3D()
            {
                M11 = RT[0, 0],
                M12 = RT[1, 0],
                M13 = RT[2, 0],
                M21 = RT[0, 1],
                M22 = RT[1, 1],
                M23 = RT[2, 1],
                M31 = RT[0, 2],
                M32 = RT[1, 2],
                M33 = RT[2, 2],
                //M11 = RT[0, 0],
                //M12 = RT[0, 1],
                //M13 = RT[0, 2],
                //M21 = RT[1, 0],
                //M22 = RT[1, 1],
                //M23 = RT[1, 2],
                //M31 = RT[2, 0],
                //M32 = RT[2, 1],
                //M33 = RT[2, 2],
                M44 = 1.0,
            };

            return R;
        } 
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D OuterProduct(this Vector3D v1, Vector3D v2)
        {
            var m11 = v1.X * v2.X;
            var m12 = v1.X * v2.Y;
            var m13 = v1.X * v2.Z;

            var m21 = v1.Y * v2.X;
            var m22 = v1.Y * v2.Y;
            var m23 = v1.Y * v2.Z;

            var m31 = v1.Z * v2.X;
            var m32 = v1.Z * v2.Y;
            var m33 = v1.Z * v2.Z;

            return new Matrix3D(m11, m12, m13, 0, m21, m22, m23, 0, m31, m32, m33, 0, 0, 0, 0, 0);
            ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Point ToPoint(this System.Windows.Vector v)
        {
            return new System.Windows.Point(v.X, v.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static global::SharpDX.Vector2 ToVector2(this System.Windows.Vector v)
        {
            return new global::SharpDX.Vector2((float)v.X, (float)v.Y);
        }
    }
}