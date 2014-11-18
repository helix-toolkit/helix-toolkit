using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Cyotek.Drawing.BitmapFont;
using System;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

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
    public class BillboardText3D : MeshGeometry3D
    {
        private BitmapFont bmpFont;

        public List<TextInfo> TextInfo { get; set; }

        public BitmapSource Texture
        {
            get { return null; }
        }

        public BillboardText3D()
        {
            Positions = new Vector3Collection();
            Indices = new IntCollection();
            Colors = new Color4Collection();
            this.TextInfo = new List<TextInfo>();

            var assembly = Assembly.GetExecutingAssembly();
            var path = Path.Combine(Path.GetDirectoryName(assembly.Location), "Textures", "arial.fnt");
            bmpFont = BitmapFontLoader.LoadFontFromFile(path);
        }

        internal void DrawText(BitmapFont bmpFont, TextInfo info)
        {
            // http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp

            int x = 0;
            int y = 0;
            var w = bmpFont.TextureSize.Width;
            var h = bmpFont.TextureSize.Height;

            char previousCharacter;

            previousCharacter = ' ';
            var normalizedText = bmpFont.NormalizeLineBreaks(info.Text);

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
                        DrawCharacter(data, new Vector3(x,y,0), w, h, kerning, info);

                        x += data.XAdvance + kerning;
                        break;
                }

                previousCharacter = character;
            }
        }

        internal void DrawCharacter(Character character, Vector3 origin, float w, float h, float kerning, TextInfo info)
        {
            var cw = character.Bounds.Width;
            var ch = character.Bounds.Height;
            
            // CCW from top left 
            var a = new Vector2(origin.X + kerning, origin.Y);
            var b = new Vector2(origin.X + kerning, origin.Y + ch);
            var c = new Vector2(origin.X + cw + kerning, origin.Y);
            var d = new Vector2(origin.X + cw + kerning, origin.Y + ch);

            var uv_a = new Vector2(origin.X/w, origin.Y/h);
            var uv_b = new Vector2(origin.X/w, (origin.Y + ch)/h);
            var uv_c = new Vector2((origin.X + cw)/w, origin.Y/h);
            var uv_d = new Vector2((origin.X + cw)/w, (origin.Y + ch)/h);

            var n = new Vector3(0, 0, 1);

            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);
            Positions.Add(info.Origin);

            Colors.Add(new Color4(0,0,0,1));
            Colors.Add(new Color4(0, 0, 0, 1));
            Colors.Add(new Color4(0, 0, 0, 1));
            Colors.Add(new Color4(0, 0, 0, 1));
            Colors.Add(new Color4(0, 0, 0, 1));
            Colors.Add(new Color4(0, 0, 0, 1));

            Normals.Add(new Vector3(0, 0, 1));
            Normals.Add(new Vector3(0, 0, 1));
            Normals.Add(new Vector3(0, 0, 1));
            Normals.Add(new Vector3(0, 0, 1));
            Normals.Add(new Vector3(0, 0, 1));
            Normals.Add(new Vector3(0, 0, 1));

            TextureCoordinates.Add(uv_a);
            TextureCoordinates.Add(uv_c);
            TextureCoordinates.Add(uv_b);
            TextureCoordinates.Add(uv_b);
            TextureCoordinates.Add(uv_c);
            TextureCoordinates.Add(uv_d);

            info.Offsets.Add(a);
            info.Offsets.Add(c);
            info.Offsets.Add(b);
            info.Offsets.Add(b);
            info.Offsets.Add(c);
            info.Offsets.Add(d);
        }
    }
}
