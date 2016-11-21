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
    using HelixToolkit.Wpf;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public PointGeometry3D Points { get; private set; }
        public BillboardText3D Text { get; private set; }

        public BillboardSingleText3D Billboard1Model { private set; get; }
        public BillboardSingleText3D Billboard2Model { private set; get; }
        public BillboardSingleText3D Billboard3Model { private set; get; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public SharpDX.Color GridColor { get; private set; }

        public Media3D.Transform3D Model1Transform { get; private set; }
        public Media3D.Transform3D Model2Transform { get; private set; }
        public Media3D.Transform3D Model3Transform { get; private set; }
        public Media3D.Transform3D GridTransform { get; private set; }

        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }

        public MainViewModel()
        {
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

            // default render technique
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            // setup lighting            
            AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            DirectionalLightColor = Color.White;
            DirectionalLightDirection = new Vector3(-2, -5, -2);

            // floor plane grid
            Grid = LineBuilder.GenerateGrid();
            GridColor = SharpDX.Color.Black;
            GridTransform = new Media3D.TranslateTransform3D(-5, -1, -5);

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

            // model trafos
            Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

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

            for (var i = 0; i < 50; i++)
            {
                for (var j = 0; j < 50; j++)
                {
                    Text.TextInfo.Add(new TextInfo("Hello World", new Vector3(i,j,0)));
                }
            }

            Billboard1Model = new BillboardSingleText3D()
            {
                TextInfo = new TextInfo("Model 1", new Vector3(0, 1, 0)),
                FontColor =Color.Blue,
                FontSize=16,
                BackgroundColor =Color.Plum,
                FontStyle= System.Windows.FontStyles.Italic
            };

            var background = Color.Blue;
            background.A = (byte)120;
            Billboard2Model = new BillboardSingleText3D()
            {
                TextInfo = new TextInfo("Model 1", new Vector3(2, 1, 0)),
                FontSize=14,
                FontColor = Color.Green,
                BackgroundColor = background,
                FontWeight= System.Windows.FontWeights.Bold
            };
            background = Color.Purple;
            background.A = (byte)50;
            Billboard3Model = new BillboardSingleText3D()
            {
                TextInfo = new TextInfo("Model 1", new Vector3(-2, 1, 0)),
                FontSize = 12,
                FontColor = Color.Red,
                BackgroundColor = background,
                FontFamily = new System.Windows.Media.FontFamily("Times New Roman"),
                FontStyle= System.Windows.FontStyles.Italic
            };
        }
    }
}
