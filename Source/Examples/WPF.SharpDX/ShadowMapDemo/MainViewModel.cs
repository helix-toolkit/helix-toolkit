// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ShadowMapDemo
{
    using DemoCore;
    using HelixToolkit.Mathematics;
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Numerics;
    using System.Windows.Media.Animation;
    using Matrix = System.Numerics.Matrix4x4;
    using Media = System.Windows.Media;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using System.Diagnostics;
    using System.Windows;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }        
        public MeshGeometry3D LightCameraModel { private set; get; }
        public MeshGeometry3D Plane { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public Matrix[] Instances { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public PhongMaterial GrayMaterial { get; private set; }

        public PhongMaterial LightCameraMaterial { get; private set; } = new PhongMaterial() { EmissiveColor = Color.Yellow };
        public Media.Color GridColor { get; private set; }

        public Media3D.Transform3D Model1Transform { get; private set; }
        public Media3D.Transform3D Model2Transform { get; private set; }
        public Media3D.Transform3D Model3Transform { get; private set; }
        public Media3D.Transform3D GridTransform { get; private set; }
        public Media3D.Transform3D PlaneTransform { get; private set; }
        public Media3D.Transform3DGroup LightCameraTransform { get; private set; } = new Media3D.Transform3DGroup();
        public Media3D.Transform3D LightDirectionTransform { get; set; }
        //public Vector3 DirectionalLightDirection { get; private set; }
        public Media.Color DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }
        public Size ShadowMapResolution { get; private set; }

        public double XValue { get { return this.xvalue; } set { this.SetXValue(value); } }
        public ProjectionCamera Camera1 { private set; get; }
        //public Camera Camera2 { private set; get; }

        public MainViewModel()
        {

            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            Title = "Shadow Map Demo";
            SubTitle = "WPF & SharpDX";

            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = Media.Colors.White;
            //this.DirectionalLightDirection = new Vector3(-1, -1, -1);
           // this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
            this.ShadowMapResolution = new Size(2048, 2048);

            // camera setup
            this.Camera = new PerspectiveCamera { Position =new Point3D(0,1,1), LookDirection = new Vector3D(0,-1,-1), UpDirection = new Vector3D(0, 1, 0) };
            Camera1 = new PerspectiveCamera { Position = new Point3D(0,5,0), LookDirection = new Vector3D(0,-1,0), UpDirection = new Vector3D(1, 0, 0) };

            // scene model3d
            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.25, 2, BoxFaces.All);
            Model = b1.ToMeshGeometry3D();
            Instances = new[] { MatrixHelper.Translation(0, 0, -1.5f), MatrixHelper.Translation(0, 0, 1.5f) };

            var b2 = new MeshBuilder();
            b2.AddBox(new Vector3(0, 0, 0), 10, 0, 10, BoxFaces.PositiveY);
            Plane = b2.ToMeshGeometry3D();
            PlaneTransform = new Media3D.TranslateTransform3D(-0, -2, -0);
            GrayMaterial = PhongMaterials.Indigo;

            // lines model3d            
            Lines = LineBuilder.GenerateBoundingBox(Model);
            //this.PropertyChanged += MainViewModel_PropertyChanged;
            // model trafos
            Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);            

            // model materials
            RedMaterial = PhongMaterials.Glass;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;
            GrayMaterial.RenderShadowMap = RedMaterial.RenderShadowMap = GreenMaterial.RenderShadowMap = BlueMaterial.RenderShadowMap = true;
            //var b3 = new MeshBuilder();
            //b3.AddBox(new Vector3(), 0.3f, 0.3f, 0.3f, BoxFaces.All);
            //b3.AddCone(new Vector3(0, 0.3f, 0), new Vector3(0, 0f, 0), 0.2f, true, 24);
            //LightCameraModel = b3.ToMesh();           
            //LightCameraTransform.Children.Add(new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(1, 0, 0), -135)));
            //LightCameraTransform.Children.Add(new Media3D.TranslateTransform3D(0, 3, 3));
            //UpdateCamera();
        }

        //private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName.Equals(nameof(LightCameraTransform)))
        //    {
        //        UpdateCamera();
        //    }
        //}

        //private void UpdateCamera()
        //{
        //    var m = LightCameraTransform.ToMatrix();
        //    var v = new Vector3(m.M21, m.M22, m.M23);
        //    Camera1.LookDirection = v.Normalized().ToVector3D();
        //    Camera1.Position = new Point3D(m.M41, m.M42, m.M43);
        //}

        private void SetXValue(double x)
        {
            Console.WriteLine("x: {0}", x);
            this.xvalue = x;
            //this.DirectionalLightDirection = new Vector3D(x, -10, -10);
            this.LightDirectionTransform = new Media3D.TranslateTransform3D(x, -10, 10);
        }
        private double xvalue;

        private Media3D.Transform3D CreateAnimatedTransform(Vector3D translate, Vector3D axis, double speed = 4)
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
    }
}