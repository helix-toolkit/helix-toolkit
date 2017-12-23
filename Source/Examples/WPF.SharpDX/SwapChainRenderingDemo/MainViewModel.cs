namespace SwapChainRenderingDemo
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
    using System.Diagnostics;

    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public MainViewModel ViewModel { get { return this; } }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Floor { get; private set; }
        public MeshGeometry3D Sphere { get; private set; }
        public LineGeometry3D CubeEdges { get; private set; }
        public Transform3D ModelTransform { get; private set; }
        public Transform3D FloorTransform { get; private set; }
        public Transform3D Light1Transform { get; private set; }
        public Transform3D Light2Transform { get; private set; }
        public Transform3D Light3Transform { get; private set; }
        public Transform3D Light4Transform { get; private set; }
        public Transform3D Light1DirectionTransform { get; private set; }
        public Transform3D Light4DirectionTransform { get; private set; }

        public PhongMaterial ModelMaterial { get; set; }
        public PhongMaterial FloorMaterial { get; set; }
        public PhongMaterial LightModelMaterial { get; set; }

        public Vector3 Light1Direction { get; set; }
        public Vector3 Light4Direction { get; set; }
        public Vector3D LightDirection4 { get; set; }
        public Color4 Light1Color { get; set; }
        public Color4 Light2Color { get; set; }
        public Color4 Light3Color { get; set; }
        public Color4 Light4Color { get; set; }
        public Color4 AmbientLightColor { get; set; }
        public Vector3 Light2Attenuation { get; set; }
        public Vector3 Light3Attenuation { get; set; }
        public Vector3 Light4Attenuation { get; set; }
        public bool RenderLight1 { get; set; }
        public bool RenderLight2 { get; set; }
        public bool RenderLight3 { get; set; }
        public bool RenderLight4 { get; set; }

        public bool RenderDiffuseMap { set; get; } = true;

        public bool RenderNormalMap { set; get; } = true;


        private string selectedDiffuseTexture = @"TextureCheckerboard2.jpg";
        public string SelectedDiffuseTexture
        {
            get
            {
                return selectedDiffuseTexture;
            }
        }

        private string selectedNormalTexture = @"TextureCheckerboard2_dot3.jpg";
        public string SelectedNormalTexture
        {
            get
            {
                return selectedNormalTexture;
            }
        }

        public System.Windows.Media.Color DiffuseColor
        {
            set
            {
                FloorMaterial.DiffuseColor = ModelMaterial.DiffuseColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.DiffuseColor.ToColor();
            }
        }


        public System.Windows.Media.Color ReflectiveColor
        {
            set
            {
                FloorMaterial.ReflectiveColor = ModelMaterial.ReflectiveColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.ReflectiveColor.ToColor();
            }
        }

        public System.Windows.Media.Color EmissiveColor
        {
            set
            {
                FloorMaterial.EmissiveColor = ModelMaterial.EmissiveColor = value.ToColor4();
            }
            get
            {
                return ModelMaterial.EmissiveColor.ToColor();
            }
        }

        public Camera Camera2 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera3 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera4 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };
        public FillMode FillMode { set; get; } = FillMode.Solid;

        public int NumberOfTriangles { set; get; } = 0;
        public int NumberOfVertices { set; get; } = 0;
        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                if (SetValue(ref showWireframe, value))
                {
                    FillMode = value ? FillMode.Wireframe : FillMode.Solid;
                }
            }
            get
            {
                return showWireframe;
            }
        }
        public LineGeometry3D LineGeo { set; get; }
        public MainViewModel()
        {
            EffectsManager = new DefaultShaderTechniqueManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            

            // ----------------------------------------------
            // titles
            this.Title = "SwapChain Top Surface Rendering Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(100,100,100), LookDirection = new Vector3D(-100,-100,-100), UpDirection = new Vector3D(0, 1, 0) };

            // ----------------------------------------------
            // setup scene
            this.AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);

            this.Light1Color = (Color4)Color.White;
            this.Light2Color = (Color4)Color.Red;
            this.Light3Color = (Color4)Color.LightYellow;
            this.Light4Color = (Color4)Color.LightBlue;

            this.Light2Attenuation = new Vector3(0.1f, 0.05f, 0.010f);
            this.Light3Attenuation = new Vector3(0.1f, 0.01f, 0.005f);
            this.Light4Attenuation = new Vector3(0.1f, 0.02f, 0.0f);

            this.Light1Direction = new Vector3(0, -10, -10);
            this.Light1Transform = new TranslateTransform3D(-Light1Direction.ToVector3D());
            this.Light1DirectionTransform = CreateAnimatedTransform2(-Light1Direction.ToVector3D(), new Vector3D(0, 1, -1), 36);

            this.Light2Transform = CreateAnimatedTransform1(new Vector3D(-100, 50, 0), new Vector3D(0, 0, 1), 3);
            this.Light3Transform = CreateAnimatedTransform1(new Vector3D(0, 50, 100), new Vector3D(0, 1, 0), 5);

            this.Light4Direction = new Vector3(0, -100, 0);
            this.Light4Transform = new TranslateTransform3D(-Light4Direction.ToVector3D());
            this.Light4DirectionTransform = CreateAnimatedTransform2(-Light4Direction.ToVector3D(), new Vector3D(1, 0, 0), 48);

            // ----------------------------------------------
            // light model3d
            var sphere = new MeshBuilder();
            sphere.AddSphere(new Vector3(0, 0, 0), 4);
            Sphere = sphere.ToMeshGeometry3D();
            this.LightModelMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = Color.Gray,
                EmissiveColor = Color.Yellow,
                SpecularColor = Color.Black,
            };
            var models = Load3ds("wall12.obj").Select(x => x.Geometry as MeshGeometry3D).ToArray();
            Floor = models[0];
            Floor.UpdateOctree();
            this.FloorTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.FloorMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f
            };

            var landerItems = Load3ds("Car.3ds").Select(x => x.Geometry as MeshGeometry3D).ToArray();
            Model = MeshGeometry3D.Merge(landerItems);
            Model.UpdateOctree();
            ModelMaterial = PhongMaterials.BlackRubber;
            var transGroup = new Media3D.Transform3DGroup();
            transGroup.Children.Add(new Media3D.ScaleTransform3D(0.04, 0.04, 0.04));
            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(new Vector3D(0, 1, 0), 90),
                Duration = TimeSpan.FromSeconds(4),
                IsCumulative = true,
            };
            var rotateTransform = new Media3D.RotateTransform3D();
            transGroup.Children.Add(rotateTransform);
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            transGroup.Children.Add(new Media3D.TranslateTransform3D(0, 60, 0));
            ModelTransform = transGroup;
            NumberOfTriangles = Floor.Indices.Count / 3 + Model.Indices.Count/3;
            NumberOfVertices = Floor.Positions.Count + Model.Positions.Count;
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

        private Media3D.Transform3D CreateAnimatedTransform1(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 90),
                Duration = TimeSpan.FromSeconds(speed / 4),
                IsCumulative = true,
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);

            return lightTrafo;
        }

        private Media3D.Transform3D CreateAnimatedTransform2(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                //By = new Media3D.AxisAngleRotation3D(axis, 180),
                From = new Media3D.AxisAngleRotation3D(axis, 135),
                To = new Media3D.AxisAngleRotation3D(axis, 225),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed / 4),
                //IsCumulative = true,                  
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);
            return lightTrafo;
        }

        public void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewport = sender as Viewport3DX;
            if (viewport == null) { return; }
            var point = e.GetPosition(viewport);
            var watch = Stopwatch.StartNew();
            var hitTests = viewport.FindHits(point);
            watch.Stop();
            Console.WriteLine("Hit test time =" + watch.ElapsedMilliseconds);
            if (hitTests.Count > 0)
            {
                var lineBuilder = new LineBuilder();
                foreach(var hit in hitTests)
                {
                    lineBuilder.AddLine(hit.PointHit.ToVector3(), (hit.PointHit + hit.NormalAtHit * 10).ToVector3());
                }
                LineGeo = lineBuilder.ToLineGeometry3D();
            }
        }
    }

}