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
    /// Represents a plane in three dimensional space.
    /// </summary>
    public static class PlaneHelper
    {
        /// <summary>
        /// Gets the plane.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="normal">The normal.</param>
        /// <returns></returns>
        public static Plane Create(Vector3 point, Vector3 normal)
        {
            normal = Vector3.Normalize(normal);
            return new Plane(normal, -Vector3.Dot(normal, point));
        }
        /// <summary>
        /// Gets the plane.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="point3">The point3.</param>
        /// <returns></returns>
        public static Plane Create(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            var p12 = point2 - point1;
            var p13 = point3 - point1;
            var normal = Vector3.Normalize(Vector3.Cross(p12, p13));
            return new Plane(normal, -Vector3.Dot(normal, point1));
            //float x1 = point2.X - point1.X;
            //float y1 = point2.Y - point1.Y;
            //float z1 = point2.Z - point1.Z;
            //float x2 = point3.X - point1.X;
            //float y2 = point3.Y - point1.Y;
            //float z2 = point3.Z - point1.Z;
            //float yz = (y1 * z2) - (z1 * y2);
            //float xz = (z1 * x2) - (x1 * z2);
            //float xy = (x1 * y2) - (y1 * x2);
            //float invPyth = 1.0f / (float)(Math.Sqrt((yz * yz) + (xz * xz) + (xy * xy)));
            //var Normal = Vector3.Zero;
            //Normal.X = yz * invPyth;
            //Normal.Y = xz * invPyth;
            //Normal.Z = xy * invPyth;
            //return new Plane(Normal, -Vector3.Dot(Normal, point1));
        }

        /// <summary>
        /// Changes the coefficients of the normal vector of the plane to make it of unit length.
        /// </summary>
        public static Plane NormalizeUnit(this Plane plane)
        {
            var length = 1 / plane.Normal.Length();
            plane.Normal *= length;
            plane.D *= length;
            return plane;
        }
        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the A, B, C, or D component, depending on the index.</value>
        /// <param name="p"></param>
        /// <param name="index">The index of the component to access. Use 0 for the A component, 1 for the B component, 2 for the C component, and 3 for the D component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public static float Get(this Plane p, int index)
        {
            switch (index)
            {
                case 0: return p.Normal.X;
                case 1: return p.Normal.Y;
                case 2: return p.Normal.Z;
                case 3: return p.D;
            }

            throw new ArgumentOutOfRangeException("index", "Indices for Plane run from 0 to 3, inclusive.");
        }

        public static void Set(ref Plane p, int index, float value)
        {
            switch (index)
            {
                case 0: p.Normal.X = value; break;
                case 1: p.Normal.Y = value; break;
                case 2: p.Normal.Z = value; break;
                case 3: p.D = value; break;
                default: throw new ArgumentOutOfRangeException("index", "Indices for Plane run from 0 to 3, inclusive.");
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the plane.
        /// </summary>
        /// <returns>A four-element array containing the components of the plane.</returns>
        public static float[] ToArray(this Plane p)
        {
            return new float[] { p.Normal.X, p.Normal.Y, p.Normal.Z, p.D };
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a point.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType Intersects(ref Plane p, ref Vector3 point)
        {
            return Collision.PlaneIntersectsPoint(ref p, ref point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a point.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static PlaneIntersectionType Intersects(this Plane p, ref Vector3 point)
        {
            return Collision.PlaneIntersectsPoint(ref p, ref point);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ray">The ray to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this Plane p, ref Ray ray)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out float distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="ray">The ray.</param>
        /// <returns></returns>
        public static bool Intersects(ref Plane p, ref Ray ray)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out float distance);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ray">The ray to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this Plane p, ref Ray ray, out float distance)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out distance);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public static bool Intersects(ref Plane p, ref Ray ray, out float distance)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out distance);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ray">The ray to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this Plane p, ref Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public static bool Intersects(ref Plane p, ref Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsPlane(ref ray, ref p, out point);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="plane">The plane to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this Plane p, ref Plane plane)
        {
            return Collision.PlaneIntersectsPlane(ref p, ref plane);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="plane">The plane.</param>
        /// <returns></returns>
        public static bool Intersects(ref Plane p, ref Plane plane)
        {
            return Collision.PlaneIntersectsPlane(ref p, ref plane);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="plane">The plane to test.</param>
        /// <param name="line">When the method completes, contains the line of intersection
        /// as a <see cref="Ray"/>, or a zero ray if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool Intersects(this Plane p, ref Plane plane, out Ray line)
        {
            return Collision.PlaneIntersectsPlane(ref p, ref plane, out line);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="plane">The plane.</param>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public static bool Intersects(ref Plane p, ref Plane plane, out Ray line)
        {
            return Collision.PlaneIntersectsPlane(ref p, ref plane, out line);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType Intersects(this Plane p, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            return Collision.PlaneIntersectsTriangle(ref p, ref vertex1, ref vertex2, ref vertex3);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="vertex1">The vertex1.</param>
        /// <param name="vertex2">The vertex2.</param>
        /// <param name="vertex3">The vertex3.</param>
        /// <returns></returns>
        public static PlaneIntersectionType Intersects(ref Plane p, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            return Collision.PlaneIntersectsTriangle(ref p, ref vertex1, ref vertex2, ref vertex3);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType Intersects(this Plane p, ref BoundingBox box)
        {
            return Collision.PlaneIntersectsBox(ref p, ref box);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="box">The box.</param>
        /// <returns></returns>
        public static PlaneIntersectionType Intersects(ref Plane p, ref BoundingBox box)
        {
            return Collision.PlaneIntersectsBox(ref p, ref box);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType Intersects(this Plane p, ref BoundingSphere sphere)
        {
            return Collision.PlaneIntersectsSphere(ref p, ref sphere);
        }
        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="sphere">The sphere.</param>
        /// <returns></returns>
        public static PlaneIntersectionType Intersects(ref Plane p, ref BoundingSphere sphere)
        { 
            return Collision.PlaneIntersectsSphere(ref p, ref sphere);
        }
        /// <summary>
        /// Check if a line intersects with plane
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static bool IntersectsLine(this Plane p, ref Vector3 p0, ref Vector3 p1, out Vector3 intersection)
        {
            // https://graphics.stanford.edu/~mdfisher/Code/Engine/Plane.cpp.html
            var diff = p0 - p1;
            var d = Vector3.Dot(diff, p.Normal);
            if (d == 0)
            {
                intersection = Vector3.Zero;
                return false;
            }
            var u = (Vector3.Dot(p0, p.Normal) + p.D) / d;
            intersection = (p0 + u * (p1 - p0));
            return true;
        }
        ///// <summary>
        ///// Check if a line intersects with plane
        ///// </summary>
        ///// <param name="p"></param>
        ///// <param name="p0"></param>
        ///// <param name="p1"></param>
        ///// <param name="intersection"></param>
        ///// <returns></returns>
        //public static bool IntersectsLine(this Plane p, Vector3 p0, Vector3 p1, out Vector3 intersection)
        //{
        //    return IntersectsLine(p, ref p0, ref p1, out intersection);
        //}
        /// <summary>
        /// Builds a matrix that can be used to reflect vectors about a plane.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="result">The result.</param>
        public static void Reflection(ref Plane p, out Matrix result)
        {
            var x = p.Normal.X;
            var y = p.Normal.Y;
            var z = p.Normal.Z;
            var x2 = -2.0f * x;
            var y2 = -2.0f * y;
            var z2 = -2.0f * z;

            result.M11 = (x2 * x) + 1.0f;
            result.M12 = y2 * x;
            result.M13 = z2 * x;
            result.M14 = 0.0f;
            result.M21 = x2 * y;
            result.M22 = (y2 * y) + 1.0f;
            result.M23 = z2 * y;
            result.M24 = 0.0f;
            result.M31 = x2 * z;
            result.M32 = y2 * z;
            result.M33 = (z2 * z) + 1.0f;
            result.M34 = 0.0f;
            result.M41 = x2 * p.D;
            result.M42 = y2 * p.D;
            result.M43 = z2 * p.D;
            result.M44 = 1.0f;
        }
        /// <summary>
        /// Builds a matrix that can be used to reflect vectors about a plane.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>The reflection matrix.</returns>
        public static Matrix Reflection(this Plane p)
        {
            Reflection(ref p, out Matrix result);
            return result;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow from the plane onto which to project the geometry as a shadow. 
        /// This plane  is assumed to be normalized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="result">When the method completes, contains the shadow matrix.</param>
        public static void Shadow(ref Plane p, ref Vector4 light, out Matrix result)
        {
            var dot = Plane.Dot(p, light);// (p.Normal.X * light.X) + (p.Normal.Y * light.Y) + (p.Normal.Z * light.Z) + (p.D * light.W);
            var x = -p.Normal.X;
            var y = -p.Normal.Y;
            var z = -p.Normal.Z;
            var d = -p.D;

            result.M11 = (x * light.X) + dot;
            result.M21 = y * light.X;
            result.M31 = z * light.X;
            result.M41 = d * light.X;
            result.M12 = x * light.Y;
            result.M22 = (y * light.Y) + dot;
            result.M32 = z * light.Y;
            result.M42 = d * light.Y;
            result.M13 = x * light.Z;
            result.M23 = y * light.Z;
            result.M33 = (z * light.Z) + dot;
            result.M43 = d * light.Z;
            result.M14 = x * light.W;
            result.M24 = y * light.W;
            result.M34 = z * light.W;
            result.M44 = (d * light.W) + dot;
        }

        /// <summary>
        /// Creates a matrix that flattens geometry into a shadow from this the plane onto which to project the geometry as a shadow. 
        /// This plane  is assumed to be normalized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <returns>The shadow matrix.</returns>
        public static Matrix Shadow(this Plane p, Vector4 light)
        {
            Shadow(ref p, ref light, out var result);
            return result;
        }

        /// <summary>
        /// Builds a Matrix3x3 that can be used to reflect vectors about a plane for which the reflection occurs. 
        /// This plane is assumed to be normalized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="result">When the method completes, contains the reflection Matrix3x3.</param>
        public static void Reflection(ref Plane p, out Matrix3x3 result)
        {
            var x = p.Normal.X;
            var y = p.Normal.Y;
            var z = p.Normal.Z;
            var x2 = -2.0f * x;
            var y2 = -2.0f * y;
            var z2 = -2.0f * z;

            result.M11 = (x2 * x) + 1.0f;
            result.M12 = y2 * x;
            result.M13 = z2 * x;
            result.M21 = x2 * y;
            result.M22 = (y2 * y) + 1.0f;
            result.M23 = z2 * y;
            result.M31 = x2 * z;
            result.M32 = y2 * z;
            result.M33 = (z2 * z) + 1.0f;
        }

        /// <summary>
        /// Builds a Matrix3x3 that can be used to reflect vectors about a plane for which the reflection occurs. 
        /// This plane is assumed to be normalized
        /// </summary>
        /// <returns>The reflection Matrix3x3.</returns>
        public static Matrix3x3 Reflection3x3(this Plane p)
        {
            Reflection(ref p, out Matrix3x3 result);
            return result;
        }

        /// <summary>
        /// Creates a Matrix3x3 that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <param name="result">When the method completes, contains the shadow Matrix3x3.</param>
        public static void Shadow(ref Vector4 light, ref Plane plane, out Matrix3x3 result)
        {
            var dot = Plane.Dot(plane, light);//(plane.Normal.X * light.X) + (plane.Normal.Y * light.Y) + (plane.Normal.Z * light.Z) + (plane.D * light.W);
            var x = -plane.Normal.X;
            var y = -plane.Normal.Y;
            var z = -plane.Normal.Z;

            result.M11 = (x * light.X) + dot;
            result.M21 = y * light.X;
            result.M31 = z * light.X;
            result.M12 = x * light.Y;
            result.M22 = (y * light.Y) + dot;
            result.M32 = z * light.Y;
            result.M13 = x * light.Z;
            result.M23 = y * light.Z;
            result.M33 = (z * light.Z) + dot;
        }

        /// <summary>
        /// Creates a Matrix3x3 that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <returns>The shadow Matrix3x3.</returns>
        public static Matrix3x3 Shadow(Vector4 light, Plane plane)
        {
            Shadow(ref light, ref plane, out var result);
            return result;
        }


        /// <summary>
        /// Scales the plane by the given scaling factor.
        /// </summary>
        /// <param name="value">The plane to scale.</param>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <param name="result">When the method completes, contains the scaled plane.</param>
        public static void Multiply(ref Plane value, float scale, out Plane result)
        {
            result.Normal = value.Normal * scale;
            result.D = value.D * scale;
        }

        /// <summary>
        /// Scales the plane by the given scaling factor.
        /// </summary>
        /// <param name="value">The plane to scale.</param>
        /// <param name="scale">The amount by which to scale the plane.</param>
        /// <returns>The scaled plane.</returns>
        public static Plane Multiply(Plane value, float scale)
        {
            return new Plane(value.Normal * scale, value.D * scale);
        }

        /// <summary>
        /// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
        /// </summary>
        /// <param name="left">The source plane.</param>
        /// <param name="right">The source vector.</param>
        /// <param name="result">When the method completes, contains the dot product of a specified vector and the normal of the Plane plus the distance value of the plane.</param>
        public static void DotCoordinate(ref Plane left, ref Vector3 right, out float result)
        {
            result = Vector3.Dot(left.Normal, right) + left.D;
        }

        /// <summary>
        /// Calculates the dot product of a specified vector and the normal of the plane plus the distance value of the plane.
        /// </summary>
        /// <param name="left">The source plane.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The dot product of a specified vector and the normal of the Plane plus the distance value of the plane.</returns>
        public static float DotCoordinate(Plane left, Vector3 right)
        {
            return Vector3.Dot(left.Normal, right) + left.D;
        }

        /// <summary>
        /// Calculates the dot product of the specified vector and the normal of the plane.
        /// </summary>
        /// <param name="left">The source plane.</param>
        /// <param name="right">The source vector.</param>
        /// <param name="result">When the method completes, contains the dot product of the specified vector and the normal of the plane.</param>
        public static void DotNormal(ref Plane left, ref Vector3 right, out float result)
        {
            result = Vector3.Dot(left.Normal, right);
        }

        /// <summary>
        /// Calculates the dot product of the specified vector and the normal of the plane.
        /// </summary>
        /// <param name="left">The source plane.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The dot product of the specified vector and the normal of the plane.</returns>
        public static float DotNormal(Plane left, Vector3 right)
        {
            return Vector3.Dot(left.Normal, right);
        }

        /// <summary>
        /// Transforms a normalized plane by a quaternion rotation.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <param name="result">When the method completes, contains the transformed plane.</param>
        public static void Transform(ref Plane plane, ref Quaternion rotation, out Plane result)
        {
            result = Plane.Transform(plane, rotation);
        }

        /// <summary>
        /// Transforms an array of normalized planes by a quaternion rotation.
        /// </summary>
        /// <param name="planes">The array of normalized planes to transform.</param>
        /// <param name="rotation">The quaternion rotation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="planes"/> is <c>null</c>.</exception>
        public static void Transform(Plane[] planes, ref Quaternion rotation)
        {
            if (planes == null)
            {
                throw new ArgumentNullException("planes");
            }

            for (var i=0; i < planes.Length; ++i)
            {
                planes[i] = Plane.Transform(planes[i], rotation);
            }
        }

        /// <summary>
        /// Transforms a normalized plane by a matrix.
        /// </summary>
        /// <param name="plane">The normalized source plane.</param>
        /// <param name="transformation">The transformation matrix.</param>
        /// <param name="result">When the method completes, contains the transformed plane.</param>
        public static void Transform(ref Plane plane, ref Matrix transformation, out Plane result)
        {
            result = Plane.Transform(plane, transformation);
        }

        /// <summary>
        /// Transforms an array of normalized planes by a matrix.
        /// </summary>
        /// <param name="planes">The array of normalized planes to transform.</param>
        /// <param name="transformation">The transformation matrix.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="planes"/> is <c>null</c>.</exception>
        public static void Transform(Plane[] planes, ref Matrix transformation)
        {
            if (planes == null)
            {
                throw new ArgumentNullException("planes");
            }

            for (var i = 0; i < planes.Length; ++i)
            {
                planes[i] = Plane.Transform(planes[i], transformation);
            }
        }
    }
}
