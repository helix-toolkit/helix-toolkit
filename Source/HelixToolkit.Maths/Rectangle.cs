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
    /// Define a Rectangle. This structure is slightly different from System.Drawing.Rectangle as it is
    /// internally storing Left,Top,Right,Bottom instead of Left,Top,Width,Height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IEquatable<Rectangle>
    {
        /// <summary>
        /// The left.
        /// </summary>
        public int Left;

        /// <summary>
        /// The top.
        /// </summary>
        public int Top;

        /// <summary>
        /// The right.
        /// </summary>
        public int Right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public int Bottom;

        /// <summary>
        /// An empty rectangle.
        /// </summary>
        public static readonly Rectangle Empty = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="x">The left.</param>
        /// <param name="y">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Right = x + width;
            Bottom = y + height;
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        public int X
        {
            readonly get
            {
                return Left;
            }
            set
            {
                Right = value + Width;
                Left = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        public int Y
        {
            readonly get
            {
                return Top;
            }
            set
            {
                Bottom = value + Height;
                Top = value;
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            readonly get { return Right - Left; }
            set { Right = Left + value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            readonly get { return Bottom - Top; }
            set { Bottom = Top + value; }
        }


        /// <summary>
        /// Gets a value that indicates whether the rectangle is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is empty]; otherwise, <c>false</c>.
        /// </value>
        public readonly bool IsEmpty
        {
            get
            {
                return (Width == 0) && (Height == 0) && (X == 0) && (Y == 0);
            }
        }

        ///// <summary>
        ///// Gets or sets the location.
        ///// </summary>
        ///// <value>
        ///// The location.
        ///// </value>
        //public Point Location
        //{
        //    get
        //    {
        //        return new Point(X, Y);
        //    }
        //    set
        //    {
        //        X = value.X;
        //        Y = value.Y;
        //    }
        //}

        ///// <summary>
        ///// Gets the Point that specifies the center of the rectangle.
        ///// </summary>
        ///// <value>
        ///// The center.
        ///// </value>
        //public Point Center
        //{
        //    get
        //    {
        //        return new Point(X + (Width / 2), Y + (Height / 2));
        //    }
        //}


        ///// <summary>
        ///// Gets or sets the size of the rectangle.
        ///// </summary>
        ///// <value>The size of the rectangle.</value>
        //public Size2 Size
        //{
        //    get
        //    {
        //        return new Size2(Width, Height);
        //    }
        //    set
        //    {
        //        Width = value.Width;
        //        Height = value.Height;
        //    }
        //}

        ///// <summary>
        ///// Gets the position of the top-left corner of the rectangle.
        ///// </summary>
        ///// <value>The top-left corner of the rectangle.</value>
        //public Point TopLeft { get { return new Point(_left, _top); } }

        ///// <summary>
        ///// Gets the position of the top-right corner of the rectangle.
        ///// </summary>
        ///// <value>The top-right corner of the rectangle.</value>
        //public Point TopRight { get { return new Point(_right, _top); } }

        ///// <summary>
        ///// Gets the position of the bottom-left corner of the rectangle.
        ///// </summary>
        ///// <value>The bottom-left corner of the rectangle.</value>
        //public Point BottomLeft { get { return new Point(_left, _bottom); } }

        ///// <summary>
        ///// Gets the position of the bottom-right corner of the rectangle.
        ///// </summary>
        ///// <value>The bottom-right corner of the rectangle.</value>
        //public Point BottomRight { get { return new Point(_right, _bottom); } }

        ///// <summary>Changes the position of the rectangle.</summary>
        ///// <param name="amount">The values to adjust the position of the rectangle by.</param>
        //public void Offset(Point amount)
        //{
        //    Offset(amount.X, amount.Y);
        //}

        /// <summary>Changes the position of the rectangle.</summary>
        /// <param name="offsetX">Change in the x-position.</param>
        /// <param name="offsetY">Change in the y-position.</param>
        public void Offset(int offsetX, int offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>Pushes the edges of the rectangle out by the horizontal and vertical values specified.</summary>
        /// <param name="horizontalAmount">Value to push the sides out by.</param>
        /// <param name="verticalAmount">Value to push the top and bottom out by.</param>
        public void Inflate(int horizontalAmount, int verticalAmount)
        {
            X -= horizontalAmount;
            Y -= verticalAmount;
            Width += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        /// <summary>Determines whether this rectangle contains a specified point represented by its x- and y-coordinates.</summary>
        /// <param name="x">The x-coordinate of the specified point.</param>
        /// <param name="y">The y-coordinate of the specified point.</param>
        public readonly bool Contains(int x, int y)
        {
            return (X <= x) && (x < Right) && (Y <= y) && (y < Bottom);
        }

        ///// <summary>Determines whether this rectangle contains a specified Point.</summary>
        ///// <param name="value">The Point to evaluate.</param>
        //public bool Contains(Point value)
        //{
        //    bool result;
        //    Contains(ref value, out result);
        //    return result;
        //}

        ///// <summary>Determines whether this rectangle contains a specified Point.</summary>
        ///// <param name="value">The Point to evaluate.</param>
        ///// <param name="result">[OutAttribute] true if the specified Point is contained within this rectangle; false otherwise.</param>
        //public void Contains(ref Point value, out bool result)
        //{
        //    result = (X <= value.X) && (value.X < Right) && (Y <= value.Y) && (value.Y < Bottom);
        //}

        /// <summary>Determines whether this rectangle entirely contains a specified rectangle.</summary>
        /// <param name="value">The rectangle to evaluate.</param>
        public readonly bool Contains(Rectangle value)
        {
            Contains(ref value, out bool result);
            return result;
        }

        /// <summary>Determines whether this rectangle entirely contains a specified rectangle.</summary>
        /// <param name="value">The rectangle to evaluate.</param>
        /// <param name="result">[OutAttribute] On exit, is true if this rectangle entirely contains the specified rectangle, or false if not.</param>
        public readonly void Contains(ref Rectangle value, out bool result)
        {
            result = (X <= value.X) && (value.Right <= Right) && (Y <= value.Y) && (value.Bottom <= Bottom);
        }

        /// <summary>
        /// Checks, if specified point is inside <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="x">X point coordinate.</param>
        /// <param name="y">Y point coordinate.</param>
        /// <returns><c>true</c> if point is inside <see cref="Rectangle"/>, otherwise <c>false</c>.</returns>
        public readonly bool Contains(float x, float y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        ///// <summary>
        ///// Checks, if specified <see cref="SharpDX.Vector2"/> is inside <see cref="SharpDX.Rectangle"/>.
        ///// </summary>
        ///// <param name="vector2D">Coordinate <see cref="SharpDX.Vector2"/>.</param>
        ///// <returns><c>true</c> if <see cref="SharpDX.Vector2"/> is inside <see cref="SharpDX.Rectangle"/>, otherwise <c>false</c>.</returns>
        //public bool Contains(Vector2 vector2D)
        //{
        //    return Contains(vector2D.X, vector2D.Y);
        //}

        /// <summary>Determines whether a specified rectangle intersects with this rectangle.</summary>
        /// <param name="value">The rectangle to evaluate.</param>
        public readonly bool Intersects(Rectangle value)
        {
            Intersects(ref value, out bool result);
            return result;
        }

        /// <summary>
        /// Determines whether a specified rectangle intersects with this rectangle.
        /// </summary>
        /// <param name="value">The rectangle to evaluate</param>
        /// <param name="result">[OutAttribute] true if the specified rectangle intersects with this one; false otherwise.</param>
        public readonly void Intersects(ref Rectangle value, out bool result)
        {
            result = (value.X < Right) && (X < value.Right) && (value.Y < Bottom) && (Y < value.Bottom);
        }

        /// <summary>
        /// Creates a rectangle defining the area where one rectangle overlaps with another rectangle.
        /// </summary>
        /// <param name="value1">The first rectangle to compare.</param>
        /// <param name="value2">The second rectangle to compare.</param>
        /// <returns>The intersection rectangle.</returns>
        public static Rectangle Intersect(Rectangle value1, Rectangle value2)
        {
            Intersect(ref value1, ref value2, out Rectangle result);
            return result;
        }

        /// <summary>Creates a rectangle defining the area where one rectangle overlaps with another rectangle.</summary>
        /// <param name="value1">The first rectangle to compare.</param>
        /// <param name="value2">The second rectangle to compare.</param>
        /// <param name="result">[OutAttribute] The area where the two first parameters overlap.</param>
        public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            int newLeft = (value1.X > value2.X) ? value1.X : value2.X;
            int newTop = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            int newRight = (value1.Right < value2.Right) ? value1.Right : value2.Right;
            int newBottom = (value1.Bottom < value2.Bottom) ? value1.Bottom : value2.Bottom;
            result = (newRight > newLeft) && (newBottom > newTop) ? new Rectangle(newLeft, newTop, newRight - newLeft, newBottom - newTop) : Empty;
        }

        /// <summary>
        /// Creates a new rectangle that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first rectangle to contain.</param>
        /// <param name="value2">The second rectangle to contain.</param>
        /// <returns>The union rectangle.</returns>
        public static Rectangle Union(Rectangle value1, Rectangle value2)
        {
            Union(ref value1, ref value2, out Rectangle result);
            return result;
        }

        /// <summary>
        /// Creates a new rectangle that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first rectangle to contain.</param>
        /// <param name="value2">The second rectangle to contain.</param>
        /// <param name="result">[OutAttribute] The rectangle that must be the union of the first two rectangles.</param>
        public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
        {
            int left = Math.Min(value1.Left, value2.Left);
            int right = Math.Max(value1.Right, value2.Right);
            int top = Math.Min(value1.Top, value2.Top);
            int bottom = Math.Max(value1.Bottom, value2.Bottom);
            result = new Rectangle(left, top, right - left, bottom - top);
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
            return obj is Rectangle rect && Equals(ref rect);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Rectangle"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Rectangle"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Rectangle"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(ref Rectangle other)
        {
            return other.Left == Left && other.Top == Top && other.Right == Right && other.Bottom == Bottom;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Rectangle"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Rectangle"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Rectangle"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public readonly bool Equals(Rectangle other)
        {
            return Equals(ref other);
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
                int result = Left;
                result = (result * 397) ^ Top;
                result = (result * 397) ^ Right;
                result = (result * 397) ^ Bottom;
                return result;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // MethodImplOptions.AggressiveInlining
        public static bool operator ==(Rectangle left, Rectangle right)
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
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !left.Equals(ref right);
        }

        ///// <summary>
        ///// Performs an implicit conversion to the <see cref="RectangleF"/> structure.
        ///// </summary>
        ///// <remarks>Performs direct converstion from int to float.</remarks>
        ///// <param name="value">The source <see cref="Rectangle"/> value.</param>
        ///// <returns>The converted structure.</returns>
        //public static implicit operator RectangleF(Rectangle value)
        //{
        //    return new RectangleF(value.X, value.Y, value.Width, value.Height);
        //}

        public override readonly string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "X:{0} Y:{1} Width:{2} Height:{3}", X, Y, Width, Height);
        }

        internal void MakeXYAndWidthHeight()
        {
            Right -= Left;
            Bottom -= Top;
        }
    }
}
