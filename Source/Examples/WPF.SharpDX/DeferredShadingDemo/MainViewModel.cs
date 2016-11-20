// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Constructor of the MainViewModel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DeferredShadingDemo
{
    using DemoCore;
    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Extensions;

    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using RotateTransform3D = System.Windows.Media.Media3D.RotateTransform3D;
    using ScaleTransform3D = System.Windows.Media.Media3D.ScaleTransform3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;


    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Plane { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public PhongMaterial PlaneMaterial { get; private set; }

        public Transform3D Model1Transform { get; private set; }
        public Transform3D Model2Transform { get; private set; }
        public Transform3D Model3Transform { get; private set; }
        public Transform3D PlaneTransform { get; private set; }

        public Color4 AmbientLightColor { get; private set; }
        public Color4 DirectionalLightColor { get; set; }
        public Vector3 DirectionalLightDirection { get; set; }

        public Transform3D PointLightTransform1 { get; private set; }
        public Transform3D PointLightTransform2 { get; private set; }
        public Transform3D PointLightTransform3 { get; private set; }

        public Element3DCollection PointLightCollection { get; set; }
        public Color4 PointLightColor
        {
            get { return this.pointLightColor; }
            set { this.pointLightColor = value; this.UpdatePointLightCollection(); }
        }
        public Vector3 PointLightAttenuation
        {
            get { return this.pointLightAttenuation; }
            set { this.pointLightAttenuation = value; this.UpdatePointLightCollection(); }
        }
        public int PointLightCount
        {
            get { return this.PointLightCollection.Count; }
            set { this.InitPointLightCollection(value); }
        }
        public int PointLightSpread
        {
            get { return this.pointLightSpread; }
            set { this.pointLightSpread = value; this.InitPointLightCollection(this.PointLightCount); }
        }

        public Element3DCollection SpotLightCollection { get; set; }
        public Color4 SpotLightColor
        {
            get { return this.spotLightColor; }
            set { this.spotLightColor = value; this.UpdateSpotLightCollection(); }
        }
        public Vector3 SpotLightAttenuation
        {
            get { return this.spotLightAttenuation; }
            set { this.spotLightAttenuation = value; this.UpdateSpotLightCollection(); }
        }
        public int SpotLightCount
        {
            get { return this.SpotLightCollection.Count; }
            set { this.InitSpotLightCollection(value); }
        }
        public double SpotLightSpread
        {
            get { return this.spotLightSpread; }
            set { this.spotLightSpread = value; this.InitSpotLightCollection(this.SpotLightCount); }
        }

        public IEnumerable<int> SamplesMSAA
        {
            get { yield return 1; yield return 2; yield return 4; yield return 8; }
        }

        /// <summary>
        /// Constructor of the MainViewModel
        /// </summary>
        public MainViewModel()
        {
            // titles
            this.Title = "Deferred Shading Demo";
            this.SubTitle = "WPF & SharpDX";

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(18, 64, 30), LookDirection = new Vector3D(-18, -64, -30), UpDirection = new Vector3D(0, 1, 0) };

            // deferred render technique
            RenderTechniquesManager = new DeferredTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred);
            EffectsManager = new DeferredEffectsManager(RenderTechniquesManager);

            //load model
            var reader = new ObjReader();
            var objModel = reader.Read(@"./Media/bunny.obj");            
            this.Model = objModel[0].Geometry as MeshGeometry3D;
            var scale = 2.0;

            // model trafos
            var transf1 = new Transform3DGroup();
            transf1.Children.Add(new ScaleTransform3D(scale, scale, scale));
            transf1.Children.Add(new RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(0, 1, 0), 40), 0.0, 0.0, 0.0));
            transf1.Children.Add(new TranslateTransform3D(0, -2, 3));
            this.Model1Transform = transf1;

            var transf2 = new Transform3DGroup();
            transf2.Children.Add(new ScaleTransform3D(scale, scale, scale));
            transf2.Children.Add(new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(0, 1, 0), 66), 0.0, 0.0, 0.0));
            transf2.Children.Add(new Media3D.TranslateTransform3D(-3.0, -2, -2.5));
            this.Model2Transform = transf2;

            var transf3 = new Transform3DGroup();
            transf3.Children.Add(new ScaleTransform3D(scale, scale, scale));
            transf3.Children.Add(new TranslateTransform3D(+3.5, -2, -1.0));
            this.Model3Transform = transf3;

            // floor plane
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Vector3(0, 0, 0), 100, 0.0, 100, BoxFaces.PositiveY);
            this.Plane = meshBuilder.ToMeshGeometry3D();            
            this.PlaneTransform = new TranslateTransform3D(0, -1.05, 0);

            // model materials
            this.RedMaterial = PhongMaterials.Red;
            this.GreenMaterial = PhongMaterials.Green;
            this.BlueMaterial = PhongMaterials.Blue;
            this.PlaneMaterial = PhongMaterials.DefaultVRML;
            this.PlaneMaterial.DiffuseMap = new BitmapImage(new System.Uri(@"./Media/TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute));
            this.PlaneMaterial.NormalMap = new BitmapImage(new System.Uri(@"./Media/TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute));

            // setup lighting            
            this.AmbientLightColor = new Color4(0.3f, 0.3f, 0.3f, 1.0f);
            this.DirectionalLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            this.DirectionalLightDirection = new Vector3(-2, -5, -2);

            this.PointLightColor = (Color4)Color.White;
            this.PointLightAttenuation = new Vector3(0.0f, 0.0f, 0.18f); //1/0/0 ; 0.1, 0.2, 0.3
            this.PointLightTransform1 = new TranslateTransform3D(new Vector3D(0, 1, 0));
            this.PointLightTransform2 = new TranslateTransform3D(new Vector3D(6, 1, 3));
            this.PointLightTransform3 = new TranslateTransform3D(new Vector3D(-3, 1, -6));

            this.SpotLightColor = (Color4)Color.AntiqueWhite;
            this.SpotLightAttenuation = new Vector3(1.0f, 0.1f, 0.01f);

            // light collection
            this.PointLightCollection = new Element3DCollection();
            this.PointLightCount = 7;
            this.PointLightSpread = 100; 

            // spotlight collection
            this.SpotLightCollection = new Element3DCollection();
            this.SpotLightCount = 7;
            this.SpotLightSpread = 100; 
        }

        /// <summary>
        /// Init the PointLight Collection
        /// </summary>
        /// <param name="numberLights"></param>
        private void InitPointLightCollection(int numberLights)
        {
            // store the current technique
            var technique = this.RenderTechnique;

            // detouch the renderer
            this.RenderTechnique = null;

            // random            
            var rndx = new Random();
            var rndy = new Random(rndx.Next());
            var rndz = new Random(rndy.Next());
            var spread = this.PointLightSpread;

            // re-generate the lights
            this.PointLightCollection.Clear();
            for (int i = 0; i < numberLights; i++)
            {
                var pointLight = new PointLight3D()
                {
                    Color = this.PointLightColor,
                    Attenuation = this.PointLightAttenuation,
                    Transform = CreateAnimatedTransform(
                    new Vector3D(rndx.NextDouble() * spread - spread / 2.0, 1, rndz.NextDouble() * spread - spread / 2),
                    new Vector3D(0, 1, 0),
                    rndx.Next(10) + 4),
                };
                this.PointLightCollection.Add(pointLight);
            }

            // attach the renderer
            this.RenderTechnique = technique;
        }

        /// <summary>
        /// Update Pointlights
        /// </summary>
        private void UpdatePointLightCollection()
        {
            if (this.PointLightCollection != null)
            {
                for (int i = 0; i < this.PointLightCollection.Count; i++)
                {
                    (this.PointLightCollection[i] as PointLight3D).Attenuation = this.PointLightAttenuation;
                    (this.PointLightCollection[i] as PointLight3D).Color = this.PointLightColor;
                }
            }
        }

        /// <summary>
        /// Init the Spotlight Collection
        /// </summary>
        /// <param name="numberLights"></param>
        private void InitSpotLightCollection(int numberLights)
        {
            // store the current technique
            var technique = this.RenderTechnique;

            // detouch the renderer
            this.RenderTechnique = null;

            // random            
            var rndx = new Random();
            var rndy = new Random(rndx.Next());
            var rndz = new Random(rndy.Next());
            var spread = this.SpotLightSpread;

            // re-generate the lights
            this.SpotLightCollection.Clear();
            for (int i = 0; i < numberLights; i++)
            {
                var spotLight = new SpotLight3D()
                {
                    Color = this.SpotLightColor,
                    Attenuation = this.SpotLightAttenuation,
                    Transform = new TranslateTransform3D(0, 20, 0),
                    //OuterAngle = 90,
                    //InnerAngle = 88,
                    Direction = new Vector3(0, -1, 0),
                    DirectionTransform = CreateAnimatedDirection(-new Vector3D(0, -1, 0), (2 * rndx.NextDouble() - 1) * new Vector3D(1, 0, 0) + (2 * rndz.NextDouble() - 1) * new Vector3D(0, 0, 1), rndx.Next(10) + 8),
                };
                this.SpotLightCollection.Add(spotLight);
            }

            // attach the renderer
            this.RenderTechnique = technique;
        }

        /// <summary>
        /// Update Spotlights
        /// </summary>
        private void UpdateSpotLightCollection()
        {
            if (this.SpotLightCollection != null)
            {
                for (int i = 0; i < this.SpotLightCollection.Count; i++)
                {
                    (this.SpotLightCollection[i] as SpotLight3D).Attenuation = this.SpotLightAttenuation;
                    (this.SpotLightCollection[i] as SpotLight3D).Color = this.SpotLightColor;
                }
            }
        }

        /// <summary>
        /// Create animation for positions
        /// </summary>
        private Transform3D CreateAnimatedTransform(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Transform3DGroup();
            lightTrafo.Children.Add(new TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 90),
                Duration = TimeSpan.FromSeconds(speed / 4),
                IsCumulative = true,
            };

            var rotateTransform = new RotateTransform3D();
            rotateTransform.BeginAnimation(RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);

            return lightTrafo;
        }

        /// <summary>
        /// Create animation for directions
        /// </summary>
        private Transform3D CreateAnimatedDirection(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Transform3DGroup();
            lightTrafo.Children.Add(new TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                From = new Media3D.AxisAngleRotation3D(axis, 90),
                To = new Media3D.AxisAngleRotation3D(axis, 270),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed / 4),
                //IsCumulative = true,                  
            };

            var rotateTransform = new RotateTransform3D();
            rotateTransform.BeginAnimation(RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);

            return lightTrafo;
        }

        /// <summary>
        /// Load OBJ file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="faces"></param>
        private void LoadModel(string filename, MeshFaces faces)
        {
            // load model
            var reader = new ObjReader();
            var objModel = reader.Read(filename, new ModelInfo() { Faces = MeshFaces.Default });
            //this.Model = objModel[0].Geometry as MeshGeometry3D;
            //this.Model.Colors = this.Model.Positions.Select(x => new Color4(1, 0, 0, 1)).ToArray();
        }



        private Vector3 pointLightAttenuation;
        private Color4 pointLightColor;
        private int pointLightSpread;

        private Vector3 spotLightAttenuation;
        private Color4 spotLightColor;
        private double spotLightSpread;

        private string meshTopology = MeshFaces.Default.ToString();
    }

    public class ColorVectorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Color4)value).ToColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((System.Windows.Media.Color)value).ToColor4();
        }
    }
}
