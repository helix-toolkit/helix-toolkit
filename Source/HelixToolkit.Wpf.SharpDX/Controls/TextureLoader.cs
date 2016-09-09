
// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.WIC;
using global::SharpDX.Mathematics;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
using Device = global::SharpDX.Direct3D11.Device;
using Format = global::SharpDX.DXGI.Format;
using BitmapSource = SharpDX.WIC.BitmapSource;
using MediaBitmapSource = System.Windows.Media.Imaging.BitmapSource;
using Rectangle = global::SharpDX.Rectangle;
using System.IO;

namespace HelixToolkit.Wpf.SharpDX
    {
       
        public class TextureLoader
        {
            public static MediaBitmapSource LoadBitmap(string filename)
            {
            FileStream fileStream =
            new FileStream(filename, FileMode.Open, FileAccess.Read);

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            img.EndInit();
            return img;
        }
            /// <summary>
            /// Loads a bitmap using WIC.
            /// </summary>
            /// <param name="deviceManager"></param>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static BitmapSource LoadBitmap(ImagingFactory factory, string filename)
            {
                var bitmapDecoder = new BitmapDecoder(
                    factory,
                    filename,
                    DecodeOptions.CacheOnDemand
                    );

                var formatConverter = new FormatConverter(factory);

                formatConverter.Initialize(
                    bitmapDecoder.GetFrame(0),
                    PixelFormat.Format32bppPRGBA,
                   BitmapDitherType.None,
                    null,
                    0.0,
                   BitmapPaletteType.Custom);

                return formatConverter;
            }
            public static Texture2D CreateTexture2DFromMediaBitamp(Device device,MediaBitmapSource bitmapSource)
            {
            int stride = bitmapSource.PixelWidth * 4;
            using (var buffer = new DataStream(bitmapSource.PixelHeight * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                byte[] array = new byte[bitmapSource.PixelHeight * stride];
                bitmapSource.CopyPixels(array, stride, 0);
                buffer.Write(array, 0, bitmapSource.PixelHeight * stride);
                return new Texture2D(device, new Texture2DDescription()
                {
                    Width = bitmapSource.PixelWidth,
                    Height = bitmapSource.PixelHeight,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = ResourceUsage.Immutable,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                }, new DataRectangle(buffer.DataPointer, stride));
            }
        }
            /// <summary>
            /// Creates a <see cref="SharpDX.Direct3D11.Texture2D"/> from a WIC <see cref="SharpDX.WIC.BitmapSource"/>
            /// </summary>
            /// <param name="device">The Direct3D11 device</param>
            /// <param name="bitmapSource">The WIC bitmap source</param>
            /// <returns>A Texture2D</returns>
            public static Texture2D CreateTexture2DFromBitmap(Device device, BitmapSource bitmapSource)
            {
                // Allocate DataStream to receive the WIC image pixels
                int stride = bitmapSource.Size.Width * 4;
                using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
                {
                // Copy the content of the WIC to the buffer
                Rectangle rect = new Rectangle();
                var methods = bitmapSource.GetType().GetMethods();
                bitmapSource.GetType().GetMethods().First(item => item.Name == "CopyPixels").Invoke(bitmapSource, new object[] { stride, buffer });
                  //  bitmapSource.CopyPixels(stride, buffer);
                    return new Texture2D(device, new Texture2DDescription()
                    {
                        Width = bitmapSource.Size.Width,
                        Height = bitmapSource.Size.Height,
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        Usage = ResourceUsage.Immutable,
                        CpuAccessFlags =CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                    }, new DataRectangle(buffer.DataPointer, stride));
                }
            }
        }
    }


   


