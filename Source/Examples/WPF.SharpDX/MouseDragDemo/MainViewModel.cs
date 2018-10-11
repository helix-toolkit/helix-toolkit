// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MouseDragDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D MeshGeometry { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public Color GridColor { get; private set; }

        public List<Matrix> Model1Instances { get; private set; }

        public Transform3D Model1Transform { get; private set; }
        public Transform3D Model2Transform { get; private set; }
        public Transform3D Model3Transform { get; private set; }
        public Transform3D GridTransform { get; private set; }


        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public RelayCommand AddCmd { get; set; }
        public RelayCommand DelCmd { get; set; }



        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            // titles
            this.Title = "Mouse Drag Demo";
            this.SubTitle = "WPF & SharpDX";

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 0, 9), LookDirection = new Vector3D(-0, -0, -9), UpDirection = new Vector3D(0, 1, 0) };

            // setup lighting            
            this.AmbientLightColor = Colors.DimGray;
            this.DirectionalLightColor = Colors.White;
            this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid(Vector3.UnitZ, -5, 5);
            this.GridColor = Colors.Black;
            this.GridTransform = new Media3D.TranslateTransform3D(-0, -0, -0);

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.65);
            b1.AddBox(new Vector3(0, 0, 0), 1, 1, 1);
            var meshGeometry = b1.ToMeshGeometry3D();
            meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            this.MeshGeometry = meshGeometry;
            this.Model1Instances = new List<Matrix>();
            for (int i = 0; i < 5; i++)
            {
                this.Model1Instances.Add(Matrix.Translation(0, i, 0));
            }

            // lines model3d
            var e1 = new LineBuilder();
            e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
            this.Lines = e1.ToLineGeometry3D();

            // model trafos
            this.Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0.0);
            this.Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            this.Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

            // model materials
            this.RedMaterial = PhongMaterials.Red;
            this.GreenMaterial = PhongMaterials.Green;
            this.BlueMaterial = PhongMaterials.Blue;

            // ---
            this.Shape3DCollection = new ObservableCollection<Shape3D>
            {
                new Shape3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.BlueMaterial,
                    Transform = this.Model3Transform,
                    Instances = new List<Matrix>{Matrix.Identity},
                    DragZ = false,
                },
                new Shape3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.RedMaterial,
                    Transform = this.Model1Transform,
                    Instances = new List<Matrix>{Matrix.Identity},
                    DragZ = true,
                },
            };

            this.Element3DCollection = new ObservableCollection<Element3D>()
            {
                new DraggableGeometryModel3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.BlueMaterial,
                    Transform = this.Model3Transform,
                },
                                
                new DraggableGeometryModel3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.RedMaterial,
                    Transform = this.Model1Transform,
                },
            };

            this.AddCmd = new RelayCommand((o) => AddShape());
            this.DelCmd = new RelayCommand((o) => DelShape());
        }


        public void AddShape()
        {
            this.Element3DCollection.Add(
                new DraggableGeometryModel3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.GreenMaterial,
                    Transform = this.Model2Transform,
                    Instances = new List<Matrix> { 
                        Matrix.Translation(-1,0,0), Matrix.Translation(+1,0,0), 
                        Matrix.Translation(0,-1,0), Matrix.Translation(0,+1,0),
                        Matrix.Translation(0,0,-1), Matrix.Translation(0,0,+1), 
                    },
                });

            var shape = new Shape3D()
            {
                Geometry = this.MeshGeometry,
                Material = this.GreenMaterial,
                Transform = this.Model2Transform,
            };
            this.Shape3DCollection.Add(shape);
        }

        public void DelShape()
        {
            //this.Element3DCollection = null;
            //this.Element3DCollection = new ObservableCollection<Element3D>();
            this.Element3DCollection.Remove((Element3D)SelectedItem);

            //this.Shape3DCollection = null;
            //this.Shape3DCollection = new ObservableCollection<Shape3D>();
            this.Shape3DCollection.Remove((Shape3D)SelectedItem);
        }

       

        public class Shape3D : BaseViewModel
        {
            public Geometry3D Geometry { get; set; }
            public System.Windows.Media.Media3D.Transform3D Transform { get; set; }
            public Material Material   { get; set; }
            public IList<Matrix> Instances { get; set; }
            public bool IsSelected { get; set; }
            public bool DragZ { get; set; }
        }

        public IList<Shape3D> Shape3DCollection { get; set; }
        public IList<Element3D> Element3DCollection { get; set; }
        public object SelectedItem { get; set; }
        
    }
}