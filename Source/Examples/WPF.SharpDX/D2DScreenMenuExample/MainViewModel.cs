using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Media = System.Windows.Media;

namespace D2DScreenMenuExample
{
    public class MainViewModel : BaseViewModel
    {
        public ViewModel3D VM3D { get; } = new ViewModel3D();
        public ViewModel2D VM2D { get; } = new ViewModel2D();
        public MainViewModel()
        {
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);

            // ----------------------------------------------
            // titles
            this.Title = "D2DScreenMenu Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Media3D.Point3D(8, 9, 7), LookDirection = new Media3D.Vector3D(-5, -12, -5), UpDirection = new Media3D.Vector3D(0, 1, 0) };
        }
    }

    public class ViewModel3D : ObservableObject
    {
        public MeshGeometry3D Model { set; get; }
        public PhongMaterial ModelMaterial { set; get; } = PhongMaterials.White;

        public Vector3 Light1Direction { get; set; } = new Vector3(1, -1, -1);

        public Color4 Light1Color { set; get; } = Color.Blue;

        public Vector3 Light2Direction { get; set; } = new Vector3(-1, -1, -1);

        public Color4 Light2Color { set; get; } = Color.Red;

        public Vector3 Light3Direction { get; set; } = new Vector3(-1, -1, 1);

        public Color4 Light3Color { set; get; } = Color.Green;

        private string NormalTexture = @"TextureCheckerboard2_dot3.jpg";

        private string Texture = @"TextureCheckerboard2.jpg";

        public ViewModel3D()
        {
            var builder = new MeshBuilder(true, true, true);
            builder.AddBox(new Vector3(0, 2.5f, 0), 5, 5, 5);
            builder.AddBox(new Vector3(0, 0, 0), 10, 0.1, 10);
            Model = builder.ToMeshGeometry3D();
            var diffuseMap = LoadFileToMemory(new System.Uri(Texture, System.UriKind.RelativeOrAbsolute).ToString());
            var normalMap = LoadFileToMemory(new System.Uri(NormalTexture, System.UriKind.RelativeOrAbsolute).ToString());
            ModelMaterial.DiffuseMap = diffuseMap;
            ModelMaterial.NormalMap = normalMap;
        }

        public static MemoryStream LoadFileToMemory(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open))
            {
                var memory = new MemoryStream();
                file.CopyTo(memory);
                return memory;
            }
        }
    }

    public class ViewModel2D : ObservableObject
    {
        public Media.Transform TextTransform
        {
            set;get;
        }

        public string Text
        {
            set; get;
        } = "Text Model 2D";

        public ViewModel2D()
        {
            //TextTransform = new Media.RotateTransform(45, 100, 0);
            TextTransform = CreateAnimatedTransform2(8);
        }

        private Media.Transform CreateAnimatedTransform2(double speed = 4)
        {
            var lightTrafo = new Media.TransformGroup();
            var rotateAnimation = new Media.Animation.DoubleAnimation
            {
                RepeatBehavior = Media.Animation.RepeatBehavior.Forever,
                By=360,
                AutoReverse = false,
                Duration = TimeSpan.FromSeconds(speed / 4),                
            };

            var rotateTransform = new Media.RotateTransform(0, 50, 10);
            rotateTransform.BeginAnimation(Media.RotateTransform.AngleProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);


            var scaleAnimation = new Media.Animation.DoubleAnimation
            {
                RepeatBehavior = Media.Animation.RepeatBehavior.Forever,
                From = 1, To = 5,
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed / 4),
            };
            var scaleTransform = new Media.ScaleTransform(1, 1, 0, 0);
            scaleTransform.BeginAnimation(Media.ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(Media.ScaleTransform.ScaleYProperty, scaleAnimation);
            lightTrafo.Children.Add(scaleTransform);
            return lightTrafo;
        }
    }
}
