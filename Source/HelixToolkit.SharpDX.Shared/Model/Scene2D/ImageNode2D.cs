/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct2D1;
using global::SharpDX.WIC;
using SharpDX;
using System;
using System.IO;
using Bitmap = SharpDX.Direct2D1.Bitmap;

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
    namespace Model.Scene2D
    {
        using Core2D;

        public class ImageNode2D : SceneNode2D
        {
            private Stream imageStream;

            public Stream ImageStream
            {
                set
                {
                    if (SetAffectsMeasure(ref imageStream, value))
                    {
                        bitmapChanged = true;
                    }
                }
                get
                {
                    return imageStream;
                }
            }

            public float Opacity
            {
                set
                {
                    (RenderCore as ImageRenderCore2D).Opacity = value;
                }
                get
                {
                    return (RenderCore as ImageRenderCore2D).Opacity;
                }
            }

            protected bool bitmapChanged { private set; get; } = true;

            protected override RenderCore2D CreateRenderCore()
            {
                return new ImageRenderCore2D();
            }

            protected override bool OnAttach(IRenderHost host)
            {
                if (base.OnAttach(host))
                {
                    bitmapChanged = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void LoadBitmap(RenderContext2D context, Stream stream)
            {
                (RenderCore as ImageRenderCore2D).Bitmap = stream == null ? null : OnLoadImage(context, stream);
            }

            protected virtual Bitmap OnLoadImage(RenderContext2D context, Stream stream)
            {
                stream.Position = 0;
                using (var decoder = new BitmapDecoder(context.DeviceResources.WICImgFactory, stream, DecodeOptions.CacheOnLoad))
                {
                    using (var frame = decoder.GetFrame(0))
                    {
                        using (var converter = new FormatConverter(context.DeviceResources.WICImgFactory))
                        {
                            converter.Initialize(frame, global::SharpDX.WIC.PixelFormat.Format32bppPBGRA);
                            return Bitmap1.FromWicBitmap(context.DeviceContext, converter);
                        }
                    }
                }
            }

            public override void Update(RenderContext2D context)
            {
                base.Update(context);
                if (bitmapChanged)
                {
                    LoadBitmap(context, ImageStream);
                    bitmapChanged = false;
                }
            }

            protected override Size2F MeasureOverride(Size2F availableSize)
            {
                if (ImageStream != null)
                {
                    var imageSize = (RenderCore as ImageRenderCore2D).ImageSize;
                    if (Width == 0 && Height == 0)
                    {
                        return new Size2F(Math.Min(availableSize.Width, imageSize.Width), Math.Min(availableSize.Height, imageSize.Height));
                    }
                    else if (imageSize.Width == 0 || imageSize.Height == 0)
                    {
                        return availableSize;
                    }
                    else
                    {
                        float aspectRatio = imageSize.Width / imageSize.Height;
                        if (Width == 0)
                        {
                            var height = Math.Min(availableSize.Height, Height);
                            return new Size2F(height / aspectRatio, height);
                        }
                        else
                        {
                            var width = Math.Min(availableSize.Width, Width);
                            return new Size2F(width, width * aspectRatio);
                        }
                    }
                }
                return new Size2F(Math.Max(0, Width), Math.Max(0, Height));
            }

            protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
            {
                hitResult = null;
                if (LayoutBoundWithTransform.Contains(mousePoint))
                {
                    hitResult = new HitTest2DResult(this);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

}