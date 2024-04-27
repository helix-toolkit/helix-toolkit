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
    /// Represents a four dimensional mathematical quaternion.
    /// </summary>
    public static class QuaternionHelper
    {
        /// <summary>
        /// The size of the <see cref="Quaternion"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = NativeHelper.SizeOf<Quaternion>();

        /// <summary>
        /// A <see cref="Quaternion"/> with all of its components set to zero.
        /// </summary>
        public static readonly Quaternion Zero = new();

        /// <summary>
        /// A <see cref="Quaternion"/> with all of its components set to one.
        /// </summary>
        public static readonly Quaternion One = new(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// The identity <see cref="Quaternion"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly Quaternion Identity = new(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public static bool IsNormalized(this Quaternion q)
        {
            return MathUtil.IsOne(Quaternion.Dot(q,q));
        }

        /// <summary>
        /// Gets the angle of the quaternion.
        /// </summary>
        /// <value>The quaternion's angle.</value>
        public static float Angle(this Quaternion q)
        {
            float length =  (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z);
            return MathUtil.IsZero(length) ? 0.0f : (float)(2.0 * Math.Acos(MathUtil.Clamp(q.W, -1f, 1f)));
        }

        /// <summary>
        /// Returns the absolute angle in radians between two <see cref="Quaternion"/>/>
        /// </summary>
        /// <param name="source">The first <see cref="Quaternion"/></param>
        /// <param name="other">The second <see cref="Quaternion"/></param>
        /// <returns></returns>
        public static float AngleBetween(this in Quaternion source, in Quaternion other)
        {
            // Ref: https://github.com/stride3d/stride/blob/master/sources/core/Stride.Core.Mathematics/Quaternion.cs
            return (float)Math.Acos(Math.Min(Math.Abs(Quaternion.Dot(source, other)), 1f)) * 2f;
        }

        /// <summary>
        /// Gets the axis components of the quaternion.
        /// </summary>
        /// <value>The axis components of the quaternion.</value>
        public static Vector3 Axis(this Quaternion q)
        {
            float length =  (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z);
            if (MathUtil.IsZero(length))
            {
                return Vector3.UnitX;
            }

            float inv = 1.0f / (float)Math.Sqrt(length);
                return new Vector3(q.X * inv, q.Y * inv, q.Z * inv);
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="q"></param>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public static float Get(this Quaternion q,int index)
        {
            return index switch
            {
                0 => q.X,
                1 => q.Y,
                2 => q.Z,
                3 => q.W,
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Indices for Quaternion run from 0 to 3, inclusive."),
            };
        }

        /// <summary>
        /// Sets the specified q.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentOutOfRangeException">index - Indices for Quaternion run from 0 to 3, inclusive.</exception>
        public static void Set(ref Quaternion q, int index, float value)
        {
                switch (index)
                {
                    case 0: q.X = value; break;
                    case 1: q.Y = value; break;
                    case 2: q.Z = value; break;
                    case 3: q.W = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index), "Indices for Quaternion run from 0 to 3, inclusive.");
                }
        }

        /// <summary>
        /// Creates an array containing the elements of the quaternion.
        /// </summary>
        /// <returns>A four-element array containing the components of the quaternion.</returns>
        public static float[] ToArray(this Quaternion q)
        {
            return new float[] { q.X, q.Y, q.Z, q.W };
        }

        /// <summary>
        /// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains a new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, float amount1, float amount2, out Quaternion result)
        {
            float a = amount1 + amount2;
            Quaternion start = Quaternion.Slerp(value1, value2, a);
            Quaternion end = Quaternion.Slerp(value1, value3, a);
            result = Quaternion.Slerp(start, end, amount2 / a);

            //Quaternion start, end;
            //Slerp(ref value1, ref value2, amount1 + amount2, out start);
            //Slerp(ref value1, ref value3, amount1 + amount2, out end);
            //Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
        }

        /// <summary>
        /// Returns a <see cref="Quaternion"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Quaternion"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="Quaternion"/> containing the 4D Cartesian coordinates of the specified point.</returns>
        public static Quaternion Barycentric(Quaternion value1, Quaternion value2, Quaternion value3, float amount1, float amount2)
        {
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <param name="result">When the method completes, contains the exponentiated quaternion.</param>
        public static void Exponential(ref Quaternion value, out Quaternion result)
        {
            float angle = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            float sin = (float)Math.Sin(angle);

            if (!MathUtil.IsZero(sin))
            {
                float coeff = sin / angle;
                result = value * coeff;
                //result.X = coeff * value.X;
                //result.Y = coeff * value.Y;
                //result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = (float)Math.Cos(angle);
        }

        /// <summary>
        /// Exponentiates a quaternion.
        /// </summary>
        /// <param name="value">The quaternion to exponentiate.</param>
        /// <returns>The exponentiated quaternion.</returns>
        public static Quaternion Exponential(Quaternion value)
        {
            Exponential(ref value, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <param name="result">When the method completes, contains the natural logarithm of the quaternion.</param>
        public static void Logarithm(ref Quaternion value, out Quaternion result)
        {
            if (Math.Abs(value.W) < 1.0)
            {
                float angle = (float)Math.Acos(value.W);
                float sin = (float)Math.Sin(angle);

                if (!MathUtil.IsZero(sin))
                {
                    float coeff = angle / sin;
                    result = value * coeff;
                    //result.X = value.X * coeff;
                    //result.Y = value.Y * coeff;
                    //result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }

            result.W = 0.0f;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified quaternion.
        /// </summary>
        /// <param name="value">The quaternion whose logarithm will be calculated.</param>
        /// <returns>The natural logarithm of the quaternion.</returns>
        public static Quaternion Logarithm(Quaternion value)
        {
            Logarithm(ref value, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Rotates a Vector3 by the specified quaternion rotation.
        /// </summary>
        /// <param name="value">The quaternion.</param>
        /// <param name="vector">The vector to rotate.</param>
        public static void Rotate(Quaternion value,ref Vector3 vector)
        {
            // Ref: https://github.com/stride3d/stride/blob/master/sources/core/Stride.Core.Mathematics/Quaternion.cs
            var pureQuaternion = new Quaternion(vector, 0);
            pureQuaternion = Quaternion.Conjugate(value) * pureQuaternion * value;

            vector.X = pureQuaternion.X;
            vector.Y = pureQuaternion.Y;
            vector.Z = pureQuaternion.Z;
        }

        /// <summary>
        /// Rotates a Vector3 by the specified quaternion rotation.
        /// </summary>
        /// <param name="value">The quaternion.</param>
        /// <param name="vector">The vector to rotate.</param>
        /// <returns>A newly Vector3 rotated by the specified quaternion rotation.</returns>
        public static Vector3 Rotate(this Quaternion value, Vector3 vector)
        {
            Rotate(value, ref vector);
            return vector;
        }

        /// <summary>
        /// Creates a quaternion given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationAxis(ref Vector3 axis, float angle, out Quaternion result)
        {
            result = Quaternion.CreateFromAxisAngle(axis, angle);
            //Vector3 normalized;
            //Vector3.Normalize(ref axis, out normalized);

            //float half = angle * 0.5f;
            //float sin = (float)Math.Sin(half);
            //float cos = (float)Math.Cos(half);

            //result.X = normalized.X * sin;
            //result.Y = normalized.Y * sin;
            //result.Z = normalized.Z * sin;
            //result.W = cos;
        }

        /// <summary>
        /// Creates a quaternion given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion RotationAxis(Vector3 axis, float angle)
        {
            return Quaternion.CreateFromAxisAngle(axis, angle);
            //Quaternion result;
            //RotationAxis(ref axis, angle, out result);
            //return result;
        }

        /// <summary>
        /// Computes a quaternion corresponding to the rotation transforming the vector <paramref name="source"/> to the vector <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source vector of the transformation.</param>
        /// <param name="target">The target vector of the transformation.</param>
        /// <returns>The resulting quaternion corresponding to the transformation of the source vector to the target vector.</returns>
        public static Quaternion BetweenDirections(ref Vector3 source, ref Vector3 target)
        {
            // Ref: https://github.com/stride3d/stride/blob/master/sources/core/Stride.Core.Mathematics/Quaternion.cs
            Quaternion result;
            float norms = (float)Math.Sqrt(source.LengthSquared() * target.LengthSquared());
            float real = norms + Vector3.Dot(source, target);
            if (real < MathUtil.ZeroTolerance * norms)
            {
                // If source and target are exactly opposite, rotate 180 degrees around an arbitrary orthogonal axis.
                // Axis normalisation can happen later, when we normalise the quaternion.
                result = Math.Abs(source.X) > Math.Abs(source.Z)
                    ? new Quaternion(-source.Y, source.X, 0.0f, 0.0f)
                    : new Quaternion(0.0f, -source.Z, source.Y, 0.0f);
            }
            else
            {
                // Otherwise, build quaternion the standard way.
                var axis = Vector3.Cross(source, target);
                result = new Quaternion(axis, real);
            }
            return Quaternion.Normalize(result);

        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref Matrix matrix, out Quaternion result)
        {
            result = Quaternion.CreateFromRotationMatrix(matrix);
            //float sqrt;
            //float half;
            //float scale = matrix.M11 + matrix.M22 + matrix.M33;

            //if (scale > 0.0f)
            //{
            //    sqrt = (float)Math.Sqrt(scale + 1.0f);
            //    result.W = sqrt * 0.5f;
            //    sqrt = 0.5f / sqrt;

            //    result.X = (matrix.M23 - matrix.M32) * sqrt;
            //    result.Y = (matrix.M31 - matrix.M13) * sqrt;
            //    result.Z = (matrix.M12 - matrix.M21) * sqrt;
            //}
            //else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
            //    half = 0.5f / sqrt;

            //    result.X = 0.5f * sqrt;
            //    result.Y = (matrix.M12 + matrix.M21) * half;
            //    result.Z = (matrix.M13 + matrix.M31) * half;
            //    result.W = (matrix.M23 - matrix.M32) * half;
            //}
            //else if (matrix.M22 > matrix.M33)
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
            //    half = 0.5f / sqrt;

            //    result.X = (matrix.M21 + matrix.M12) * half;
            //    result.Y = 0.5f * sqrt;
            //    result.Z = (matrix.M32 + matrix.M23) * half;
            //    result.W = (matrix.M31 - matrix.M13) * half;
            //}
            //else
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
            //    half = 0.5f / sqrt;

            //    result.X = (matrix.M31 + matrix.M13) * half;
            //    result.Y = (matrix.M32 + matrix.M23) * half;
            //    result.Z = 0.5f * sqrt;
            //    result.W = (matrix.M12 - matrix.M21) * half;
            //}
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationMatrix(ref Matrix3x3 matrix, out Quaternion result)
        {
            result = Quaternion.CreateFromRotationMatrix(matrix.ToMatrix());
            //float sqrt;
            //float half;
            //float scale = matrix.M11 + matrix.M22 + matrix.M33;

            //if (scale > 0.0f)
            //{
            //    sqrt = (float)Math.Sqrt(scale + 1.0f);
            //    result.W = sqrt * 0.5f;
            //    sqrt = 0.5f / sqrt;

            //    result.X = (matrix.M23 - matrix.M32) * sqrt;
            //    result.Y = (matrix.M31 - matrix.M13) * sqrt;
            //    result.Z = (matrix.M12 - matrix.M21) * sqrt;
            //}
            //else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
            //    half = 0.5f / sqrt;

            //    result.X = 0.5f * sqrt;
            //    result.Y = (matrix.M12 + matrix.M21) * half;
            //    result.Z = (matrix.M13 + matrix.M31) * half;
            //    result.W = (matrix.M23 - matrix.M32) * half;
            //}
            //else if (matrix.M22 > matrix.M33)
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
            //    half = 0.5f / sqrt;

            //    result.X = (matrix.M21 + matrix.M12) * half;
            //    result.Y = 0.5f * sqrt;
            //    result.Z = (matrix.M32 + matrix.M23) * half;
            //    result.W = (matrix.M31 - matrix.M13) * half;
            //}
            //else
            //{
            //    sqrt = (float)Math.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
            //    half = 0.5f / sqrt;

            //    result.X = (matrix.M31 + matrix.M13) * half;
            //    result.Y = (matrix.M32 + matrix.M23) * half;
            //    result.Z = 0.5f * sqrt;
            //    result.W = (matrix.M12 - matrix.M21) * half;
            //}
        }

        /// <summary>
        /// Creates a left-handed, look-at quaternion.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        public static void LookAtLH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Quaternion result)
        {
            Matrix3x3.LookAtLH(ref eye, ref target, ref up, out Matrix3x3 matrix);
            RotationMatrix(ref matrix, out result);
        }

        /// <summary>
        /// Creates a left-handed, look-at quaternion.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at quaternion.</returns>
        public static Quaternion LookAtLH(Vector3 eye, Vector3 target, Vector3 up)
        {
            LookAtLH(ref eye, ref target, ref up, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, look-at quaternion.
        /// </summary>
        /// <param name="forward">The camera's forward direction.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        public static void RotationLookAtLH(ref Vector3 forward, ref Vector3 up, out Quaternion result)
        {
            Vector3 eye = Vector3.Zero;
            LookAtLH(ref eye, ref forward, ref up, out result);
        }

        /// <summary>
        /// Creates a left-handed, look-at quaternion.
        /// </summary>
        /// <param name="forward">The camera's forward direction.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at quaternion.</returns>
        public static Quaternion RotationLookAtLH(Vector3 forward, Vector3 up)
        {
            RotationLookAtLH(ref forward, ref up, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, look-at quaternion.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        public static void LookAtRH(ref Vector3 eye, ref Vector3 target, ref Vector3 up, out Quaternion result)
        {
            Matrix3x3.LookAtRH(ref eye, ref target, ref up, out Matrix3x3 matrix);
            RotationMatrix(ref matrix, out result);
        }

        /// <summary>
        /// Creates a right-handed, look-at quaternion.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at quaternion.</returns>
        public static Quaternion LookAtRH(Vector3 eye, Vector3 target, Vector3 up)
        {
            LookAtRH(ref eye, ref target, ref up, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, look-at quaternion.
        /// </summary>
        /// <param name="forward">The camera's forward direction.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at quaternion.</param>
        public static void RotationLookAtRH(ref Vector3 forward, ref Vector3 up, out Quaternion result)
        {
            Vector3 eye = Vector3.Zero;
            LookAtRH(ref eye, ref forward, ref up, out result);
        }

        /// <summary>
        /// Creates a right-handed, look-at quaternion.
        /// </summary>
        /// <param name="forward">The camera's forward direction.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at quaternion.</returns>
        public static Quaternion RotationLookAtRH(Vector3 forward, Vector3 up)
        {
            RotationLookAtRH(ref forward, ref up, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard quaternion.</param>
        public static void BillboardLH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Quaternion result)
        {
            Matrix3x3.BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out Matrix3x3 matrix);
            RotationMatrix(ref matrix, out result);
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>The created billboard quaternion.</returns>
        public static Quaternion BillboardLH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        {
            BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard quaternion.</param>
        public static void BillboardRH(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUpVector, ref Vector3 cameraForwardVector, out Quaternion result)
        {
            Matrix3x3.BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out Matrix3x3 matrix);
            RotationMatrix(ref matrix, out result);
        }

        /// <summary>
        /// Creates a right-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>The created billboard quaternion.</returns>
        public static Quaternion BillboardRH(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
        {
            BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion RotationMatrix(Matrix matrix)
        {
            RotationMatrix(ref matrix, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created quaternion.</param>
        public static void RotationYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
        {
            result = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            //float halfRoll = roll * 0.5f;
            //float halfPitch = pitch * 0.5f;
            //float halfYaw = yaw * 0.5f;

            //float sinRoll = (float)Math.Sin(halfRoll);
            //float cosRoll = (float)Math.Cos(halfRoll);
            //float sinPitch = (float)Math.Sin(halfPitch);
            //float cosPitch = (float)Math.Cos(halfPitch);
            //float sinYaw = (float)Math.Sin(halfYaw);
            //float cosYaw = (float)Math.Cos(halfYaw);

            //result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
            //result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
            //result.Z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
            //result.W = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
        }

        /// <summary>
        /// Creates a quaternion given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <returns>The newly created quaternion.</returns>
        public static Quaternion RotationYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            //Quaternion result;
            //RotationYawPitchRoll(yaw, pitch, roll, out result);
            //return result;
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the quaternions.</param>
        public static void Squad(ref Quaternion value1, ref Quaternion value2, ref Quaternion value3, ref Quaternion value4, float amount, out Quaternion result)
        {
            Quaternion start = Quaternion.Slerp(value1, value4, amount);
            Quaternion end = Quaternion.Slerp(value2, value3, amount);
            result = Quaternion.Slerp(start, end, 2.0f * amount * (1.0f - amount));
        }

        /// <summary>
        /// Interpolates between quaternions, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <returns>The spherical quadrangle interpolation of the quaternions.</returns>
        public static Quaternion Squad(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4, float amount)
        {
            Squad(ref value1, ref value2, ref value3, ref value4, amount, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source quaternion.</param>
        /// <param name="value2">Second source quaternion.</param>
        /// <param name="value3">Third source quaternion.</param>
        /// <param name="value4">Fourth source quaternion.</param>
        /// <returns>An array of three quaternions that represent control points for spherical quadrangle interpolation.</returns>
        public static Quaternion[] SquadSetup(Quaternion value1, Quaternion value2, Quaternion value3, Quaternion value4)
        {
            Quaternion q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
            Quaternion q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
            Quaternion q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
            Quaternion q1 = value2;

            Exponential(ref q1, out Quaternion q1Exp);
            Exponential(ref q2, out Quaternion q2Exp);

            Quaternion[] results = new Quaternion[3];
            results[0] = q1 * Exponential((Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)) * -0.25f);
            results[1] = q2 * Exponential((Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)) * -0.25f);
            results[2] = q2;

            return results;
        }

        public static Matrix ToMatrix(this Quaternion rotation)
        {
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            Matrix result = Matrix.Identity;
            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (zz + xx));
            result.M23 = 2.0f * (yz + xw);
            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (yy + xx));
            return result;
        }
    }
}
