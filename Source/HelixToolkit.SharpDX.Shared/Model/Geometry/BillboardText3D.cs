using System.Collections.Generic;
using SharpDX;

#if NETFX_CORE

#else
using System.IO;
using System.Reflection;
using System;
using Media = System.Windows.Media;
using Cyotek.Drawing.BitmapFont;
using System.Linq;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;

    public class TextInfo
    {
        public string Text { get; set; }
        public Vector3 Origin { get; set; }

        public Color4 Foreground { set; get; } = Color.Black;

        public Color4 Background { set; get; } = Color.Transparent;

        public TextInfo()
        {
        }

        public TextInfo(string text, Vector3 origin)
        {
            Text = text;
            Origin = origin;
        }
    }
#if !NETFX_CORE
    [Serializable]

    public class BillboardText3D : BillboardBase
    {
        private readonly static BitmapFont bmpFont;

        public static Stream TextureStatic { get; private set; }

        static BillboardText3D()
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.fnt");

            bmpFont = new BitmapFont();
            bmpFont.Load(texDescriptionStream);// BitmapFontLoader.LoadFontFromFile(texDescriptionFilePath);
            texDescriptionStream.Dispose();
            //Read the texture          
            var texImageStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.png");
            TextureStatic = MemoryStream.Synchronized(texImageStream);
        }

        public override BillboardType Type
        {
            get
            {
                return BillboardType.MultipleText;
            }
        }

        public override Stream Texture
        {
            get
            {
                return TextureStatic;            
            }
        }

        public List<TextInfo> TextInfo { get; } = new List<TextInfo>();

        //public override IList<Vector2> TextureOffsets { get { return TextInfo.SelectMany(x => x.Offsets).ToArray(); } }

        public override void DrawTexture()
        {
            BillboardVertices.Clear();
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
            UpdateBounds();
        }

        private void DrawCharacter(Character character, Vector3 origin, float w, float h, float kerning, TextInfo info)
        {
            var cw = character.Bounds.Width;
            var ch = character.Bounds.Height;
            var cu = character.Bounds.Left;
            var cv = character.Bounds.Top;


            var bl = new Vector2(origin.X + kerning, origin.Y);
            var tl = new Vector2(origin.X + kerning, origin.Y + ch);
            var br = new Vector2(origin.X + cw + kerning, origin.Y);
            var tr = new Vector2(origin.X + cw + kerning, origin.Y + ch);

            var uv_tl = new Vector2(cu / w, cv / h);
            var uv_tr = new Vector2(cu / w, (cv + ch) / h);
            var uv_bl = new Vector2((cu + cw) / w, cv / h);
            var uv_br = new Vector2((cu + cw) / w, (cv + ch) / h);

            BillboardVertices.Add(new BillboardVertex()
            {
                Position = info.Origin.ToVector4(),
                Foreground = info.Foreground,
                Background = info.Background,
                TexTL = uv_tl,
                TexTR = uv_tr,
                TexBL = uv_bl,
                TexBR = uv_br,
                OffP0 = tl,
                OffP1 = bl,
                OffP2 = tr,
                OffP3 = br
            });
        }
    }
#endif
}
