#region MIT License

/*
 * Copyright (c) 2009-2010 Nick Gravelyn (nick@gravelyn.com), Markus Ewald (cygon@nuclex.org)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 * 
 */

#endregion

using SharpDX;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities.ImagePacker
#else
namespace HelixToolkit.UWP.Utilities.ImagePacker
#endif
{
    /// <summary>Base class for rectangle packing algorithms</summary>
    /// <remarks>
    ///   <para>
    ///     By uniting all rectangle packers under this common base class, you can
    ///     easily switch between different algorithms to find the most efficient or
    ///     performant one for a given job.
    ///   </para>
    ///   <para>
    ///     An almost exhaustive list of packing algorithms can be found here:
    ///     http://www.csc.liv.ac.uk/~epa/surveyhtml.html
    ///   </para>
    /// </remarks>
    internal abstract class RectanglePacker
	{
		/// <summary>Maximum width the packing area is allowed to have</summary>
		protected int PackingAreaWidth { get; private set; }

		/// <summary>Maximum height the packing area is allowed to have</summary>
		protected int PackingAreaHeight { get; private set; }

		/// <summary>Initializes a new rectangle packer</summary>
		/// <param name="packingAreaWidth">Width of the packing area</param>
		/// <param name="packingAreaHeight">Height of the packing area</param>
		protected RectanglePacker(int packingAreaWidth, int packingAreaHeight)
		{
			PackingAreaWidth = packingAreaWidth;
			PackingAreaHeight = packingAreaHeight;
		}

		/// <summary>Allocates space for a rectangle in the packing area</summary>
		/// <param name="rectangleWidth">Width of the rectangle to allocate</param>
		/// <param name="rectangleHeight">Height of the rectangle to allocate</param>
		/// <returns>The location at which the rectangle has been placed</returns>
		public virtual Point Pack(int rectangleWidth, int rectangleHeight)
		{
			Point point;

			if (!TryPack(rectangleWidth, rectangleHeight, out point))
				throw new OutOfSpaceException("Rectangle does not fit in packing area");

			return point;
		}

		/// <summary>Tries to allocate space for a rectangle in the packing area</summary>
		/// <param name="rectangleWidth">Width of the rectangle to allocate</param>
		/// <param name="rectangleHeight">Height of the rectangle to allocate</param>
		/// <param name="placement">Output parameter receiving the rectangle's placement</param>
		/// <returns>True if space for the rectangle could be allocated</returns>
		public abstract bool TryPack(int rectangleWidth, int rectangleHeight, out Point placement);
	}
}