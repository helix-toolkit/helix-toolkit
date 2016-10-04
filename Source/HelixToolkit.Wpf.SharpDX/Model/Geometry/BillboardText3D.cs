using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Cyotek.Drawing.BitmapFont;
using System;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System.Linq;
namespace HelixToolkit.Wpf.SharpDX
{
    public class TextInfo
    {
        public List<Vector2> Offsets { get; set; }
        public string Text { get; set; }
        public Vector3 Origin { get; set; }

        public TextInfo()
        {
            Offsets = new List<Vector2>();
        }

        public TextInfo(string text, Vector3 origin)
        {
            Offsets = new List<Vector2>();
            Text = text;
            Origin = origin;
        }
    }

    [Serializable]
    public class BillboardText3D : MeshGeometry3D, IBillboardText
    {
        private static bool isInitialized = false;

        private static BitmapFont bmpFont;

        public static BitmapSource TextureStatic { get; private set; }

        public BitmapSource Texture { get { return TextureStatic; } }

        public List<TextInfo> TextInfo { get; private set; }

        public IList<Vector2> TextInfoOffsets { get { return TextInfo.SelectMany(x => x.Offsets).ToArray(); } }

        private System.Windows.Media.Color mFontColor = System.Windows.Media.Colors.Black;
        public System.Windows.Media.Color FontColor
        {
            set { mFontColor = value; }
            get { return mFontColor; }
        }

        public float Width
        {
            get { return 0; }
        }

        public float Height
        {
            get { return 0; }
        }

        public BillboardText3D()
        {
            Positions = new Vector3Collection();
            Colors = new Color4Collection();
            TextureCoordinates = new Vector2Collection();
            TextInfo = new List<TextInfo>();

            Initialize();
        }

        private static void Initialize()
        {
            if (isInitialized)
                return;

            var assembly = Assembly.GetExecutingAssembly();

            var texDescriptionFilePath = Path.GetTempFileName();
            var texImageFilePath = Path.GetTempFileName();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.fnt");
            using (var fileStream = File.Create(texDescriptionFilePath))
            {
                texDescriptionStream.CopyTo(fileStream);
            }

            bmpFont = BitmapFontLoader.LoadFontFromFile(texDescriptionFilePath);

            //Read the texture          
            var texImageStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.png");
            using (var fileStream = File.Create(texImageFilePath))
            {
                texImageStream.CopyTo(fileStream);
            }

            TextureStatic = new BitmapImage(new Uri(texImageFilePath));
            TextureStatic.Freeze();
            //Cleanup the temp files
            if (File.Exists(texDescriptionFilePath))
            {
                File.Delete(texDescriptionFilePath);
            }

            isInitialized = true;
        }

        public void DrawText()
        {
            //Positions.Clear();
            //Colors.Clear();
            //TextureCoordinates.Clear();

            // http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
            foreach (var textInfo in TextInfo)
            {
                int x = 0;
                int y = 0;
                var w = bmpFont.TextureSize.Width;
                var h = bmpFont.TextureSize.Height;

                char previousCharacter;

                previousCharacter = ' ';
                var normalizedText = textInfo.Text;

                foreach (char character in normalizedText)
                {
                    switch (character)
                    {
                        case '\n':
                            x = 0;
                            y -= bmpFont.LineHeight;
                            break;
                        default:
                            Character data = bmpFont[character];
                            int kerning = bmpFont.GetKerning(previousCharacter, character);

                            //DrawCharacter(data, x + data.Offset.X + kerning, y + data.Offset.Y, builder);
                            DrawCharacter(data, new Vector3(x, y, 0), w, h, kerning, textInfo);

                            x += data.XAdvance + kerning;
                            break;
                    }

                    previousCharacter = character;
                }
            }
        }

        private void DrawCharacter(Character character, Vector3 origin, float w, float h, float kerning, TextInfo info)
        {
            var cw = character.Bounds.Width;
            var ch = character.Bounds.Height;
            var cu = character.Bounds.Left;
            var cv = character.Bounds.Top;

            // CCW from top left 
            var a = new Vector2(origin.X + kerning, origin.Y);
            var b = new Vector2(origin.X + kerning, origin.Y + ch);
            var c = new Vector2(origin.X + cw + kerning, origin.Y);
            var d = new Vector2(origin.X + cw + kerning, origin.Y + ch);

            var uv_a = new Vector2(cu / w, cv / h);
            var uv_b = new Vector2(cu / w, (cv + ch) / h);
            var uv_c = new Vector2((cu + cw) / w, cv / h);
            var uv_d = new Vector2((cu + cw) / w, (cv + ch) / h);

            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);

            var color = FontColor.ToColor4();
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);
            Colors.Add(color);

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
