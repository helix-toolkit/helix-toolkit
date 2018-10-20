//Flag.jpg image is created by Luis_molinero - Freepik.com

using Cyotek.Drawing.BitmapFont;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Utilities.ImagePacker;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BillboardDemo
{
    public class MainViewModel : DemoCore.BaseViewModel
    {
        public Geometry3D SphereModel { get; }
        public PhongMaterial EarthMaterial { get; }
        public BillboardImage3D FlagsBillboard { get; }
        public BillboardSingleText3D SelectedFlagBillboard
        {
            get;
        } = new BillboardSingleText3D()
            {
                FontColor = Color.Blue,
                FontWeight = FontWeights.Bold,
                BackgroundColor = new Color4(0.8f, 0.8f, 0.8f, 0.8f),
                Padding = new Thickness(2),
                IsDynamic = true // Mark dynamic because it will change frequently
            };
        public BillboardText3D LandmarkBillboards { get; }
            = new BillboardText3D() { IsDynamic = true };// Mark dynamic because it will change frequently
        public BillboardText3D LandmarkBillboards2 { get; }
        public BillboardImage3D BatchedText { private set; get; }
        public Stream BackgroundTexture { private set; get; }
        public Flag[] Flags { get => FlagsCollection.Flags; }
        private bool fixedSize = true;
        public bool FixedSize
        {
            set { SetValue(ref fixedSize, value); }
            get { return fixedSize; }
        }
        private Flag selectedFlag;
        public Flag SelectedFlag
        {
            set
            {
                SetValue(ref selectedFlag, value);
                UpdateSelectedFlagBillboard(value);
            }
            get { return selectedFlag; }
        }

        private Color4 prevLocColor, prevLocColor2;
        private Color4 prevLocBackColor, prevLocBackColor2;
        private TextInfo highlightedLoc, highlightedLoc2;

        public MainViewModel()
        {
            Title = "HelixToolkit Billboard Demo";
            SubTitle = "Wpf SharpDX";
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera()
            {
                Position = new System.Windows.Media.Media3D.Point3D(0, -10, 0),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 10, 0),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 1),
                FarPlaneDistance = 1000,
                NearPlaneDistance = 0.1,
                Width = 10
            };
            var builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 4, 16, 16);
            SphereModel = builder.ToMesh();
            SphereModel.UpdateOctree();
            EarthMaterial = PhongMaterials.White;
            EarthMaterial.SpecularShininess = 10;
            EarthMaterial.SpecularColor = new Color4(0.3f, 0.3f, 0.3f, 1);
            EarthMaterial.DiffuseMap = LoadFileToMemory("earthmap.jpg");
            EarthMaterial.SpecularColorMap = LoadFileToMemory("earthspec.jpg");
            EarthMaterial.DisplacementMap = LoadFileToMemory("earthbump.jpg");
            EarthMaterial.NormalMap = LoadFileToMemory("earthNormal.jpg");
            EarthMaterial.DisplacementMapScaleMask = new Vector4(0.2f, 0.2f, 0.2f, 0);
            EarthMaterial.EnableTessellation = true;
            EarthMaterial.MaxDistanceTessellationFactor = 1;
            EarthMaterial.MinDistanceTessellationFactor = 3;
            EarthMaterial.MaxTessellationDistance = 500;
            EarthMaterial.MinDistanceTessellationFactor = 2;
            EarthMaterial.EnableAutoTangent = true;
            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.DarkBlue, Position = 0f },
                    new SharpDX.Direct2D1.GradientStop(){ Color = Color.Black, Position = 1f }
                });

            FlagsBillboard = new BillboardImage3D(LoadFileToMemory("Flags.jpg"));
            foreach (var info in FlagsCollection.Flags.Where(x => x.Position != Vector3.Zero))
            {
                FlagsBillboard.ImageInfos.Add(info);
            }

            var segoeFont = new BitmapFont();
            segoeFont.Load(@"Fonts\SegoeScript.fnt");
            LandmarkBillboards2 = new BillboardText3D(segoeFont, LoadFileToMemory(@"Fonts\SegoeScript.dds"));
            AddLocations();
            AddBatchedText();
        }

        private void AddLocations()
        {
            float offset = 4.5f;
            float scale = 0.8f;
            LandmarkBillboards.TextInfo.Add(new TextInfo("Arctic", Vector3.UnitZ * offset)
            { Foreground = Color.Red, Scale = scale * 2 });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Antarctica", Vector3.UnitZ * -offset)
            { Foreground = Color.Blue, Scale = scale * 2 });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Equator",
                new Vector3(0, offset, 0))
            { Foreground = Color.White, Scale = scale });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Equator",
                new Vector3((float)Math.Cos(Math.PI / 6) * offset, -(float)Math.Sin(Math.PI / 6) * offset, 0))
            { Foreground = Color.White, Scale = scale });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Equator",
                new Vector3(-(float)Math.Cos(Math.PI / 6) * offset, -(float)Math.Sin(Math.PI / 6) * offset, 0))
            { Foreground = Color.White, Scale = scale });

            LandmarkBillboards.TextInfo.Add(new TextInfo("Pacific", new Vector3(3.922917f, 0.2635128f, 2.084114f))
            { Foreground = Color.White, Background = Color.Green, Scale = scale * 1.4f });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Indian", new Vector3(-0.8591346f, -4.321474f, -0.2010482f))
            { Foreground = Color.White, Background = Color.Green, Scale = scale * 1.4f });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Atlantic", new Vector3(-2.595731f, 1.984212f, 3.021353f))
            { Foreground = Color.White, Background = Color.Green, Scale = scale * 1.4f });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Southern", new Vector3(0.08587439f, -2.127402f, -3.936893f))
            { Foreground = Color.White, Background = Color.Green, Scale = scale * 1.4f });
            LandmarkBillboards.TextInfo.Add(new TextInfo("Arctic Ocean", new Vector3(-0.7553688f, -0.6352348f, 4.379822f))
            { Foreground = Color.White, Background = Color.Green, Scale = scale * 1.4f });

            LandmarkBillboards2.TextInfo.Add(new TextInfo("Asia", new Vector3(-0.8280244f, -3.665166f, 2.356252f))
            { Foreground = Color.Red, Background = Color.DarkGray, Scale = 1});
            LandmarkBillboards2.TextInfo.Add(new TextInfo("Europe", new Vector3(-2.531648f, -1.158762f, 3.501563f))
            { Foreground = Color.Blue, Background = Color.DarkGray, Scale = 1 });
            LandmarkBillboards2.TextInfo.Add(new TextInfo("North America", new Vector3(0.1436681f, 3.24761f, 3.080247f))
            { Foreground = Color.Red, Background = Color.DarkGray, Scale = 1 });
            LandmarkBillboards2.TextInfo.Add(new TextInfo("South America", new Vector3(-1.96404f, 3.929909f, -0.6905473f))
            { Foreground = Color.Orchid, Background = Color.DarkGray, Scale = 1 });
            LandmarkBillboards2.TextInfo.Add(new TextInfo("Oceania", new Vector3(2.306917f, -3.398433f, -1.661219f))
            { Foreground = Color.Green, Background = Color.DarkGray, Scale = 1 });
        }

        private void AddBatchedText()
        {
            var texts = new TextInfoExt[]
            {
                new TextInfoExt()
                {
                    Text = "English",
                    Foreground = Color.Indigo,
                    Background = Color.LightCoral,
                    FontWeight = SharpDX.DirectWrite.FontWeight.Light,
                    FontFamily = "Segoe UI",
                    Padding = new Vector4(4),
                    Origin = new Vector3(-10, 0, -4),
                    Size = 18
                },
                new TextInfoExt()
                {
                    Text = "中文",
                    Foreground = Color.Green,
                    Background = Color.White,
                    FontStyle = SharpDX.DirectWrite.FontStyle.Italic,
                    Origin = new Vector3(-10, 0, -2),
                    Padding = new Vector4(4,2,4,2),
                    FontFamily = "Microsoft YaHei",
                    Size = 16
                },
                new TextInfoExt()
                {
                    Text = "日本語",
                    Foreground = Color.Blue,
                    Background = Color.Green,
                    FontWeight = SharpDX.DirectWrite.FontWeight.Bold,
                    Origin = new Vector3(-10, 0, 0),
                    Padding = new Vector4(2,4,2,4),
                    Size = 18
                },
                new TextInfoExt()
                {
                    Text = "Français",
                    Foreground = Color.White,
                    Background = Color.Black,
                    Origin = new Vector3(-10, 0, 2),
                    Padding = new Vector4(8,4,2,4),
                    FontFamily = "Calibri",
                    Size = 20
                },
                new TextInfoExt()
                {
                    Text = "Español",
                    Foreground = Color.DarkSeaGreen,
                    Background = Color.LightCyan,
                    Origin = new Vector3(-10, 0, 4),
                    Padding = new Vector4(6),
                    FontFamily = "Times New Roman",
                    Size = 22
                },
                new TextInfoExt()
                {
                    Text = "繁體中文",
                    Foreground = Color.Red,
                    Background = Color.Blue,
                    Padding = new Vector4(2,2,2,2),
                    Origin = new Vector3(-14, 0, -4),
                    FontStyle = SharpDX.DirectWrite.FontStyle.Oblique,
                    Size = 14
                },
                new TextInfoExt()
                {
                    Text = "한국어",
                    Foreground = Color.LightSalmon,
                    Background = Color.DarkSlateBlue,
                    
                    Origin = new Vector3(-14, 0, -2),
                    Padding = new Vector4(4,2,4,2),
                    Size = 16
                },
                new TextInfoExt()
                {
                    Text = "Deutsch",
                    Foreground = Color.Blue,
                    Background = Color.White,
                    FontWeight = SharpDX.DirectWrite.FontWeight.Bold,
                    Origin = new Vector3(-14, 0, 0),
                    Padding = new Vector4(2,4,2,4),
                    FontFamily = "Garamond",
                    Size = 18
                },
                new TextInfoExt()
                {
                    Text = "Português",
                    Foreground = Color.DarkRed,
                    Background = Color.Lavender,
                    Origin = new Vector3(-14, 0, 2),
                    Padding = new Vector4(8,4,2,4),
                    FontFamily = "Tahoma",
                    Size = 20
                },
                new TextInfoExt()
                {
                    Text = "Below are batched \ntexts rendering \nwith different styles",
                    Foreground = Color.PaleGoldenrod,
                    Background = Color.Transparent,
                    Origin = new Vector3(-12, 0, 8),
                    Padding = new Vector4(6),
                    FontFamily = "Consolas",
                    Size = 24
                }
            };
            BatchedText = texts.ToBillboardImage3D(EffectsManager);           
        }

        private void UpdateSelectedFlagBillboard(Flag flag)
        {
            if (flag.Position != Vector3.Zero)
            {
                SelectedFlagBillboard.TextInfo = new TextInfo(flag.Name, flag.Position) { Scale = 0.015f };
            }
            else
            {
                SelectedFlagBillboard.TextInfo = null;
            }
        }

        public void OnMouseUpHandler(object sender, MouseUp3DEventArgs e)
        {
            
            if (e.HitTestResult != null && e.HitTestResult.ModelHit is BillboardTextModel3D model
                && e.HitTestResult is BillboardHitResult res)
            {

                if (model.Geometry == FlagsBillboard)
                {
                    SelectedFlag = FlagsBillboard.ImageInfos[res.TextInfoIndex] as Flag;
                }
                else if (model.Geometry == LandmarkBillboards)
                {
                    RestoreLocColor();
                    BackupLocColor(LandmarkBillboards.TextInfo[res.TextInfoIndex]);
                    highlightedLoc.Background = Color.Yellow;
                    highlightedLoc.Foreground = Color.Black;
                    LandmarkBillboards.Invalidate();
                }
                else if(model.Geometry == LandmarkBillboards2)
                {
                    RestoreLocColor2();
                    BackupLocColor2(LandmarkBillboards2.TextInfo[res.TextInfoIndex]);
                    highlightedLoc2.Background = Color.Yellow;
                    highlightedLoc2.Foreground = Color.Black;
                    LandmarkBillboards2.Invalidate();
                }
            }
        }

        private void BackupLocColor(TextInfo loc)
        {
            highlightedLoc = loc;
            prevLocBackColor = highlightedLoc.Background;
            prevLocColor = highlightedLoc.Foreground;
        }

        private void RestoreLocColor()
        {
            if (highlightedLoc != null)
            {
                highlightedLoc.Background = prevLocBackColor;
                highlightedLoc.Foreground = prevLocColor;
                LandmarkBillboards.Invalidate();
            }
            highlightedLoc = null;
        }


        private void BackupLocColor2(TextInfo loc)
        {
            highlightedLoc2 = loc;
            prevLocBackColor2 = highlightedLoc2.Background;
            prevLocColor2 = highlightedLoc2.Foreground;
        }

        private void RestoreLocColor2()
        {
            if (highlightedLoc2 != null)
            {
                highlightedLoc2.Background = prevLocBackColor2;
                highlightedLoc2.Foreground = prevLocColor2;
                LandmarkBillboards2.Invalidate();
            }
            highlightedLoc2 = null;
        }

        public void OnFlag_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData("Flag") is Flag flag && sender is Viewport3DX viewport)
            {
                var point = e.GetPosition(sender as IInputElement);
                var hits = viewport.FindHits(point);
                if (hits.Count == 0)
                {
                    return;
                }
                if (hits[0].ModelHit is GeometryModel3D model && model.Geometry == SphereModel)
                {
                    var pos = hits[0].PointHit;
                    var normal = hits[0].NormalAtHit;
                    flag.Position = pos + normal * 0.5f;
                    FlagsBillboard.ImageInfos.Remove(flag);
                    FlagsBillboard.ImageInfos.Add(flag);
                }
            }
        }

        //private DataObject dragData;
        private ListBox dragSource;
        public void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragSource = sender as ListBox;
        }

        public void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                var parent = sender as ListBox;
                if (parent == null || parent != dragSource)
                {
                    return;
                }
                if (e.OriginalSource is FrameworkElement dp && dp.DataContext is Flag flag)
                {
                    DataObject dragData = new DataObject("Flag", flag);
                    DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Move);
                    dragSource = null;
                }
            }

        }
    }
}
