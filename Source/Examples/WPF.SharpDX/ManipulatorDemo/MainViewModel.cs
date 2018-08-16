// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ManipulatorDemo
{
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using AxisAngleRotation3D = System.Windows.Media.Media3D.AxisAngleRotation3D;
    using Color = System.Windows.Media.Color;
    using Colors = System.Windows.Media.Colors;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using RotateTransform3D = System.Windows.Media.Media3D.RotateTransform3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Model2 { private set; get; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public PhongMaterial Material1 { get; private set; }
        public PhongMaterial Material2 { get; private set; }
        public PhongMaterial Material3 { get; private set; }
        public Color GridColor { get; private set; }

        public Transform3D Model1Transform { get; set; }
        public Transform3D Model2Transform { get; set; }
        public Transform3D Model3Transform { get; set; }
        public Transform3D GridTransform { get; set; }

        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public Element3D Target { set; get; }
        public Vector3 CenterOffset { set; get; }

        public ICommand ResetTransformsCommand { private set; get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];

            this.Title = "Manipulator Demo";
            this.SubTitle = null;

            // camera setup
            this.Camera = new OrthographicCamera { Position = new Point3D(0, 0, 5), LookDirection = new Vector3D(0, 0, -5), UpDirection = new Vector3D(0, 1, 0) };

            // setup lighting            
            this.AmbientLightColor = Colors.DimGray;
            this.DirectionalLightColor = Colors.White;
            this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = Colors.Black;
            this.GridTransform = new TranslateTransform3D(-5, -1, -5);

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 1.5, BoxFaces.All);
            this.Model = b1.ToMeshGeometry3D();
            var m1 = Load3ds("suzanne.3ds");
            this.Model2 = m1[0].Geometry as MeshGeometry3D;
            //Manully set an offset for test
            for(int i=0; i < Model2.Positions.Count; ++i)
            {
                Model2.Positions[i] = Model2.Positions[i] + new Vector3(2, 3, 4);
            }
            Model2.UpdateBounds();
            
            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 1.5);
            this.Lines = e1.ToLineGeometry3D();

            // model trafos
            this.Model1Transform = new TranslateTransform3D(0, 0, 0);
            this.Model2Transform = new TranslateTransform3D(-3, 0, 0);
            this.Model3Transform = new TranslateTransform3D(+3, 0, 0);

            // model materials
            this.Material1 = PhongMaterials.Orange;
            this.Material2 = PhongMaterials.Orange;
            this.Material3 = PhongMaterials.Red;

            var dr = Colors.DarkRed;
            Console.WriteLine(dr);
            ResetTransformsCommand = new RelayCommand((o) => 
            {
                this.Model1Transform = new TranslateTransform3D(0, 0, 0);
                this.Model2Transform = new TranslateTransform3D(-3, 0, 0);
                this.Model3Transform = new TranslateTransform3D(+3, 0, 0);
            });
        }

        public void OnMouseDown3DHandler(object sender, MouseDown3DEventArgs e)
        {
            if(e.HitTestResult != null && e.HitTestResult.ModelHit is MeshGeometryModel3D m && (m.Geometry == Model || m.Geometry == Model2))
            {
                Target = null;
                CenterOffset = m.Geometry.Bound.Center; // Must update this before updating target
                Target = e.HitTestResult.ModelHit as Element3D;
                
            }
        }

        public List<Object3D> Load3ds(string path)
        {
            if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
            {
                var reader = new ObjReader();
                var list = reader.Read(path);
                return list;
            }
            else if (path.EndsWith(".3ds", StringComparison.CurrentCultureIgnoreCase))
            {
                var reader = new StudioReader();
                var list = reader.Read(path);
                return list;
            }
            else
            {
                return new List<Object3D>();
            }
        }
    }
}