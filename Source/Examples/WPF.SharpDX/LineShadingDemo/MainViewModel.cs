// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LineShadingDemo
{
    using System;
    using System.Linq;

    using DemoCore;
    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;

    using SharpDX;
    using Media = System.Windows.Media;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Color = System.Windows.Media.Color;
    using Plane = SharpDX.Plane;
    using Vector3 = SharpDX.Vector3;
    using Colors = System.Windows.Media.Colors;
    using Color4 = SharpDX.Color4;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public double LineThickness { get; set; }
        public double LineSmoothness { get; set; }
        public bool LinesEnabled { get; set; }
        public bool GridEnabled { get; set; }
                
        public PhongMaterial Material1 { get; private set; }
        public PhongMaterial Material2 { get; private set; }
        public PhongMaterial Material3 { get; private set; }        
        public LineMaterial LineMaterial { get; private set; }
        public LineMaterial GridMaterial { private set; get; }
        public Color GridColor { get; private set; }

        public Transform3D Model1Transform { get; private set; }
        public Transform3D Model2Transform { get; private set; }
        public Transform3D Model3Transform { get; private set; }
        public Transform3D Model4Transform { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        private bool enableArrowHeadTail = false;
        public bool EnableArrowHeadTail
        {
            set
            {
                if(SetValue(ref enableArrowHeadTail, value))
                {
                    var texture = LineMaterial.Texture;
                    var tscale = LineMaterial.TextureScale;
                    LineMaterial = value ?
                        new LineArrowHeadTailMaterial() { ArrowSize = 0.04, Color = Colors.White, Texture = texture, TextureScale = tscale} 
                    : new LineArrowHeadMaterial() { ArrowSize = 0.04, Color = Colors.White, Texture = texture, TextureScale = tscale };
                    OnPropertyChanged(nameof(LineMaterial));
                }
            }
            get { return enableArrowHeadTail; }
        }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            this.Title = "Line Shading Demo (HelixToolkitDX)";
            this.SubTitle = null;

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 5, 5), LookDirection = new Vector3D(-0, -5, -5), UpDirection = new Vector3D(0, 1, 0) };

            // setup lighting            
            this.AmbientLightColor = Colors.DimGray;
            this.DirectionalLightColor = Colors.White;
            this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = Media.Colors.Black;
            this.GridTransform = new TranslateTransform3D(-5, -1, -5);

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
            this.Model = b1.ToMeshGeometry3D();

            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
            //this.Lines = e1.ToLineGeometry3D().ToUnshared();
            this.Lines = e1.ToLineGeometry3D(true);
            this.Lines.Colors = new Color4Collection();
            var linesCount = this.Lines.Indices.Count;
            var rnd = new Random();
            while (linesCount-- > 0)
            {
                this.Lines.Colors.Add(rnd.NextColor());
            }

            // lines params
            this.LineThickness = 2;
            this.LineSmoothness = 2.0;
            this.LinesEnabled = true;
            this.GridEnabled = true;

            // model trafos
            this.Model1Transform = new TranslateTransform3D(0, 0, 0);
            this.Model2Transform = new TranslateTransform3D(-2, 0, 0);
            this.Model3Transform = new TranslateTransform3D(+2, 0, 0);
            this.Model4Transform = new TranslateTransform3D(0, 2, 0);
            // model materials
            this.Material1 = PhongMaterials.PolishedGold;
            this.Material2 = PhongMaterials.Copper;
            this.Material3 = PhongMaterials.Glass;
            this.LineMaterial = new LineArrowHeadMaterial() { ArrowSize = 0.04, Color = Colors.White, TextureScale = 0.4 };
            this.GridMaterial = new LineMaterial() { Color = Colors.Red, TextureScale = 0.4};
            var dash = LoadFileToMemory("Dash.png");
            var dotLine = LoadFileToMemory("DotLine.png");
            GridMaterial.Texture = dotLine;
            LineMaterial.Texture = dash;
        }
    }
}