using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System.Windows;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Extensions;
using Media = System.Windows.Media;
using System;

namespace HelixToolkit.Wpf.SharpDX
{
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

        private IList<Vector2> mTextInfoOffsets = null;
        public override IList<Vector2> TextureOffsets
        {
            get { return mTextInfoOffsets; }
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

        public BillboardSingleImage3D(Media.Imaging.BitmapSource bitmapSource)
        {
            this.Texture = bitmapSource;
            Positions = new Vector3Collection(6);
            Colors = new Color4Collection(6);
            TextureCoordinates = new Vector2Collection(6);
            MaskColor = Color.Transparent;
        }

        public override void DrawTexture()
        {
            Positions.Clear();
            Colors.Clear();
            TextureCoordinates.Clear();
            mTextInfoOffsets = new List<Vector2>(6);
            var w = Texture.PixelWidth;
            var h = Texture.PixelHeight;
            // CCW from top left 
            var a = new Vector2(-w / 2, -h / 2);
            var b = new Vector2(-w / 2, h / 2);
            var c = new Vector2(w / 2, -h / 2);
            var d = new Vector2(w / 2, h / 2);

            var uv_a = new Vector2(0, 0);
            var uv_b = new Vector2(0, 1);
            var uv_c = new Vector2(1, 0);
            var uv_d = new Vector2(1, 1);

            ///Create foreground data
            Positions.Add(Center);
            Positions.Add(Center);
            Positions.Add(Center);
            Positions.Add(Center);
            Positions.Add(Center);
            Positions.Add(Center);

            Colors.Add(MaskColor);
            Colors.Add(MaskColor);
            Colors.Add(MaskColor);
            Colors.Add(MaskColor);
            Colors.Add(MaskColor);
            Colors.Add(MaskColor);

            TextureCoordinates.Add(uv_b);
            TextureCoordinates.Add(uv_d);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_d);
            TextureCoordinates.Add(uv_c);

            mTextInfoOffsets.Add(a);
            mTextInfoOffsets.Add(c);
            mTextInfoOffsets.Add(b);
            mTextInfoOffsets.Add(b);
            mTextInfoOffsets.Add(c);
            mTextInfoOffsets.Add(d);
        }
    }
}
