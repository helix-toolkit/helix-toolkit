// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleDemo
{
    using System.Linq;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;

    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Color = System.Windows.Media.Color;
    using Plane = SharpDX.Plane;
    using Vector3 = SharpDX.Vector3;
    using Colors = System.Windows.Media.Colors;
    using Color4 = SharpDX.Color4;
    using HelixToolkit.Wpf;
    using System.Windows.Media.Imaging;
    using System.IO;
    using System.Windows.Input;
    using System;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D TextModel { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public PointGeometry3D Points { get; private set; }
        public BillboardText3D Text { get; private set; }

        public BillboardSingleText3D Billboard1Model { private set; get; }
        public BillboardSingleText3D Billboard2Model { private set; get; }
        public BillboardSingleText3D Billboard3Model { private set; get; }
        public BillboardSingleImage3D BillboardImageModel { private set; get; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public Color GridColor { get; private set; }

        public Transform3D Model1Transform { get; private set; }
        public Transform3D Model2Transform { get; private set; }
        public Transform3D Model3Transform { get; private set; }
        public Transform3D Model4Transform { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public Vector3D UpDirection { set; get; } = new Vector3D(0, 1, 0);
        public Stream BackgroundTexture { get; }
        public ICommand UpXCommand { private set; get; }
        public ICommand UpYCommand { private set; get; }
        public ICommand UpZCommand { private set; get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            // titles
            Title = "Simple Demo";
            SubTitle = "WPF & SharpDX";

            // camera setup
            Camera = new PerspectiveCamera { 
                Position = new Point3D(3, 3, 5), 
                LookDirection = new Vector3D(-3, -3, -5), 
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000000
            };

            // setup lighting            
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // floor plane grid
            Grid = LineBuilder.GenerateGrid(new Vector3(0, 1, 0), -5, 5, -5, 5);
            GridColor = Colors.Black;
            GridTransform = new Media3D.TranslateTransform3D(0, -3, 0);

            // scene model3d
            var b1 = new MeshBuilder();            
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
           
            var meshGeometry = b1.ToMeshGeometry3D();
            meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            Model = meshGeometry;

            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
            Lines = e1.ToLineGeometry3D();

            var textBuilder = new MeshBuilder();
            textBuilder.ExtrudeText("HelixToolkit.SharpDX", "Arial", System.Windows.FontStyles.Normal, System.Windows.FontWeights.Bold,
                14, new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            TextModel = textBuilder.ToMesh();

            // model trafos
            Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);
            Model4Transform = new Media3D.TranslateTransform3D(-8, 0, -5);

            // model materials
            RedMaterial = PhongMaterials.Red;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;
            //var diffColor = this.RedMaterial.DiffuseColor;
            //diffColor.Alpha = 0.5f;
            //this.RedMaterial.DiffuseColor = diffColor;   

            Points = new PointGeometry3D();
            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        ptIdx.Add(ptPos.Count);
                        ptPos.Add(new Vector3(x, y, z));
                    }
                }
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;

            Text = new BillboardText3D();
            int numRows = 11;
            int numColumns = 11;
            string[] texts = new string[]
            {
                "HelixToolkit",
                "abcde",
                "random",
                "SharpDX",
                "DirectX"
            };
            float angle = 0;
            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numColumns; j++)
                {
                    angle += (float)Math.PI / 10;
                    Text.TextInfo.Add(new TextInfo(texts[(i + j) % texts.Length], new Vector3((i - numRows / 2), 0.0f, (j - numColumns / 2)))
                    {
                        Foreground = new Color4((float)i / numRows, 1- (float)i / numRows, (float)(numColumns - j) / numColumns, 1f),
                        Background = new Color4(1 - (float)i / numRows, (float)(numColumns - j) / numColumns, (float)i / numRows, 0.8f),
                        Scale = Math.Max(0.01f, (float)i / numRows * 0.02f),
                        Angle = angle
                    });
                }
            }

            Billboard1Model = new BillboardSingleText3D()
            {
                TextInfo = new TextInfo("Model 1", new Vector3(0, 1, 0)) { Angle = 0},
                FontColor =Colors.Blue.ToColor4(),
                FontSize=12,
                BackgroundColor =Colors.Plum.ToColor4(),
                FontStyle= System.Windows.FontStyles.Italic,
                Padding = new System.Windows.Thickness(2), 
            };

            var background = Colors.Blue;
            background.A = (byte)120;
            Billboard2Model = new BillboardSingleText3D()
            {
                TextInfo = new TextInfo("Model 2", new Vector3(2, 1, 0)) { Angle = -(float)Math.PI / 3 },
                FontSize =12,
                FontColor = Colors.Green.ToColor4(),
                BackgroundColor = background.ToColor4(),
                FontWeight = System.Windows.FontWeights.Bold,
                Padding = new System.Windows.Thickness(2), 
            };
            background = Colors.Purple;
            background.A = (byte)50;
            Billboard3Model = new BillboardSingleText3D(2,0.8f)
            {
                TextInfo = new TextInfo("Model 3", new Vector3(-2, 1, 0)) { Angle = -(float)Math.PI / 6 },
                FontSize = 12,
                FontColor = Colors.Red.ToColor4(),
                BackgroundColor = background.ToColor4(),
                FontFamily = "Times New Roman",
                FontStyle= System.Windows.FontStyles.Italic,
                Padding = new System.Windows.Thickness(2),               
            };


            //BillboardImageModel = new BillboardSingleImage3D(CreateBitmapSample()) { MaskColor = Color.Black };
            BillboardImageModel = new BillboardSingleImage3D(CreatePNGSample(), 1, 1) { Angle = -(float)Math.PI / 5 };
            BillboardImageModel.Center = new Vector3(2, 2, 0);

            UpXCommand = new RelayCommand(x => { UpDirection = new Vector3D(1, 0, 0); });
            UpYCommand = new RelayCommand(x => { UpDirection = new Vector3D(0, 1, 0); });
            UpZCommand = new RelayCommand(x => { UpDirection = new Vector3D(0, 0, 1); });
            BackgroundTexture =
                BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                    new SharpDX.Direct2D1.GradientStop(){ Color = Colors.White.ToColor4(), Position = 0f },
                    new SharpDX.Direct2D1.GradientStop(){ Color = Colors.DarkGray.ToColor4(), Position = 1f }
                });
        }

        private BitmapSource CreateBitmapSample()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream("SimpleDemo.Sample.png");
            var decoder = new PngBitmapDecoder(texDescriptionStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
            return decoder.Frames[0];
        }

        private Stream CreatePNGSample()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            //Read the texture description           
            var texDescriptionStream = assembly.GetManifestResourceStream("SimpleDemo.Sample.png");
            texDescriptionStream.Position = 0;
            return texDescriptionStream;
        }
    }
}
