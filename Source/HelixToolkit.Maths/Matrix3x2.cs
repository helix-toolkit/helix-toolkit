/*
The MIT License (MIT)
Copyright (c) 2022 Helix Toolkit contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Original code from:
SharpDX project. https://github.com/sharpdx/SharpDX
SlimMath project. http://code.google.com/p/slimmath/

Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
The MIT License (MIT)
Copyright (c) 2007-2011 SlimDX Group
The MIT License (MIT)
*/
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;

namespace HelixToolkit.Maths
{
    /// <summary>
    /// Direct2D Matrix3x2. Supports implicit cast from <see cref="Matrix"/>.
    /// </summary>
    public static class Matrix3x2Helper
    {
        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        /// <value>The identity matrix.</value>
        public readonly static Matrix3x2 Identity = new Matrix3x2(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Gets or sets the first row in the matrix; that is M11 and M12.
        /// <paramref name="m"/>>
        /// </summary>
        public static Vector2 Row1(this Matrix3x2 m)
        {
            return new Vector2(m.M11, m.M12);
        }
        /// <summary>
        /// Sets the row1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow1(ref Matrix3x2 m, ref Vector2 value)
        {
            m.M11 = value.X;
            m.M12 = value.Y;
        }
        /// <summary>
        /// Sets the row1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow1(ref Matrix3x2 m, Vector2 value)
        {
            m.M11 = value.X;
            m.M12 = value.Y;
        }
        /// <summary>
        /// Gets or sets the second row in the matrix; that is M21 and M22.
        /// </summary>
        public static Vector2 Row2(this Matrix3x2 m)
        {
            return new Vector2(m.M21, m.M22);
        }
        /// <summary>
        /// Sets the row2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow2(ref Matrix3x2 m, ref Vector2 value)
        {
            m.M21 = value.X; m.M22 = value.Y;
        }
        /// <summary>
        /// Sets the row2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow2(ref Matrix3x2 m, Vector2 value)
        {
            m.M21 = value.X; m.M22 = value.Y;
        }
        /// <summary>
        /// Gets or sets the third row in the matrix; that is M31 and M32.
        /// </summary>
        public static Vector2 Row3(this Matrix3x2 m)
        {
            return new Vector2(m.M31, m.M32);
        }
        /// <summary>
        /// Sets the row3.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow3(ref Matrix3x2 m, ref Vector2 value)
        {
            m.M31 = value.X; m.M32 = value.Y;
        }
        /// <summary>
        /// Sets the row3.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetRow3(ref Matrix3x2 m, Vector2 value)
        {
            m.M31 = value.X; m.M32 = value.Y;
        }
        /// <summary>
        /// Gets or sets the first column in the matrix; that is M11, M21, and M31.
        /// </summary>
        public static Vector3 Column1(this Matrix3x2 m)
        {
            return new Vector3(m.M11, m.M21, m.M31);
        }
        /// <summary>
        /// Sets the column1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetColumn1(ref Matrix3x2 m, ref Vector3 value)
        {
            m.M11 = value.X; m.M21 = value.Y; m.M31 = value.Z;
        }
        /// <summary>
        /// Sets the column1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetColumn1(ref Matrix3x2 m, Vector3 value)
        {
            m.M11 = value.X; m.M21 = value.Y; m.M31 = value.Z;
        }

        /// <summary>
        /// Gets or sets the second column in the matrix; that is M12, M22, and M32.
        /// </summary>
        public static Vector3 Column2(this Matrix3x2 m)
        {
            return new Vector3(m.M12, m.M22, m.M32);
        }
        /// <summary>
        /// Sets the column2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetColumn2(ref Matrix3x2 m, ref Vector3 value)
        {
            m.M12 = value.X; m.M22 = value.Y; m.M32 = value.Z;
        }
        /// <summary>
        /// Sets the column2.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetColumn2(ref Matrix3x2 m, Vector3 value)
        {
            m.M12 = value.X; m.M22 = value.Y; m.M32 = value.Z;
        }
        /// <summary>
        /// Sets the translation.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetTranslation(ref Matrix3x2 m, ref Vector2 value)
        {
            m.M31 = value.X;
            m.M32 = value.Y;
        }
        /// <summary>
        /// Sets the translation.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetTranslation(ref Matrix3x2 m, Vector2 value)
        {
            m.M31 = value.X;
            m.M32 = value.Y;
        }
        /// <summary>
        /// Create a matrix from translation vector.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Matrix3x2 Translation(Vector2 value)
        {
            var m = Matrix3x2.Identity;
            SetTranslation(ref m, ref value);
            return m;
        }

        /// <summary>
        /// Gets or sets the scale of the matrix; that is M11 and M22.
        /// </summary>
        public static Vector2 ScaleVector(this Matrix3x2 m)
        {
            return new Vector2(m.M11, m.M22);
        }
        /// <summary>
        /// Sets the scale vector.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetScaleVector(ref Matrix3x2 m, Vector2 value)
        {
            m.M11 = value.X; m.M22 = value.Y;
        }
        /// <summary>
        /// Sets the scale vector.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="value">The value.</param>
        public static void SetScaleVector(ref Matrix3x2 m, ref Vector2 value)
        {
            m.M11 = value.X; m.M22 = value.Y;
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="m"></param>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 5].</exception>
        public static float Get(this Matrix3x2 m, int index)
        {
                switch (index)
                {
                    case 0:  return m.M11;
                    case 1:  return m.M12;
                    case 2:  return m.M21;
                    case 3:  return m.M22;
                    case 4:  return m.M31;
                    case 5:  return m.M32;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x2 run from 0 to 5, inclusive.");
        }
        /// <summary>
        /// Sets the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException">index - Indices for Matrix3x2 run from 0 to 5, inclusive.</exception>
        public static void Set(ref Matrix3x2 m, int index, float value)
        {
                switch (index)
                {
                    case 0: m.M11 = value; break;
                    case 1: m.M12 = value; break;
                    case 2: m.M21 = value; break;
                    case 3: m.M22 = value; break;
                    case 4: m.M31 = value; break;
                    case 5: m.M32 = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Matrix3x2 run from 0 to 5, inclusive.");
                }
        }
        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="m"></param>
        /// <param name="row">The row of the matrix to access.</param>
        /// <param name="column">The column of the matrix to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
        public static float Get(this Matrix3x2 m, int row, int column)
        {
                if (row < 0 || row > 2)
            {
                throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
            }

            return column < 0 || column > 1
                ? throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 1, inclusive.")
                : m.Get((row * 2) + column);
        }
        /// <summary>
        /// Sets the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// row - Rows and columns for matrices run from 0 to 2, inclusive.
        /// or
        /// column - Rows and columns for matrices run from 0 to 1, inclusive.
        /// </exception>
        public static void Set(ref Matrix3x2 m, int row, int column, float value)
        {
            if (row < 0 || row > 2)
            {
                throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 2, inclusive.");
            }

            if (column < 0 || column > 1)
            {
                throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 1, inclusive.");
            }

            Set(ref m, (row * 2) + column, value);
        }

        /// <summary>
        /// Creates an array containing the elements of the matrix.
        /// </summary>
        /// <returns>A sixteen-element array containing the components of the matrix.</returns>
        public static float[] ToArray(this Matrix3x2 m)
        {
            return new[] { m.M11, m.M12, m.M21, m.M22, m.M31, m.M32 };
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two matrices.</param>
        public static void SmoothStep(ref Matrix3x2 start, ref Matrix3x2 end, float amount, out Matrix3x2 result)
        {
            amount = MathUtil.SmoothStep(amount);
            result = Matrix3x2.Lerp(start, end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two matrices.</returns>
        public static Matrix3x2 SmoothStep(Matrix3x2 start, Matrix3x2 end, float amount)
        {
            amount = MathUtil.SmoothStep(amount);
            return Matrix3x2.Lerp(start, end, amount);
        }

        /// <summary>
        /// Transforms a vector by this matrix.
        /// </summary>
        /// <param name="matrix">The matrix to use as a transformation matrix.</param>
        /// <param name="point">The original vector to apply the transformation.</param>
        /// <returns>The result of the transformation for the input vector.</returns>
        public static Vector2 TransformPoint(Matrix3x2 matrix, Vector2 point)
        {
            return Vector2.Transform(point, matrix);
        }

        /// <summary>
        /// Transforms a vector by this matrix.
        /// </summary>
        /// <param name="matrix">The matrix to use as a transformation matrix.</param>
        /// <param name="point">The original vector to apply the transformation.</param>
        /// <param name="result">The result of the transformation for the input vector.</param>
        /// <returns></returns>
        public static void TransformPoint(ref Matrix3x2 matrix, ref Vector2 point, out Vector2 result)
        {
            result = Vector2.Transform(point, matrix);
        }

        /// <summary>
        /// Creates a skew matrix.
        /// </summary>
        /// <param name="angleX">Angle of skew along the X-axis in radians.</param>
        /// <param name="angleY">Angle of skew along the Y-axis in radians.</param>
        /// <returns>The created skew matrix.</returns>
        public static Matrix3x2 Skew(float angleX, float angleY)
        {
            Matrix3x2 result;
            Skew(angleX, angleY, out result);
            return result;
        }

        /// <summary>
        /// Creates a skew matrix.
        /// </summary>
        /// <param name="angleX">Angle of skew along the X-axis in radians.</param>
        /// <param name="angleY">Angle of skew along the Y-axis in radians.</param>
        /// <param name="result">When the method completes, contains the created skew matrix.</param>
        public static void Skew(float angleX, float angleY, out Matrix3x2 result)
        {
            result = Identity;
            result.M12 = (float) Math.Tan(angleX);
            result.M21 = (float) Math.Tan(angleY);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Matrix"/> to <see cref="Matrix3x2"/>.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the conversion.</returns>
        public static Matrix3x2 ToMatrix3x2(this Matrix matrix)
        {
            return new Matrix3x2
            {
                M11 = matrix.M11,
                M12 = matrix.M12,
                M21 = matrix.M21,
                M22 = matrix.M22,
                M31 = matrix.M41,
                M32 = matrix.M42
            };
        }
    }
}
