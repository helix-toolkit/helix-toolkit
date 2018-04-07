using GalaSoft.MvvmLight;
using HelixToolkit.Logger;
using HelixToolkit.UWP;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleDemoW10
{
    public class MainPageViewModel : ObservableObject
    {

        public Geometry3D Geometry { private set; get; }

        public Geometry3D LineGeometry { private set; get; }

        public Geometry3D PointGeometry { private set; get; }

        public Geometry3D FloorModel { private set; get; }

        public BillboardText3D AxisLabelGeometry { private set; get; }

        public PhongMaterial Material { private set; get; }

        public PhongMaterial FloorMaterial { private set; get; }

        public IEffectsManager EffectsManager { private set; get; }

        public PerspectiveCamera Camera { private set; get; }

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

        private Matrix transform4 = Matrix.Translation(-3, 4, 3);
        public Matrix Transform4
        {
            set
            {
                Set(ref transform4, value);
            }
            get { return transform4; }
        }

        public Vector3 DirectionalLightDirection { get; } = new Vector3(-0.5f, -1, 0);
        private DispatcherTimer timer;

        private float scale = 1;
        private float rotationSpeed = 1;

        public MainPageViewModel()
        {
            EffectsManager = new DefaultEffectsManager(new Logger());

            Camera = new PerspectiveCamera() { Position = new Vector3(0, 0, -15), LookDirection = new Vector3(0, 0, 15), UpDirection = new Vector3(0, 1, 0) };

            var builder = new MeshBuilder(true, true, true);
            builder.AddBox(new SharpDX.Vector3(0, 0, 0), 2, 2, 2);
            builder.AddSphere(new Vector3(0, 2, 0), 1.5);
            Geometry = builder.ToMesh();
            Material = new PhongMaterial()
            {
                AmbientColor = Color.Gray,
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Color.White,
                SpecularShininess = 100f,
            };
            Material.DiffuseMap = LoadTexture("TextureCheckerboard2.jpg");
            Material.NormalMap = LoadTexture("TextureCheckerboard2_dot3.jpg");
            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(Vector3.Zero, new Vector3(5, 0, 0));
            lineBuilder.AddLine(Vector3.Zero, new Vector3(0, 5, 0));
            lineBuilder.AddLine(Vector3.Zero, new Vector3(0, 0, 5));
            LineGeometry = lineBuilder.ToLineGeometry3D();
            LineGeometry.Colors = new HelixToolkit.UWP.Core.Color4Collection() { Color.Red, Color.Red, Color.Green, Color.Green, Color.Blue, Color.Blue };

            builder = new MeshBuilder();
            builder.AddSphere(new Vector3(), 3);
            var mesh = builder.ToMesh();
            PointGeometry = new PointGeometry3D() { Positions = mesh.Positions };

            AxisLabelGeometry = new BillboardText3D();
            AxisLabelGeometry.TextInfo.Add(new TextInfo("X", new Vector3(5.5f, 0, 0)) { Foreground = Color.Red });
            AxisLabelGeometry.TextInfo.Add(new TextInfo("Y", new Vector3(0, 5.5f, 0)) { Foreground = Color.Green });
            AxisLabelGeometry.TextInfo.Add(new TextInfo("Z", new Vector3(0, 0, 5.5f)) { Foreground = Color.Blue });

            builder = new MeshBuilder();
            builder.AddBox(new Vector3(0, -6, 0), 30, 0.5, 30);
            FloorModel = builder.ToMesh();

            FloorMaterial = PhongMaterials.LightGray;

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 16);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var time = (float)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
            Transform = global::SharpDX.Matrix.Scaling((float)this.scale) * global::SharpDX.Matrix.RotationX(rotationSpeed * time)
                    * global::SharpDX.Matrix.RotationY(rotationSpeed * time * 2.0f) * global::SharpDX.Matrix.RotationZ(rotationSpeed * time * .7f);
            Transform1 = global::SharpDX.Matrix.Scaling((float)this.scale) * global::SharpDX.Matrix.RotationX(-rotationSpeed * time * .7f)
                * global::SharpDX.Matrix.RotationY(-rotationSpeed * time * 1.0f) * global::SharpDX.Matrix.RotationZ(rotationSpeed * time) * Matrix.Translation(3, -3, -3);
            Transform2 = global::SharpDX.Matrix.Scaling((float)this.scale) * global::SharpDX.Matrix.RotationX(-rotationSpeed * time * -.7f)
                    * global::SharpDX.Matrix.RotationY(-rotationSpeed * time * 1.0f) * global::SharpDX.Matrix.RotationZ(rotationSpeed * time) * Matrix.Translation(3, 3, 3);
            Transform3 = global::SharpDX.Matrix.Scaling((float)this.scale) * global::SharpDX.Matrix.RotationX(-rotationSpeed * time * .7f)
                    * global::SharpDX.Matrix.RotationY(-rotationSpeed * time * -0.5f) * global::SharpDX.Matrix.RotationZ(rotationSpeed * time) * Matrix.Translation(-5, -5, 5);
        }

        private Stream LoadTexture(string file)
        {
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "");
            var bytecode = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\" + file);
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
    }
}
