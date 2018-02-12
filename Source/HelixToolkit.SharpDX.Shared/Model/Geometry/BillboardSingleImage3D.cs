/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System;
using System.IO;
using SharpDX.Toolkit.Graphics;
#if NETFX_CORE

#else
using System.Windows.Media.Imaging;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;

    public class BillboardSingleImage3D : BillboardBase
    {
        /// <summary>
        /// Billboard type, <see cref="BillboardType"/>
        /// </summary>
        public override BillboardType Type
        {
            get
            {
                return BillboardType.SingleImage;
            }
        }

        /// <summary>
        /// Billboard center location
        /// </summary>
        public Vector3 Center
        {
            set;
            get;
        }

        /// <summary>
        /// If color in image is equal to the mask color, the color will set to transparent in image.
        /// Default color is Transparent, which did not mask any color.
        /// </summary>
        public Color4 MaskColor
        {
            set;get;
        }

        protected BillboardSingleImage3D()
        {
            MaskColor = Color.Transparent;
        }

#if !NETFX_CORE
        public BillboardSingleImage3D(BitmapSource bitmapSource)
            : this()
        {
            this.Texture = bitmapSource.ToMemoryStream();
            Width = bitmapSource.PixelWidth;
            Height = bitmapSource.PixelHeight;
        }
#endif
        public BillboardSingleImage3D(Stream imageStream)
            : this()
        {
            this.Texture = imageStream;
            using (Image image = Image.Load(imageStream))
            {
                Width = image.Description.Width;
                Height = image.Description.Height;
            }
            Texture.Position = 0;
        }

        public BillboardSingleImage3D(Stream imageStream, float width, float height)
            : this()
        {
            this.Texture = imageStream;
            Width = width;
            Height = height;
            Texture.Position = 0;
        }

        protected override void OnDrawTexture(IDeviceResources deviceResources)
        {
            var w = Width;
            var h = Height;
            // CCW from bottom left 
            var tl = new Vector2(-w / 2, h / 2);
            var br = new Vector2(w / 2, -h / 2);

            var uv_tl = new Vector2(0, 0);
            var uv_br = new Vector2(1, 1);

            BillboardVertices.Add(new BillboardVertex()
            {
                Position = Center.ToVector4(),
                Foreground = Color.White,
                Background = Color.White,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = tl,
                OffBR = br
            });
        }
    }
}
