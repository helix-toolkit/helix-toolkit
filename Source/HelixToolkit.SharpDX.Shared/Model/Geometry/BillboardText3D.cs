/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using SharpDX;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Cyotek.Drawing.BitmapFont;
#if CORE

#else
#if NETFX_CORE
using Media = Windows.UI.Xaml.Media;
#else
using Media = System.Windows.Media;
#endif
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
#endif
    public class BillboardText3D : BillboardBase
    {
        private readonly static BitmapFont bmpFont;

        public static Stream TextureStatic { get; private set; }

        static BillboardText3D()
        {
#if CORE
            var assembly = typeof(BillboardText3D).GetTypeInfo().Assembly;
            Stream fontInfo = assembly.GetManifestResourceStream(@"HelixToolkit.SharpDX.Core.Resources.arial.fnt");
            bmpFont = new BitmapFont();
            bmpFont.Load(fontInfo);
            Stream font = assembly.GetManifestResourceStream(@"HelixToolkit.SharpDX.Core.Resources.arial.png");
            TextureStatic = font;
#else
#if !NETFX_CORE
            var assembly = Assembly.GetExecutingAssembly();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.fnt");

            bmpFont = new BitmapFont();
            bmpFont.Load(texDescriptionStream);// BitmapFontLoader.LoadFontFromFile(texDescriptionFilePath);
            texDescriptionStream.Dispose();
            //Read the texture          
            var texImageStream = assembly.GetManifestResourceStream("HelixToolkit.Wpf.SharpDX.Textures.arial.png");
            TextureStatic = MemoryStream.Synchronized(texImageStream);
#else
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.UWP");
            var sampleFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\Resources\arial.fnt");
            bmpFont = new BitmapFont();
            var fileStream = new MemoryStream(sampleFile);
            bmpFont.Load(fileStream);

            var texFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\Resources\arial.png");
            TextureStatic = new MemoryStream(texFile);         
#endif
#endif
        }

        public override BillboardType Type
        {
            get
            {
                return BillboardType.MultipleText;
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
            Texture = TextureStatic;
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

        public override void UpdateBounds()
        {
            if (TextInfo.Count == 0)
            {
                Bound = new BoundingBox();
                BoundingSphere = new BoundingSphere();
            }
            else
            {
                var sphere = TextInfo[0].BoundSphere;
                var bound = BoundingBox.FromSphere(sphere);
                foreach(var info in TextInfo)
                {
                    sphere = BoundingSphere.Merge(sphere, info.BoundSphere);
                    bound = BoundingBox.Merge(bound, BoundingBox.FromSphere(info.BoundSphere));
                }
                BoundingSphere = sphere;
                Bound = bound;
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
            object originalSource, bool fixedSize)
        {
            var h = false;
            var result = new BillboardHitResult();
            result.Distance = double.MaxValue;

            if (context == null || Width == 0 || Height == 0)
            {
                return false;
            }
            var scale = modelMatrix.ScaleVector;
            var projectionMatrix = context.ProjectionMatrix;
            var viewMatrix = context.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var visualToScreen = context.ScreenViewProjectionMatrix;
            int index = -1;
            foreach (var info in TextInfo)
            {
                ++index;
                var c = Vector3.TransformCoordinate(info.Origin, modelMatrix);
                var dir = c - rayWS.Position;
                dir.Normalize();
                if (Vector3.Dot(dir, rayWS.Direction.Normalized()) < 0)
                {
                    continue;
                }

                if (!fixedSize && !info.BoundSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS))
                {
                    continue;
                }
                var left = -(info.ActualWidth * scale.X) / 2;
                var right = -left;
                var top = -(info.AcutalHeight * scale.Y) / 2;
                var bottom = -top;
                var b = GetHitTestBound(c, 
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
                        result.TextInfo = info;
                        result.TextInfoIndex = index;
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

}
