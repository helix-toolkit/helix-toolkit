/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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

        public virtual BitmapFont BitmapFont
        {
            get
            {
                return bmpFont;
            }
        }

        public List<TextInfo> TextInfo { get; } = new List<TextInfo>();

        //public override IList<Vector2> TextureOffsets { get { return TextInfo.SelectMany(x => x.Offsets).ToArray(); } }

        public override void DrawTexture(IDeviceResources deviceResources)
        {
            BillboardVertices.Clear();
            // http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp

            var tempList = new List<BillboardVertex>(100);
            foreach (var textInfo in TextInfo)
            {
                tempList.Clear();
                int x = 0;
                int y = 0;
                var w = BitmapFont.TextureSize.Width;
                var h = BitmapFont.TextureSize.Height;

                char previousCharacter;

                previousCharacter = ' ';
                var normalizedText = textInfo.Text;
                var rect = new RectangleF(textInfo.Origin.X, textInfo.Origin.Y, 0, 0);
                foreach (char character in normalizedText)
                {
                    switch (character)
                    {
                        case '\n':
                            x = 0;
                            y -= BitmapFont.LineHeight;
                            break;
                        default:
                            Character data = BitmapFont[character];
                            int kerning = BitmapFont.GetKerning(previousCharacter, character);
                            tempList.Add(DrawCharacter(data, new Vector3(x + data.Offset.X, y - data.Offset.Y, 0), w, h, kerning, textInfo));

                            x += data.XAdvance + kerning;
                            break;
                    }
                    previousCharacter = character;
                    if (tempList.Count > 0)
                    {
                        rect.Width = Math.Max(rect.Width, x);
                        rect.Height = Math.Max(rect.Height, Math.Abs(tempList.Last().OffBR.Y));
                    }
                }
                BillboardVertices.Add(new BillboardVertex()
                {
                    Position = textInfo.Origin.ToVector4(),
                    Background = textInfo.Background,
                    TexTL = Vector2.Zero,
                    TexBR = Vector2.Zero,
                    OffTL = Vector2.Zero,
                    OffBR = new Vector2(rect.Width, -rect.Height),
                });
                tempList.ForEach(BillboardVertices.Add);
            }
            UpdateBounds();
        }

        private BillboardVertex DrawCharacter(Character character, Vector3 origin, float w, float h, float kerning, TextInfo info)
        {
            var cw = character.Bounds.Width;
            var ch = character.Bounds.Height;
            var cu = character.Bounds.Left;
            var cv = character.Bounds.Top;
            var tl = new Vector2(origin.X + kerning, origin.Y );
            var br = new Vector2(origin.X + cw + kerning, origin.Y - ch);

            var uv_tl = new Vector2(cu / w, cv / h);
            var uv_br = new Vector2((cu + cw) / w, (cv + ch) / h);

            return new BillboardVertex()
            {
                Position = info.Origin.ToVector4(),
                Foreground = info.Foreground,
                Background = Color.Transparent,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = tl,
                OffBR = br
            };
        }
    }
#endif
}
