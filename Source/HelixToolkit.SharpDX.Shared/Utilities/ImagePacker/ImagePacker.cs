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
    public sealed class ImagePacker : IDisposable
    {
        private struct TextLayoutInfo
        {
            public readonly TextLayout TextLayout;
            public readonly Color4 Foreground;
            public readonly Color4 Background;
            public readonly Vector4 Padding;
            public TextLayoutInfo(TextLayout layout, Color4 foreground, Color4 background, Vector4 padding)
            {
                TextLayout = layout;
                Foreground = foreground;
                Background = background;
                Padding = padding;
            }
        }
        // various properties of the resulting image
        private bool requirePow2, requireSquare;
        private int padding;
        private int outputWidth, outputHeight;

        // the input list of image files
        private KeyValuePair<int, Bitmap>[] bitmaps;
        private KeyValuePair<int, TextLayoutInfo>[] texts;
        // some dictionaries to hold the image sizes and destination rectangles
        private readonly Dictionary<int, Size2F> imageSizes = new Dictionary<int, Size2F>();
        private readonly Dictionary<int, RectangleF> imagePlacement = new Dictionary<int, RectangleF>();
        private readonly IDevice2DResources deviceRes2D;

        public ImagePacker(IDevice2DResources deviceResources)
        {
            deviceRes2D = deviceResources;
        }
        /// <summary>
        /// Packs a collection of images into a single image.
        /// </summary>
        /// <param name="imageFiles">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="generateMap">Whether or not to generate the map dictionary.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement rectangles for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public FailCode PackImage(
            IEnumerable<KeyValuePair<int, Bitmap>> imageFiles,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            bool generateMap,
            out Bitmap outputImage,
            out Dictionary<int, RectangleF> outputMap)
        {
            if(deviceRes2D.Device2D==null || deviceRes2D.Device2D.IsDisposed)
            {
                outputImage = null;
                outputMap = null;
                return FailCode.DeviceFailed;
            }
            texts = null;
            bitmaps = imageFiles.ToArray();

            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            outputWidth = maximumWidth;
            outputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageSizes.Clear();
            imagePlacement.Clear();

            // get the sizes of all the images
            foreach (var image in imageFiles)
            {
                imageSizes.Add(image.Key, image.Value.Size.ToSizeF());
            }

            // sort our files by file size so we place large sprites first
            Array.Sort(bitmaps,
                (f1, f2) =>
                {
                    var b1 = f1.Value.Size;
                    var b2 = f2.Value.Size;

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
            outputImage = CreateOutputImage((target) =>
            {
                // draw all the images into the output image
                foreach (var image in bitmaps)
                {
                    var location = imagePlacement[image.Key];
                    var img = image.Value;
                    using (var bmp = global::SharpDX.Direct2D1.Bitmap.FromWicBitmap(target, img))
                    {
                        target.DrawBitmap(bmp,
                            new RawRectangleF(location.Left, location.Top, location.Right, location.Bottom),
                            1, global::SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                    }
                }
            });
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
            bitmaps = null;
            // clear our dictionaries just to free up some memory
            imageSizes.Clear();
            imagePlacement.Clear();

            return 0;
        }

        public FailCode PackText(IEnumerable<KeyValuePair<int, TextInfoExt>> textInfos,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            bool generateMap,
            out Bitmap outputImage,
            out Dictionary<int, RectangleF> outputMap)
        {
            if (deviceRes2D.Device2D == null || deviceRes2D.Device2D.IsDisposed)
            {
                outputImage = null;
                outputMap = null;
                return FailCode.DeviceFailed;
            }
            bitmaps = null;
            texts = textInfos.Select(x=> 
            {
                var textLayout = BitmapExtensions
                .GetTextLayoutMetrices(x.Value.Text, deviceRes2D, x.Value.Size, x.Value.FontFamily,
                x.Value.FontWeight, x.Value.FontStyle);
                return new KeyValuePair<int, TextLayoutInfo>(x.Key, 
                    new TextLayoutInfo(textLayout, x.Value.Foreground, x.Value.Background, x.Value.Padding));
            }).ToArray();

            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            outputWidth = maximumWidth;
            outputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageSizes.Clear();
            imagePlacement.Clear();

            // get the sizes of all the images
            foreach (var text in texts)
            {
                var width = (float)Math.Ceiling(text.Value.TextLayout.Metrics.WidthIncludingTrailingWhitespace 
                    + text.Value.Padding.X + text.Value.Padding.W);
                var height = (float)Math.Ceiling(text.Value.TextLayout.Metrics.Height
                    + text.Value.Padding.Y + text.Value.Padding.Z);
                imageSizes.Add(text.Key, new Size2F(text.Value.TextLayout.Metrics.Width, 
                    text.Value.TextLayout.Metrics.Height));
            }

            // sort our files by file size so we place large sprites first
            Array.Sort(texts,
                (f1, f2) =>
                {
                    var b1 = f1.Value;
                    var b2 = f2.Value;

                    int c = -b1.TextLayout.Metrics.Width.CompareTo(b2.TextLayout.Metrics.Width);
                    if (c != 0)
                        return c;

                    c = -b1.TextLayout.Metrics.Height.CompareTo(b2.TextLayout.Metrics.Height);
                    if (c != 0)
                        return c;

                    return 0;
                });

            // try to pack the images
            if (!PackImageRectangles())
                return FailCode.FailedToPackImage;

            // make our output image
            outputImage = CreateOutputImage((target)=> 
            {
                // draw all the images into the output image
                foreach (var text in texts)
                {
                    var location = imagePlacement[text.Key];
                    var t = text.Value;
                    using (var brush = new SolidColorBrush(target, t.Background))
                        target.FillRectangle(location, brush);
                    using (var brush = new SolidColorBrush(target, t.Foreground))
                        target.DrawTextLayout(new Vector2(location.Left, location.Top), t.TextLayout, brush, DrawTextOptions.None);
                }
            });
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
                    var r = imagePlacement[k];

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

            // clear our dictionaries just to free up some memory
            textInfos = null;
            imageSizes.Clear();
            imagePlacement.Clear();

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
            int testWidth = outputWidth;
            int testHeight = outputHeight;

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
                if (testWidth == outputWidth && testHeight == outputHeight)
                {
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                }

                // save the test results as our last known good results
                outputWidth = testWidth;
                outputHeight = testHeight;

                // subtract the smallest image size out for the next test iteration
                if (!shrinkVertical)
                    testWidth -= smallestWidth;
                testHeight -= smallestHeight;
            }
        }

        private bool TestPackingImages(int testWidth, int testHeight, Dictionary<int, RectangleF> testImagePlacement)
        {
            // create the rectangle packer
            ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);
            if(bitmaps != null && bitmaps.Length > 0)
            {
                foreach (var image in bitmaps)
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
            }
            else if(texts != null && texts.Length > 0)
            {
                foreach (var text in texts)
                {
                    // get the bitmap for this file
                    var size = imageSizes[text.Key];

                    // pack the image
                    if (!rectanglePacker.TryPack((int)Math.Ceiling(size.Width + padding), (int)Math.Ceiling(size.Height + padding), out Point origin))
                    {
                        return false;
                    }

                    // add the destination rectangle to our dictionary
                    testImagePlacement.Add(text.Key, new RectangleF(origin.X, origin.Y, size.Width + padding, size.Height + padding));
                }
            }
            else
            { return false; }
            return true;
        }

        private Bitmap CreateOutputImage(Action<WicRenderTarget> action)
        {
            try
            {
                var bitmap =
                    new Bitmap(deviceRes2D.WICImgFactory, outputWidth, outputHeight,
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