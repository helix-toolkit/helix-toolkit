/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Cyotek.Drawing.BitmapFont;
using HelixToolkit.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Matrix = System.Numerics.Matrix4x4;
#if CORE

#else
#if NETFX_CORE
using Media = Windows.UI.Xaml.Media;
#else
#endif
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
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
        const float textureScale = 0.66f;
        const string FontName = "arial";
        static BillboardText3D()
        {
#if CORE
            var assembly = typeof(BillboardText3D).GetTypeInfo().Assembly;
            Stream fontInfo = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Core.Resources.{FontName}.fnt");
            bmpFont = new BitmapFont();
            bmpFont.Load(fontInfo);
            Stream font = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Core.Resources.{FontName}.dds");
            TextureStatic = font;
#else
#if !NETFX_CORE
            var assembly = Assembly.GetExecutingAssembly();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream($"HelixToolkit.Wpf.SharpDX.Textures.{FontName}.fnt");

            bmpFont = new BitmapFont();
            bmpFont.Load(texDescriptionStream);// BitmapFontLoader.LoadFontFromFile(texDescriptionFilePath);
            texDescriptionStream.Dispose();
            //Read the texture          
            var texImageStream = assembly.GetManifestResourceStream($"HelixToolkit.Wpf.SharpDX.Textures.{FontName}.dds");
            TextureStatic = MemoryStream.Synchronized(texImageStream);
#else
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.UWP");
            var sampleFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + $"\\Resources\\{FontName}.fnt");
            bmpFont = new BitmapFont();
            var fileStream = new MemoryStream(sampleFile);
            bmpFont.Load(fileStream);

            var texFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + $"\\Resources\\{FontName}.dds");
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

        public BillboardText3D()
        {
            textInfo.CollectionChanged += CollectionChanged;
        }

        protected override void OnAssignTo(Geometry3D target)
        {
            base.OnAssignTo(target);
            if(target is BillboardText3D billboard)
            {
                billboard.TextInfo = this.TextInfo;
            }
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
                        rect.Width = Math.Max(rect.Width, x * textInfo.Scale * textureScale);
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
            var off_tl = tl * info.Scale * textureScale;
            var off_br = br * info.Scale * textureScale;
            return new BillboardVertex()
            {
                Position = info.Origin.ToVector4(),
                Foreground = info.Foreground,
                Background = Color.Transparent,
                TexTL = uv_tl,
                TexBR = uv_br,
                OffTL = off_tl,
                OffBR = off_br
            };
        }

        public override bool HitTest(RenderContext context, Matrix modelMatrix, ref Ray rayWS, ref List<HitTestResult> hits, 
            object originalSource, bool fixedSize)
        {
            var h = false;
            var result = new BillboardHitResult
            {
                Distance = double.MaxValue
            };

            if (context == null || Width == 0 || Height == 0)
            {
                return false;
            }
            var scale = modelMatrix.ScaleVector();
            var projectionMatrix = context.ProjectionMatrix;
            var viewMatrix = context.ViewMatrix;
            var viewMatrixInv = viewMatrix.PsudoInvert();
            var visualToScreen = context.ScreenViewProjectionMatrix;
            int index = -1;
            var rayDir = Vector3.Normalize(rayWS.Direction);
            foreach (var info in TextInfo)
            {
                ++index;
                var c = Vector3Helper.TransformCoordinate(info.Origin, modelMatrix);
                var dir = Vector3.Normalize(c - rayWS.Position);
                if (Vector3.Dot(dir, rayDir) < 0)
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
                    if (Collision.RayIntersectsBox(ref rayWS, ref b, out float distance))
                    {
                        h = true;
                        result.ModelHit = originalSource;
                        result.IsValid = true;
                        result.PointHit = rayWS.Position + (rayWS.Direction * distance);
                        result.Distance = distance;
                        result.TextInfo = info;
                        result.TextInfoIndex = index;
                        result.Geometry = this;
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
