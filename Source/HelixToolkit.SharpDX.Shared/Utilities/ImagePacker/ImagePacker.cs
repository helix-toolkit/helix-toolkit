#region MIT License

/*
 * Copyright (c) 2018 HelixToolkit Contributors (MIT License)
 * Modified from https://github.com/nickgravelyn/SpriteSheetPacker
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
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Linq;
using Bitmap = SharpDX.WIC.Bitmap;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities.ImagePacker
#else
namespace HelixToolkit.UWP.Utilities.ImagePacker
#endif
{
    /// <summary>
    /// Pack a list of WIC.Bitmap into a single large bitmap
    /// </summary>
    public sealed class ImagePacker : SpritePackerBase<Bitmap, Bitmap>
    {
        public ImagePacker(IDevice2DResources deviceResources) : base(deviceResources)
        {
        }

        protected override void DrawOntoOutputTarget(WicRenderTarget target)
        {
            // draw all the images into the output image
            foreach (var image in ItemArray)
            {
                var location = ImagePlacement[image.Key];
                var img = image.Value;
                using (var bmp = global::SharpDX.Direct2D1.Bitmap.FromWicBitmap(target, img))
                {
                    target.DrawBitmap(bmp,
                        new RawRectangleF(location.Left, location.Top, location.Right, location.Bottom),
                        1, global::SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                }
            }
        }

        protected override KeyValuePair<int, Bitmap>[] GetArray(IEnumerable<Bitmap> items)
        {
            return items.Select((x,i)=>new KeyValuePair<int, Bitmap>(i, x)).ToArray();
        }

        protected override Size2F GetSize(Bitmap value)
        {
            return value.Size.ToSizeF();
        }
    }
}