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
    public enum ImagePackReturnCode
    {
        Succeed = 0,
        FailedToPackImage,
        FailedToCreateImage,
        DeviceFailed
    }

    public abstract class SpritePackerBase<T, E> : IDisposable
    {
        // various properties of the resulting image
        private bool requirePow2, requireSquare;
        private int padding;
        protected int OutputWidth { private set; get; }
        protected int OutputHeight { private set; get; }

        // the input list of image files
        protected KeyValuePair<int, E>[] ItemArray { private set; get; }
        // some dictionaries to hold the image sizes and destination rectangles
        protected Size2F[] ImageSizes { private set; get; }
        protected RectangleF[] ImagePlacement { private set; get; }
        protected readonly IDevice2DResources deviceRes2D;

        public SpritePackerBase(IDevice2DResources deviceResources)
        {
            deviceRes2D = deviceResources;
        }
        /// <summary>
        /// Packs a collection of items into a single image.
        /// </summary>
        /// <param name="items">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement rectangles for the images.</param>
        /// <param name="imageHeight">Output the merged image height</param>
        /// <param name="imageWidth">Output the merged image width</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public ImagePackReturnCode Pack(
            IEnumerable<T> items,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            out Bitmap outputImage, out int imageWidth, out int imageHeight,
            out RectangleF[] outputMap)
        {
            imageWidth = 0;
            imageHeight = 0;
            if (deviceRes2D.Device2D == null || deviceRes2D.Device2D.IsDisposed)
            {
                outputImage = null;
                outputMap = null;
                return ImagePackReturnCode.DeviceFailed;
            }
            ItemArray = GetArray(items);

            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            OutputWidth = maximumWidth;
            OutputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            ImageSizes = ItemArray.Select(x => GetSize(x.Value)).ToArray();
            ImagePlacement = new RectangleF[ItemArray.Length];
            // sort our files by file size so we place large sprites first
            Array.Sort(ItemArray,
                (f1, f2) =>
                {
                    var b1 = GetSize(f1.Value);
                    var b2 = GetSize(f2.Value);

                    int c = -b1.Width.CompareTo(b2.Width);
                    if (c != 0)
                        return c;

                    c = -b1.Height.CompareTo(b2.Height);
                    if (c != 0)
                        return c;

                    return 0;
                });

            // try to pack the images
            if (!PackImageRectangles())
                return ImagePackReturnCode.FailedToPackImage;

            // make our output image
            outputImage = CreateOutputImage(DrawOntoOutputTarget);
            if (outputImage == null)
                return ImagePackReturnCode.FailedToCreateImage;

            // copy the placement dictionary to the output
            outputMap = ImagePlacement;
            imageWidth = OutputWidth;
            imageHeight = OutputHeight;
            return ImagePackReturnCode.Succeed;
        }

        // This method does some trickery type stuff where we perform the TestPackingImages method over and over, 
        // trying to reduce the image size until we have found the smallest possible image we can fit.
        private bool PackImageRectangles()
        {
            // we need a couple values for testing
            int testWidth = OutputWidth;
            int testHeight = OutputHeight;
           
            // try to pack the images into our current test size
            if (!TestPackingImages(testWidth, testHeight, ImagePlacement, out int packedWidth, out int packedHeight))
            {
                return false;
            }
            testWidth = packedWidth;
            testHeight = packedHeight;
            // if we require a power of two texture, find the next power of two that can fit this image
            if (requirePow2)
            {
                testWidth = MiscHelper.FindNextPowerOfTwo(testWidth);
                testHeight = MiscHelper.FindNextPowerOfTwo(testHeight);
            }

            // if we require a square texture, set the width and height to the larger of the two
            if (requireSquare)
            {
                int max = Math.Max(testWidth, testHeight);
                testWidth = testHeight = max;
            }

            // save the test results as our last known good results
            OutputWidth = testWidth;
            OutputHeight = testHeight;
            return true;
        }

        private bool TestPackingImages(int testWidth, int testHeight, RectangleF[] testImagePlacement,
            out int packedWidth, out int packedHeight)
        {
            packedWidth = packedHeight = 0;
            // create the rectangle packer
            var rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);
            foreach (var image in ItemArray)
            {
                // get the bitmap for this file
                var size = ImageSizes[image.Key];

                // pack the image
                if (!rectanglePacker.TryPack((int)Math.Ceiling(size.Width + padding), (int)Math.Ceiling(size.Height + padding), out Point origin))
                {
                    return false;
                }

                // add the destination rectangle to our dictionary
                testImagePlacement[image.Key] = new RectangleF(origin.X, origin.Y, size.Width, size.Height);
            }
            packedWidth = rectanglePacker.ActualPackingAreaWidth;
            packedHeight = rectanglePacker.ActualPackingAreaHeight;
            return true;
        }

        private Bitmap CreateOutputImage(Action<WicRenderTarget> action)
        {
            try
            {
                var bitmap =
                    new Bitmap(deviceRes2D.WICImgFactory, OutputWidth, OutputHeight,
                    global::SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                        BitmapCreateCacheOption.CacheOnDemand);
                using (var target = new WicRenderTarget(deviceRes2D.Factory2D, bitmap,
                    new RenderTargetProperties()
                    {
                        DpiX = 96,
                        DpiY = 96,
                        MinLevel = FeatureLevel.Level_DEFAULT,
                        PixelFormat = new global::SharpDX.Direct2D1.PixelFormat(global::SharpDX.DXGI.Format.Unknown, AlphaMode.Unknown)
                    }))
                {
                    target.Transform = Matrix3x2.Identity;
                    target.BeginDraw();
                    action(target);
                    target.EndDraw();
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        protected abstract KeyValuePair<int, E>[] GetArray(IEnumerable<T> items);
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected abstract Size2F GetSize(E value);
        /// <summary>
        /// Draws the onto output target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected abstract void DrawOntoOutputTarget(WicRenderTarget target);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ImagePacker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}