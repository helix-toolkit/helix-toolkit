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
    /// Represents a three dimensional mathematical vector.
    /// </summary>
    public static class Vector3Helper
    {
        /// <summary>
        /// The size of the <see cref="Vector3"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = NativeHelper.SizeOf<Vector3>();

        /// <summary>
        /// A <see cref="Vector3"/> with all of its components set to zero.
        /// </summary>
        public static readonly Vector3 Zero = new();

        /// <summary>
        /// The X unit <see cref="Vector3"/> (1, 0, 0).
        /// </summary>
        public static readonly Vector3 UnitX = new(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The Y unit <see cref="Vector3"/> (0, 1, 0).
        /// </summary>
        public static readonly Vector3 UnitY = new(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The Z unit <see cref="Vector3"/> (0, 0, 1).
        /// </summary>
        public static readonly Vector3 UnitZ = new(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// A <see cref="Vector3"/> with all of its components set to one.
        /// </summary>
        public static readonly Vector3 One = new(1.0f, 1.0f, 1.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating up (0, 1, 0).
        /// </summary>
        public static readonly Vector3 Up = new(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating down (0, -1, 0).
        /// </summary>
        public static readonly Vector3 Down = new(0.0f, -1.0f, 0.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating left (-1, 0, 0).
        /// </summary>
        public static readonly Vector3 Left = new(-1.0f, 0.0f, 0.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating right (1, 0, 0).
        /// </summary>
        public static readonly Vector3 Right = new(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating forward in a right-handed coordinate system (0, 0, -1).
        /// </summary>
        public static readonly Vector3 ForwardRH = new(0.0f, 0.0f, -1.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating forward in a left-handed coordinate system (0, 0, 1).
        /// </summary>
        public static readonly Vector3 ForwardLH = new(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating backward in a right-handed coordinate system (0, 0, 1).
        /// </summary>
        public static readonly Vector3 BackwardRH = new(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// A unit <see cref="Vector3"/> designating backward in a left-handed coordinate system (0, 0, -1).
        /// </summary>
        public static readonly Vector3 BackwardLH = new(0.0f, 0.0f, -1.0f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Min(this Vector3 v1, ref Vector3 v2)
        {
            return Vector3.Min(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Min(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Min(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Max(this Vector3 v1, ref Vector3 v2)
        {
            return Vector3.Max(v1, v2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Max(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Max(v1, v2);
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNormalized(this Vector3 v)
        {
            return MathUtil.IsOne(Vector3.Dot(v, v));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector3 v)
        {
            return v.X == 0 && v.Y == 0 && v.Z == 0;
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, or Z component, depending on the index.</value>
        /// <param name="v"></param>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, and 2 for the Z component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
        public static float Get(this Vector3 v, int index)
        {
            return index switch
            {
                0 => v.X,
                1 => v.Y,
                2 => v.Z,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3 run from 0 to 2, inclusive."),
            };
        }

        public static void Set(ref Vector3 v, int index, float value)
        {
            switch (index)
            {
                case 0: v.X = value; break;
                case 1: v.Y = value; break;
                case 2: v.Z = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Vector3 run from 0 to 2, inclusive.");
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the vector.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public static float[] ToArray(this Vector3 v)
        {
            return new float[] { v.X, v.Y, v.Z };
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains the 3D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
        {
            result = value1 + amount1 * (value2 - value1) + amount2 * (value3 - value1);
            //result = new Vector3((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
            //    (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)),
            //    (value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)));
        }

        /// <summary>
        /// Returns a <see cref="Vector3"/> containing the 3D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 3D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="Vector3"/> containing the 3D Cartesian coordinates of the specified point.</returns>
        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(Vector3 left, Vector3 right, Vector3 epsilon)
        {
            return NearEqual(ref left, ref right, ref epsilon);
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(ref Vector3 left, ref Vector3 right, ref Vector3 epsilon)
        {
            return MathUtil.WithinEpsilon(left.X, right.X, epsilon.X) &&
                    MathUtil.WithinEpsilon(left.Y, right.Y, epsilon.Y) &&
                    MathUtil.WithinEpsilon(left.Z, right.Z, epsilon.Z);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two vectors.</param>
        public static void SmoothStep(ref Vector3 start, ref Vector3 end, float amount, out Vector3 result)
        {
            amount = MathUtil.SmoothStep(amount);
            result = Vector3.Lerp(start, end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two vectors.</returns>
        public static Vector3 SmoothStep(Vector3 start, Vector3 end, float amount)
        {
            SmoothStep(ref start, ref end, amount, out Vector3 result);
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
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = (2.0f * cubed) - (3.0f * squared) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = cubed - (2.0f * squared) + amount;
            float part4 = cubed - squared;

            result = value1 * part1 + value2 * part2 + tangent1 * part3 + tangent2 * part4;

            //result.X = (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4);
            //result.Y = (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4);
            //result.Z = (((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4);
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
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out Vector3 result);
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
        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
        {
            float squared = amount * amount;
            float cubed = amount * squared;

            result = 0.5f * ((2f * value2) + ((-value1 + value3) * amount) +
                (((2f * value1) - (5f * value2) + (4 * value3) - value4) * squared) +
                ((-value1 + (3f * value2) - (3f * value3) + value4) * cubed));

            //result.X = 0.5f * ((((2.0f * value2.X) + ((-value1.X + value3.X) * amount)) +
            //(((((2.0f * value1.X) - (5.0f * value2.X)) + (4.0f * value3.X)) - value4.X) * squared)) +
            //((((-value1.X + (3.0f * value2.X)) - (3.0f * value3.X)) + value4.X) * cubed));

            //result.Y = 0.5f * ((((2.0f * value2.Y) + ((-value1.Y + value3.Y) * amount)) +
            //    (((((2.0f * value1.Y) - (5.0f * value2.Y)) + (4.0f * value3.Y)) - value4.Y) * squared)) +
            //    ((((-value1.Y + (3.0f * value2.Y)) - (3.0f * value3.Y)) + value4.Y) * cubed));

            //result.Z = 0.5f * ((((2.0f * value2.Z) + ((-value1.Z + value3.Z) * amount)) +
            //    (((((2.0f * value1.Z) - (5.0f * value2.Z)) + (4.0f * value3.Z)) - value4.Z) * squared)) +
            //    ((((-value1.Z + (3.0f * value2.Z)) - (3.0f * value3.Z)) + value4.Z) * cubed));
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
        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <param name="result">When the method completes, contains the vector in screen space.</param>
        public static void Project(ref Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, ref Matrix worldViewProjection, out Vector3 result)
        {
            TransformCoordinate(ref vector, ref worldViewProjection, out Vector3 v);

            result = new Vector3(((1.0f + v.X) * 0.5f * width) + x, ((1.0f - v.Y) * 0.5f * height) + y, (v.Z * (maxZ - minZ)) + minZ);
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <returns>The vector in screen space.</returns>
        public static Vector3 Project(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix worldViewProjection)
        {
            Project(ref vector, x, y, width, height, minZ, maxZ, ref worldViewProjection, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Projects a 3D vector from screen space into object space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <param name="result">When the method completes, contains the vector in object space.</param>
        public static void Unproject(ref Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, ref Matrix worldViewProjection, out Vector3 result)
        {
            Vector3 v = new();
            Matrix.Invert(worldViewProjection, out Matrix matrix);

            v.X = ((vector.X - x) / width * 2.0f) - 1.0f;
            v.Y = -(((vector.Y - y) / height * 2.0f) - 1.0f);
            v.Z = (vector.Z - minZ) / (maxZ - minZ);

            TransformCoordinate(ref v, ref matrix, out result);
        }

        /// <summary>
        /// Projects a 3D vector from screen space into object space. 
        /// </summary>
        /// <param name="vector">The vector to project.</param>
        /// <param name="x">The X position of the viewport.</param>
        /// <param name="y">The Y position of the viewport.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        /// <param name="minZ">The minimum depth of the viewport.</param>
        /// <param name="maxZ">The maximum depth of the viewport.</param>
        /// <param name="worldViewProjection">The combined world-view-projection matrix.</param>
        /// <returns>The vector in object space.</returns>
        public static Vector3 Unproject(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix worldViewProjection)
        {
            Unproject(ref vector, x, y, width, height, minZ, maxZ, ref worldViewProjection, out Vector3 result);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Orthogonalize(Vector3[] destination, params Vector3[] source)
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
                Vector3 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= Vector3.Dot(destination[r], newvector) / Vector3.Dot(destination[r], destination[r]) * destination[r];
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Orthonormalize(Vector3[] destination, params Vector3[] source)
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
                Vector3 newvector = source[i];

                for (int r = 0; r < i; ++r)
                {
                    newvector -= Vector3.Dot(destination[r], newvector) * destination[r];
                }

                destination[i] = Vector3.Normalize(newvector);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] source, ref Quaternion rotation, Vector3[] destination)
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
                destination[i] = Vector3.Transform(source[i], rotation);
            }
            //float x = rotation.X + rotation.X;
            //float y = rotation.Y + rotation.Y;
            //float z = rotation.Z + rotation.Z;
            //float wx = rotation.W * x;
            //float wy = rotation.W * y;
            //float wz = rotation.W * z;
            //float xx = rotation.X * x;
            //float xy = rotation.X * y;
            //float xz = rotation.X * z;
            //float yy = rotation.Y * y;
            //float yz = rotation.Y * z;
            //float zz = rotation.Z * z;

            //float num1 = ((1.0f - yy) - zz);
            //float num2 = (xy - wz);
            //float num3 = (xz + wy);
            //float num4 = (xy + wz);
            //float num5 = ((1.0f - xx) - zz);
            //float num6 = (yz - wx);
            //float num7 = (xz - wy);
            //float num8 = (yz + wx);
            //float num9 = ((1.0f - xx) - yy);

            //for (int i = 0; i < source.Length; ++i)
            //{
            //    destination[i] = new Vector3(
            //        ((source[i].X * num1) + (source[i].Y * num2)) + (source[i].Z * num3),
            //        ((source[i].X * num4) + (source[i].Y * num5)) + (source[i].Z * num6),
            //        ((source[i].X * num7) + (source[i].Y * num8)) + (source[i].Z * num9));
            //}
        }


        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix3x3"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector3"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector3 vector, ref Matrix3x3 transform, out Vector3 result)
        {
            result = Vector3.Transform(vector, (Matrix)transform);
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix3x3"/>.</param>
        /// <returns>The transformed <see cref="Vector3"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 vector, Matrix3x3 transform)
        {
            Transform(ref vector, ref transform, out Vector3 result);
            return result;
        }
        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix3x3"/>.</param>
        /// <returns>The transformed <see cref="Vector3"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Transform(Vector3 vector, ref Matrix3x3 transform)
        {
            Transform(ref vector, ref transform, out Vector3 result);
            return result;
        }
        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="Vector4"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(ref Vector3 vector, ref Matrix transform, out Vector4 result)
        {
            result = Vector4.Transform(new Vector4(vector, 1), transform);
        }

        /// <summary>
        /// Transforms an array of 3D vectors by the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform(Vector3[] source, ref Matrix transform, Vector4[] destination)
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
                Transform(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed coordinates.</param>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transform, out Vector3 result)
        {
            Vector4 v = Vector4.Transform(new Vector4(coordinate, 1), transform);
            v /= v.W;
            result = new Vector3(v.X, v.Y, v.Z);

            //Vector4 vector = new Vector4();
            //vector.X = (coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41;
            //vector.Y = (coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42;
            //vector.Z = (coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43;
            //vector.W = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44);

            //result = new Vector3(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W);
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed coordinates.</returns>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformCoordinate(this Vector3 coordinate, Matrix transform)
        {
            Vector4 v = Vector4.Transform(new Vector4(coordinate, 1), transform);
            v /= v.W;
            return new Vector3(v.X, v.Y, v.Z);
        }
        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <returns>The transformed coordinates.</returns>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformCoordinate(this Vector3 coordinate, ref Matrix transform)
        {
            Vector4 v = Vector4.Transform(new Vector4(coordinate, 1), transform);
            v /= v.W;
            return new Vector3(v.X, v.Y, v.Z);
        }
        /// <summary>
        /// Performs a coordinate transformation on an array of vectors using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of coordinate vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformCoordinate(Vector3[] source, ref Matrix transform, Vector3[] destination)
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
                TransformCoordinate(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed normal.</param>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformNormal(ref Vector3 normal, ref Matrix transform, out Vector3 result)
        {
            result = Vector3.TransformNormal(normal, transform);
        }
        /// <summary>
        /// Performs a normal transformation using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 TransformNormal(this Vector3 normal, ref Matrix transform)
        {
            return Vector3.TransformNormal(normal, transform);
        }
        /// <summary>
        /// Performs a normal transformation on an array of vectors using the given <see cref="Matrix"/>.
        /// </summary>
        /// <param name="source">The array of normal vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformNormal(Vector3[] source, ref Matrix transform, Vector3[] destination)
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
                TransformNormal(ref source[i], ref transform, out destination[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(ref Vector3 value, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            float x = Math.Max(min.X, Math.Min(value.X, max.X));
            float y = Math.Max(min.Y, Math.Min(value.Y, max.Y));
            float z = Math.Max(min.Z, Math.Min(value.Z, max.Z));
            result = new Vector3(x, y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            float x = Math.Max(min.X, Math.Min(value.X, max.X));
            float y = Math.Max(min.Y, Math.Min(value.Y, max.Y));
            float z = Math.Max(min.Z, Math.Min(value.Z, max.Z));
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Determines whether any components of the vector are undefined (NaN).
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        /// <c>true</c> if the vector has at least one undefined component; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyUndefined(this Vector3 vector)
        {
            return float.IsNaN(vector.X) || float.IsNaN(vector.Y) || float.IsNaN(vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyInfinity(this Vector3 vector)
        {
            return float.IsInfinity(vector.X) || float.IsInfinity(vector.Y) || float.IsInfinity(vector.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FindAnyPerpendicular(this Vector3 value)
        {
            Vector3 n = Vector3.Normalize(value);
            Vector3 u = Vector3.Cross(new Vector3(0, 1, 0), n);
            if (u.LengthSquared() < 1e-3)
            {
                u = Vector3.Cross(new Vector3(1, 0, 0), n);
            }
            return u;
        }

        /// <summary>
        /// Calculates the angle (in radians) between two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The angle (in radians) between the vectors.</returns>
        /// <remarks>
        /// Note that the returned angle is never bigger than the constant <see cref="Math.PI"/>.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleBetween(this Vector3 vector1, Vector3 vector2)
        {
            // Ref: https://github.com/opentk/opentk/blob/master/src/OpenTK.Mathematics/Vector/Vector3.cs
            var dot = Vector3.Dot(vector1, vector2);
            var cosAngle = MathUtil.Clamp(dot / (vector1.Length() * vector2.Length()), -1f, 1f);
            return (float)Math.Acos(cosAngle);
        }

        /// <summary>
        /// Calculates the signed angle (in radians) between two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="axis">The vector around which the other vectors are rotated.</param>
        /// <returns>The signed angle (in radians) between two vectors.</returns>
        /// <remarks>
        /// The sign of the angle is positive in a counter-clockwise direction and negative in a clockwise direction
        /// when viewed from the side specified by the axis.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SignedAngleBetween(this Vector3 vector1, Vector3 vector2, Vector3 axis)
        {
            // Ref: https://github.com/godotengine/godot/blob/master/core/math/vector3.h
            // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Vector3.cs
            float unsignedAngle = Vector3Helper.AngleBetween(vector1, vector2);
            Vector3 cross = Vector3.Cross(vector1, vector2);
            int sign = Vector3.Dot(axis, cross) < 0f ? -1 : 1;
            return sign * unsignedAngle;
        }

        /// <summary>
        /// Rotates the source around the target by the rotation angle around the supplied axis. 
        /// </summary>
        /// <param name="source">The position to rotate.</param>
        /// <param name="target">The point to rotate around.</param>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The rotated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RotateAround(this in Vector3 source, in Vector3 target, in Vector3 axis, float angle)
        {
            // Ref: https://github.com/stride3d/stride/blob/master/sources/core/Stride.Core.Mathematics/Vector3.csv
            Vector3 local = source - target;
            Quaternion q = Quaternion.CreateFromAxisAngle(axis, angle);
            QuaternionHelper.Rotate(q, ref local);
            return target + local;
        }

        /// <summary>
        /// Point to plane position. Front/Back/Intersecting.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plane">The plane.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PlaneIntersectionType PointToPlanePosition(this Vector3 point, ref Plane plane)
        {
            var normal = plane.Normal * (plane.D >= 0 ? 1 : -1);
            var v1 = new Vector4(normal, Math.Abs(plane.D));
            var v2 = new Vector4(point, 1);
            var ret = Vector4.Dot(v1, v2);
            return ret > 0 ? PlaneIntersectionType.Front : ret == 0 ? PlaneIntersectionType.Intersecting : PlaneIntersectionType.Back;
        }

        /// <summary>
        /// Point to plane position. Front/Back/Intersecting.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plane">The plane.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PlaneIntersectionType PointToPlanePosition(this Vector3 point, Plane plane)
        {
            var v1 = new Vector4((plane.Normal * (plane.D > 0 ? 1 : -1)), Math.Abs(plane.D));
            var v2 = new Vector4(point, 1);
            var ret = Vector4.Dot(v1, v2);
            return ret > 0 ? PlaneIntersectionType.Front : ret == 0 ? PlaneIntersectionType.Intersecting : PlaneIntersectionType.Back;
        }

    }
}
