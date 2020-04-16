/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DirectWrite;
using System.IO;
using System;
using System.Collections.Generic;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Utilities.ImagePacker;

    public enum Direct2DImageFormat
    {
        Png, Gif, Ico, Jpeg, Wmp, Tiff, Bmp
    }
    public static class BitmapExtensions
    {
        public static MemoryStream ToBitmapStream(this string text, int fontSize, Color4 foreground,
            Color4 background, string fontFamily, FontWeight fontWeight, FontStyle fontStyle, Vector4 padding, ref float width, ref float height, bool predefinedSize,
            IDevice2DResources deviceResources)
        {
            using (var layout = GetTextLayoutMetrices(text, deviceResources, fontSize, fontFamily, fontWeight, fontStyle))
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

                using (var bitmap = CreateBitmapStream(deviceResources, (int)width, (int)height, Direct2DImageFormat.Bmp, (target) =>
                 {
                     target.Clear(background);
                     using (var brush = new SolidColorBrush(target, foreground))
                     {
                         target.DrawTextLayout(new Vector2(padding.X, padding.Y), layout, brush);
                     }
                 }))
                {
                    return bitmap.ToMemoryStream(deviceResources, Direct2DImageFormat.Bmp);
                }
            }
        }

        public static TextLayout GetTextLayoutMetrices(this string text, IDevice2DResources deviceResources, int fontSize, string fontFamily, FontWeight fontWeight, FontStyle fontStyle,
            float maxWidth = float.MaxValue, float maxHeight = float.MaxValue)
        {
            using (var format = new TextFormat(deviceResources.DirectWriteFactory, fontFamily, fontWeight, fontStyle, fontSize))
            {
                return new TextLayout(deviceResources.DirectWriteFactory, text, format, maxWidth, maxHeight);
            }
        }

        public static Guid ToWICImageFormat(this Direct2DImageFormat format)
        {
            switch (format)
            {
                case Direct2DImageFormat.Bmp:
                    return ContainerFormatGuids.Bmp;
                case Direct2DImageFormat.Ico:
                    return ContainerFormatGuids.Ico;
                case Direct2DImageFormat.Gif:
                    return ContainerFormatGuids.Gif;
                case Direct2DImageFormat.Jpeg:
                    return ContainerFormatGuids.Jpeg;
                case Direct2DImageFormat.Png:
                    return ContainerFormatGuids.Png;
                case Direct2DImageFormat.Tiff:
                    return ContainerFormatGuids.Tiff;
                case Direct2DImageFormat.Wmp:
                    return ContainerFormatGuids.Wmp;
            }
            throw new NotSupportedException();
        }

        public static global::SharpDX.WIC.Bitmap CreateBitmapStream(IDevice2DResources deviceResources, int width, int height, Direct2DImageFormat imageType, Action<RenderTarget> drawingAction)
        {
            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var bitmap = new global::SharpDX.WIC.Bitmap(deviceResources.WICImgFactory, width, height, global::SharpDX.WIC.PixelFormat.Format32bppBGR,
                BitmapCreateCacheOption.CacheOnDemand);
            using (var target = new WicRenderTarget(deviceResources.Factory2D, bitmap,
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
                drawingAction(target);
                target.EndDraw();
            }
            return bitmap;
        }


        public static MemoryStream ToMemoryStream(this global::SharpDX.WIC.Bitmap bitmap,
            IDevice2DResources deviceResources,
            Direct2DImageFormat imageType = Direct2DImageFormat.Bmp)
        {
            if (bitmap == null)
            {
                return null;
            }

            var systemStream = new MemoryStream();

            using (var stream = new WICStream(deviceResources.WICImgFactory, systemStream))
            {
                using (var encoder = new BitmapEncoder(deviceResources.WICImgFactory, imageType.ToWICImageFormat()))
                {
                    encoder.Initialize(stream);
                    using (var frameEncoder = new BitmapFrameEncode(encoder))
                    {
                        frameEncoder.Initialize();
                        frameEncoder.SetSize(bitmap.Size.Width, bitmap.Size.Height);
                        frameEncoder.WriteSource(bitmap);
                        frameEncoder.Commit();
                        encoder.Commit();
                        return systemStream;
                    }
                }
            }
        }

        public static MemoryStream CreateSolidColorBitmapStream(IDevice2DResources deviceResources,
            int width, int height, Direct2DImageFormat imageType, Color4 color)
        {
            using (var bmp = CreateBitmapStream(deviceResources, width, height, imageType, (target) =>
            {
                using (var brush = new SolidColorBrush(target, color, new BrushProperties() { Opacity = color.Alpha }))
                {
                    target.FillRectangle(new RawRectangleF(0, 0, width, height), brush);
                }
            }))
            {
                return bmp.ToMemoryStream(deviceResources, imageType);
            }
        }

        public static MemoryStream CreateLinearGradientBitmapStream(IDevice2DResources deviceResources,
            int width, int height, Direct2DImageFormat imageType, Vector2 startPoint, Vector2 endPoint, GradientStop[] gradients,
            ExtendMode extendMode = ExtendMode.Clamp, Gamma gamma = Gamma.StandardRgb)
        {
            using (var bmp = CreateBitmapStream(deviceResources, width, height, imageType, (target) =>
             {
                 using(var gradientCol = new GradientStopCollection(target, gradients, gamma, extendMode))
                 {
                     using (var brush = new LinearGradientBrush(target, new LinearGradientBrushProperties()
                     {
                         StartPoint = startPoint,
                         EndPoint = endPoint
                     }, gradientCol))
                     {
                         target.FillRectangle(new RawRectangleF(0, 0, width, height), brush);
                     }
                 }
             }))
            {
                return bmp.ToMemoryStream(deviceResources, imageType);
            }
        }

        public static MemoryStream CreateRadiusGradientBitmapStream(IDevice2DResources deviceResources,
            int width, int height, Direct2DImageFormat imageType, Vector2 center, Vector2 gradientOriginOffset,
            float radiusX, float radiusY, GradientStop[] gradients,
            ExtendMode extendMode = ExtendMode.Clamp, Gamma gamma = Gamma.StandardRgb)
        {
            using (var bmp = CreateBitmapStream(deviceResources, width, height, imageType, (target) =>
            {
                using(var gradientCol = new GradientStopCollection(target, gradients, gamma, extendMode))
                {
                    using (var brush = new RadialGradientBrush(target, new RadialGradientBrushProperties()
                    {
                        Center = center,
                        GradientOriginOffset = gradientOriginOffset,
                        RadiusX = radiusX,
                        RadiusY = radiusY,
                    }, gradientCol))
                    {
                        target.FillRectangle(new RawRectangleF(0, 0, width, height), brush);
                    }
                }
            }))
            {
                return bmp.ToMemoryStream(deviceResources, imageType);
            }
        }

        public static MemoryStream CreateViewBoxTexture(IDevice2DResources deviceResources, string front, string back, string left, string right, string top, string down,
            Color4 frontFaceColor, Color4 backFaceColor, Color4 leftFaceColor, Color4 rightFaceColor, Color4 topFaceColor, Color4 bottomFaceColor,
            Color4 frontTextColor, Color4 backTextColor, Color4 leftTextColor, Color4 rightTextColor, Color4 topTextColor, Color4 bottomTextColor,
            string fontFamily = "Arial",
            FontWeight fontWeight = FontWeight.SemiBold, FontStyle fontStyle = FontStyle.Normal, int fontSize = 64, int faceSize = 100)
        {
            using (var bmp = CreateBitmapStream(deviceResources, faceSize * 6, faceSize, Direct2DImageFormat.Bmp, (target) =>
             {
                 target.Clear(Color.Black);
                 RectangleF faceRect = new RectangleF(0, 0, faceSize, faceSize);
                 Color4[] faceColors = new Color4[] { frontFaceColor, backFaceColor, leftFaceColor, rightFaceColor, topFaceColor, bottomFaceColor };
                 Color4[] textColors = new Color4[] { frontTextColor, backTextColor, leftTextColor, rightTextColor, topTextColor, bottomTextColor };
                 string[] texts = new string[] { front, back, right, left, top, down };
                 for (int i = 0; i < 6; ++i)
                 {
                     using (var layout = GetTextLayoutMetrices(texts[i], deviceResources, fontSize, fontFamily, fontWeight, fontStyle, faceSize, faceSize))
                     {
                         var metrices = layout.Metrics;
                         var offset = new Vector2((faceSize - metrices.WidthIncludingTrailingWhitespace) / 2, (faceSize - metrices.Height) / 2);
                         offset.X += faceRect.Left;
                         using (var brush = new SolidColorBrush(target, faceColors[i]))
                         {
                             target.FillRectangle(faceRect, brush);
                         }
                         using (var brush = new SolidColorBrush(target, textColors[i]))
                         {
                             target.DrawTextLayout(offset, layout, brush);
                         }
                     }
                     faceRect.Left += faceSize;
                     faceRect.Width = faceSize;
                 }
             }))
            {
                return bmp.ToMemoryStream(deviceResources, Direct2DImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Create a <see cref="BillboardImage3D"/> from a list of <see cref="TextInfoExt"/>
        /// <para>
        /// This is used to create a batched text billboard with a single merged texture. 
        /// And use <see cref="BillboardImage3D"/> for rendering.
        /// </para>
        /// <para>This is designed to substitute <see cref="BillboardSingleText3D"/> or <see cref="BillboardText3D"/>
        /// when user needs to render many different texts with different text properties (such as font style, font size, etc) and languages
        /// </para>
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="effectsManager">The effects manager.</param>
        /// <param name="maxWidth">The maximum width.</param>
        /// <param name="maxHeight">The maximum height.</param>
        /// <param name="squareImage">if set to <c>true</c> [square image].</param>
        /// <returns></returns>
        public static BillboardImage3D ToBillboardImage3D(this IEnumerable<TextInfoExt> items,
            IEffectsManager effectsManager, int maxWidth = 2048, int maxHeight = 2048, bool squareImage = true)
        {
            using (var imagePacker = new TextInfoExtPacker(effectsManager))
            {
                var code = imagePacker.Pack(items, true, squareImage, maxWidth, maxHeight, 2,
                    out global::SharpDX.WIC.Bitmap bitmap, out int imageWidth, out int imageHeight,
                    out var map);
                if(code == ImagePackReturnCode.Succeed)
                {
                    using (bitmap)
                    {
                        var stream = bitmap.ToMemoryStream(effectsManager, Direct2DImageFormat.Png);
                        var model = new BillboardImage3D(stream);
                        foreach (var imageInfo in items.Select((x, i) =>
                         {
                             var rect = map[i];
                             return new ImageInfo()
                             {
                                 Width = rect.Width,
                                 Height = rect.Height,
                                 Position = x.Origin,
                                 UV_TopLeft = new Vector2(rect.Left / imageWidth, rect.Top / imageHeight),
                                 UV_BottomRight = new Vector2(rect.Right / imageWidth, rect.Bottom / imageHeight)
                             };
                         }))
                        {

                            model.ImageInfos.Add(imageInfo);
                        }
                        return model;
                    }
                }
                else
                {
                    effectsManager.Logger.Log(Logger.LogLevel.Error, $"Failed to pack TextInfoExts, Error Code = {code.ToString()}");
                    return null;
                }
            }
        }
    }
}
