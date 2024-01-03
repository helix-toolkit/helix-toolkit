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
namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a four dimensional mathematical vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Int4 : IEquatable<Int4>, IFormattable
    {
        /// <summary>
        /// The size of the <see cref = "Int4" /> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = NativeHelper.SizeOf<Int4>();

        /// <summary>
        /// A <see cref = "Int4" /> with all of its components set to zero.
        /// </summary>
        public static readonly Int4 Zero = new();

        /// <summary>
        /// The X unit <see cref = "Int4" /> (1, 0, 0, 0).
        /// </summary>
        public static readonly Int4 UnitX = new(1, 0, 0, 0);

        /// <summary>
        /// The Y unit <see cref = "Int4" /> (0, 1, 0, 0).
        /// </summary>
        public static readonly Int4 UnitY = new(0, 1, 0, 0);

        /// <summary>
        /// The Z unit <see cref = "Int4" /> (0, 0, 1, 0).
        /// </summary>
        public static readonly Int4 UnitZ = new(0, 0, 1, 0);

        /// <summary>
        /// The W unit <see cref = "Int4" /> (0, 0, 0, 1).
        /// </summary>
        public static readonly Int4 UnitW = new(0, 0, 0, 1);

        /// <summary>
        /// A <see cref = "Int4" /> with all of its components set to one.
        /// </summary>
        public static readonly Int4 One = new(1, 1, 1, 1);

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public int Z;

        /// <summary>
        /// The W component of the vector.
        /// </summary>
        public int W;

        /// <summary>
        /// Initializes a new instance of the <see cref = "Int4" /> struct.
        /// </summary>
        /// <param name = "value">The value that will be assigned to all components.</param>
        public Int4(int value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "Int4" /> struct.
        /// </summary>
        /// <param name = "x">Initial value for the X component of the vector.</param>
        /// <param name = "y">Initial value for the Y component of the vector.</param>
        /// <param name = "z">Initial value for the Z component of the vector.</param>
        /// <param name = "w">Initial value for the W component of the vector.</param>
        public Int4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref = "Int4" /> struct.
        /// </summary>
        /// <param name = "values">The values to assign to the X, Y, Z, and W components of the vector. This must be an array with four elements.</param>
        /// <exception cref = "ArgumentNullException">Thrown when <paramref name = "values" /> is <c>null</c>.</exception>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown when <paramref name = "values" /> contains more or less than four elements.</exception>
        public Int4(int[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(values),
                                                      "There must be four and only four input values for Int4.");
            }

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name = "index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">Thrown when the <paramref name = "index" /> is out of the range [0, 3].</exception>
        public int this[int index]
        {
            readonly get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    3 => W,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Int4 run from 0 to 3, inclusive."),
                };
            }

            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index), "Indices for Int4 run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A four-element array containing the components of the vector.</returns>
        public readonly int[] ToArray()
        {
            return new int[] {X, Y, Z, W};
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name = "left">The first vector to add.</param>
        /// <param name = "right">The second vector to add.</param>
        /// <param name = "result">When the method completes, contains the sum of the two vectors.</param>
        public static void Add(ref Int4 left, ref Int4 right, out Int4 result)
        {
            result = new Int4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name = "left">The first vector to add.</param>
        /// <param name = "right">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static Int4 Add(Int4 left, Int4 right)
        {
            return new Int4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name = "left">The first vector to subtract.</param>
        /// <param name = "right">The second vector to subtract.</param>
        /// <param name = "result">When the method completes, contains the difference of the two vectors.</param>
        public static void Subtract(ref Int4 left, ref Int4 right, out Int4 result)
        {
            result = new Int4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name = "left">The first vector to subtract.</param>
        /// <param name = "right">The second vector to subtract.</param>
        /// <returns>The difference of the two vectors.</returns>
        public static Int4 Subtract(Int4 left, Int4 right)
        {
            return new Int4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <param name = "result">When the method completes, contains the scaled vector.</param>
        public static void Multiply(ref Int4 value, int scale, out Int4 result)
        {
            result = new Int4(value.X*scale, value.Y*scale, value.Z*scale, value.W*scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Int4 Multiply(Int4 value, int scale)
        {
            return new Int4(value.X*scale, value.Y*scale, value.Z*scale, value.W*scale);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name = "left">The first vector to modulate.</param>
        /// <param name = "right">The second vector to modulate.</param>
        /// <param name = "result">When the method completes, contains the modulated vector.</param>
        public static void Modulate(ref Int4 left, ref Int4 right, out Int4 result)
        {
            result = new Int4(left.X*right.X, left.Y*right.Y, left.Z*right.Z, left.W*right.W);
        }

        /// <summary>
        /// Modulates a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name = "left">The first vector to modulate.</param>
        /// <param name = "right">The second vector to modulate.</param>
        /// <returns>The modulated vector.</returns>
        public static Int4 Modulate(Int4 left, Int4 right)
        {
            return new Int4(left.X*right.X, left.Y*right.Y, left.Z*right.Z, left.W*right.W);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <param name = "result">When the method completes, contains the scaled vector.</param>
        public static void Divide(ref Int4 value, int scale, out Int4 result)
        {
            result = new Int4(value.X/scale, value.Y/scale, value.Z/scale, value.W/scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Int4 Divide(Int4 value, int scale)
        {
            return new Int4(value.X/scale, value.Y/scale, value.Z/scale, value.W/scale);
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name = "value">The vector to negate.</param>
        /// <param name = "result">When the method completes, contains a vector facing in the opposite direction.</param>
        public static void Negate(ref Int4 value, out Int4 result)
        {
            result = new Int4(-value.X, -value.Y, -value.Z, -value.W);
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name = "value">The vector to negate.</param>
        /// <returns>A vector facing in the opposite direction.</returns>
        public static Int4 Negate(Int4 value)
        {
            return new Int4(-value.X, -value.Y, -value.Z, -value.W);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name = "value">The value to clamp.</param>
        /// <param name = "min">The minimum value.</param>
        /// <param name = "max">The maximum value.</param>
        /// <param name = "result">When the method completes, contains the clamped value.</param>
        public static void Clamp(ref Int4 value, ref Int4 min, ref Int4 max, out Int4 result)
        {
            int x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            int y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            int z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            int w = value.W;
            w = (w > max.W) ? max.W : w;
            w = (w < min.W) ? min.W : w;

            result = new Int4(x, y, z, w);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name = "value">The value to clamp.</param>
        /// <param name = "min">The minimum value.</param>
        /// <param name = "max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static Int4 Clamp(Int4 value, Int4 min, Int4 max)
        {
            Clamp(ref value, ref min, ref max, out Int4 result);
            return result;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name = "left">The first source vector.</param>
        /// <param name = "right">The second source vector.</param>
        /// <param name = "result">When the method completes, contains an new vector composed of the largest components of the source vectors.</param>
        public static void Max(ref Int4 left, ref Int4 right, out Int4 result)
        {
            result.X = (left.X > right.X) ? left.X : right.X;
            result.Y = (left.Y > right.Y) ? left.Y : right.Y;
            result.Z = (left.Z > right.Z) ? left.Z : right.Z;
            result.W = (left.W > right.W) ? left.W : right.W;
        }

        /// <summary>
        /// Returns a vector containing the largest components of the specified vectors.
        /// </summary>
        /// <param name = "left">The first source vector.</param>
        /// <param name = "right">The second source vector.</param>
        /// <returns>A vector containing the largest components of the source vectors.</returns>
        public static Int4 Max(Int4 left, Int4 right)
        {
            Max(ref left, ref right, out Int4 result);
            return result;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name = "left">The first source vector.</param>
        /// <param name = "right">The second source vector.</param>
        /// <param name = "result">When the method completes, contains an new vector composed of the smallest components of the source vectors.</param>
        public static void Min(ref Int4 left, ref Int4 right, out Int4 result)
        {
            result.X = (left.X < right.X) ? left.X : right.X;
            result.Y = (left.Y < right.Y) ? left.Y : right.Y;
            result.Z = (left.Z < right.Z) ? left.Z : right.Z;
            result.W = (left.W < right.W) ? left.W : right.W;
        }

        /// <summary>
        /// Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name = "left">The first source vector.</param>
        /// <param name = "right">The second source vector.</param>
        /// <returns>A vector containing the smallest components of the source vectors.</returns>
        public static Int4 Min(Int4 left, Int4 right)
        {
            Min(ref left, ref right, out Int4 result);
            return result;
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name = "left">The first vector to add.</param>
        /// <param name = "right">The second vector to add.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static Int4 operator +(Int4 left, Int4 right)
        {
            return new Int4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        /// <summary>
        /// Assert a vector (return it unchanged).
        /// </summary>
        /// <param name = "value">The vector to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) vector.</returns>
        public static Int4 operator +(Int4 value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name = "left">The first vector to subtract.</param>
        /// <param name = "right">The second vector to subtract.</param>
        /// <returns>The difference of the two vectors.</returns>
        public static Int4 operator -(Int4 left, Int4 right)
        {
            return new Int4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        /// <summary>
        /// Reverses the direction of a given vector.
        /// </summary>
        /// <param name = "value">The vector to negate.</param>
        /// <returns>A vector facing in the opposite direction.</returns>
        public static Int4 operator -(Int4 value)
        {
            return new Int4(-value.X, -value.Y, -value.Z, -value.W);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Int4 operator *(int scale, Int4 value)
        {
            return new Int4(value.X*scale, value.Y*scale, value.Z*scale, value.W*scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Int4 operator *(Int4 value, int scale)
        {
            return new Int4(value.X*scale, value.Y*scale, value.Z*scale, value.W*scale);
        }

        /// <summary>
        /// Scales a vector by the given value.
        /// </summary>
        /// <param name = "value">The vector to scale.</param>
        /// <param name = "scale">The amount by which to scale the vector.</param>
        /// <returns>The scaled vector.</returns>
        public static Int4 operator /(Int4 value, int scale)
        {
            return new Int4(value.X/scale, value.Y/scale, value.Z/scale, value.W/scale);
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name = "left">The first value to compare.</param>
        /// <param name = "right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name = "left" /> has the same value as <paramref name = "right" />; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Int4 left, Int4 right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name = "left">The first value to compare.</param>
        /// <param name = "right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name = "left" /> has a different value than <paramref name = "right" />; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(Int4 left, Int4 right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref = "Int4" /> to <see cref = "Vector2" />.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector2(Int4 value)
        {
            return new Vector2(value.X, value.Y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref = "Int4" /> to <see cref = "Vector3" />.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(Int4 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref = "Int4" /> to <see cref = "Vector4" />.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector4(Int4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <param name = "format">The format.</param>
        /// <returns>
        /// A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public readonly string ToString(string format)
        {
            return format == null
                ? ToString()
                : string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}",
                                 X.ToString(format, CultureInfo.CurrentCulture),
                                 Y.ToString(format, CultureInfo.CurrentCulture),
                                 Z.ToString(format, CultureInfo.CurrentCulture),
                                 W.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <param name = "formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public readonly string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <param name = "format">The format.</param>
        /// <param name = "formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format == null && formatProvider == null 
                ? string.Empty
                : format == null
                ? ToString(formatProvider!)
                : string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider),
                                 Y.ToString(format, formatProvider), Z.ToString(format, formatProvider),
                                 W.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                hashCode = (hashCode * 397) ^ W;
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref = "Int4" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "Int4" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref = "Int4" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(ref Int4 other)
        {
            return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
        }

        /// <summary>
        /// Determines whether the specified <see cref = "Int4" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "Int4" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref = "Int4" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(Int4 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Int4 int4 && Equals(ref int4);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> array to <see cref="Int4"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Int4(int[] input)
        {
            return new Int4(input);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int4"/> to <see cref="System.Int32"/> array.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int[](Int4 input)
        {
            return input.ToArray();
        }
    }
}
