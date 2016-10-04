using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.Wpf.SharpDX
{
    public class BillboardSingleText3D: MeshGeometry3D, IBillboardText
    {
        private volatile bool isInitialized = false;

        public BitmapSource Texture { get; private set; }

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

        public IList<Vector2> TextInfoOffsets { get { return TextInfo.Offsets; } }

        public float Width { private set; get; }

        public float Height { private set; get; }

        private Color4 mFontColor = Color.Black;
        public Color4 FontColor
        {
            set { mFontColor = value; }
            get { return mFontColor; }
        }

        private int mFontSize = 12;
        public int FontSize
        {
            set { mFontSize = value; }
            get { return mFontSize; }
        }

        public BillboardSingleText3D()
        {
            Positions = new Vector3Collection();
            Colors = new Color4Collection();
            TextureCoordinates = new Vector2Collection();
            TextInfo = new TextInfo();
        }

        public void DrawText()
        {
            if (!isInitialized)
            {
                if (!string.IsNullOrEmpty(TextInfo.Text))
                {
                    Texture = TextInfo.Text.StringToBitmapSource(FontSize, System.Windows.Media.Colors.White, System.Windows.Media.Colors.Black);
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
        }
    }
}
