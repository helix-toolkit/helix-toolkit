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
using Microsoft.Extensions.Logging;

namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a 4x4 mathematical matrix.
    /// </summary>
    public static class MatrixHelper
    {
        private static readonly ILogger logger = Logger.LogManager.Create(nameof(MatrixHelper));
        /// <summary>
        /// The size of the <see cref="Matrix"/> type, in bytes.
        /// </summary>
        public static readonly uint SizeInBytes = 4 * 4 * sizeof(float);

        /// <summary>
        /// A <see cref="Matrix"/> with all of its components set to zero.
        /// </summary>
        public static readonly Matrix Zero = new();

        /// <summary>
        /// The identity <see cref="Matrix"/>.
        /// </summary>
        public static readonly Matrix Identity = new() { M11 = 1.0f, M22 = 1.0f, M33 = 1.0f, M44 = 1.0f };


        /// <summary>
        /// Gets or sets the up <see cref="Vector3"/> of the matrix; that is M21, M22, and M23.
        /// </summary>
        public static Vector3 Up(this Matrix m)
        {
            return new Vector3(m.M21, m.M22, m.M23);
        }

        public static void SetUp(ref Matrix m, ref Vector3 up)
        {
            m.M21 = up.X;
            m.M22 = up.Y;
            m.M23 = up.Z;
        }

        /// <summary>
        /// Gets or sets the down <see cref="Vector3"/> of the matrix; that is -M21, -M22, and -M23.
        /// </summary>
        public static Vector3 Down(this Matrix m)
        {
            return new Vector3(-m.M21, -m.M22, -m.M23);
        }

        public static void SetDown(ref Matrix m, ref Vector3 value)
        {
            m.M21 = -value.X;
            m.M22 = -value.Y;
            m.M23 = -value.Z;
        }

        /// <summary>
        /// Gets or sets the right <see cref="Vector3"/> of the matrix; that is M11, M12, and M13.
        /// </summary>
        public static Vector3 Right(this Matrix m)
        {
            return new Vector3(m.M11, m.M12, m.M13);
        }

        public static void SetRight(ref Matrix m, ref Vector3 value)
        {
            m.M11 = value.X;
            m.M12 = value.Y;
            m.M13 = value.Z;
        }

        /// <summary>
        /// Gets or sets the left <see cref="Vector3"/> of the matrix; that is -M11, -M12, and -M13.
        /// </summary>
        public static Vector3 Left(this Matrix m)
        {
            return new Vector3(-m.M11, -m.M12, -m.M13);
        }

        public static void SetLeft(ref Matrix m, ref Vector3 value)
        {
            m.M11 = -value.X;
            m.M12 = -value.Y;
            m.M13 = -value.Z;
        }
        /// <summary>
        /// Gets or sets the forward <see cref="Vector3"/> of the matrix; that is -M31, -M32, and -M33.
        /// </summary>
        public static Vector3 Forward(this Matrix m)
        {
            return new Vector3(-m.M31, -m.M32, -m.M33);
        }

        public static void SetForward(ref Matrix m, ref Vector3 value)
        {
            m.M31 = -value.X;
            m.M32 = -value.Y;
            m.M33 = -value.Z;
        }
        /// <summary>
        /// Gets or sets the backward <see cref="Vector3"/> of the matrix; that is M31, M32, and M33.
        /// </summary>
        public static Vector3 Backward(this Matrix m)
        {
            return new Vector3(m.M31, m.M32, m.M33);
        }

        public static void SetBackward(ref Matrix m, ref Vector3 value)
        {
            m.M31 = value.X;
            m.M32 = value.Y;
            m.M33 = value.Z;
        }


        /// <summary>
        /// Gets or sets the first row in the matrix; that is M11, M12, M13, and M14.
        /// </summary>
        public static Vector4 Row1(this Matrix m)
        {
            return new Vector4(m.M11, m.M12, m.M13, m.M14);
        }

        public static void SetRow1(ref Matrix m, ref Vector4 value)
        {
            m.M11 = value.X; m.M12 = value.Y; m.M13 = value.Z; m.M14 = value.W;
        }
        public static void SetRow1(ref Matrix m, Vector4 value)
        {
            m.M11 = value.X; m.M12 = value.Y; m.M13 = value.Z; m.M14 = value.W;
        }
        /// <summary>
        /// Gets or sets the second row in the matrix; that is M21, M22, M23, and M24.
        /// </summary>
        public static Vector4 Row2(this Matrix m)
        {
            return new Vector4(m.M21, m.M22, m.M23, m.M24);
        }

        public static void SetRow2(ref Matrix m, ref Vector4 value)
        {
            m.M21 = value.X; m.M22 = value.Y; m.M23 = value.Z; m.M24 = value.W;
        }
        public static void SetRow2(ref Matrix m, Vector4 value)
        {
            m.M21 = value.X; m.M22 = value.Y; m.M23 = value.Z; m.M24 = value.W;
        }
        /// <summary>
        /// Gets or sets the third row in the matrix; that is M31, M32, M33, and M34.
        /// </summary>
        public static Vector4 Row3(this Matrix m)
        {
            return new Vector4(m.M31, m.M32, m.M33, m.M34);
        }

        public static void SetRow3(ref Matrix m, ref Vector4 value)
        {
            m.M31 = value.X; m.M32 = value.Y; m.M33 = value.Z; m.M34 = value.W;
        }
        public static void SetRow3(ref Matrix m, Vector4 value)
        {
            m.M31 = value.X; m.M32 = value.Y; m.M33 = value.Z; m.M34 = value.W;
        }
        /// <summary>
        /// Gets or sets the fourth row in the matrix; that is M41, M42, M43, and M44.
        /// </summary>
        public static Vector4 Row4(this Matrix m)
        {
            return new Vector4(m.M41, m.M42, m.M43, m.M44);
        }

        public static void SetRow4(ref Matrix m, ref Vector4 value)
        {
            m.M41 = value.X; m.M42 = value.Y; m.M43 = value.Z; m.M44 = value.W;
        }
        public static void SetRow4(ref Matrix m, Vector4 value)
        {
            m.M41 = value.X; m.M42 = value.Y; m.M43 = value.Z; m.M44 = value.W;
        }

        /// <summary>
        /// Gets the row. Zero based index. Row Index = 0 will get row 1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="rowIdx">Index of the row.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Row Index out of bound.</exception>
        public static Vector4 GetRow(this Matrix m, int rowIdx)
        {
            return rowIdx switch
            {
                0 => m.Row1(),
                1 => m.Row2(),
                2 => m.Row3(),
                3 => m.Row4(),
                _ => throw new ArgumentException("Row Index out of bound."),
            };
        }
        /// <summary>
        /// Sets the row by index. Index 0 sets row 1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="rowIdx">Index of the row.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">Row Index out of bound.</exception>
        public static void SetRow(ref Matrix m, int rowIdx, Vector4 value)
        {
            switch (rowIdx)
            {
                case 0: SetRow1(ref m, ref value); break;
                case 1: SetRow2(ref m, ref value); break;
                case 2: SetRow3(ref m, ref value); break;
                case 3: SetRow4(ref m, ref value); break;
                default: throw new ArgumentException("Row Index out of bound.");
            }
        }
        /// <summary>
        /// Gets the column. Zero based index. column Index = 0 will get column 1.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="columnIdx">Index of the row.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Column Index out of bound.</exception>
        public static Vector4 GetColumn(this Matrix m, int columnIdx)
        {
            return columnIdx switch
            {
                0 => m.Column1(),
                1 => m.Column2(),
                2 => m.Column3(),
                3 => m.Column4(),
                _ => throw new ArgumentException("Column Index out of bound."),
            };
        }
        /// <summary>
        /// Sets the column by index. Index 0 sets column 1;
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="columnIdx">Index of the column.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">Column Index out of bound.</exception>
        public static void SetColumn(ref Matrix m, int columnIdx, Vector4 value)
        {
            switch (columnIdx)
            {
                case 0: SetColumn1(ref m, ref value); break;
                case 1: SetColumn2(ref m, ref value); break;
                case 2: SetColumn3(ref m, ref value); break;
                case 3: SetColumn4(ref m, ref value); break;
                default: throw new ArgumentException("Column Index out of bound.");
            }
        }

        /// <summary>
        /// Gets or sets the first column in the matrix; that is M11, M21, M31, and M41.
        /// </summary>
        public static Vector4 Column1(this Matrix m)
        {
            return new Vector4(m.M11, m.M21, m.M31, m.M41);
        }

        public static void SetColumn1(ref Matrix m, ref Vector4 value)
        {
            m.M11 = value.X; m.M21 = value.Y; m.M31 = value.Z; m.M41 = value.W;
        }
        public static void SetColumn1(ref Matrix m, Vector4 value)
        {
            m.M11 = value.X; m.M21 = value.Y; m.M31 = value.Z; m.M41 = value.W;
        }
        /// <summary>
        /// Gets or sets the second column in the matrix; that is M12, M22, M32, and M42.
        /// </summary>
        public static Vector4 Column2(this Matrix m)
        {
            return new Vector4(m.M12, m.M22, m.M32, m.M42);
        }

        public static void SetColumn2(ref Matrix m, ref Vector4 value)
        {
            m.M12 = value.X; m.M22 = value.Y; m.M32 = value.Z; m.M42 = value.W;
        }
        public static void SetColumn2(ref Matrix m, Vector4 value)
        {
            m.M12 = value.X; m.M22 = value.Y; m.M32 = value.Z; m.M42 = value.W;
        }
        /// <summary>
        /// Gets or sets the third column in the matrix; that is M13, M23, M33, and M43.
        /// </summary>
        public static Vector4 Column3(this Matrix m)
        {
            return new Vector4(m.M13, m.M23, m.M33, m.M43);
        }

        public static void SetColumn3(ref Matrix m, ref Vector4 value)
        {
            m.M13 = value.X; m.M23 = value.Y; m.M33 = value.Z; m.M43 = value.W;
        }
        public static void SetColumn3(ref Matrix m, Vector4 value)
        {
            m.M13 = value.X; m.M23 = value.Y; m.M33 = value.Z; m.M43 = value.W;
        }
        /// <summary>
        /// Gets or sets the fourth column in the matrix; that is M14, M24, M34, and M44.
        /// </summary>
        public static Vector4 Column4(this Matrix m)
        {
            return new Vector4(m.M14, m.M24, m.M34, m.M44);
        }

        public static void SetColumn4(ref Matrix m, ref Vector4 value)
        {
            m.M14 = value.X; m.M24 = value.Y; m.M34 = value.Z; m.M44 = value.W;
        }
        public static void SetColumn4(ref Matrix m, Vector4 value)
        {
            m.M14 = value.X; m.M24 = value.Y; m.M34 = value.Z; m.M44 = value.W;
        }
        /// <summary>
        /// Gets or sets the scale of the matrix; that is M11, M22, and M33.
        /// </summary>
        public static Vector3 ScaleVector(this Matrix m)
        {
            return new Vector3(m.M11, m.M22, m.M33);
        }

        public static void SetScaleVector(ref Matrix m, ref Vector3 value)
        {
            m.M11 = value.X; m.M22 = value.Y; m.M33 = value.Z;
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the matrix component, depending on the index.</value>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <param name="m"></param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
        public static float Get(this Matrix m, int index)
        {
            return index switch
            {
                0 => m.M11,
                1 => m.M12,
                2 => m.M13,
                3 => m.M14,
                4 => m.M21,
                5 => m.M22,
                6 => m.M23,
                7 => m.M24,
                8 => m.M31,
                9 => m.M32,
                10 => m.M33,
                11 => m.M34,
                12 => m.M41,
                13 => m.M42,
                14 => m.M43,
                15 => m.M44,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive."),
            };
        }

        public static void Set(ref Matrix m, int index, float value)
        {
            switch (index)
            {
                case 0: m.M11 = value; break;
                case 1: m.M12 = value; break;
                case 2: m.M13 = value; break;
                case 3: m.M14 = value; break;
                case 4: m.M21 = value; break;
                case 5: m.M22 = value; break;
                case 6: m.M23 = value; break;
                case 7: m.M24 = value; break;
                case 8: m.M31 = value; break;
                case 9: m.M32 = value; break;
                case 10: m.M33 = value; break;
                case 11: m.M34 = value; break;
                case 12: m.M41 = value; break;
                case 13: m.M42 = value; break;
                case 14: m.M43 = value; break;
                case 15: m.M44 = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Matrix run from 0 to 15, inclusive.");
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
        public static float Get(this Matrix m, int row, int column)
        {
            return row < 0 || row > 3
                ? throw new ArgumentOutOfRangeException(nameof(row), "Rows and columns for matrices run from 0 to 3, inclusive.")
                : column < 0 || column > 3
                ? throw new ArgumentOutOfRangeException(nameof(column), "Rows and columns for matrices run from 0 to 3, inclusive.")
                : m.Get((row * 4) + column);
        }

        public static void Set(ref Matrix m, int row, int column, float value)
        {
            if (row < 0 || row > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(row), "Rows and columns for matrices run from 0 to 3, inclusive.");
            }

            if (column < 0 || column > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "Rows and columns for matrices run from 0 to 3, inclusive.");
            }

            Set(ref m, (row * 4) + column, value);
        }

        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthogonalize(ref Matrix m)
        {
            Orthogonalize(ref m, out m);
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthonormalize(ref Matrix m)
        {
            Orthonormalize(ref m, out m);
        }

        /// <summary>
        /// Decomposes a matrix into an orthonormalized matrix Q and a right triangular matrix R.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
        /// <param name="R">When the method completes, contains the right triangular matrix of the decomposition.</param>
        public static void DecomposeQR(this Matrix m, out Matrix Q, out Matrix R)
        {
            Matrix temp = Matrix.Transpose(m);
            Orthonormalize(ref temp, out Q);
            Q = Matrix.Transpose(Q);

            R = new Matrix
            {
                M11 = Vector4.Dot(Q.Column1(), m.Column1()),
                M12 = Vector4.Dot(Q.Column1(), m.Column2()),
                M13 = Vector4.Dot(Q.Column1(), m.Column3()),
                M14 = Vector4.Dot(Q.Column1(), m.Column4()),

                M22 = Vector4.Dot(Q.Column2(), m.Column2()),
                M23 = Vector4.Dot(Q.Column2(), m.Column3()),
                M24 = Vector4.Dot(Q.Column2(), m.Column4()),

                M33 = Vector4.Dot(Q.Column3(), m.Column3()),
                M34 = Vector4.Dot(Q.Column3(), m.Column4()),

                M44 = Vector4.Dot(Q.Column4(), m.Column4())
            };
        }

        /// <summary>
        /// Decomposes a matrix into a lower triangular matrix L and an orthonormalized matrix Q.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="L">When the method completes, contains the lower triangular matrix of the decomposition.</param>
        /// <param name="Q">When the method completes, contains the orthonormalized matrix of the decomposition.</param>
        public static void DecomposeLQ(this Matrix m, out Matrix L, out Matrix Q)
        {
            Orthonormalize(ref m, out Q);

            L = new Matrix
            {
                M11 = Vector4.Dot(Q.Row1(), m.Row1()),

                M21 = Vector4.Dot(Q.Row1(), m.Row2()),
                M22 = Vector4.Dot(Q.Row2(), m.Row2()),

                M31 = Vector4.Dot(Q.Row1(), m.Row3()),
                M32 = Vector4.Dot(Q.Row2(), m.Row3()),
                M33 = Vector4.Dot(Q.Row3(), m.Row3()),

                M41 = Vector4.Dot(Q.Row1(), m.Row4()),
                M42 = Vector4.Dot(Q.Row2(), m.Row4()),
                M43 = Vector4.Dot(Q.Row3(), m.Row4()),
                M44 = Vector4.Dot(Q.Row4(), m.Row4())
            };
        }

        /// <summary>
        /// Decomposes a uniform scale matrix into a scale, rotation, and translation.
        /// A uniform scale matrix has the same scale in every axis.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="scale">When the method completes, contains the scaling component of the decomposed matrix.</param>
        /// <param name="rotation">When the method completes, contains the rotation component of the decomposed matrix.</param>
        /// <param name="translation">When the method completes, contains the translation component of the decomposed matrix.</param>
        /// <remarks>
        /// This method is designed to decompose only an SRT transformation matrix that has the same scale in every axis.
        /// </remarks>
        public static bool DecomposeUniformScale(this Matrix m, out float scale, out Quaternion rotation, out Vector3 translation)
        {
            //Get the translation.
            translation.X = m.M41;
            translation.Y = m.M42;
            translation.Z = m.M43;

            //Scaling is the length of the rows. ( just take one row since this is a uniform matrix)
            scale = (float)Math.Sqrt((m.M11 * m.M11) + (m.M12 * m.M12) + (m.M13 * m.M13));
            float inv_scale = 1f / scale;

            //If any of the scaling factors are zero, then the rotation matrix can not exist.
            if (Math.Abs(scale) < MathUtil.ZeroTolerance)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            //The rotation is the left over matrix after dividing out the scaling.
            Matrix rotationmatrix = new()
            {
                M11 = m.M11 * inv_scale,
                M12 = m.M12 * inv_scale,
                M13 = m.M13 * inv_scale,

                M21 = m.M21 * inv_scale,
                M22 = m.M22 * inv_scale,
                M23 = m.M23 * inv_scale,

                M31 = m.M31 * inv_scale,
                M32 = m.M32 * inv_scale,
                M33 = m.M33 * inv_scale,

                M44 = 1f
            };

            rotation = Quaternion.CreateFromRotationMatrix(rotationmatrix);
            return true;
        }

        /// <summary>
        /// Exchanges two rows in the matrix.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="firstRow">The first row to exchange. This is an index of the row starting at zero.</param>
        /// <param name="secondRow">The second row to exchange. This is an index of the row starting at zero.</param>
        public static void ExchangeRows(ref Matrix m, int firstRow, int secondRow)
        {
            if (firstRow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(firstRow), "The parameter firstRow must be greater than or equal to zero.");
            }

            if (firstRow > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(firstRow), "The parameter firstRow must be less than or equal to three.");
            }

            if (secondRow < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(secondRow), "The parameter secondRow must be greater than or equal to zero.");
            }

            if (secondRow > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(secondRow), "The parameter secondRow must be less than or equal to three.");
            }

            if (firstRow == secondRow)
            {
                return;
            }

            float temp0 = m.Get(secondRow, 0);
            float temp1 = m.Get(secondRow, 1);
            float temp2 = m.Get(secondRow, 2);
            float temp3 = m.Get(secondRow, 3);

            Set(ref m, secondRow, 0, m.Get(firstRow, 0));
            Set(ref m, secondRow, 1, m.Get(firstRow, 1));
            Set(ref m, secondRow, 2, m.Get(firstRow, 2));
            Set(ref m, secondRow, 3, m.Get(firstRow, 3));

            Set(ref m, firstRow, 0, temp0);
            Set(ref m, firstRow, 1, temp1);
            Set(ref m, firstRow, 2, temp2);
            Set(ref m, firstRow, 3, temp3);
        }

        /// <summary>
        /// Exchanges two columns in the matrix.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="firstColumn">The first column to exchange. This is an index of the column starting at zero.</param>
        /// <param name="secondColumn">The second column to exchange. This is an index of the column starting at zero.</param>
        public static void ExchangeColumns(ref Matrix m, int firstColumn, int secondColumn)
        {
            if (firstColumn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(firstColumn), "The parameter firstColumn must be greater than or equal to zero.");
            }

            if (firstColumn > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(firstColumn), "The parameter firstColumn must be less than or equal to three.");
            }

            if (secondColumn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(secondColumn), "The parameter secondColumn must be greater than or equal to zero.");
            }

            if (secondColumn > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(secondColumn), "The parameter secondColumn must be less than or equal to three.");
            }

            if (firstColumn == secondColumn)
            {
                return;
            }

            float temp0 = m.Get(0, secondColumn);
            float temp1 = m.Get(1, secondColumn);
            float temp2 = m.Get(2, secondColumn);
            float temp3 = m.Get(3, secondColumn);

            Set(ref m, 0, secondColumn, m.Get(0, firstColumn));
            Set(ref m, 1, secondColumn, m.Get(1, firstColumn));
            Set(ref m, 2, secondColumn, m.Get(2, firstColumn));
            Set(ref m, 3, secondColumn, m.Get(3, firstColumn));

            Set(ref m, 0, firstColumn, temp0);
            Set(ref m, 1, firstColumn, temp1);
            Set(ref m, 2, firstColumn, temp2);
            Set(ref m, 3, firstColumn, temp3);
        }

        /// <summary>
        /// Creates an array containing the elements of the matrix.
        /// </summary>
        /// <param name="m"></param>
        /// <returns>A sixteen-element array containing the components of the matrix.</returns>
        public static float[] ToArray(this Matrix m)
        {
            return new[] { m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44 };
        }

        /// <summary>
        /// Performs the exponential operation on a matrix.
        /// </summary>
        /// <param name="value">The matrix to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the matrix to.</param>
        /// <param name="result">When the method completes, contains the exponential matrix.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static void Exponent(ref Matrix value, int exponent, out Matrix result)
        {
            //Source: http://rosettacode.org
            //Reference: http://rosettacode.org/wiki/Matrix-exponentiation_operator

            if (exponent < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exponent), "The exponent can not be negative.");
            }

            if (exponent == 0)
            {
                result = Matrix.Identity;
                return;
            }

            if (exponent == 1)
            {
                result = value;
                return;
            }

            Matrix identity = Matrix.Identity;
            Matrix temp = value;

            while (true)
            {
                if ((exponent & 1) != 0)
                {
                    identity *= temp;
                }

                exponent /= 2;

                if (exponent > 0)
                {
                    temp *= temp;
                }
                else
                {
                    break;
                }
            }

            result = identity;
        }

        /// <summary>
        /// Performs the exponential operation on a matrix.
        /// </summary>
        /// <param name="value">The matrix to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the matrix to.</param>
        /// <returns>The exponential matrix.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static Matrix Exponent(Matrix value, int exponent)
        {
            Exponent(ref value, exponent, out Matrix result);
            return result;
        }

        /// <summary>
        /// Negates a matrix.
        /// </summary>
        /// <param name="value">The matrix to be negated.</param>
        /// <param name="result">When the method completes, contains the negated matrix.</param>
        public static void Negate(ref Matrix value, out Matrix result)
        {
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two matrices.</param>
        public static void SmoothStep(ref Matrix start, ref Matrix end, float amount, out Matrix result)
        {
            amount = MathUtil.SmoothStep(amount);
            result = Matrix.Lerp(start, end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start matrix.</param>
        /// <param name="end">End matrix.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two matrices.</returns>
        public static Matrix SmoothStep(Matrix start, Matrix end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out Matrix result);
            return result;
        }

        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthogonalize.</param>
        /// <param name="result">When the method completes, contains the orthogonalized matrix.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthogonalize(ref Matrix value, out Matrix result)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            result = value;
            Vector4 row1 = result.Row1();
            SetRow2(ref result, result.Row2() - Vector4.Dot(row1, result.Row2()) / Vector4.Dot(row1, row1) * row1);
            Vector4 row2 = result.Row2();
            SetRow3(ref result, result.Row3() - Vector4.Dot(row1, result.Row3()) / Vector4.Dot(row1, row1) * row1);
            SetRow3(ref result, result.Row3() - Vector4.Dot(row2, result.Row3()) / Vector4.Dot(row2, row2) * row2);
            Vector4 row3 = result.Row3();
            SetRow4(ref result, result.Row4() - Vector4.Dot(row1, result.Row4()) / Vector4.Dot(row1, row1) * row1);
            SetRow4(ref result, result.Row4() - Vector4.Dot(row2, result.Row4()) / Vector4.Dot(row2, row2) * row2);
            SetRow4(ref result, result.Row4() - Vector4.Dot(row3, result.Row4()) / Vector4.Dot(row3, row3) * row3);
        }

        /// <summary>
        /// Orthogonalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthogonalize.</param>
        /// <returns>The orthogonalized matrix.</returns>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the matrix will be orthogonal to any other given row in the
        /// matrix.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static Matrix Orthogonalize(Matrix value)
        {
            Orthogonalize(ref value, out Matrix result);
            return result;
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthonormalize.</param>
        /// <param name="result">When the method completes, contains the orthonormalized matrix.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthonormalize(ref Matrix value, out Matrix result)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthonormalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            result = value;

            SetRow1(ref result, Vector4.Normalize(result.Row1()));
            Vector4 row1 = result.Row1();
            SetRow2(ref result, result.Row2() - Vector4.Dot(row1, result.Row2()) * row1);
            SetRow2(ref result, Vector4.Normalize(result.Row2()));
            Vector4 row2 = result.Row2();
            SetRow3(ref result, result.Row3() - Vector4.Dot(row1, result.Row3()) * row1);
            SetRow3(ref result, result.Row3() - Vector4.Dot(row2, result.Row3()) * row2);
            SetRow3(ref result, Vector4.Normalize(result.Row3()));
            Vector4 row3 = result.Row3();
            SetRow4(ref result, result.Row4() - Vector4.Dot(row1, result.Row4()) * row1);
            SetRow4(ref result, result.Row4() - Vector4.Dot(row2, result.Row4()) * row2);
            SetRow4(ref result, result.Row4() - Vector4.Dot(row3, result.Row4()) * row3);
            SetRow4(ref result, Vector4.Normalize(result.Row4()));
        }

        /// <summary>
        /// Orthonormalizes the specified matrix.
        /// </summary>
        /// <param name="value">The matrix to orthonormalize.</param>
        /// <returns>The orthonormalized matrix.</returns>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting matrix
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the matrix rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static Matrix Orthonormalize(Matrix value)
        {
            Orthonormalize(ref value, out Matrix result);
            return result;
        }

        /// <summary>
        /// Brings the matrix into upper triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The matrix to put into upper triangular form.</param>
        /// <param name="result">When the method completes, contains the upper triangular matrix.</param>
        /// <remarks>
        /// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static void UpperTriangularForm(ref Matrix value, out Matrix result)
        {
            //Adapted from the row echelon code.
            result = value;
            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                {
                    return;
                }

                int i = r;

                while (MathUtil.IsZero(result.Get(i, lead)))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                        {
                            return;
                        }
                    }
                }

                if (i != r)
                {
                    ExchangeRows(ref result, i, r);
                }

                float multiplier = 1f / result.Get(r, lead);

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        SetRow(ref result, i, result.GetRow(i) - result.GetRow(r) * multiplier * result.Get(i, lead));

                        //Set(ref result, i, 0, result.Get(i, 0) - result.Get(r, 0) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 1, result.Get(i, 1) - result.Get(r, 1) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 2, result.Get(i, 2) - result.Get(r, 2) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 3, result.Get(i, 3) - result.Get(r, 3) * multiplier * result.Get(i, lead));
                    }
                }

                lead++;
            }
        }

        /// <summary>
        /// Brings the matrix into upper triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The matrix to put into upper triangular form.</param>
        /// <returns>The upper triangular matrix.</returns>
        /// <remarks>
        /// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static Matrix UpperTriangularForm(Matrix value)
        {
            UpperTriangularForm(ref value, out Matrix result);
            return result;
        }

        /// <summary>
        /// Brings the matrix into lower triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The matrix to put into lower triangular form.</param>
        /// <param name="result">When the method completes, contains the lower triangular matrix.</param>
        /// <remarks>
        /// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static void LowerTriangularForm(ref Matrix value, out Matrix result)
        {
            //Adapted from the row echelon code.
            Matrix temp = value;
            result = Matrix.Transpose(temp);

            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                {
                    return;
                }

                int i = r;

                while (MathUtil.IsZero(result.Get(i, lead)))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                        {
                            return;
                        }
                    }
                }

                if (i != r)
                {
                    ExchangeRows(ref result, i, r);
                }

                float multiplier = 1f / result.Get(r, lead);

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        SetRow(ref result, i, result.GetRow(i) - result.GetRow(r) * multiplier * result.Get(i, lead));

                        //Set(ref result, i, 0, result.Get(i, 0) - result.Get(r, 0) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 1, result.Get(i, 1) - result.Get(r, 1) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 2, result.Get(i, 2) - result.Get(r, 2) * multiplier * result.Get(i, lead));
                        //Set(ref result, i, 3, result.Get(i, 3) - result.Get(r, 3) * multiplier * result.Get(i, lead));
                    }
                }

                lead++;
            }

            result = Matrix.Transpose(result);
        }

        /// <summary>
        /// Brings the matrix into lower triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The matrix to put into lower triangular form.</param>
        /// <returns>The lower triangular matrix.</returns>
        /// <remarks>
        /// If the matrix is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the matrix represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static Matrix LowerTriangularForm(Matrix value)
        {
            LowerTriangularForm(ref value, out Matrix result);
            return result;
        }

        /// <summary>
        /// Brings the matrix into row echelon form using elementary row operations;
        /// </summary>
        /// <param name="value">The matrix to put into row echelon form.</param>
        /// <param name="result">When the method completes, contains the row echelon form of the matrix.</param>
        public static void RowEchelonForm(ref Matrix value, out Matrix result)
        {
            //Source: Wikipedia pseudo code
            //Reference: http://en.wikipedia.org/wiki/Row_echelon_form#Pseudocode

            result = value;
            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                {
                    return;
                }

                int i = r;

                while (MathUtil.IsZero(result.Get(i, lead)))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                        {
                            return;
                        }
                    }
                }

                if (i != r)
                {
                    ExchangeRows(ref result, i, r);
                }

                float multiplier = 1f / result.Get(r, lead);
                SetRow(ref result, r, result.GetRow(r) * multiplier);
                //result[r, 0] *= multiplier;
                //result[r, 1] *= multiplier;
                //result[r, 2] *= multiplier;
                //result[r, 3] *= multiplier;

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        SetRow(ref result, i, result.GetRow(i) - result.GetRow(r) * result.Get(i, lead));
                        //result[i, 0] -= result[r, 0] * result[i, lead];
                        //result[i, 1] -= result[r, 1] * result[i, lead];
                        //result[i, 2] -= result[r, 2] * result[i, lead];
                        //result[i, 3] -= result[r, 3] * result[i, lead];
                    }
                }

                lead++;
            }
        }

        /// <summary>
        /// Brings the matrix into row echelon form using elementary row operations;
        /// </summary>
        /// <param name="value">The matrix to put into row echelon form.</param>
        /// <returns>When the method completes, contains the row echelon form of the matrix.</returns>
        public static Matrix RowEchelonForm(Matrix value)
        {
            RowEchelonForm(ref value, out Matrix result);
            return result;
        }

        /// <summary>
        /// Brings the matrix into reduced row echelon form using elementary row operations.
        /// </summary>
        /// <param name="value">The matrix to put into reduced row echelon form.</param>
        /// <param name="augment">The fifth column of the matrix.</param>
        /// <param name="result">When the method completes, contains the resultant matrix after the operation.</param>
        /// <param name="augmentResult">When the method completes, contains the resultant fifth column of the matrix.</param>
        /// <remarks>
        /// <para>The fifth column is often called the augmented part of the matrix. This is because the fifth
        /// column is really just an extension of the matrix so that there is a place to put all of the
        /// non-zero components after the operation is complete.</para>
        /// <para>Often times the resultant matrix will the identity matrix or a matrix similar to the identity
        /// matrix. Sometimes, however, that is not possible and numbers other than zero and one may appear.</para>
        /// <para>This method can be used to solve systems of linear equations. Upon completion of this method,
        /// the <paramref name="augmentResult"/> will contain the solution for the system. It is up to the user
        /// to analyze both the input and the result to determine if a solution really exists.</para>
        /// </remarks>
        public static void ReducedRowEchelonForm(ref Matrix value, ref Vector4 augment, out Matrix result, out Vector4 augmentResult)
        {
            //Source: http://rosettacode.org
            //Reference: http://rosettacode.org/wiki/Reduced_row_echelon_form

            float[,] matrix = new float[4, 5];

            matrix[0, 0] = value.M11;
            matrix[0, 1] = value.M12;
            matrix[0, 2] = value.M13;
            matrix[0, 3] = value.M14;
            matrix[0, 4] = augment.X;

            matrix[1, 0] = value.M21;
            matrix[1, 1] = value.M22;
            matrix[1, 2] = value.M23;
            matrix[1, 3] = value.M24;
            matrix[1, 4] = augment.Y;

            matrix[2, 0] = value.M31;
            matrix[2, 1] = value.M32;
            matrix[2, 2] = value.M33;
            matrix[2, 3] = value.M34;
            matrix[2, 4] = augment.Z;

            matrix[3, 0] = value.M41;
            matrix[3, 1] = value.M42;
            matrix[3, 2] = value.M43;
            matrix[3, 3] = value.M44;
            matrix[3, 4] = augment.W;

            int lead = 0;
            int rowcount = 4;
            int columncount = 5;

            for (int r = 0; r < rowcount; r++)
            {
                if (columncount <= lead)
                {
                    break;
                }

                int i = r;

                while (matrix[i, lead] == 0)
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (columncount == lead)
                        {
                            break;
                        }
                    }
                }

                for (int j = 0; j < columncount; j++)
                {
                    (matrix[i, j], matrix[r, j]) = (matrix[r, j], matrix[i, j]);
                }

                float div = matrix[r, lead];

                for (int j = 0; j < columncount; j++)
                {
                    matrix[r, j] /= div;
                }

                for (int j = 0; j < rowcount; j++)
                {
                    if (j != r)
                    {
                        float sub = matrix[j, lead];
                        for (int k = 0; k < columncount; k++)
                        {
                            matrix[j, k] -= sub * matrix[r, k];
                        }
                    }
                }

                lead++;
            }

            result.M11 = matrix[0, 0];
            result.M12 = matrix[0, 1];
            result.M13 = matrix[0, 2];
            result.M14 = matrix[0, 3];

            result.M21 = matrix[1, 0];
            result.M22 = matrix[1, 1];
            result.M23 = matrix[1, 2];
            result.M24 = matrix[1, 3];

            result.M31 = matrix[2, 0];
            result.M32 = matrix[2, 1];
            result.M33 = matrix[2, 2];
            result.M34 = matrix[2, 3];

            result.M41 = matrix[3, 0];
            result.M42 = matrix[3, 1];
            result.M43 = matrix[3, 2];
            result.M44 = matrix[3, 3];

            augmentResult.X = matrix[0, 4];
            augmentResult.Y = matrix[1, 4];
            augmentResult.Z = matrix[2, 4];
            augmentResult.W = matrix[3, 4];
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard matrix.</param>
        public static void BillboardLH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Matrix result)
        {
            Vector3 difference = cameraPosition - objectPosition;

            float lengthSq = difference.LengthSquared();
            if (MathUtil.IsZero(lengthSq))
            {
                difference = -cameraForwardVector;
            }
            else
            {
                difference *= (float)(1.0 / Math.Sqrt(lengthSq));
            }

            Vector3 crossed = Vector3.Normalize(Vector3.Cross(cameraUpVector, difference));
            Vector3 final = Vector3.Cross(difference, crossed);

            result.M11 = crossed.X;
            result.M12 = crossed.Y;
            result.M13 = crossed.Z;
            result.M14 = 0.0f;
            result.M21 = final.X;
            result.M22 = final.Y;
            result.M23 = final.Z;
            result.M24 = 0.0f;
            result.M31 = difference.X;
            result.M32 = difference.Y;
            result.M33 = difference.Z;
            result.M34 = 0.0f;
            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>The created billboard matrix.</returns>
        public static Matrix BillboardLH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        {
            BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard matrix.</param>
        public static void BillboardRH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Matrix result)
        {
            result = Matrix.CreateBillboard(objectPosition, cameraPosition, cameraUpVector, cameraForwardVector);
            //Vector3 crossed;
            //Vector3 final;
            //Vector3 difference = objectPosition - cameraPosition;

            //float lengthSq = difference.LengthSquared();
            //if (MathUtil.IsZero(lengthSq))
            //    difference = -cameraForwardVector;
            //else
            //    difference *= (float)(1.0 / Math.Sqrt(lengthSq));

            //Vector3.Cross(ref cameraUpVector, ref difference, out crossed);
            //crossed.Normalize();
            //Vector3.Cross(ref difference, ref crossed, out final);

            //result.M11 = crossed.X;
            //result.M12 = crossed.Y;
            //result.M13 = crossed.Z;
            //result.M14 = 0.0f;
            //result.M21 = final.X;
            //result.M22 = final.Y;
            //result.M23 = final.Z;
            //result.M24 = 0.0f;
            //result.M31 = difference.X;
            //result.M32 = difference.Y;
            //result.M33 = difference.Z;
            //result.M34 = 0.0f;
            //result.M41 = objectPosition.X;
            //result.M42 = objectPosition.Y;
            //result.M43 = objectPosition.Z;
            //result.M44 = 1.0f;
        }

        /// <summary>
        /// Creates a right-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>The created billboard matrix.</returns>
        public static Matrix BillboardRH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        {
            return Matrix.CreateBillboard(objectPosition, cameraPosition, cameraUpVector, cameraForwardVector);
            //Matrix result;
            //BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at matrix.</param>
        public static void LookAtLH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Matrix result)
        {
            Vector3 zaxis = Vector3.Normalize(Vector3.Subtract(target, eye));
            Vector3 xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            result = Matrix.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            result.M41 = -Vector3.Dot(xaxis, eye);
            result.M42 = -Vector3.Dot(yaxis, eye);
            result.M43 = -Vector3.Dot(zaxis, eye);
        }

        /// <summary>
        /// Creates a left-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at matrix.</returns>
        public static Matrix LookAtLH(Vector3 eye, Vector3 target, Vector3 up)
        {
            LookAtLH(ref eye, ref target, ref up, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at matrix.</param>
        public static void LookAtRH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Matrix result)
        {
            result = Matrix.CreateLookAt(eye, target, up);
            //Vector3 xaxis, yaxis, zaxis;
            //Vector3.Subtract(ref eye, ref target, out zaxis); zaxis.Normalize();
            //Vector3.Cross(ref up, ref zaxis, out xaxis); xaxis.Normalize();
            //Vector3.Cross(ref zaxis, ref xaxis, out yaxis);

            //result = Matrix.Identity;
            //result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            //result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            //result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            //Vector3.Dot(ref xaxis, ref eye, out result.M41);
            //Vector3.Dot(ref yaxis, ref eye, out result.M42);
            //Vector3.Dot(ref zaxis, ref eye, out result.M43);

            //result.M41 = -result.M41;
            //result.M42 = -result.M42;
            //result.M43 = -result.M43;
        }

        /// <summary>
        /// Creates a right-handed, look-at matrix.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at matrix.</returns>
        public static Matrix LookAtRH(Vector3 eye, Vector3 target, Vector3 up)
        {
            return Matrix.CreateLookAt(eye, target, up);
            //Matrix result;
            //LookAtRH(ref eye, ref target, ref up, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void OrthoLH(float width, float height, float znear, float zfar, out Matrix result)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            OrthoOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a left-handed, orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix OrthoLH(float width, float height, float znear, float zfar)
        {
            OrthoLH(width, height, znear, zfar, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void OrthoRH(float width, float height, float znear, float zfar, out Matrix result)
        {
            result = Matrix.CreateOrthographic(width, height, znear, zfar);
            //float halfWidth = width * 0.5f;
            //float halfHeight = height * 0.5f;

            //OrthoOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a right-handed, orthographic projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix OrthoRH(float width, float height, float znear, float zfar)
        {
            return Matrix.CreateOrthographic(width, height, znear, zfar);
            //Matrix result;
            //OrthoRH(width, height, znear, zfar, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, customized orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void OrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result)
        {
            float zRange = 1.0f / (zfar - znear);

            result = Matrix.Identity;
            result.M11 = 2.0f / (right - left);
            result.M22 = 2.0f / (top - bottom);
            result.M33 = zRange;
            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = -znear * zRange;
        }

        /// <summary>
        /// Creates a left-handed, customized orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix OrthoOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, customized orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void OrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result)
        {
            result = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, znear, zfar);
            //OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            //result.M33 *= -1.0f;
        }

        /// <summary>
        /// Creates a right-handed, customized orthographic projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix OrthoOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            return Matrix.CreateOrthographicOffCenter(left, right, bottom, top, znear, zfar);
            //Matrix result;
            //OrthoOffCenterRH(left, right, bottom, top, znear, zfar, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveLH(float width, float height, float znear, float zfar, out Matrix result)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            PerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a left-handed, perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveLH(float width, float height, float znear, float zfar)
        {
            PerspectiveLH(width, height, znear, zfar, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveRH(float width, float height, float znear, float zfar, out Matrix result)
        {
            result = Matrix.CreatePerspective(width, height, znear, zfar);
            //float halfWidth = width * 0.5f;
            //float halfHeight = height * 0.5f;

            //PerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a right-handed, perspective projection matrix.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveRH(float width, float height, float znear, float zfar)
        {
            return Matrix.CreatePerspective(width, height, znear, zfar);
            //Matrix result;
            //PerspectiveRH(width, height, znear, zfar, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveFovLH(float fov, float aspect, float znear, float zfar, out Matrix result)
        {
            float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
            float q = zfar / (zfar - znear);

            result = new Matrix
            {
                M11 = yScale / aspect,
                M22 = yScale,
                M33 = q,
                M34 = 1.0f,
                M43 = -q * znear
            };
        }

        /// <summary>
        /// Creates a left-handed, perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveFovLH(float fov, float aspect, float znear, float zfar)
        {
            PerspectiveFovLH(fov, aspect, znear, zfar, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveFovRH(float fov, float aspect, float znear, float zfar, out Matrix result)
        {
            result = Matrix.CreatePerspectiveFieldOfView(fov, aspect, znear, zfar);
            //float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
            //float q = zfar / (znear - zfar);

            //result = new Matrix();
            //result.M11 = yScale / aspect;
            //result.M22 = yScale;
            //result.M33 = q;
            //result.M34 = -1.0f;
            //result.M43 = q * znear;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveFovRH(float fov, float aspect, float znear, float zfar)
        {
            return Matrix.CreatePerspectiveFieldOfView(fov, aspect, znear, zfar);
            //Matrix result;
            //PerspectiveFovRH(fov, aspect, znear, zfar, out result);
            //return result;
        }

        /// <summary>
        /// Creates a left-handed, customized perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result)
        {
            float zRange = zfar / (zfar - znear);

            result = new Matrix
            {
                M11 = 2.0f * znear / (right - left),
                M22 = 2.0f * znear / (top - bottom),
                M31 = (left + right) / (left - right),
                M32 = (top + bottom) / (bottom - top),
                M33 = zRange,
                M34 = 1.0f,
                M43 = -znear * zRange
            };
        }

        /// <summary>
        /// Creates a left-handed, customized perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveOffCenterLH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, customized perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection matrix.</param>
        public static void PerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar, out Matrix result)
        {
            result = Matrix.CreatePerspectiveOffCenter(left, right, bottom, top, znear, zfar);
            //PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            //result.M31 *= -1.0f;
            //result.M32 *= -1.0f;
            //result.M33 *= -1.0f;
            //result.M34 *= -1.0f;
        }

        /// <summary>
        /// Creates a right-handed, customized perspective projection matrix.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection matrix.</returns>
        public static Matrix PerspectiveOffCenterRH(float left, float right, float bottom, float top, float znear, float zfar)
        {
            return Matrix.CreatePerspectiveOffCenter(left, right, bottom, top, znear, zfar);
            //Matrix result;
            //PerspectiveOffCenterRH(left, right, bottom, top, znear, zfar, out result);
            //return result;
        }

        /// <summary>
        /// Creates a skew/shear matrix by means of a translation vector, a rotation vector, and a rotation angle.
        /// shearing is performed in the direction of translation vector, where translation vector and rotation vector define the shearing plane.
        /// The effect is such that the skewed rotation vector has the specified angle with rotation itself.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        /// <param name="rotationVec">The rotation vector</param>
        /// <param name="transVec">The translation vector</param>
        /// <param name="matrix">Contains the created skew/shear matrix. </param>
        public static void Skew(float angle, ref Vector3 rotationVec, ref Vector3 transVec, out Matrix matrix)
        {
            //http://elckerlyc.ewi.utwente.nl/browser/Elckerlyc/Hmi/HmiMath/src/hmi/math/Mat3f.java
            float MINIMAL_SKEW_ANGLE = 0.000001f;

            Vector3 e0 = rotationVec;
            Vector3 e1 = Vector3.Normalize(transVec);

            float rv1 = Vector3.Dot(rotationVec, e1);

            e0 += rv1 * e1;
            float rv0 = Vector3.Dot(rotationVec, e0);

            float cosa = (float)Math.Cos(angle);
            float sina = (float)Math.Sin(angle);
            float rr0 = rv0 * cosa - rv1 * sina;
            float rr1 = rv0 * sina + rv1 * cosa;

            if (rr0 < MINIMAL_SKEW_ANGLE)
            {
                throw new ArgumentException("illegal skew angle");
            }

            float d = (rr1 / rr0) - (rv1 / rv0);

            matrix = Matrix.Identity;
            matrix.M11 = d * e1.X * e0.X + 1.0f;
            matrix.M12 = d * e1.X * e0.Y;
            matrix.M13 = d * e1.X * e0.Z;
            matrix.M21 = d * e1.Y * e0.X;
            matrix.M22 = d * e1.Y * e0.Y + 1.0f;
            matrix.M23 = d * e1.Y * e0.Z;
            matrix.M31 = d * e1.Z * e0.X;
            matrix.M32 = d * e1.Z * e0.Y;
            matrix.M33 = d * e1.Z * e0.Z + 1.0f;
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void AffineTransformation(float scaling, ref Quaternion rotation, ref Vector3 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix AffineTransformation(float scaling, Quaternion rotation, Vector3 translation)
        {
            return Matrix.CreateScale(scaling) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void AffineTransformation(float scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling) * Matrix.CreateTranslation(-rotationCenter)
                * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(rotationCenter)
                * Matrix.CreateTranslation(translation);
            //result = Scaling(scaling) * Translation(-rotationCenter) * RotationQuaternion(rotation) *
            //    Translation(rotationCenter) * Translation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix AffineTransformation(float scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            return Matrix.CreateScale(scaling) * Matrix.CreateTranslation(-rotationCenter)
                * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(rotationCenter)
                * Matrix.CreateTranslation(translation);
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void AffineTransformation2D(float scaling, float rotation, ref Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling, scaling, 1) * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateTranslation(new Vector3(translation, 0));
            //Scaling(scaling, scaling, 1.0f) * RotationZ(rotation) * Translation((Vector3)translation);
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix AffineTransformation2D(float scaling, float rotation, Vector2 translation)
        {
            AffineTransformation2D(scaling, rotation, ref translation, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation matrix.</param>
        public static void AffineTransformation2D(float scaling, ref Vector2 rotationCenter, float rotation, ref Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateScale(scaling, scaling, 1) * Matrix.CreateTranslation(new Vector3(-rotationCenter, 0))
                * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(rotationCenter, 0))
                * Matrix.CreateTranslation(new Vector3(translation, 0));
            //Scaling(scaling, scaling, 1.0f) * Translation((Vector3)(-rotationCenter)) * RotationZ(rotation) *
            //Translation((Vector3)rotationCenter) * Translation((Vector3)translation);
        }

        /// <summary>
        /// Creates a 2D affine transformation matrix.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation matrix.</returns>
        public static Matrix AffineTransformation2D(float scaling, Vector2 rotationCenter, float rotation, Vector2 translation)
        {
            AffineTransformation2D(scaling, ref rotationCenter, rotation, ref translation, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created transformation matrix.</param>
        public static void Transformation(ref Vector3 scalingCenter, ref Quaternion scalingRotation, ref Vector3 scaling, ref Vector3 rotationCenter, ref Quaternion rotation, ref Vector3 translation, out Matrix result)
        {
            Matrix sr = Matrix.CreateFromQuaternion(scalingRotation);
            result = Matrix.CreateTranslation(-scalingCenter) * Matrix.Transpose(sr)
                * Matrix.CreateScale(scaling) * sr * Matrix.CreateTranslation(scalingCenter)
                * Matrix.CreateTranslation(-rotationCenter)
                * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(rotationCenter)
                * Matrix.CreateTranslation(translation);
            //Matrix sr = RotationQuaternion(scalingRotation);

            //result = Translation(-scalingCenter) * Transpose(sr) * Scaling(scaling) * sr 
            //    * Translation(scalingCenter) * Translation(-rotationCenter) *
            //    RotationQuaternion(rotation) * Translation(rotationCenter) * Translation(translation);       
        }

        /// <summary>
        /// Creates a transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created transformation matrix.</returns>
        public static Matrix Transformation(Vector3 scalingCenter, Quaternion scalingRotation, Vector3 scaling, Vector3 rotationCenter, Quaternion rotation, Vector3 translation)
        {
            Transformation(ref scalingCenter, ref scalingRotation, ref scaling, ref rotationCenter, ref rotation, ref translation, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a 2D transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created transformation matrix.</param>
        public static void Transformation2D(ref Vector2 scalingCenter, float scalingRotation, ref Vector2 scaling, ref Vector2 rotationCenter, float rotation, ref Vector2 translation, out Matrix result)
        {
            result = Matrix.CreateTranslation(new Vector3(-scalingCenter, 0))
                * Matrix.CreateRotationZ(-scalingRotation) * Matrix.CreateScale(new Vector3(scaling, 0))
                * Matrix.CreateRotationZ(scalingRotation) * Matrix.CreateTranslation(new Vector3(scalingCenter, 0))
                * Matrix.CreateTranslation(new Vector3(-rotationCenter, 0))
                * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(rotationCenter, 0))
                * Matrix.CreateTranslation(new Vector3(translation, 0));
            //result = Translation((Vector3)(-scalingCenter)) * RotationZ(-scalingRotation) * Scaling((Vector3)scaling) 
            //    * RotationZ(scalingRotation) * Translation((Vector3)scalingCenter) * 
            //    Translation((Vector3)(-rotationCenter)) * RotationZ(rotation) * Translation((Vector3)rotationCenter)
            //    * Translation((Vector3)translation);

            result.M33 = 1f;
            result.M44 = 1f;
        }

        /// <summary>
        /// Creates a 2D transformation matrix.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created transformation matrix.</returns>
        public static Matrix Transformation2D(Vector2 scalingCenter, float scalingRotation, Vector2 scaling, Vector2 rotationCenter, float rotation, Vector2 translation)
        {
            Transformation2D(ref scalingCenter, scalingRotation, ref scaling, ref rotationCenter, rotation, ref translation, out Matrix result);
            return result;
        }

        /// <summary>
        /// Scalings the specified scaling.
        /// </summary>
        /// <param name="scaling">The scaling.</param>
        /// <returns></returns>
        public static Matrix Scaling(float scaling)
        {
            return Matrix.CreateScale(scaling);
        }

        /// <summary>
        /// Scalings the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public static Matrix Scaling(float x, float y, float z)
        {
            return Matrix.CreateScale(x, y, z);
        }
        /// <summary>
        /// Scalings the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static Matrix Scaling(Vector3 v)
        {
            return Matrix.CreateScale(v);
        }
        /// <summary>
        /// Scalings the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="center">The center.</param>
        /// <returns></returns>
        public static Matrix Scaling(Vector3 v, Vector3 center)
        {
            return Matrix.CreateScale(v, center);
        }
        /// <summary>
        /// Translations the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public static Matrix Translation(float x, float y, float z)
        {
            return Matrix.CreateTranslation(x, y, z);
        }
        /// <summary>
        /// Translations the specified v.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static Matrix Translation(Vector3 v)
        {
            return Matrix.CreateTranslation(v);
        }
        /// <summary>
        /// Rotations the axis. Angle is radian.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        public static Matrix RotationAxis(Vector3 axis, float angle)
        {
            return Matrix.CreateFromAxisAngle(axis, angle);
        }
        /// <summary>
        /// Rotations the x.Angle is radian.
        /// </summary>
        /// <param name="angle">The radian.</param>
        /// <returns></returns>
        public static Matrix RotationX(float angle)
        {
            return Matrix.CreateRotationX(angle);
        }
        /// <summary>
        /// Rotations the y.Angle is radian.
        /// </summary>
        /// <param name="angle">The radian.</param>
        /// <returns></returns>
        public static Matrix RotationY(float angle)
        {
            return Matrix.CreateRotationY(angle);
        }
        /// <summary>
        /// Rotations the z.Angle is radian.
        /// </summary>
        /// <param name="angle">The radian.</param>
        /// <returns></returns>
        public static Matrix RotationZ(float angle)
        {
            return Matrix.CreateRotationZ(angle);
        }

        /// <summary>
        /// Pseudo inversion. Usually use to perform fast view matrix inversion.
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <returns></returns>
        public static Matrix PsudoInvert(ref Matrix viewMatrix)
        {
            float x = viewMatrix.M41 * viewMatrix.M11 + viewMatrix.M42 * viewMatrix.M12 + viewMatrix.M43 * viewMatrix.M13;
            float y = viewMatrix.M41 * viewMatrix.M21 + viewMatrix.M42 * viewMatrix.M22 + viewMatrix.M43 * viewMatrix.M23;
            float z = viewMatrix.M41 * viewMatrix.M31 + viewMatrix.M42 * viewMatrix.M32 + viewMatrix.M43 * viewMatrix.M33;

            return new Matrix(
                viewMatrix.M11, viewMatrix.M21, viewMatrix.M31, 0,
                viewMatrix.M12, viewMatrix.M22, viewMatrix.M32, 0,
                viewMatrix.M13, viewMatrix.M23, viewMatrix.M33, 0, -x, -y, -z, 1);
        }
        /// <summary>
        /// Pseudo inversion. Usually use to perform fast view matrix inversion.
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <returns></returns>
        public static Matrix PsudoInvert(this Matrix viewMatrix)
        {
            return PsudoInvert(ref viewMatrix);
        }

        /// <summary>
        /// Return inverted matrix if the operation succeeded.
        /// Otherwise, return <see cref="Matrix4x4.Identity"/>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 Inverted(this Matrix4x4 matrix)
        {
            if (Matrix4x4.Invert(matrix, out Matrix4x4 result))
            {
                return result;
            }
            logger.LogError("Matrix inversion has failed");
            return new Matrix4x4();
        }
    }
}
