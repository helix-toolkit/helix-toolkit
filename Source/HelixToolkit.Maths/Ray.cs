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
    /// Represents a three dimensional line based on a point in space and a direction.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Ray : IEquatable<Ray>, IFormattable
    {
        /// <summary>
        /// The position in three dimensional space where the ray starts.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The normalized direction in which the ray points.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray"/> struct.
        /// </summary>
        /// <param name="position">The position in three dimensional space of the origin of the ray.</param>
        /// <param name="direction">The normalized direction of the ray.</param>
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = Vector3.Normalize(direction);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Vector3 point)
        {
            return Collision.RayIntersectsPoint(ref this, ref point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Ray ray)
        {
            return Collision.RayIntersectsRay(ref this, ref ray, out _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsRay(ref this, ref ray, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Plane plane)
        {
            return Collision.RayIntersectsPlane(ref this, ref plane, out float _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Plane plane, out float distance)
        {
            return Collision.RayIntersectsPlane(ref this, ref plane, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Plane plane, out Vector3 point)
        {
            return Collision.RayIntersectsPlane(ref this, ref plane, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            return Collision.RayIntersectsTriangle(ref this, ref vertex1, ref vertex2, ref vertex3, out float _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out float distance)
        {
            return Collision.RayIntersectsTriangle(ref this, ref vertex1, ref vertex2, ref vertex3, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out Vector3 point)
        {
            return Collision.RayIntersectsTriangle(ref this, ref vertex1, ref vertex2, ref vertex3, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingBox box)
        {
            return Collision.RayIntersectsBox(ref this, ref box, out float _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(BoundingBox box)
        {
            return Intersects(ref box);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingBox box, out float distance)
        {
            return Collision.RayIntersectsBox(ref this, ref box, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingBox box, out Vector3 point)
        {
            return Collision.RayIntersectsBox(ref this, ref box, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingSphere sphere)
        {
            return Collision.RayIntersectsSphere(ref this, ref sphere, out float _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(BoundingSphere sphere)
        {
            return Intersects(ref sphere);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingSphere sphere, out float distance)
        {
            return Collision.RayIntersectsSphere(ref this, ref sphere, out distance);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="point">When the method completes, contains the point of intersection,
        /// or <see cref="Vector3.Zero"/> if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingSphere sphere, out Vector3 point)
        {
            return Collision.RayIntersectsSphere(ref this, ref sphere, out point);
        }
        /// <summary>
        /// Planes the intersection.
        /// </summary>
        /// <param name="planePosition">The plane position.</param>
        /// <param name="planeNormal">The plane normal.</param>
        /// <param name="intersect">The point intersection</param>
        /// <returns></returns>
        public bool PlaneIntersection(Vector3 planePosition, Vector3 planeNormal, out Vector3 intersect)
        {
            Plane plane = PlaneHelper.Create(planePosition, planeNormal);
            return Collision.RayIntersectsPlane(ref this, ref plane, out intersect);
        }

        /// <summary>
        /// Planes the intersection.
        /// </summary>
        /// <param name="plane">The plane</param>
        /// <param name="intersect">The point intersection</param>
        /// <returns></returns>
        public bool PlaneIntersection(Plane plane, out Vector3 intersect)
        {
            return Collision.RayIntersectsPlane(ref this, ref plane, out intersect);
        }
        /// <summary>
        /// Calculates a world space <see cref="Ray"/> from 2d screen coordinates.
        /// </summary>
        /// <param name="x">X coordinate on 2d screen.</param>
        /// <param name="y">Y coordinate on 2d screen.</param>
        /// <param name="viewport"><see cref="ViewportF"/>.</param>
        /// <param name="worldViewProjection">Transformation <see cref="Matrix"/>.</param>
        /// <returns>Resulting <see cref="Ray"/>.</returns>
        public static Ray GetPickRay(int x, int y, ViewportF viewport, Matrix worldViewProjection)
        {
            Vector3 nearPoint = new(x, y, 0);
            Vector3 farPoint = new(x, y, 1);

            nearPoint = Vector3Helper.Unproject(nearPoint, viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinDepth,
                                        viewport.MaxDepth, worldViewProjection);
            farPoint = Vector3Helper.Unproject(farPoint, viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinDepth,
                                        viewport.MaxDepth, worldViewProjection);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            return new Ray(nearPoint, direction);
        }

        /// <summary>
        /// Gets the point on the ray that is nearest the specified point.
        /// </summary>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <returns>
        /// The nearest point on the ray.
        /// </returns>
        public Vector3 GetNearest(Vector3 point)
        {
            return this.Position
                   + (Vector3.Dot(point - this.Position, this.Direction)
                   / this.Direction.LengthSquared() * this.Direction);
        }
        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Ray left, Ray right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(Ray left, Ray right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Position:{0} Direction:{1}", Position.ToString(), Direction.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string format)
        {
            return string.Format(CultureInfo.CurrentCulture, "Position:{0} Direction:{1}", Position.ToString(format, CultureInfo.CurrentCulture),
                Direction.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "Position:{0} Direction:{1}", Position.ToString(), Direction.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format(formatProvider, "Position:{0} Direction:{1}", Position.ToString(format, formatProvider),
                Direction.ToString(format, formatProvider));
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
                return (Position.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector4"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector4"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Vector4"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(ref Ray value)
        {
            return Position == value.Position && Direction == value.Direction;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector4"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector4"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Vector4"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(Ray value)
        {
            return Equals(ref value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Ray ray && Equals(ref ray);
        }
    }
}
