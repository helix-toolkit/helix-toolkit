﻿using Cyotek.Drawing.BitmapFont;
using SharpDX;
using System.Collections.ObjectModel;
using System.Reflection;

namespace HelixToolkit.SharpDX;

[Serializable]
public class BillboardText3D : BillboardBase
{
    private readonly static BitmapFont bmpFont;

    public static Stream? TextureStatic
    {
        get; private set;
    }
    const float textureScale = 0.66f;
    const string FontName = "arial";
    static BillboardText3D()
    {
        // todo

        var assembly = typeof(BillboardText3D).GetTypeInfo().Assembly;
        Stream? fontInfo = assembly?.GetManifestResourceStream($"HelixToolkit.SharpDX.Resources.{FontName}.fnt");
        bmpFont = new BitmapFont();
        bmpFont.Load(fontInfo);
        Stream? font = assembly?.GetManifestResourceStream($"HelixToolkit.SharpDX.Resources.{FontName}.dds");
        TextureStatic = font;

        /*
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
                    var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                    var sampleFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + $"\\HelixToolkit.UWP\\Resources\\{FontName}.fnt");
                    bmpFont = new BitmapFont();
                    var fileStream = new MemoryStream(sampleFile);
                    bmpFont.Load(fileStream);

                    var texFile = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + $"\\HelixToolkit.UWP\\Resources\\{FontName}.dds");
                    TextureStatic = new MemoryStream(texFile);         
        #endif
        #endif
        */
    }

    public override BillboardType Type
    {
        get
        {
            return BillboardType.MultipleText;
        }
    }

    public BitmapFont BitmapFont
    {
        get;
    }

    private ObservableCollection<TextInfo> textInfo = new();
    public ObservableCollection<TextInfo> TextInfo
    {
        set
        {
            var old = textInfo;
            if (Set(ref textInfo, value))
            {
                old.CollectionChanged -= CollectionChanged;
                IsInitialized = false;
                if (value != null)
                {
                    value.CollectionChanged += CollectionChanged;
                }
            }
        }
        get
        {
            return textInfo;
        }
    }

    public BillboardText3D()
    {
        textInfo.CollectionChanged += CollectionChanged;
        Texture = TextureStatic;
        BitmapFont = bmpFont;
    }

    public BillboardText3D(BitmapFont bitmapFont, Stream fontTexture)
    {
        textInfo.CollectionChanged += CollectionChanged;
        Texture = fontTexture;
        BitmapFont = bitmapFont;
    }

    public BillboardText3D(BitmapFont bitmapFont, TextureModel? fontTexture)
    {
        textInfo.CollectionChanged += CollectionChanged;
        Texture = fontTexture;
        BitmapFont = bitmapFont;
    }

    protected override void OnAssignTo(Geometry3D target)
    {
        base.OnAssignTo(target);
        if (target is BillboardText3D billboard)
        {
            billboard.TextInfo = this.TextInfo;
        }
    }

    private void CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        IsInitialized = false;
    }

    protected override void OnUpdateTextureAndBillboardVertices(IDeviceResources deviceResources)
    {
        Width = 0;
        Height = 0;
        // http://www.cyotek.com/blog/angelcode-bitmap-font-parsing-using-csharp
        var tempList = new List<BillboardVertex>(100);

        foreach (var textInfo in TextInfo)
        {
            var tempPrevCount = tempList.Count;
            var x = 0;
            var y = 0;
            var w = BitmapFont.TextureSize.Width;
            var h = BitmapFont.TextureSize.Height;
            char previousCharacter;

            previousCharacter = ' ';
            var normalizedText = textInfo.Text;
            var rect = new RectangleF(textInfo.Origin.X, textInfo.Origin.Y, 0, 0);
            foreach (var character in normalizedText)
            {
                switch (character)
                {
                    case '\n':
                        x = 0;
                        y -= BitmapFont.LineHeight;
                        break;
                    default:
                        var data = BitmapFont[character];
                        var kerning = BitmapFont.GetKerning(previousCharacter, character);
                        tempList.Add(DrawCharacter(data, new Vector3(x + data.XOffset, y - data.YOffset, 0), w, h, kerning, textInfo));

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
            var transform = textInfo.Angle != 0 ? Matrix3x2.CreateRotation(textInfo.Angle) : Matrix3x2.Identity;
            GetQuadOffset(rect.Width, rect.Height, textInfo.HorizontalAlignment, textInfo.VerticalAlignment, out var tl, out var br);
            var tr = new Vector2(br.X, tl.Y);
            var bl = new Vector2(tl.X, br.Y);
            //Add backbround vertex first. This is also used for hit test
            BillboardVertices.Add(new BillboardVertex()
            {
                Position = new Vector4(textInfo.Origin, 1f),
                Background = textInfo.Background,
                TexTL = Vector2.Zero,
                TexBR = Vector2.Zero,
                OffTL = Matrix3x2Helper.TransformPoint(transform, tl) + textInfo.Offset,
                OffBR = Matrix3x2Helper.TransformPoint(transform, br) + textInfo.Offset,
                OffTR = Matrix3x2Helper.TransformPoint(transform, tr) + textInfo.Offset,
                OffBL = Matrix3x2Helper.TransformPoint(transform, bl) + textInfo.Offset,
            });

            textInfo.UpdateTextInfo(rect.Width, rect.Height);
            var halfW = rect.Width / 2;
            var halfH = rect.Height / 2;
            for (var k = tempPrevCount; k < tempList.Count; ++k)
            {
                var v = tempList[k];
                v.OffTL = Matrix3x2Helper.TransformPoint(transform, v.OffTL + tl) + textInfo.Offset;
                v.OffBR = Matrix3x2Helper.TransformPoint(transform, v.OffBR + tl) + textInfo.Offset;
                v.OffTR = Matrix3x2Helper.TransformPoint(transform, v.OffTR + tl) + textInfo.Offset;
                v.OffBL = Matrix3x2Helper.TransformPoint(transform, v.OffBL + tl) + textInfo.Offset;
                tempList[k] = v;
            }
            Width += rect.Width;
            Height += rect.Height;
        }

        foreach (var v in tempList)
        {
            BillboardVertices.Add(v);
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
            foreach (var info in TextInfo)
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
        var cw = character.Width;
        var ch = character.Height;
        var cu = character.X;
        var cv = character.Y;
        var tl = new Vector2(origin.X + kerning, origin.Y);
        var br = new Vector2(origin.X + cw + kerning, origin.Y - ch);
        var offTL = tl * info.Scale * textureScale;
        var offBR = br * info.Scale * textureScale;
        var offTR = new Vector2(offBR.X, offTL.Y);
        var offBL = new Vector2(offTL.X, offBR.Y);
        var uv_tl = new Vector2(cu / w, cv / h);
        var uv_br = new Vector2((cu + cw) / w, (cv + ch) / h);

        return new BillboardVertex()
        {
            Position = new Vector4(info.Origin, 1f),
            Foreground = info.Foreground,
            Background = Color.Transparent,
            TexTL = uv_tl,
            TexBR = uv_br,
            OffTL = offTL,
            OffBL = offBL,
            OffBR = offBR,
            OffTR = offTR
        };
    }

    public override bool HitTest(HitTestContext? context, Matrix modelMatrix, ref List<HitTestResult> hits,
        object? originalSource, bool fixedSize)
    {
        if (context is null)
        {
            return false;
        }

        var rayWS = context.RayWS;
        if (!IsInitialized || context == null || Width == 0 || Height == 0
            || (!fixedSize && !BoundingSphere.TransformBoundingSphere(modelMatrix).Intersects(ref rayWS)))
        {
            return false;
        }

        return fixedSize ? HitTestFixedSize(context, ref modelMatrix, ref hits, originalSource, textInfo.Count)
            : HitTestNonFixedSize(context, ref modelMatrix, ref hits, originalSource, textInfo.Count);
    }

    protected override void AssignResultAdditional(BillboardHitResult result, int index)
    {
        base.AssignResultAdditional(result, index);
        result.TextInfo = textInfo[index];
        result.TextInfoIndex = index;
    }
}
