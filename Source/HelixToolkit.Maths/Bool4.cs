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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a four dimensional mathematical vector of bool (32 bits per bool value).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Bool4 : IEquatable<Bool4>, IFormattable
    {
        /// <summary>
        /// The size of the <see cref = "Bool4" /> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = NativeHelper.SizeOf<Bool4>();

        /// <summary>
        /// A <see cref = "Bool4" /> with all of its components set to false.
        /// </summary>
        public static readonly Bool4 False = new Bool4();

        /// <summary>
        /// The X unit <see cref = "Bool4" /> (true, 0, 0, 0).
        /// </summary>
        public static readonly Bool4 UnitX = new Bool4(true, false, false, false);

        /// <summary>
        /// The Y unit <see cref = "Bool4" /> (0, true, 0, 0).
        /// </summary>
        public static readonly Bool4 UnitY = new Bool4(false, true, false, false);

        /// <summary>
        /// The Z unit <see cref = "Bool4" /> (0, 0, true, 0).
        /// </summary>
        public static readonly Bool4 UnitZ = new Bool4(false, false, true, false);

        /// <summary>
        /// The W unit <see cref = "Bool4" /> (0, 0, 0, true).
        /// </summary>
        public static readonly Bool4 UnitW = new Bool4(false, false, false, true);

        /// <summary>
        /// A <see cref = "Bool4" /> with all of its components set to true.
        /// </summary>
        public static readonly Bool4 One = new Bool4(true, true, true, true);

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        internal int iX;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        internal int iY;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        internal int iZ;

        /// <summary>
        /// The W component of the vector.
        /// </summary>
        internal int iW;

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public bool X
        {
            get
            {
                return iX != 0;
            }
            set
            {
                iX = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public bool Y
        {
            get
            {
                return iY != 0;
            }
            set
            {
                iY = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        public bool Z
        {
            get
            {
                return iZ != 0;
            }
            set
            {
                iZ = value ? 1 : 0;
            }
        }

        /// <summary>
        /// The W component of the vector.
        /// </summary>
        public bool W
        {
            get
            {
                return iW != 0;
            }
            set
            {
                iW = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "Bool4" /> struct.
        /// </summary>
        /// <param name = "value">The value that will be assigned to all components.</param>
        public Bool4(bool value)
        {
            iX = value ? 1 : 0;
            iY = value ? 1 : 0;
            iZ = value ? 1 : 0;
            iW = value ? 1 : 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "Bool4" /> struct.
        /// </summary>
        /// <param name = "x">Initial value for the X component of the vector.</param>
        /// <param name = "y">Initial value for the Y component of the vector.</param>
        /// <param name = "z">Initial value for the Z component of the vector.</param>
        /// <param name = "w">Initial value for the W component of the vector.</param>
        public Bool4(bool x, bool y, bool z, bool w)
        {
            iX = x ? 1 : 0;
            iY = y ? 1 : 0;
            iZ = z ? 1 : 0;
            iW = w ? 1 : 0; 
        }


        /// <summary>
        /// Initializes a new instance of the <see cref = "Bool4" /> struct.
        /// </summary>
        /// <param name = "values">The values to assign to the X, Y, Z, and W components of the vector. This must be an array with four elements.</param>
        /// <exception cref = "ArgumentNullException">Thrown when <paramref name = "values" /> is <c>null</c>.</exception>
        /// <exception cref = "ArgumentOutOfRangeException">Thrown when <paramref name = "values" /> contains more or less than four elements.</exception>
        public Bool4(bool[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Length != 4)
            {
                throw new ArgumentOutOfRangeException(
                   "values",
                   "There must be four and only four input values for Bool4.");
            }
            iX = values[0] ? 1 : 0;
            iY = values[1] ? 1 : 0;
            iZ = values[2] ? 1 : 0;
            iW = values[3] ? 1 : 0;
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name = "index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">Thrown when the <paramref name = "index" /> is out of the range [0, 3].</exception>
        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Bool4 run from 0 to 3, inclusive.");
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
                        throw new ArgumentOutOfRangeException("index", "Indices for Bool4 run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A four-element array containing the components of the vector.</returns>
        public bool[] ToArray()
        {
            return new bool[] {X, Y, Z, W};
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name = "left">The first value to compare.</param>
        /// <param name = "right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name = "left" /> has the same value as <paramref name = "right" />; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Bool4 left, Bool4 right)
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
        public static bool operator !=(Bool4 left, Bool4 right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Returns a <see cref = "System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "[X:{0} Y:{1} Z:{2} W:{3}]",
                X.ToString(formatProvider), Y.ToString(formatProvider), Z.ToString(formatProvider), W.ToString(formatProvider));
        }
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format == null && formatProvider == null 
                ? string.Empty 
                : format == null 
                ? ToString(formatProvider!)
                : string.Format(formatProvider, format, X, Y, Z, W);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = iX;
                hashCode = (hashCode * 397) ^ iY;
                hashCode = (hashCode * 397) ^ iZ;
                hashCode = (hashCode * 397) ^ iW;
                return hashCode;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref = "Bool4" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "Bool4" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref = "Bool4" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public bool Equals(ref Bool4 other)
        {
            return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
        }

        /// <summary>
        /// Determines whether the specified <see cref = "Bool4" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "Bool4" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref = "Bool4" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public bool Equals(Bool4 other)
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
        public override bool Equals(object? obj)
        {
            return obj is Bool4 b && Equals(ref b);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> array to <see cref="Bool4"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Bool4(bool[] input)
        {
            return new Bool4(input);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Bool4"/> to <see cref="System.Int32"/> array.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool[](Bool4 input)
        {
            return input.ToArray();
        }
    }
}
