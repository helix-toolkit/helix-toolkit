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
            Positions = new Vector3Collection(6);
            Colors = new Color4Collection(6);
            TextureCoordinates = new Vector2Collection(6);
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

        public BillboardSingleImage3D(BitmapSource bitmapSource, Stream imageStream)
            : this(bitmapSource)
        {
            this.AlphaTexture = imageStream;
            using (Image image = Image.Load(imageStream))
            {
                Width = Math.Max(this.Width, image.Description.Width);
                Height = Math.Max(this.Height, image.Description.Height);
            }
            AlphaTexture.Position = 0;
        }
#endif
        public BillboardSingleImage3D(Stream imageStream)
            : this()
        {
            this.AlphaTexture = imageStream;
            using (Image image = Image.Load(imageStream))
            {
                Width = image.Description.Width;
                Height = image.Description.Height;
            }
            AlphaTexture.Position = 0;
        }

        public BillboardSingleImage3D(Stream imageStream, float width, float height)
            : this()
        {
            this.AlphaTexture = imageStream;
            Width = width;
            Height = height;
            AlphaTexture.Position = 0;
        }

        public override void DrawTexture()
        {
            var w = Width;
            var h = Height;
            // CCW from bottom left 
            var bl = new Vector2(-w / 2, -h / 2);
            var tl = new Vector2(-w / 2, h / 2);
            var br = new Vector2(w / 2, -h / 2);
            var tr = new Vector2(w / 2, h / 2);

            var uv_tl = new Vector2(0, 0);
            var uv_tr = new Vector2(0, 1);
            var uv_bl = new Vector2(1, 0);
            var uv_br = new Vector2(1, 1);

            BillboardVertices.Add(new BillboardVertex()
            {
                Position = Center.ToVector4(),
                Foreground = Color.White,
                Background = Color.White,
                TexTL = uv_tl,
                TexTR = uv_tr,
                TexBL = uv_bl,
                TexBR = uv_br,
                OffP0 = tl,
                OffP1 = bl,
                OffP2 = tr,
                OffP3 = br
            });
            UpdateBounds();
        }
    }
}
