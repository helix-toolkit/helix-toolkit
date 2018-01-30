/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI.Xaml.Media;
using Windows.UI.Text;
#else
using System.Windows;
using Media = System.Windows.Media;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
using Core;
    using Extensions;
    public class BillboardSingleText3D : BillboardBase
    {
        private volatile bool isInitialized = false;
        private readonly bool predefinedSize = false;
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
#if NETFX_CORE
        private FontStyle mFontStyle = FontStyle.Normal;
#else
        private FontStyle mFontStyle = FontStyles.Normal;
#endif
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
        }
        public BillboardSingleText3D(float width, float height)
        {
            TextInfo = new TextInfo();
            Width = width;
            Height = height;
            predefinedSize = true;
        }
        public override void DrawTexture(IDeviceResources deviceResources)
        {
            if (!isInitialized)
            {
                BillboardVertices.Clear();
                if (!string.IsNullOrEmpty(TextInfo.Text))
                {
#if NETFX_CORE

#else
                    //var bitmap = TextInfo.Text.StringToBitmapSource(FontSize, Media.Colors.White, Media.Colors.Black,
                    //    this.FontFamily, this.FontWeight, this.FontStyle, Padding);
                    //Texture = bitmap.ToMemoryStream();

                    var w = Width;
                    var h = Height;
                    Texture = TextInfo.Text.ToBitmapStream(FontSize, Color.White, Color.Black, "Arial", FontWeight.ToDXFontWeight(), FontStyle.ToDXFontStyle(),
                        new Vector4((float)Padding.Left, (float)Padding.Top, (float)Padding.Right, (float)Padding.Bottom), ref w, ref h, predefinedSize, deviceResources);
#endif
                    if (!predefinedSize)
                    {
                        Width = w;
                        Height = h;
                    }
                    DrawCharacter(TextInfo.Text, TextInfo.Origin, Width, Height, TextInfo);
                }
                else
                {
                    Texture = null;
                    if (!predefinedSize)
                    {
                        Width = 0;
                        Height = 0;
                    }
                }
                isInitialized = true;
                UpdateBounds();
            }
        }

        private void DrawCharacter(string text, Vector3 origin, float w, float h, TextInfo info)
        {
            // CCW from bottom left 
            var tl = new Vector2(-w / 2, h / 2);
            var br = new Vector2(w / 2, -h / 2);

            var uv_tl = new Vector2(0, 0);
            var uv_br = new Vector2(1, 1);

            BillboardVertices.Add(new BillboardVertex()
            {
                Position = info.Origin.ToVector4(),
                Foreground = FontColor,
                Background = BackgroundColor,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = tl,
                OffBR = br
            });
        }
    }
}
