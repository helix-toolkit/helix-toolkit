namespace CrossSectionDemo
{
    using System;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
   // using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Color = System.Windows.Media.Color;
    using System.Numerics;
    using HelixToolkit.Mathematics;
    using Colors = System.Windows.Media.Colors;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows;
    using System.Windows.Threading;

    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public MainViewModel ViewModel { get { return this; } }
        public MeshGeometry3D Model { get; private set; }

        public MeshGeometry3D FloorModel { private set; get; }
        public PhongMaterial FloorMaterial { private set; get; }
        public Transform3D ModelTransform { get; private set; }

        public MeshGeometry3D Plane1Model { private set; get; }

        public MeshGeometry3D Plane2Model { private set; get; }
        public TranslateTransform3D Plane1Transform { private set; get; }

        public TranslateTransform3D Plane2Transform { private set; get; }

        public PhongMaterial PlaneMaterial { private set; get; }

        public PhongMaterial ModelMaterial { get; set; }

        public PhongMaterial LightModelMaterial { get; set; }

        public Color Light1Color { get; set; }

        public bool EnablePlane1 { set; get; } = true;
        public Plane Plane1 { set; get; } = new Plane(new Vector3(0, -1, 0), -8);
        private float plane1Factor = 0.05f;

        public bool EnablePlane2 { set; get; } = true;
        public Plane Plane2 { set; get; } = new Plane(new Vector3(-1, 0, 0), -8);
        private float plane2Factor = 0.05f;
        private int cuttingOperationIndex;
        public int CuttingOperationIndex
        {
            set
            {
                if (SetValue(ref cuttingOperationIndex, value))
                {
                    CuttingOperation = (CuttingOperation)value;
                }
            }
            get { return cuttingOperationIndex; }
        }

        public CuttingOperation CuttingOperation { set; get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            // ----------------------------------------------
            // titles
            this.Title = "SwapChain Top Surface Rendering Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera
            {
                Position = new Point3D(20, 20, 20),
                LookDirection = new Vector3D(-20, -20, -20),
                UpDirection = new Vector3D(0, 1, 0)
            };
            // ----------------------------------------------
            // setup scene

            this.Light1Color = Colors.White;


            var builder = new MeshBuilder(true, false, false);
            builder.AddBox(new Vector3(), 40, 0.1, 40);
            Plane1Model = FloorModel = builder.ToMeshGeometry3D();

            builder = new MeshBuilder(true, false, false);
            builder.AddBox(new Vector3(), 0.1, 40, 40);
            Plane2Model = builder.ToMeshGeometry3D();

            FloorMaterial = new PhongMaterial();
            FloorMaterial.DiffuseColor = new Color4(1f, 1f, 1f, 0.2f);
            FloorMaterial.AmbientColor = new Color4(0, 0, 0, 0);
            FloorMaterial.ReflectiveColor = new Color4(0, 0, 0, 0);
            FloorMaterial.SpecularColor = new Color4(0, 0, 0, 0);

            PlaneMaterial = new PhongMaterial() { DiffuseColor = new Color4(0.1f, 0.1f, 0.8f, 0.2f) };

            var landerItems = Load3ds("Car.3ds").Select(x => x.Geometry as MeshGeometry3D).ToArray();
            Model = MeshGeometry3D.Merge(landerItems);
            Model.UpdateOctree();
            ModelMaterial = PhongMaterials.Bronze;
            var transGroup = new Media3D.Transform3DGroup();
            transGroup.Children.Add(new Media3D.ScaleTransform3D(0.01, 0.01, 0.01));
            transGroup.Children.Add(new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Media3D.Vector3D(1, 0, 0), -90)));
            transGroup.Children.Add(new Media3D.TranslateTransform3D(new Media3D.Vector3D(0, 6, 0)));

            ModelTransform = transGroup;

            Plane1Transform = new TranslateTransform3D(new Vector3D(0, 15, 0));
            Plane2Transform = new TranslateTransform3D(new Vector3D(15, 0, 0));
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


        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }
    }

}