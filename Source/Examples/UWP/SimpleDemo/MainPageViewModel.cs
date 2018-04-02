using GalaSoft.MvvmLight;
using HelixToolkit.UWP;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleDemoW10
{
    public class MainPageViewModel : ObservableObject
    {

        public Geometry3D Geometry { private set; get; }

        public PhongMaterial Material { private set; get; }

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

        private DispatcherTimer timer;

        private float scale = 1;
        private float rotationSpeed = 1;

        public MainPageViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            Camera = new PerspectiveCamera() { Position = new Vector3(0, 0, -10), LookDirection = new Vector3(0, 0, 10), UpDirection = new Vector3(0, 1, 0) };

            var builder = new MeshBuilder();
            builder.AddBox(new SharpDX.Vector3(0, 0, 0), 2, 2, 2);
            Geometry = builder.ToMesh();
            Material = new PhongMaterial()
            {
                DiffuseColor = Color.Red,
            };

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var time = (float)Stopwatch.GetTimestamp() / Stopwatch.Frequency;
            Transform = global::SharpDX.Matrix.Scaling((float)this.scale) * global::SharpDX.Matrix.RotationX(rotationSpeed * time)
                    * global::SharpDX.Matrix.RotationY(rotationSpeed * time * 2.0f) * global::SharpDX.Matrix.RotationZ(rotationSpeed * time * .7f);
        }
    }
}
