using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System.Windows;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Extensions;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardSingleText3D : BillboardBase
    {
        private volatile bool isInitialized = false;

        /// <summary>
        /// Billboard type, <see cref="BillboardType"/>
        /// </summary>
        public override BillboardType Type
        {
            get
            {
                return BillboardType.SingleText;
            }
        }

        private TextInfo mTextInfo = new TextInfo("", new Vector3());
        public TextInfo TextInfo
        {
            get { return mTextInfo; }
            set
            {
                mTextInfo = value;
                isInitialized = false;
            }
        }

        public override IList<Vector2> TextureOffsets { get { return TextInfo.Offsets; } }

        private Color4 mFontColor = Color.Black;
        public Color4 FontColor
        {
            set { mFontColor = value; }
            get { return mFontColor; }
        }

        private Color4 mBackgroundColor = Color.Transparent;
        public Color4 BackgroundColor
        {
            set { mBackgroundColor = value; }
            get { return mBackgroundColor; }
        }

        private int mFontSize = 12;
        public int FontSize
        {
            set { mFontSize = value; }
            get { return mFontSize; }
        }

        private Media.FontFamily mFontFamily = new Media.FontFamily("Arial");
        public Media.FontFamily FontFamily
        {
            set
            {
                mFontFamily = value;
            }
            get
            {
                return mFontFamily;
            }
        }

        private FontWeight mFontWeight = FontWeights.Normal;
        public FontWeight FontWeight
        {
            set
            {
                mFontWeight = value;
            }
            get
            {
                return mFontWeight;
            }
        }

        private FontStyle mFontStyle = FontStyles.Normal;
        public FontStyle FontStyle
        {
            set
            {
                mFontStyle = value;
            }
            get
            {
                return mFontStyle;
            }
        }

        private Thickness mPadding = new Thickness(0);
        public Thickness Padding
        {
            set
            {
                mPadding = value;
            }get
            {
                return mPadding;
            }
        }

        public BillboardSingleText3D()
        {
            Positions = new Vector3Collection(12);
            Colors = new Color4Collection(12);
            TextureCoordinates = new Vector2Collection(12);
            TextInfo = new TextInfo();
        }

        public override void DrawTexture()
        {
            if (!isInitialized)
            {
                if (!string.IsNullOrEmpty(TextInfo.Text))
                {
                    Texture = TextInfo.Text.StringToBitmapSource(FontSize, Media.Colors.White, Media.Colors.Black, 
                        this.FontFamily, this.FontWeight, this.FontStyle, Padding);
                    Texture.Freeze();
                    Width = (float)Texture.Width;
                    Height = (float)Texture.Height;
                    DrawCharacter(TextInfo.Text, TextInfo.Origin, (float)Texture.Width, (float)Texture.Height, TextInfo);
                }
                else
                {
                    Texture = null;
                    Width = 0; 
                    Height = 0;
                    Positions.Clear();
                    Colors.Clear();
                    TextureCoordinates.Clear();
                    TextInfo.Offsets.Clear();
                }
                isInitialized = true;
            }
        }

        private void DrawCharacter(string text, Vector3 origin, float w, float h, TextInfo info)
        {
            Positions.Clear();
            Colors.Clear();
            TextureCoordinates.Clear();
            info.Offsets.Clear();
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
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);

            Colors.Add(FontColor);
            Colors.Add(FontColor);
            Colors.Add(FontColor);
            Colors.Add(FontColor);
            Colors.Add(FontColor);
            Colors.Add(FontColor);

            TextureCoordinates.Add(uv_b);
            TextureCoordinates.Add(uv_d);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_d);
            TextureCoordinates.Add(uv_c);

            info.Offsets.Add(a);
            info.Offsets.Add(c);
            info.Offsets.Add(b);
            info.Offsets.Add(b);
            info.Offsets.Add(c);
            info.Offsets.Add(d);

            ///Create background data
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);

            Colors.Add(BackgroundColor);
            Colors.Add(BackgroundColor);
            Colors.Add(BackgroundColor);
            Colors.Add(BackgroundColor);
            Colors.Add(BackgroundColor);
            Colors.Add(BackgroundColor);

            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_a);

            info.Offsets.Add(a);
            info.Offsets.Add(c);
            info.Offsets.Add(b);
            info.Offsets.Add(b);
            info.Offsets.Add(c);
            info.Offsets.Add(d);
        }
    }
}
