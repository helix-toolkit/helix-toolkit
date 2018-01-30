/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DirectWrite;
using System.IO;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using global::SharpDX.Direct2D1;
    using global::SharpDX.WIC;
    using System;
    public static class BitmapExtensions
    {
        public static MemoryStream ToBitmapStream(this string text, int fontSize, Color4 foreground,
            Color4 background, string fontFamily, FontWeight fontWeight, FontStyle fontStyle, Vector4 padding, ref float width, ref float height, bool predefinedSize,
            IDeviceResources deviceResources)
        {
            using (var factory = new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Isolated))
            {
                using (var format = new TextFormat(factory, fontFamily, fontSize))
                {
                    using (var layout = new TextLayout(factory, text, format, float.MaxValue, float.MaxValue))
                    {
                        var metrices = layout.Metrics;
                        if (!predefinedSize)
                        {
                            width = (float)Math.Ceiling(metrices.WidthIncludingTrailingWhitespace + padding.X + padding.W);
                            height = (float)Math.Ceiling(metrices.Height + padding.Y + padding.Z);
                        }
                        else
                        {
                            var scale = width / height;
                            width = (float)Math.Ceiling(metrices.WidthIncludingTrailingWhitespace + padding.X + padding.W);
                            height = width / scale;
                        }

                        using (var imgFactory = new ImagingFactory())
                        {
                            using (var bitmap = new global::SharpDX.WIC.Bitmap(imgFactory, (int)width, (int)height, global::SharpDX.WIC.PixelFormat.Format32bppBGR,
                                BitmapCreateCacheOption.CacheOnLoad))
                            {

                                using (var target = new WicRenderTarget(deviceResources.DeviceContext2D.Factory, bitmap,
                                    new RenderTargetProperties()
                                    {
                                        DpiX = deviceResources.DeviceContext2D.DotsPerInch.Width,
                                        DpiY = deviceResources.DeviceContext2D.DotsPerInch.Height,
                                        MinLevel = FeatureLevel.Level_DEFAULT,
                                        PixelFormat = new global::SharpDX.Direct2D1.PixelFormat(global::SharpDX.DXGI.Format.Unknown, AlphaMode.Unknown)
                                    }))
                                {
                                    target.Transform = Matrix3x2.Identity;
                                    target.BeginDraw();
                                    target.Clear(background);
                                    using (var brush = new SolidColorBrush(target, foreground))
                                    {
                                        target.DrawTextLayout(new Vector2(padding.X, padding.Y), layout, brush);
                                    }
                                    target.EndDraw();
                                }

                                var systemStream = new MemoryStream();

                                using (var stream = new WICStream(imgFactory, systemStream))
                                {
                                    using (var encoder = new BitmapEncoder(imgFactory, ContainerFormatGuids.Png))
                                    {
                                        encoder.Initialize(stream);
                                        using (var frameEncoder = new BitmapFrameEncode(encoder))
                                        {
                                            frameEncoder.Initialize();
                                            frameEncoder.SetSize((int)width, (int)height);
                                            frameEncoder.WriteSource(bitmap);
                                            frameEncoder.Commit();
                                            encoder.Commit();
                                            return systemStream;
                                        }
                                    }
                                }                         
                            }
                        }
                    }
                }
            }
        }
    }

//    public class Direct2DImageEncoder
//    {
//        private readonly Direct2DFactoryManager factoryManager;
//        private readonly Bitmap wicBitmap;
//        private readonly WicRenderTarget renderTarget;

//        private readonly int imageWidth, imageHeight;

//        public Direct2DImageEncoder(int imageWidth, int imageHeight, int imageDpi)
//        {
//            this.imageWidth = imageWidth;
//            this.imageHeight = imageHeight;

//            factoryManager = new Direct2DFactoryManager();

//            wicBitmap = new Bitmap(factoryManager.WicFactory, imageWidth, imageHeight, global::SharpDX.WIC.PixelFormat.Format32bppBGR, BitmapCreateCacheOption.CacheOnLoad);
//            var renderTargetProperties = new RenderTargetProperties(RenderTargetType.Default, new PixelFormat(Format.Unknown, AlphaMode.Unknown), imageDpi, imageDpi, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);
//            renderTarget = new WicRenderTarget(factoryManager.D2DFactory, wicBitmap, renderTargetProperties);
//            renderTarget.BeginDraw();
//            renderTarget.Clear(Colors.White);
//        }

//        public void Save(Stream systemStream, Direct2DImageFormat format)
//        {
//            renderTarget.EndDraw();

//            var stream = new WICStream(factoryManager.WicFactory, systemStream);
//            var encoder = new BitmapEncoder(factoryManager.WicFactory, Direct2DConverter.ConvertImageFormat(format));
//            encoder.Initialize(stream);

//            var bitmapFrameEncode = new BitmapFrameEncode(encoder);
//            bitmapFrameEncode.Initialize();
//            bitmapFrameEncode.SetSize(imageWidth, imageHeight);
//            bitmapFrameEncode.PixelFormat = global::SharpDX.WIC.PixelFormat.FormatDontCare;
//            bitmapFrameEncode.WriteSource(wicBitmap);

//            bitmapFrameEncode.Commit();
//            encoder.Commit();

//            bitmapFrameEncode.Dispose();
//            encoder.Dispose();
//            stream.Dispose();
//        }
//    }

//    public class Direct2DFactoryManager
//    {
//        private readonly global::SharpDX.WIC.ImagingFactory wicFactory;
//        private readonly global::SharpDX.Direct2D1.Factory d2DFactory;
//        private readonly global::SharpDX.DirectWrite.Factory dwFactory;

//        public Direct2DFactoryManager()
//        {
//            wicFactory = new global::SharpDX.WIC.ImagingFactory();
//            d2DFactory = new global::SharpDX.Direct2D1.Factory();
//            dwFactory = new global::SharpDX.DirectWrite.Factory();
//        }

//        public global::SharpDX.WIC.ImagingFactory WicFactory
//        {
//            get
//            {
//                return wicFactory;
//            }
//        }

//        public global::SharpDX.Direct2D1.Factory D2DFactory
//        {
//            get
//            {
//                return d2DFactory;
//            }
//        }

//        public global::SharpDX.DirectWrite.Factory DwFactory
//        {
//            get
//            {
//                return dwFactory;
//            }
//        }
//    }

//    public enum Direct2DImageFormat
//    {
//        Png, Gif, Ico, Jpeg, Wmp, Tiff, Bmp
//    }

//    public class Direct2DConverter
//    {
//        public static Guid ConvertImageFormat(Direct2DImageFormat format)
//        {
//            switch (format)
//            {
//                case Direct2DImageFormat.Bmp:
//                    return ContainerFormatGuids.Bmp;
//                case Direct2DImageFormat.Ico:
//                    return ContainerFormatGuids.Ico;
//                case Direct2DImageFormat.Gif:
//                    return ContainerFormatGuids.Gif;
//                case Direct2DImageFormat.Jpeg:
//                    return ContainerFormatGuids.Jpeg;
//                case Direct2DImageFormat.Png:
//                    return ContainerFormatGuids.Png;
//                case Direct2DImageFormat.Tiff:
//                    return ContainerFormatGuids.Tiff;
//                case Direct2DImageFormat.Wmp:
//                    return ContainerFormatGuids.Wmp;
//            }
//            throw new NotSupportedException();
//        }

//#endregion
   // }
}
