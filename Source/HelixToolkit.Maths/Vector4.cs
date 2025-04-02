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
using System.Diagnostics;

namespace HelixToolkit.Maths
{
    /// <summary>
    /// Represents a four dimensional mathematical vector.
    /// </summary>
    public static class Vector4Helper
    {
        /// <summary>
        /// The size of the <see cref="Vector4"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = NativeHelper.SizeOf<Vector4>();

        /// <summary>
        /// A <see cref="Vector4"/> with all of its components set to zero.
        /// </summary>
        public static readonly Vector4 Zero = new();

        /// <summary>
        /// The X unit <see cref="Vector4"/> (1, 0, 0, 0).
        /// </summary>
        public static readonly Vector4 UnitX = new(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// The Y unit <see cref="Vector4"/> (0, 1, 0, 0).
        /// </summary>
        public static readonly Vector4 UnitY = new(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The Z unit <see cref="Vector4"/> (0, 0, 1, 0).
        /// </summary>
        public static readonly Vector4 UnitZ = new(0.0f, 0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The W unit <see cref="Vector4"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly Vector4 UnitW = new(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// A <see cref="Vector4"/> with all of its components set to one.
        /// </summary>
        public static readonly Vector4 One = new(1.0f, 1.0f, 1.0f, 1.0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Min(this Vector4 v1, ref Vector4 v2)
        {
            return Vector4.Min(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Min(this Vector4 v1, Vector4 v2)
        {
            return Vector4.Min(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Max(this Vector4 v1, ref Vector4 v2)
        {
            return Vector4.Max(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Max(this Vector4 v1, Vector4 v2)
        {
            return Vector4.Max(v1, v2);
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormalized(this Vector4 v)
        {
            return MathUtil.IsOne(Vector4.Dot(v, v));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector4 v)
        {
            return v.X == 0 && v.Y == 0 && v.Z == 0 && v.W == 0;
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="v"></param>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public static float Get(this Vector4 v, int index)
        {
            return index switch
            {
                0 => v.X,
                1 => v.Y,
                2 => v.Z,
                3 => v.W,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector4 run from 0 to 3, inclusive."),
            };
        }

        public static void Set(ref Vector4 v, int index, float value)
        {
            switch (index)
            {
                case 0: v.X = value; break;
                case 1: v.Y = value; break;
                case 2: v.Z = value; break;
                case 3: v.W = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector4 run from 0 to 3, inclusive.");
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A four-element array containing the components of the vector.</returns>
        public static float[] ToArray(this Vector4 v)
        {
            return new float[] { v.X, v.Y, v.Z, v.W };
        }

        /// <summary>
        /// Returns a <see cref="Vector4"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 4D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains the 4D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, float amount1, float amount2, out Vector4 result)
        {
            result = value1 + amount1 * (value2 - value1) + amount2 * (value3 - value1);
            //result = new Vector4((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
            //    (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)),
            //    (value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)),
            //    (value1.W + (amount1 * (value2.W - value1.W))) + (amount2 * (value3.W - value1.W)));
        }

        /// <summary>
        /// Returns a <see cref="Vector4"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 4D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector4"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="Vector4"/> containing the 4D Cartesian coordinates of the specified point.</returns>
        public static Vector4 Barycentric(Vector4 value1, Vector4 value2, Vector4 value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two vectors.</param>
        public static void SmoothStep(ref Vector4 start, ref Vector4 end, float amount, out Vector4 result)
        {
            amount = MathUtil.SmoothStep(amount);
            result = Vector4.Lerp(start, end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two vectors.</returns>
        public static Vector4 SmoothStep(Vector4 start, Vector4 end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">When the method completes, contains the result of the Hermite spline interpolation.</param>
        public static void Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = (2.0f * cubed) - (3.0f * squared) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = cubed - (2.0f * squared) + amount;
            float part4 = cubed - squared;

            result = value1 * part1 + value2 * part2 + tangent1 * part3 + tangent2 * part4;
            //new Vector4((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4),
            //(((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4),
            //(((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4),
            //(((value1.W * part1) + (value2.W * part2)) + (tangent1.W * part3)) + (tangent2.W * part4));
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position vector.</param>
        /// <param name="tangent1">First source tangent vector.</param>
        /// <param name="value2">Second source position vector.</param>
        /// <param name="tangent2">Second source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
        {
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">When the method completes, contains the result of the Catmull-Rom interpolation.</param>
        public static void CatmullRom(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, ref Vector4 value4, float amount, out Vector4 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            result = 0.5f * ((2.0f * value2) + ((-value1 + value3) * amount) + (((2.0f * value1) - (5.0f * value2) + (4.0f * value3) - value4) * squared) + ((-value1 + (3.0f * value2) - (3.0f * value3) + value4) * cubed));
            //result.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) + (((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) + ((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));
            //result.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) + (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) + ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));
            //result.Z = 0.5f * ((((2.0f * value2.Z) + ((-value1.Z + value3.Z) * amount)) + (((((2.0f * value1.Z) - (5.0f * value2.Z)) + (4.0f * value3.Z)) - value4.Z) * squared)) + ((((-value1.Z + (3.0f * value2.Z)) - (3.0f * value3.Z)) + value4.Z) * cubed));
            //result.W = 0.5f * ((((2.0f * value2.W) + ((-value1.W + value3.W) * amount)) + (((((2.0f * value1.W) - (5.0f * value2.W)) + (4.0f * value3.W)) - value4.W) * squared)) + ((((-value1.W + (3.0f * value2.W)) - (3.0f * value3.W)) + value4.W) * cubed));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A vector that is the result of the Catmull-Rom interpolation.</returns>
        public static Vector4 CatmullRom(Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4, float amount)
        {
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out Vector4 result);
            return result;
        }

        /// <summary>
        /// Orthogonalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthogonalized vectors.</param>
        /// <param name="source">The list of vectors to orthogonalize.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all vectors orthogonal to each other. This
        /// means that any given vector in the list will be orthogonal to any other given vector in the
        /// list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthogonalize(Vector4[] destination, params Vector4[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3
            //q5 = ...

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");
            }

            for (int i = 0; i < source.Length; ++i)
            {
                Vector4 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= Vector4.Dot(destination[r], newvector) / Vector4.Dot(destination[r], destination[r]) * destination[r];
                }

                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Orthonormalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthonormalized vectors.</param>
        /// <param name="source">The list of vectors to orthonormalize.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all vectors orthogonal to each
        /// other and making all vectors of unit length. This means that any given vector will
        /// be orthogonal to any other given vector in the list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthonormalize(Vector4[] destination, params Vector4[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|
            //q5 = ...

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");
            }

            for (int i = 0; i < source.Length; ++i)
            {
                Vector4 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= Vector4.Dot(destination[r], newvector) * destination[r];
                }
                destination[i] = Vector4.Normalize(newvector);
            }
        }

        /// <summary>
        /// Transforms an array of vectors by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(Vector4[] source, ref Quaternion rotation, Vector4[] destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");
            }

            for (int i = 0; i < source.Length; ++i)
            {
                destination[i] = Vector4.Transform(source[i], rotation);
            }
        }

        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector4 vector, ref Matrix transform, out Vector4 result)
        {
            result = Vector4.Transform(vector, transform);
        }
        /// <summary>
        /// Transforms a 4D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Transform(this Vector4 vector, ref Matrix transform)
        {
            return Vector4.Transform(vector, transform);
        }
        /// <summary>
        /// Transforms an array of 4D vectors by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Transform(this Vector4[] source, ref Matrix transform, Vector4[] destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Length < source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "The destination array must be of same length or larger length than the source array.");
            }
            var i = 0;
#if NET6_0_OR_GREATER
            if (MathSettings.EnableSIMD)
            {
                if (Sse.IsSupported)
                {
                    // Load matrix rows into Vector128 vectors
                    Vector128<float> row0X = Vector128.Create(transform.M11, transform.M12, transform.M13, transform.M14);
                    Vector128<float> row1X = Vector128.Create(transform.M21, transform.M22, transform.M23, transform.M24);
                    Vector128<float> row2X = Vector128.Create(transform.M31, transform.M32, transform.M33, transform.M34);
                    Vector128<float> row3X = Vector128.Create(transform.M41, transform.M42, transform.M43, transform.M44);
                    unsafe
                    {
                        fixed(void* dst = destination)
                        {
                            float* dstPtr = (float*)dst;
                            for (; i < source.Length; i++, dstPtr += 4)
                            {
                                ref Vector4 vector = ref source[i];

                                // Broadcast vector components into Vector128 vectors
                                Vector128<float> vX = Vector128.Create(vector.X);
                                Vector128<float> vY = Vector128.Create(vector.Y);
                                Vector128<float> vZ = Vector128.Create(vector.Z);
                                Vector128<float> vW = Vector128.Create(vector.W);

                                // Multiply vector components with matrix rows
                                vX = Sse.Multiply(vX, row0X);
                                vY = Sse.Multiply(vY, row1X);
                                vZ = Sse.Multiply(vZ, row2X);
                                vW = Sse.Multiply(vW, row3X);

                                // Perform horizontal addition to get the dot products
                                Vector128<float> rowResults = Sse.Add(Sse.Add(vX, vY), Sse.Add(vZ, vW));
                                Sse.Store(dstPtr, rowResults);
                            }
                        }
                    }
                }
            }
#endif
            for (; i < source.Length; ++i)
            {
                Transform(ref source[i], ref transform, out destination[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref Vector4 value, ref Vector4 min, ref Vector4 max, out Vector4 result)
        {
#if NET6_0_OR_GREATER
            if (MathSettings.EnableSIMD && Sse.IsSupported)
            {
                Vector128<float> min128 = Vector128.Create(min.X, min.Y, min.Z, min.W);
                Vector128<float> max128 = Vector128.Create(max.X, max.Y, max.Z, min.W);
                Vector128<float> value128 = Vector128.Create(value.X, value.Y, value.Z, value.W);
                value128 = Sse.Max(value128, min128);
                value128 = Sse.Min(value128, max128);
                result = new Vector4(value128.GetElement(0), value128.GetElement(1), value128.GetElement(2), value128.GetElement(3));
                return;
            }
#endif
            float x = Math.Max(min.X, Math.Min(value.X, max.X));
            float y = Math.Max(min.Y, Math.Min(value.Y, max.Y));
            float z = Math.Max(min.Z, Math.Min(value.Z, max.Z));
            float w = Math.Max(min.W, Math.Min(value.W, max.W));
            result = new Vector4(x, y, z, w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Clamp(this Vector4 value, Vector4 min, Vector4 max)
        {
            Clamp(ref value, ref min, ref max, out var result);
            return result;
        }
    }
}
