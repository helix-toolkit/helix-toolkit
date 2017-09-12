namespace CrossSectionDemo
{
    using System;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using HelixToolkit.Wpf;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using SharpDX.Direct3D11;
    using System.Windows.Data;
    using System.Windows;

    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public MainViewModel ViewModel { get { return this; } }
        public MeshGeometry3D Model { get; private set; }
        public Transform3D ModelTransform { get; private set; }
        public PhongMaterial ModelMaterial { get; set; }

        public PhongMaterial LightModelMaterial { get; set; }

        public Vector3 Light1Direction { get; set; }
        public Color4 Light1Color { get; set; }
        private Media3D.Vector3D camLookDir = new Media3D.Vector3D(-10, -10, -10);
        public Media3D.Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                    Light1Direction = camLookDir.ToVector3();
                }
            }
            get
            {
                return camLookDir;
            }
        }

        public bool EnablePlane1 { set; get; } = true;
        public Plane Plane1 { set; get; } = new Plane(new Vector3(0, -1, 0), 0);

        public bool EnablePlane2 { set; get; } = true;
        public Plane Plane2 { set; get; } = new Plane(new Vector3(-1, 0, 0), 0);

        public bool EnablePlane3 { set; get; } = false;
        public Plane Plane3 { set; get; } = new Plane(new Vector3(0, 1, 1), 0);

        public bool EnablePlane4 { set; get; } = false;
        public Plane Plane4 { set; get; } = new Plane(new Vector3(0, 1, 0), 0);
        public MainViewModel()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            // ----------------------------------------------
            // titles
            this.Title = "SwapChain Top Surface Rendering Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new OrthographicCamera { Position = new Point3D(100,100,100), LookDirection = new Vector3D(-100,-100,-100),
                UpDirection = new Vector3D(0, 1, 0) };
            (Camera as OrthographicCamera).FarPlaneDistance = 10000;
            this.Light1Direction = new Vector3(-100, -100, -100);
            SetupCameraBindings(this.Camera);
            // ----------------------------------------------
            // setup scene

            this.Light1Color = (Color4)Color.White;

            
           // var models = Load3ds("wall12.obj").Select(x => x.Geometry as MeshGeometry3D).ToArray();

            var landerItems = Load3ds("Car.3ds").Select(x => x.Geometry as MeshGeometry3D).ToArray();
            Model = MeshGeometry3D.Merge(landerItems);
            ModelMaterial = PhongMaterials.BlackRubber;
            var transGroup = new Media3D.Transform3DGroup();
            transGroup.Children.Add(new Media3D.ScaleTransform3D(0.01, 0.01, 0.01));
            //transGroup.Children.Add(new Media3D.TranslateTransform3D(0, 60, 0));
            ModelTransform = transGroup;
        }

        public List<Object3D> Load3ds(string path)
        {
            if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
            {
                var reader = new ObjReader();
                var list = reader.Read(path);
                return list;
            }
            else if(path.EndsWith(".3ds", StringComparison.CurrentCultureIgnoreCase))
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
        public void SetupCameraBindings(Camera camera)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding("CamLookDir", camera, ProjectionCamera.LookDirectionProperty, this);
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