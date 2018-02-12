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
                if(Set(ref mTextInfo, value))
                {
                    IsInitialized = false;
                }
            }
        }

        private Color4 mFontColor = Color.Black;
        public Color4 FontColor
        {
            set
            {
                if(Set(ref mFontColor, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mFontColor; }
        }

        private Color4 mBackgroundColor = Color.Transparent;
        public Color4 BackgroundColor
        {
            set
            {
                if(Set(ref mBackgroundColor, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mBackgroundColor; }
        }

        private int mFontSize = 12;
        public int FontSize
        {
            set
            {
                if(Set(ref mFontSize, value))
                {
                    IsInitialized = false;
                }
            }
            get { return mFontSize; }
        }

        private string mFontFamily = "Arial";
        public string FontFamily
        {
            set
            {
                if(Set(ref mFontFamily, value))
                {
                    IsInitialized = false;
                }
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
                if(Set(ref mFontWeight, value))
                {
                    IsInitialized = false;
                }
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
                if(Set(ref mFontStyle, value))
                {
                    IsInitialized = false;
                }
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
                if(Set(ref mPadding, value))
                {
                    IsInitialized = false;
                }
            }
            get
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

        protected override void OnDrawTexture(IDeviceResources deviceResources)
        {
            if (!string.IsNullOrEmpty(TextInfo.Text))
            {
                var w = Width;
                var h = Height;
                Texture = TextInfo.Text.ToBitmapStream(FontSize, Color.White, Color.Black, FontFamily, FontWeight.ToDXFontWeight(), FontStyle.ToDXFontStyle(),
                    new Vector4((float)Padding.Left, (float)Padding.Top, (float)Padding.Right, (float)Padding.Bottom), ref w, ref h, predefinedSize, deviceResources);
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
            TextInfo.UpdateTextInfo(Width, Height);
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
