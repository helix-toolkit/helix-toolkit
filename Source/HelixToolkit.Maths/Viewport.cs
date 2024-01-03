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
    /// Defines the viewport dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Viewport : IEquatable<Viewport>
    {
        /// <summary>
        /// Position of the pixel coordinate of the upper-left corner of the viewport.
        /// </summary>
        public int X;

        /// <summary>
        /// Position of the pixel coordinate of the upper-left corner of the viewport.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width dimension of the viewport.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height dimension of the viewport.
        /// </summary>
        public int Height;

        /// <summary>
        /// Gets or sets the minimum depth of the clip volume.
        /// </summary>
        public float MinDepth;

        /// <summary>
        /// Gets or sets the maximum depth of the clip volume.
        /// </summary>
        public float MaxDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        public Viewport(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = 0f;
            MaxDepth = 1f;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Viewport(uint x, uint y, uint width, uint height) 
            : this((int)x, (int)y, (int)width, (int)height)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        /// <param name="minDepth">The minimum depth of the clip volume.</param>
        /// <param name="maxDepth">The maximum depth of the clip volume.</param>
        public Viewport(int x, int y, int width, int height, float minDepth, float maxDepth)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        public Viewport(uint x, uint y, uint width, uint height, float minDepth, float maxDepth)
            : this((int)x, (int)y, (int)width, (int)height, minDepth, maxDepth)
        {    
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> struct.
        /// </summary>
        /// <param name="bounds">A bounding box that defines the location and size of the viewport in a render target.</param>
        public Viewport(Rectangle bounds)
        {
            X = bounds.X;
            Y = bounds.Y;
            Width = bounds.Width;
            Height = bounds.Height;
            MinDepth = 0f;
            MaxDepth = 1f;
        }

        /// <summary>
        /// Gets the size of this resource.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds
        {
            readonly get
            {
                return new Rectangle(X, Y, Width, Height);
            }

            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Viewport"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Viewport"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Viewport"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(ref Viewport other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && MathUtil.NearEqual(MinDepth, other.MinDepth) && MathUtil.NearEqual(MaxDepth, other.MaxDepth);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Viewport"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Viewport"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Viewport"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(Viewport other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is Viewport v && Equals(ref v);
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
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                hashCode = (hashCode * 397) ^ MinDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxDepth.GetHashCode();
                return hashCode;
            }
        }
        
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Viewport left, Viewport right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator !=(Viewport left, Viewport right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Retrieves a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override readonly string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X:{0} Y:{1} Width:{2} Height:{3} MinDepth:{4} MaxDepth:{5}}}", X, Y, Width, Height, MinDepth, MaxDepth);
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="world">The world matrix.</param>
        /// <returns>The projected vector.</returns>
        public readonly Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = world * view * projection;
            //Matrix.Multiply(ref world, ref view, out matrix);
            //Matrix.Multiply(ref matrix, ref projection, out matrix);

            Project(ref source, ref matrix, out Vector3 vector);
            return vector;
        }

        /// <summary>
        /// Projects a 3D vector from object space into screen space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="matrix">A combined WorldViewProjection matrix.</param>
        /// <param name="vector">The projected vector.</param>
        public readonly void Project(ref Vector3 source, ref Matrix matrix, out Vector3 vector)
        {
            vector = Vector3Helper.TransformCoordinate(source, matrix);
            float a = (source.X * matrix.M14) + (source.Y * matrix.M24) + (source.Z * matrix.M34) + matrix.M44;

            if (!MathUtil.IsOne(a))
            {
                vector /= a;
            }

            vector.X = ((vector.X + 1f) * 0.5f * Width) + X;
            vector.Y = ((-vector.Y + 1f) * 0.5f * Height) + Y;
            vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;
        }

        /// <summary>
        /// Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="world">The world matrix.</param>
        /// <returns>The unprojected Vector.</returns>
        public readonly Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix.Invert(world * view * projection, out Matrix matrix);
            //Matrix.Multiply(ref world, ref view, out matrix);
            //Matrix.Multiply(ref matrix, ref projection, out matrix);
            //Matrix.Invert(matrix, out matrix);

            Unproject(ref source, ref matrix, out Vector3 vector);
            return vector;
        }

        /// <summary>
        /// Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="matrix">An inverted combined WorldViewProjection matrix.</param>
        /// <param name="vector">The unprojected vector.</param>
        public readonly void Unproject(ref Vector3 source, ref Matrix matrix, out Vector3 vector)
        {
            vector.X = ((source.X - X) / Width * 2f) - 1f;
            vector.Y = -(((source.Y - Y) / Height * 2f) - 1f);
            vector.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);

            float a = (vector.X * matrix.M14) + (vector.Y * matrix.M24) + (vector.Z * matrix.M34) + matrix.M44;
            vector = Vector3Helper.TransformCoordinate(vector, matrix);

            if (!MathUtil.IsOne(a))
            {
                vector /= a;
            }
        }

        /// <summary>
        /// Gets the aspect ratio used by the viewport.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public readonly float AspectRatio
        {
            get
            {
                return Height != 0 ? Width / (float)Height : 0f;
            }
        }

        public readonly ViewportF ToViewportF()
        {
            return new ViewportF(X, Y, Width, Height);
        }
    }
}
