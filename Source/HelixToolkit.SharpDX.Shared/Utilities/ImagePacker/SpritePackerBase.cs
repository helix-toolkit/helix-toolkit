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
    public enum FailCode
    {
        FailedParsingArguments = 1,
        ImageExporter,
        MapExporter,
        NoImages,
        ImageNameCollision,

        FailedToLoadImage,
        FailedToPackImage,
        FailedToCreateImage,
        FailedToSaveImage,
        FailedToSaveMap,
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
        protected readonly Dictionary<int, Size2F> imageSizes = new Dictionary<int, Size2F>();
        protected readonly Dictionary<int, RectangleF> imagePlacement = new Dictionary<int, RectangleF>();
        protected readonly IDevice2DResources deviceRes2D;

        public SpritePackerBase(IDevice2DResources deviceResources)
        {
            deviceRes2D = deviceResources;
        }
        /// <summary>
        /// Packs a collection of images into a single image.
        /// </summary>
        /// <param name="items">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="generateMap">Whether or not to generate the map dictionary.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement rectangles for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public FailCode Pack(
            IEnumerable<KeyValuePair<int, T>> items,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            bool generateMap,
            out Bitmap outputImage, out int imageWidth, out int imageHeight,
            out Dictionary<int, RectangleF> outputMap)
        {
            imageWidth = 0;
            imageHeight = 0;
            if (deviceRes2D.Device2D == null || deviceRes2D.Device2D.IsDisposed)
            {
                outputImage = null;
                outputMap = null;
                return FailCode.DeviceFailed;
            }
            ItemArray = GetArray(items);

            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            OutputWidth = maximumWidth;
            OutputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageSizes.Clear();
            imagePlacement.Clear();

            // get the sizes of all the images
            foreach (var image in ItemArray)
            {
                imageSizes.Add(image.Key, GetSize(image.Value));
            }

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
                return FailCode.FailedToPackImage;

            // make our output image
            outputImage = CreateOutputImage(DrawOntoOutputTarget);
            if (outputImage == null)
                return FailCode.FailedToSaveImage;

            if (generateMap)
            {
                // go through our image placements and replace the width/height found in there with
                // each image's actual width/height (since the ones in imagePlacement will have padding)
                int[] keys = new int[imagePlacement.Keys.Count];
                imagePlacement.Keys.CopyTo(keys, 0);
                foreach (var k in keys)
                {
                    // get the actual size
                    var s = imageSizes[k];

                    // get the placement rectangle
                    RectangleF r = imagePlacement[k];

                    // set the proper size
                    r.Width = s.Width;
                    r.Height = s.Height;

                    // insert back into the dictionary
                    imagePlacement[k] = r;
                }

                // copy the placement dictionary to the output
                outputMap = new Dictionary<int, RectangleF>();
                foreach (var pair in imagePlacement)
                {
                    outputMap.Add(pair.Key, pair.Value);
                }
            }
            ItemArray = null;
            // clear our dictionaries just to free up some memory
            imageSizes.Clear();
            imagePlacement.Clear();
            imageWidth = OutputWidth;
            imageHeight = OutputHeight;
            return 0;
        }

        // This method does some trickery type stuff where we perform the TestPackingImages method over and over, 
        // trying to reduce the image size until we have found the smallest possible image we can fit.
        private bool PackImageRectangles()
        {
            // create a dictionary for our test image placements
            var testImagePlacement = new Dictionary<int, RectangleF>();

            // get the size of our smallest image
            int smallestWidth = int.MaxValue;
            int smallestHeight = int.MaxValue;
            foreach (var size in imageSizes)
            {
                smallestWidth = (int)Math.Ceiling(Math.Min(smallestWidth, size.Value.Width));
                smallestHeight = (int)Math.Ceiling(Math.Min(smallestHeight, size.Value.Height));
            }

            // we need a couple values for testing
            int testWidth = OutputWidth;
            int testHeight = OutputHeight;

            bool shrinkVertical = false;

            // just keep looping...
            while (true)
            {
                // make sure our test dictionary is empty
                testImagePlacement.Clear();

                // try to pack the images into our current test size
                if (!TestPackingImages(testWidth, testHeight, testImagePlacement))
                {
                    // if that failed...

                    // if we have no images in imagePlacement, i.e. we've never succeeded at PackImages,
                    // show an error and return false since there is no way to fit the images into our
                    // maximum size texture
                    if (imagePlacement.Count == 0)
                        return false;

                    // otherwise return true to use our last good results
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                    testWidth += smallestWidth + padding + padding;
                    testHeight += smallestHeight + padding + padding;
                    continue;
                }

                // clear the imagePlacement dictionary and add our test results in
                imagePlacement.Clear();
                foreach (var pair in testImagePlacement)
                    imagePlacement.Add(pair.Key, pair.Value);

                // figure out the smallest bitmap that will hold all the images
                testWidth = testHeight = 0;
                foreach (var pair in imagePlacement)
                {
                    testWidth = (int)Math.Ceiling(Math.Max(testWidth, pair.Value.Right));
                    testHeight = (int)Math.Ceiling(Math.Max(testHeight, pair.Value.Bottom));
                }

                // subtract the extra padding on the right and bottom
                if (!shrinkVertical)
                    testWidth -= padding;
                testHeight -= padding;

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

                // if the test results are the same as our last output results, we've reached an optimal size,
                // so we can just be done
                if (testWidth == OutputWidth && testHeight == OutputHeight)
                {
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                }

                // save the test results as our last known good results
                OutputWidth = testWidth;
                OutputHeight = testHeight;

                // subtract the smallest image size out for the next test iteration
                if (!shrinkVertical)
                    testWidth -= smallestWidth;
                testHeight -= smallestHeight;
            }
        }

        private bool TestPackingImages(int testWidth, int testHeight, Dictionary<int, RectangleF> testImagePlacement)
        {
            // create the rectangle packer
            var rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);
            foreach (var image in ItemArray)
            {
                // get the bitmap for this file
                var size = imageSizes[image.Key];

                // pack the image
                if (!rectanglePacker.TryPack((int)Math.Ceiling(size.Width + padding), (int)Math.Ceiling(size.Height + padding), out Point origin))
                {
                    return false;
                }

                // add the destination rectangle to our dictionary
                testImagePlacement.Add(image.Key, new RectangleF(origin.X, origin.Y, size.Width + padding, size.Height + padding));
            }           
            return true;
        }

        private Bitmap CreateOutputImage(Action<WicRenderTarget> action)
        {
            try
            {
                var bitmap =
                    new Bitmap(deviceRes2D.WICImgFactory, OutputWidth, OutputHeight,
                    global::SharpDX.WIC.PixelFormat.Format32bppBGR,
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
        protected abstract KeyValuePair<int, E>[] GetArray(IEnumerable<KeyValuePair<int, T>> items);
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