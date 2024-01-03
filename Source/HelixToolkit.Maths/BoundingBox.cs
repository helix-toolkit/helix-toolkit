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
    /// Represents an axis-aligned bounding box in three dimensional space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoundingBox : IEquatable<BoundingBox>, IFormattable
    {
        /// <summary>
        /// The minimum point of the box.
        /// </summary>
        public Vector3 Minimum;

        /// <summary>
        /// The maximum point of the box.
        /// </summary>
        public Vector3 Maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> struct.
        /// </summary>
        /// <param name="minimum">The minimum vertex of the bounding box.</param>
        /// <param name="maximum">The maximum vertex of the bounding box.</param>
        public BoundingBox(Vector3 minimum, Vector3 maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Returns the width of the bounding box
        /// </summary>
        public readonly float Width
        {
            get { return this.Maximum.X - this.Minimum.X; }
        }

        /// <summary>
        /// Returns the height of the bounding box
        /// </summary>
        public readonly float Height
        {
            get { return this.Maximum.Y - this.Minimum.Y; }
        }

        /// <summary>
        /// Returns the height of the bounding box
        /// </summary>
        public readonly float Depth
        {
            get { return this.Maximum.Z - this.Minimum.Z; }
        }

        /// <summary>
        /// Returns the size of the bounding box
        /// </summary>
        public readonly Vector3 Size
        {
            get { return this.Maximum - this.Minimum; }
        }

        /// <summary>
        /// Returns the size of the bounding box
        /// </summary>
        public readonly Vector3 Center
        {
            get { return (this.Maximum + this.Minimum) * 0.5f; }
        }

        /// <summary>
        /// Retrieves the eight corners of the bounding box.
        /// </summary>
        /// <returns>An array of points representing the eight corners of the bounding box.</returns>
        public readonly Vector3[] GetCorners()
        {
            Vector3[] results = new Vector3[8];
            GetCorners(results);
            return results;
        }

        /// <summary>
        /// Retrieves the eight corners of the bounding box.
        /// </summary>
        /// <returns>An array of points representing the eight corners of the bounding box.</returns>
        public readonly void GetCorners(Vector3[] corners)
        {
            corners[0] = new Vector3(Minimum.X, Maximum.Y, Maximum.Z);
            corners[1] = new Vector3(Maximum.X, Maximum.Y, Maximum.Z);
            corners[2] = new Vector3(Maximum.X, Minimum.Y, Maximum.Z);
            corners[3] = new Vector3(Minimum.X, Minimum.Y, Maximum.Z);
            corners[4] = new Vector3(Minimum.X, Maximum.Y, Minimum.Z);
            corners[5] = new Vector3(Maximum.X, Maximum.Y, Minimum.Z);
            corners[6] = new Vector3(Maximum.X, Minimum.Y, Minimum.Z);
            corners[7] = new Vector3(Minimum.X, Minimum.Y, Minimum.Z);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Ray ray)
        {
            return Collision.RayIntersectsBox(ref ray, ref this, out float _);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="distance">When the method completes, contains the distance of the intersection,
        /// or 0 if there was no intersection.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Ray ray, out float distance)
        {
            return Collision.RayIntersectsBox(ref ray, ref this, out distance);
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
            return Collision.RayIntersectsBox(ref ray, ref this, out point);
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="Plane"/>.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref Plane plane)
        {
            return Collision.PlaneIntersectsBox(ref plane, ref this);
        }

        /* This implementation is wrong
        /// <summary>
        /// Determines if there is an intersection between the current object and a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            return Collision.BoxIntersectsTriangle(ref this, ref vertex1, ref vertex2, ref vertex3);
        }
        */

        /// <summary>
        /// Determines if there is an intersection between the current object and a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingBox box)
        {
            return Collision.BoxIntersectsBox(ref this, ref box);
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
        /// Determines if there is an intersection between the current object and a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public bool Intersects(ref BoundingSphere sphere)
        {
            return Collision.BoxIntersectsSphere(ref this, ref sphere);
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
        /// Determines whether the current objects contains a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(ref Vector3 point)
        {
            return Collision.BoxContainsPoint(ref this, ref point);
        }

        /// <summary>
        /// Determines whether the current objects contains a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(Vector3 point)
        {
            return Contains(ref point);
        }

        /* This implementation is wrong
        /// <summary>
        /// Determines whether the current objects contains a triangle.
        /// </summary>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
        {
            return Collision.BoxContainsTriangle(ref this, ref vertex1, ref vertex2, ref vertex3);
        }
        */

        /// <summary>
        /// Determines whether the current objects contains a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(ref BoundingBox box)
        {
            return Collision.BoxContainsBox(ref this, ref box);
        }

        /// <summary>
        /// Determines whether the current objects contains a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(BoundingBox box)
        {
            return Contains(ref box);
        }

        /// <summary>
        /// Determines whether the current objects contains a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(ref BoundingSphere sphere)
        {
            return Collision.BoxContainsSphere(ref this, ref sphere);
        }

        /// <summary>
        /// Determines whether the current objects contains a <see cref="BoundingSphere"/>.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public ContainmentType Contains(BoundingSphere sphere)
        {
            return Contains(ref sphere);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the box.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding box.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public static void FromPoints(Vector3[] points, out BoundingBox result)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            Vector3 min = new(float.MaxValue);
            Vector3 max = new(float.MinValue);

            for (int i = 0; i < points.Length; ++i)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            result = new BoundingBox(min, max);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that fully contains the given points.
        /// </summary>
        /// <param name="points">The points that will be contained by the box.</param>
        /// <returns>The newly constructed bounding box.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        public static BoundingBox FromPoints(Vector3[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            Vector3 min = new(float.MaxValue);
            Vector3 max = new(float.MinValue);

            for (int i = 0; i < points.Length; ++i)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> from a given sphere.
        /// </summary>
        /// <param name="sphere">The sphere that will designate the extents of the box.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding box.</param>
        public static void FromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Minimum = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
            result.Maximum = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> from a given sphere.
        /// </summary>
        /// <param name="sphere">The sphere that will designate the extents of the box.</param>
        /// <returns>The newly constructed bounding box.</returns>
        public static BoundingBox FromSphere(BoundingSphere sphere)
        {
            BoundingBox box;
            box.Minimum = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
            box.Maximum = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
            return box;
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that is as large as the total combined area of the two specified boxes.
        /// </summary>
        /// <param name="value1">The first box to merge.</param>
        /// <param name="value2">The second box to merge.</param>
        /// <param name="result">When the method completes, contains the newly constructed bounding box.</param>
        public static void Merge(ref BoundingBox value1, ref BoundingBox value2, out BoundingBox result)
        {
            result.Minimum = Vector3.Min(value1.Minimum, value2.Minimum);
            result.Maximum = Vector3.Max(value1.Maximum, value2.Maximum);
        }

        /// <summary>
        /// Constructs a <see cref="BoundingBox"/> that is as large as the total combined area of the two specified boxes.
        /// </summary>
        /// <param name="value1">The first box to merge.</param>
        /// <param name="value2">The second box to merge.</param>
        /// <returns>The newly constructed bounding box.</returns>
        public static BoundingBox Merge(BoundingBox value1, BoundingBox value2)
        {
            return new BoundingBox(Vector3.Min(value1.Minimum, value2.Minimum), Vector3.Max(value1.Maximum, value2.Maximum));
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(BoundingBox left, BoundingBox right)
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
        public static bool operator !=(BoundingBox left, BoundingBox right)
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
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
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
            return format == null
                ? ToString()
                : string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(format, CultureInfo.CurrentCulture),
                Maximum.ToString(format, CultureInfo.CurrentCulture));
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
            return string.Format(formatProvider, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
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
            return format == null && formatProvider == null
                ? string.Empty
                : format == null
                ? ToString(formatProvider!)
                : string.Format(formatProvider, "Minimum:{0} Maximum:{1}", Minimum.ToString(format, formatProvider),
                Maximum.ToString(format, formatProvider));
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
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
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
        public readonly bool Equals(ref BoundingBox value)
        {
            return Minimum == value.Minimum && Maximum == value.Maximum;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector4"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector4"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Vector4"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(BoundingBox value)
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
            return obj is BoundingBox b && Equals(ref b);
        }
    }
}
