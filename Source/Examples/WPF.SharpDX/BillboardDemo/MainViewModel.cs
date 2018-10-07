//Flag.jpg image is created by Luis_molinero - Freepik.com

using HelixToolkit.Wpf.SharpDX;
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

        public Stream BackgroundTexture { get; }
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

        private Color4 prevLocColor;
        private Color4 prevLocBackColor;
        private TextInfo highlightedLoc;

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

            AddLocations();
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
            RestoreLocColor();
            if (e.HitTestResult != null && e.HitTestResult.ModelHit is BillboardTextModel3D model
                && e.HitTestResult is BillboardHitResult res)
            {

                if (model.Geometry == FlagsBillboard)
                {
                    SelectedFlag = FlagsBillboard.ImageInfos[res.TextInfoIndex] as Flag;
                }
                else if (model.Geometry == LandmarkBillboards)
                {
                    BackupLocColor(LandmarkBillboards.TextInfo[res.TextInfoIndex]);
                    highlightedLoc.Background = Color.Yellow;
                    highlightedLoc.Foreground = Color.Black;
                    LandmarkBillboards.Invalidate();
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
