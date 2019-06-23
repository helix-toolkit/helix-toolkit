// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Matrix3DExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for Matrix3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides extension methods for <see cref="Matrix3D"/>.
    /// </summary>
    /// <remarks>
    /// Note that the Matrix3D contains row vectors.
    /// </remarks>
    public static class Matrix3DExtensions
    {
        /// <summary>
        /// Returns the inverted matrix.
        /// </summary>
        /// <param name="m">The matrix to invert.</param>
        /// <returns>The inverted <see cref="Matrix3D"/>.</returns>
        public static Matrix3D Inverse(this Matrix3D m)
        {
            m.Invert();
            return m;
        }

        /// <summary>
        /// Convert the <see cref="Matrix3D"/> to a two-dimensional <see cref="Array"/>.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <returns>
        /// Two-dimensional array, the indexing is Array[row,column]
        /// </returns>
        public static double[,] ToArray(this Matrix3D matrix)
        {
            //// http://steve.hollasch.net/cgindex/math/matrix/column-vec.html
            //// http://en.wikipedia.org/wiki/Row_vector
            //// http://en.wikipedia.org/wiki/Column_vector

            var m = new double[4, 4];
            m[0, 0] = matrix.M11;
            m[0, 1] = matrix.M12;
            m[0, 2] = matrix.M13;
            m[0, 3] = matrix.M14;
            m[1, 0] = matrix.M21;
            m[1, 1] = matrix.M22;
            m[1, 2] = matrix.M23;
            m[1, 3] = matrix.M24;
            m[2, 0] = matrix.M31;
            m[2, 1] = matrix.M32;
            m[2, 2] = matrix.M33;
            m[2, 3] = matrix.M34;
            m[3, 0] = matrix.OffsetX;
            m[3, 1] = matrix.OffsetY;
            m[3, 2] = matrix.OffsetZ;
            m[3, 3] = matrix.M44;
            return m;
        }

        /// <summary>
        /// Convert the matrix to a string using invariant culture and '\t' and '\n' as separators.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="columnWidth">
        /// The column Width.
        /// </param>
        /// <returns>
        /// The to string.
        /// </returns>
        public static string ToString(this Matrix3D matrix, int columnWidth)
        {
            return matrix.ConvertToString("N" + columnWidth, 20);
        }

        /// <summary>
        /// Convert the matrix to a string
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="columnWidth">
        /// The column Width.
        /// </param>
        /// <returns>
        /// The to string.
        /// </returns>
        public static string ToString(this Matrix3D matrix, string format, int columnWidth)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            return matrix.ConvertToString(format, "\t", "\n", columnWidth, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert the matrix to a string
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="columnSeparator">
        /// The column Separator.
        /// </param>
        /// <param name="lineSeparator">
        /// The line Separator.
        /// </param>
        /// <param name="columnWidth">
        /// The column Width.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string ToString(
            this Matrix3D matrix,
            string format,
            string columnSeparator,
            string lineSeparator,
            int columnWidth,
            CultureInfo provider)
        {
            return matrix.ConvertToString(format, columnSeparator, lineSeparator, columnWidth, provider);
        }

        /// <summary>
        /// Converts to string using the specified format and the invariant culture
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="format">
        /// Format string, e.g. "N8"
        /// </param>
        /// <param name="columnWidth">
        /// Width of column, number of characters
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        internal static string ConvertToString(this Matrix3D matrix, string format, int columnWidth)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            return matrix.ConvertToString(format, "\t", "\n", columnWidth, provider);
        }

        /// <summary>
        /// Converts to string using the specified format and the invariant culture
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="columnSeparator">
        /// The column separator.
        /// </param>
        /// <param name="lineSeparator">
        /// The line separator.
        /// </param>
        /// <param name="columnWidth">
        /// Width of the column.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// A string.
        /// </returns>
        internal static string ConvertToString(
            this Matrix3D matrix,
            string format,
            string columnSeparator,
            string lineSeparator,
            int columnWidth,
            CultureInfo provider)
        {
            var formatString = "{0:" + format + "}";
            double[,] m = matrix.ToArray();

            // indexing: m[row,column]
            var sb = new StringBuilder();
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    string s = string.Format(provider, formatString, m[i, j]).PadLeft(columnWidth);
                    sb.Append(s);
                    if (j < 3)
                    {
                        sb.Append(columnSeparator);
                    }
                }

                if (i < 3)
                {
                    sb.Append(lineSeparator);
                }
            }

            return sb.ToString();
        }
    }
}