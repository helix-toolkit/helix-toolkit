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
            using (var layout = GetTextLayoutMetrices(text, fontSize, fontFamily, fontWeight, fontStyle))
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

                return CreateBitmapStream((int)width, (int)height, deviceResources.DeviceContext2D, Direct2DImageFormat.Bmp, (target) =>
                {
                    target.Clear(background);
                    using (var brush = new SolidColorBrush(target, foreground))
                    {
                        target.DrawTextLayout(new Vector2(padding.X, padding.Y), layout, brush);
                    }
                });
            }
        }

        public static TextLayout GetTextLayoutMetrices(this string text, int fontSize, string fontFamily, FontWeight fontWeight, FontStyle fontStyle, 
            float maxWidth = float.MaxValue, float maxHeight = float.MaxValue)
        {
            using (var factory = new global::SharpDX.DirectWrite.Factory(global::SharpDX.DirectWrite.FactoryType.Isolated))
            {
                using (var format = new TextFormat(factory, fontFamily, fontWeight, fontStyle, fontSize))
                {
                    return new TextLayout(factory, text, format, maxWidth, maxHeight);
                }
            }
        }


        public enum Direct2DImageFormat
        {
            Png, Gif, Ico, Jpeg, Wmp, Tiff, Bmp
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

        public static MemoryStream CreateBitmapStream(int width, int height, DeviceContext context, Direct2DImageFormat imageType, Action<RenderTarget> drawingAction)
        {
            using (var imgFactory = new ImagingFactory())
            {
                using (var bitmap = new global::SharpDX.WIC.Bitmap(imgFactory, (int)width, (int)height, global::SharpDX.WIC.PixelFormat.Format32bppBGR,
                    BitmapCreateCacheOption.CacheOnLoad))
                {

                    using (var target = new WicRenderTarget(context.Factory, bitmap,
                        new RenderTargetProperties()
                        {
                            DpiX = context.DotsPerInch.Width,
                            DpiY = context.DotsPerInch.Height,
                            MinLevel = FeatureLevel.Level_DEFAULT,
                            PixelFormat = new global::SharpDX.Direct2D1.PixelFormat(global::SharpDX.DXGI.Format.Unknown, AlphaMode.Unknown)
                        }))
                    {
                        target.Transform = Matrix3x2.Identity;
                        target.BeginDraw();
                        drawingAction(target);
                        target.EndDraw();
                    }

                    var systemStream = new MemoryStream();

                    using (var stream = new WICStream(imgFactory, systemStream))
                    {
                        using (var encoder = new BitmapEncoder(imgFactory, imageType.ToWICImageFormat()))
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

        public static MemoryStream CreateViewBoxTexture(DeviceContext context, string front, string back, string left, string right, string top, string down, 
            Color4 frontFaceColor, Color4 backFaceColor, Color4 leftFaceColor, Color4 rightFaceColor, Color4 topFaceColor, Color4 bottomFaceColor,
            Color4 frontTextColor, Color4 backTextColor, Color4 leftTextColor, Color4 rightTextColor, Color4 topTextColor, Color4 bottomTextColor,
            string fontFamily = "Arial",
            FontWeight fontWeight = FontWeight.SemiBold, FontStyle fontStyle = FontStyle.Normal, int fontSize = 64, int faceSize = 100)
        {
            return CreateBitmapStream(faceSize * 6, faceSize, context, Direct2DImageFormat.Bmp, (target) =>
            {
                target.Clear(Color.Black);
                RectangleF faceRect = new RectangleF(0, 0, faceSize, faceSize);
                Color4[] faceColors = new Color4[] { frontFaceColor, backFaceColor, leftFaceColor, rightFaceColor, topFaceColor, bottomFaceColor };
                Color4[] textColors = new Color4[] { frontTextColor, backTextColor, leftTextColor, rightTextColor, topTextColor, bottomTextColor };
                string[] texts = new string[] { front, back, left, right, top, down };
                for(int i=0; i<6; ++i)
                {
                    using (var layout = GetTextLayoutMetrices(texts[i], fontSize, fontFamily, fontWeight, fontStyle, faceSize, faceSize))
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
            });
        }
    }
}
