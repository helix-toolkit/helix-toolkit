using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HelixToolkit.Logger;
using HelixToolkit.Mathematics;
using HelixToolkit.UWP;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Matrix = System.Numerics.Matrix4x4;

namespace SimpleDemoW10
{
    public class MainPageViewModel : ObservableObject
    {
        public ParticleViewModel ParticleVM { get; } = new ParticleViewModel();

        public OITDemoViewModel OITVM { get; } = new OITDemoViewModel();

        private Vector3 upDirection = Vector3.UnitY;
        public Vector3 UpDirection {
            private set
            {
                if(Set(ref upDirection, value))
                {
                    ResetCamera();
                }
            }
            get { return upDirection; }
        }
        public Geometry3D Sphere { private set; get; }
        public Geometry3D Geometry { private set; get; }

        public Geometry3D LineGeometry { private set; get; }

        public Geometry3D PointGeometry { private set; get; }

        public Geometry3D FloorModel { private set; get; }

        public BillboardText3D AxisLabelGeometry { private set; get; }

        public PhongMaterial Material { private set; get; }
        public PhongMaterial Material1 { private set; get; }
        public PhongMaterial FloorMaterial { private set; get; }

        public IEffectsManager EffectsManager { private set; get; }

        public Camera Camera { private set; get; }

        public Camera Camera1 { private set; get; }

        private Matrix transform = Matrix.Identity;
        public Matrix Transform
        {
            set
            {
                Set(ref transform, value);
            }
            get { return transform; }
        }
        private Matrix transform1 = Matrix.Identity;
        public Matrix Transform1
        {
            set
            {
                Set(ref transform1, value);
            }
            get { return transform1; }
        }

        private Matrix transform2 = Matrix.Identity;
        public Matrix Transform2
        {
            set
            {
                Set(ref transform2, value);
            }
            get { return transform2; }
        }

        private Matrix transform3 = Matrix.Identity;
        public Matrix Transform3
        {
            set
            {
                Set(ref transform3, value);
            }
            get { return transform3; }
        }

        private Matrix transform4 = Matrix.CreateTranslation(-3, 4, 3);
        public Matrix Transform4
        {
            set
            {
                Set(ref transform4, value);
            }
            get { return transform4; }
        }

        public Stream EnvironmentMap { private set; get; }

        public Vector3 DirectionalLightDirection { get; } = new Vector3(-0.5f, -1, 0);
        private DispatcherTimer timer;

        private float scale = 1;
        private float rotationSpeed = 1;

        #region Commands
        public ICommand UpDirXCommand { private set; get; }
        public ICommand UpDirYCommand { private set; get; }
        public ICommand UpDirZCommand { private set; get; }
        #endregion

        public MainPageViewModel()
        {
            EffectsManager = new DefaultEffectsManager(new Logger());

            Camera = new PerspectiveCamera() { Position = new Vector3(40, 10, 100), LookDirection = new Vector3(0, -10, -100), UpDirection = UpDirection, FarPlaneDistance = 500, NearPlaneDistance = 0.1 };
            Camera1 = new OrthographicCamera() { Position = new Vector3(60, 10, 100), LookDirection = new Vector3(0, -10, -100), UpDirection = upDirection, Width = 30, FarPlaneDistance = 2000, NearPlaneDistance = 20};
            var builder = new MeshBuilder(true, true, true);
            builder.AddBox(new Vector3(0, 0, 0), 2, 2, 2);
            builder.AddSphere(new Vector3(0, 2, 0), 1.5);
            Geometry = builder.ToMesh();
            Geometry.UpdateOctree();
            builder = new MeshBuilder();
            builder.AddSphere(new Vector3(0, 2, 0), 2);
            Sphere = builder.ToMesh();
            Sphere.UpdateOctree();

            Material = new PhongMaterial()
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 10f,
                ReflectiveColor = new Color4(0.2f, 0.2f, 0.2f, 0.5f),
                RenderEnvironmentMap= true
            };
            Material.DiffuseMap = LoadTexture("TextureCheckerboard2.jpg");
            Material.NormalMap = LoadTexture("TextureCheckerboard2_dot3.jpg");
            Material1 = Material.CloneMaterial();
            Material1.ReflectiveColor = Color.Silver;
            Material1.RenderDiffuseMap = false;
            Material1.RenderNormalMap = false;
            Material1.RenderEnvironmentMap = true;
           
            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(Vector3.Zero, new Vector3(5, 0, 0));
            lineBuilder.AddLine(Vector3.Zero, new Vector3(0, 5, 0));
            lineBuilder.AddLine(Vector3.Zero, new Vector3(0, 0, 5));
            LineGeometry = lineBuilder.ToLineGeometry3D();
            LineGeometry.Colors = new HelixToolkit.UWP.Core.Color4Collection() { Color.Red, Color.Red, Color.Green, Color.Green, Color.Blue, Color.Blue };

            builder = new MeshBuilder();
            builder.AddSphere(new Vector3(), 3);
            var mesh = builder.ToMesh();
            mesh.UpdateOctree();
            PointGeometry = new PointGeometry3D() { Positions = mesh.Positions };

            AxisLabelGeometry = new BillboardText3D();
            AxisLabelGeometry.TextInfo.Add(new TextInfo("X", new Vector3(5.5f, 0, 0)) { Foreground = Color.Red });
            AxisLabelGeometry.TextInfo.Add(new TextInfo("Y", new Vector3(0, 5.5f, 0)) { Foreground = Color.Green });
            AxisLabelGeometry.TextInfo.Add(new TextInfo("Z", new Vector3(0, 0, 5.5f)) { Foreground = Color.Blue });

            builder = new MeshBuilder();
            builder.AddBox(new Vector3(0, -6, 0), 30, 0.5, 30);
            FloorModel = builder.ToMesh();

            FloorMaterial = PhongMaterials.Obsidian;
            FloorMaterial.ReflectiveColor = Color.Silver;

            EnvironmentMap = LoadTexture("Cubemap_Grandcanyon.dds");

            UpDirXCommand = new RelayCommand(() => { UpDirection = Vector3.UnitX; }, ()=> { return UpDirection != Vector3.UnitX; });
            UpDirYCommand = new RelayCommand(() => { UpDirection = Vector3.UnitY; }, () => { return UpDirection != Vector3.UnitY; });
            UpDirZCommand = new RelayCommand(() => { UpDirection = Vector3.UnitZ; }, () => { return UpDirection != Vector3.UnitZ; });

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var time = (float)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
            Transform = MatrixHelper.Scaling((float)this.scale) * MatrixHelper.RotationX(rotationSpeed * time)
                    * MatrixHelper.RotationY(rotationSpeed * time * 2.0f) * MatrixHelper.RotationZ(rotationSpeed * time * .7f);
            Transform1 = MatrixHelper.Scaling((float)this.scale) * MatrixHelper.RotationX(-rotationSpeed * time * .7f)
                * MatrixHelper.RotationY(-rotationSpeed * time * 1.0f) * MatrixHelper.RotationZ(rotationSpeed * time) * MatrixHelper.Translation(3, -3, -3);
            Transform2 = MatrixHelper.Scaling((float)this.scale) * MatrixHelper.RotationX(-rotationSpeed * time * -.7f)
                    * MatrixHelper.RotationY(-rotationSpeed * time * 1.0f) * MatrixHelper.RotationZ(rotationSpeed * time) * MatrixHelper.Translation(3, 3, 3);
            Transform3 = MatrixHelper.Scaling((float)this.scale) * MatrixHelper.RotationX(-rotationSpeed * time * .7f)
                    * MatrixHelper.RotationY(-rotationSpeed * time * -0.5f) * MatrixHelper.RotationZ(rotationSpeed * time) * MatrixHelper.Translation(-5, -5, 5);
        }

        private Stream LoadTexture(string file)
        {
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "");
            var bytecode = SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\" + file);
            return new MemoryStream(bytecode);
        }

        public class Logger : ILogger
        {
            public void Log<MsgType>(LogLevel logLevel, MsgType msg, string className, string methodName, int lineNumber)
            {
                switch (logLevel)
                {
                    case LogLevel.Warning:
                    case LogLevel.Error:
                        Console.WriteLine($"Level:{logLevel}; Msg:{msg}");
                        break;
                }
            }
        }

        private void ResetCamera()
        {
            Camera.UpDirection = UpDirection;
            if(UpDirection == Vector3.UnitY || UpDirection == Vector3.UnitX)
            {
                Camera.Position = new Vector3(0, 0, -15);
                Camera.LookDirection = new Vector3(0, 0, 15);   
            }
            else
            {
                Camera.Position = new Vector3(0, -15, 0);
                Camera.LookDirection = new Vector3(0, 15, 0);
            }
        }
    }
}
