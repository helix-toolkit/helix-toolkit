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
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public class TextInfo
    {
        public string Text { get; set; }
        public Vector3 Origin { get; set; }

        public Color4 Foreground { set; get; } = Color.Black;

        public Color4 Background { set; get; } = Color.Transparent;

        public float ActualWidth { protected set; get; }

        public float AcutalHeight { protected set; get; }

        public float Scale { set; get; } = 1;

        public TextInfo()
        {
        }

        public TextInfo(string text, Vector3 origin)
        {
            Text = text;
            Origin = origin;
        }

        public virtual void UpdateTextInfo(float actualWidth, float actualHeight)
        {
            ActualWidth = actualWidth;
            AcutalHeight = actualHeight;
            BoundSphere = new BoundingSphere(Origin, Math.Max(actualWidth, actualHeight) / 2);
        }

        public BoundingSphere BoundSphere { get; private set; }
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

        private ObservableCollection<TextInfo> textInfo = new ObservableCollection<TextInfo>();
        public ObservableCollection<TextInfo> TextInfo
        {
            set
            {
                var old = textInfo;
                if(Set(ref textInfo, value))
                {
                    old.CollectionChanged -= CollectionChanged;
                    IsInitialized = false;
                    if (value != null)
                    {
                        value.CollectionChanged += CollectionChanged;
                    }
                }
            }
            get { return textInfo; }
        }

        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsInitialized = false;
        }

        protected override void OnDrawTexture(IDeviceResources deviceResources)
        {
            Width = 0;
            Height = 0;
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
                        rect.Width = Math.Max(rect.Width, x * textInfo.Scale);
                        rect.Height = Math.Max(rect.Height, Math.Abs(tempList.Last().OffBR.Y));
                    }
                }
                var halfW = rect.Width / 2;
                var halfH = rect.Height / 2;
                BillboardVertices.Add(new BillboardVertex()
                {
                    Position = textInfo.Origin.ToVector4(),
                    Background = textInfo.Background,
                    TexTL = Vector2.Zero,
                    TexBR = Vector2.Zero,
                    OffTL = new Vector2(-halfW, halfH),
                    OffBR = new Vector2(halfW, -halfH),
                });

                textInfo.UpdateTextInfo(rect.Width, rect.Height);

                foreach(var vert in tempList)
                {
                    var v = vert;
                    v.OffTL += new Vector2(-halfW, halfH);
                    v.OffBR += new Vector2(-halfW, halfH);
                    BillboardVertices.Add(v);
                }
                Width += rect.Width;
                Height += rect.Height;
            }
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
                OffTL = tl * info.Scale,
                OffBR = br * info.Scale
            };
        }

        public override bool HitTest(IRenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, 
            IRenderable originalSource, bool fixedSize)
        {
            var h = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;

            if (context == null || Width == 0 || Height == 0 || (fixedSize && !BoundingSphere.Intersects(ref rayWS)))
            {
                return false;
            }

            var projectionMatrix = context.ProjectionMatrix;
            var viewMatrix = context.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var visualToScreen = context.ScreenViewProjectionMatrix;
            foreach (var info in TextInfo)
            {
                var left = -info.ActualWidth / 2;
                var right = -left;
                var top = -info.AcutalHeight / 2;
                var bottom = -top;
                var b = GetHitTestBound(Vector3.TransformCoordinate(info.Origin, modelMatrix), 
                    left, right, top, bottom, ref projectionMatrix, ref viewMatrix, ref viewMatrixInv, ref visualToScreen,
                    fixedSize, (float)context.ActualWidth, (float)context.ActualHeight);

                if (rayWS.Intersects(ref b))
                {
                    float distance;
                    if (Collision.RayIntersectsBox(ref rayWS, ref b, out distance))
                    {
                        h = true;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.PointHit = rayWS.Position + (rayWS.Direction * distance);
                        result.Distance = distance;
                        result.Tag = info;
                        Debug.WriteLine($"Hit; Text:{info.Text}; HitPoint:{result.PointHit};");
                        break;
                    }
                }
            }
            if (h)
            {
                hits.Add(result);
            }
            return h;
        }
    }
#endif
}
