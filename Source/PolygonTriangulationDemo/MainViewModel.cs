
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PolygonTriangulationDemo
{
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public double LineThickness { get; set; }
        public double LineSmoothness { get; set; }
        public LineGeometry3D Grid { get; private set; }
        public SharpDX.Color GridColor { get; private set; }
        public SharpDX.Color TriangulationColor { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3 DirectionalLightDirection { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }
        public PhongMaterial Material { get; private set; }
        public Transform3D ModelTransform { get; private set; }
        public Transform3D ModelLineTransform { get; private set; }


        public MainViewModel()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            this.Title = "Polygon Triangulation Demo";
            this.SubTitle = null;

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 5, 5), LookDirection = new Vector3D(-0, -5, -5), UpDirection = new Vector3D(0, 1, 0) };
            
            // lines params
            this.LineThickness = 2;
            this.LineSmoothness = 2.0;

            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(-2, -5, -2);
            
            // model trafos
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);
            this.ModelLineTransform = new TranslateTransform3D(0, 0.01, 0);
            
            // model materials
            this.Material = PhongMaterials.PolishedGold;
            this.TriangulationColor = SharpDX.Color.Red;

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = SharpDX.Color.Black;
            this.GridTransform = new TranslateTransform3D(-5, -1, -1);
        }
    }
}