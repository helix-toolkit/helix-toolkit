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
        private BillboardSingleText3D selectedFlagBillboard;
        public BillboardSingleText3D SelectedFlagBillboard
        {
            set { SetValue(ref selectedFlagBillboard, value); }
            get { return selectedFlagBillboard; }
        }
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
                FarPlaneDistance = 1000, NearPlaneDistance = 0.1, Width = 10
            };
            var builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 4, 16, 16);
            SphereModel = builder.ToMesh();
            SphereModel.UpdateOctree();
            EarthMaterial = PhongMaterials.White;
            EarthMaterial.SpecularShininess = 1;
            EarthMaterial.DiffuseMap = LoadFileToMemory("earthmap.jpg");
            //EarthMaterial.SpecularColorMap = LoadFileToMemory("earthspec.jpg");
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
            foreach (var info in FlagsCollection.Flags.Where(x=>x.Position != Vector3.Zero))
            {
                FlagsBillboard.ImageInfos.Add(info);
            }

            SelectedFlagBillboard = new BillboardSingleText3D()
            {
                FontColor = Color.Blue, FontWeight = FontWeights.Bold,
                BackgroundColor = new Color4(0.8f, 0.8f, 0.8f, 0.8f),
                Padding = new Thickness(2)
            };
        }

        private void UpdateSelectedFlagBillboard(Flag flag)
        {
            if (flag.Position != Vector3.Zero)
            {
                SelectedFlagBillboard.TextInfo = new TextInfo(flag.Name, flag.Position);
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
                SelectedFlag = FlagsBillboard.ImageInfos[res.TextInfoIndex] as Flag;
            }
        }

        public void OnFlag_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetData("Flag") is Flag flag && sender is Viewport3DX viewport)
            {
                var point = e.GetPosition(sender as IInputElement);
                var hits = viewport.FindHits(point);
                if (hits.Count == 0)
                {
                    return;
                }
                if(hits[0].ModelHit is GeometryModel3D model && model.Geometry == SphereModel)
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
            if(e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                var parent = sender as ListBox;
                if(parent == null || parent != dragSource)
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
