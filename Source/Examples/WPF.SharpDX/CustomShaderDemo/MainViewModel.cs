// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomShaderDemo
{
    using System.Linq;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;

    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public PointGeometry3D Points { get; private set; }
        public BillboardText3D Text { get; private set; }

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

            // Create a custom render techniques manager that 
            // only supports Phong and Blinn
            RenderTechniquesManager = new CustomRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques["RenderCustom"];
            EffectsManager = new CustomEffectsManager(RenderTechniquesManager);

            // setup lighting            
            AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            DirectionalLightColor = Color.White;
            DirectionalLightDirection = new Vector3(-2, -5, -2);

            // floor plane grid
            Grid = LineBuilder.GenerateGrid();
            GridColor = Color.Black;
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

            // model transform
            Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

            // model materials
            RedMaterial = PhongMaterials.Red;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;

            Points = new PointGeometry3D();
            var ptPos = new Vector3Collection();
            var ptIdx = new IntCollection();

            Text = new BillboardText3D();

            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    ptIdx.Add(ptPos.Count);
                    ptPos.Add(new Vector3(x, -1, y));
                    Text.TextInfo.Add(new TextInfo(string.Format("{0}:{1}", x, y), new Vector3(x, -1, y)));
                }
            }

            Points.Positions = ptPos;
            Points.Indices = ptIdx;
        }
    }
}
