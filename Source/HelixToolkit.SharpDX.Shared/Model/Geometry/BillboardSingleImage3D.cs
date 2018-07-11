/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using System.Numerics;
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
    /// <summary>
    /// 
    /// </summary>
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
        } = Color.Transparent;

#if !NETFX_CORE        
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="bitmapSource">The bitmap source.</param>
        public BillboardSingleImage3D(BitmapSource bitmapSource)
        {
            this.Texture = bitmapSource.ToMemoryStream();
            Width = bitmapSource.PixelWidth;
            Height = bitmapSource.PixelHeight;
        }
#endif        
        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        public BillboardSingleImage3D(Stream imageStream)
        {
            this.Texture = imageStream;
            using (Image image = Image.Load(imageStream))
            {
                Width = image.Description.Width;
                Height = image.Description.Height;
            }
            Texture.Position = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardSingleImage3D"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BillboardSingleImage3D(Stream imageStream, float width, float height)
        {
            this.Texture = imageStream;
            Width = width;
            Height = height;
            Texture.Position = 0;
        }

        /// <summary>
        /// Updates the bounds.
        /// </summary>
        public override void UpdateBounds()
        {
            BoundingSphere = new BoundingSphere(Center, (float)Math.Sqrt(Width * Width + Height * Height) / 2);
            Bound = BoundingBox.FromSphere(BoundingSphere);
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardSingleImage3D billboard)
            {
                billboard.Center = Center;
                billboard.MaskColor = MaskColor;
            }
        }
        /// <summary>
        /// Called when [draw texture].
        /// </summary>
        /// <param name="deviceResources">The device resources.</param>
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
                Background = MaskColor,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = tl,
                OffBR = br
            });
        }
    }
}
