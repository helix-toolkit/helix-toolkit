// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace LightingDemo
{
    using DemoCore;
    using HelixToolkit.Mathematics;
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Numerics;
    using System.Windows.Media.Animation;
    using Color = System.Windows.Media.Color;
    using Colors = System.Windows.Media.Colors;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public MainViewModel ViewModel { get { return this; } }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Floor { get; private set; }
        public MeshGeometry3D Sphere { get; private set; }
        public MeshGeometry3D FlyingObject { get; private set; }
        public LineGeometry3D CubeEdges { get; private set; }
        public Transform3D ModelTransform { get; private set; }
        public Transform3D Model1Transform { get; private set; }
        public Transform3D FloorTransform { get; private set; }
        public Transform3D Light1Transform { get; private set; }
        public Transform3D Light2Transform { get; private set; }
        public Transform3D Light3Transform { get; private set; }
        public Transform3D Light4Transform { get; private set; }
        public Transform3D Light1DirectionTransform { get; private set; }
        public Transform3D Light4DirectionTransform { get; private set; }

        public Transform3D Object1Transform { get; private set; }
        public Transform3D Object2Transform { get; private set; }
        public Transform3D Object3Transform { get; private set; }
        public Transform3D Object4Transform { get; private set; }

        public Transform3D Object5Transform { get; private set; }
        public Transform3D Object6Transform { get; private set; }
        public Transform3D Object7Transform { get; private set; }
        public Transform3D Object8Transform { get; private set; }

        public PhongMaterial ModelMaterial { get; set; }
        public PhongMaterial ReflectMaterial { get; set; }
        public PhongMaterial FloorMaterial { get; set; }
        public PhongMaterial LightModelMaterial { get; set; }
        public PhongMaterial ObjectMaterial { set; get; } = PhongMaterials.Red;

        public Vector3D Light1Direction { get; set; }
        public Vector3D Light4Direction { get; set; }
        public Vector3D LightDirection4 { get; set; }
        public Color Light1Color { get; set; }
        public Color Light2Color { get; set; }
        public Color Light3Color { get; set; }
        public Color Light4Color { get; set; }
        public Color AmbientLightColor { get; set; }
        public Vector3D Light2Attenuation { get; set; }
        public Vector3D Light3Attenuation { get; set; }
        public Vector3D Light4Attenuation { get; set; }
        public bool RenderLight1 { get; set; }
        public bool RenderLight2 { get; set; }
        public bool RenderLight3 { get; set; }
        public bool RenderLight4 { get; set; }
        private bool renderDiffuseMap = true;
        public bool RenderDiffuseMap {
            set
            {
                if(SetValue(ref renderDiffuseMap, value))
                {
                    ModelMaterial.RenderDiffuseMap = FloorMaterial.RenderDiffuseMap = value;
                }
            }
            get { return renderDiffuseMap; }
        }

        private bool renderNormalMap = true;
        public bool RenderNormalMap {
            set
            {
                if(SetValue(ref renderNormalMap, value))
                {
                    ModelMaterial.RenderNormalMap = FloorMaterial.RenderNormalMap = value;
                }
            }
            get { return renderNormalMap; }
        }

        public string[] TextureFiles { get; } = new string[] { @"TextureCheckerboard2.jpg", @"TextureCheckerboard3.jpg", @"TextureNoise1.jpg", @"TextureNoise1_dot3.jpg", @"TextureCheckerboard2_dot3.jpg" };

        private string selectedDiffuseTexture = @"TextureCheckerboard2.jpg";
        public string SelectedDiffuseTexture
        {
            set
            {
                if(SetValue(ref selectedDiffuseTexture, value))
                {
                    ModelMaterial.DiffuseMap = LoadFileToMemory(new System.Uri(value, System.UriKind.RelativeOrAbsolute).ToString());
                    FloorMaterial.DiffuseMap = ModelMaterial.DiffuseMap;
                }
            }
            get
            {
                return selectedDiffuseTexture;
            }
        }

        private string selectedNormalTexture = @"TextureCheckerboard2_dot3.jpg";
        public string SelectedNormalTexture
        {
            set
            {
                if (SetValue(ref selectedNormalTexture, value))
                {
                    ModelMaterial.NormalMap = LoadFileToMemory(new System.Uri(value, System.UriKind.RelativeOrAbsolute).ToString());
                    FloorMaterial.NormalMap = ModelMaterial.NormalMap;
                }
            }
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

        public MSAALevel MSAA
        {
            set; get;
        } = MSAALevel.Disable;

        public MSAALevel[] MSAAs { get; } = new MSAALevel[] { MSAALevel.Disable, MSAALevel.Two, MSAALevel.Four, MSAALevel.Eight, MSAALevel.Maximum };

        public FXAALevel FXAA
        {
            set; get;
        } = FXAALevel.None;

        public FXAALevel[] FXAAs { get; } = new FXAALevel[] { FXAALevel.None, FXAALevel.Low, FXAALevel.Medium, FXAALevel.High, FXAALevel.Ultra };

        public Camera Camera2 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera3 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public Camera Camera4 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

        public MainViewModel()
        {
            //    RenderTechniquesManager = new DefaultRenderTechniquesManager();           
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];
            // ----------------------------------------------
            // titles
            this.Title = "Lighting Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

            // ----------------------------------------------
            // setup scene
            this.AmbientLightColor = Colors.DarkGray;

            this.RenderLight1 = true;
            this.RenderLight2 = true;
            this.RenderLight3 = true;
            this.RenderLight4 = true;

            this.Light1Color = Colors.White;
            this.Light2Color = Colors.Red;
            this.Light3Color = Colors.LightYellow;
            this.Light4Color = Colors.LightBlue;

            this.Light2Attenuation = new Vector3D(1.0f, 0.5f, 0.10f);
            this.Light3Attenuation = new Vector3D(1.0f, 0.1f, 0.05f);
            this.Light4Attenuation = new Vector3D(0.1f, 0.1f, 0.0f);

            this.Light1Direction = new Vector3D(0, -10, 0);
            this.Light1Transform = CreateAnimatedTransform1(-Light1Direction, new Vector3D(1, 0, 0), 24);
            this.Light1DirectionTransform = CreateAnimatedTransform2(-Light1Direction, new Vector3D(0, 1, -1), 24);

            this.Light2Transform = CreateAnimatedTransform1(new Vector3D(-4, 0, 0), new Vector3D(0, 0, 1), 3);
            this.Light3Transform = CreateAnimatedTransform1(new Vector3D(0, 0, 4), new Vector3D(0, 1, 0), 5);

            this.Light4Direction = new Vector3D(0, -5, -1);
            this.Light4Transform = CreateAnimatedTransform2(-Light4Direction * 2, new Vector3D(0,1,0), 24);
            this.Light4DirectionTransform = CreateAnimatedTransform2(-Light4Direction, new Vector3D(1, 0, 0), 12);

            var transformGroup = new Media3D.Transform3DGroup();
            transformGroup.Children.Add(new Media3D.ScaleTransform3D(10, 10, 10));
            transformGroup.Children.Add(new Media3D.TranslateTransform3D(2, -4, 2));
            Model1Transform = transformGroup;
            // ----------------------------------------------
            // light model3d
            var sphere = new MeshBuilder();
            sphere.AddSphere(new Vector3(0, 0, 0), 0.2);
            Sphere = sphere.ToMeshGeometry3D();
            this.LightModelMaterial = new PhongMaterial
            {
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = Colors.Gray.ToColor4(),
                EmissiveColor = Colors.Yellow.ToColor4(),
                SpecularColor = Colors.Black.ToColor4(),
            };

            // ----------------------------------------------
            // scene model3d
            var b1 = new MeshBuilder(true, true, true);
            b1.AddSphere(new Vector3(0.25f, 0.25f, 0.25f), 0.75, 24, 24);
            b1.AddBox(-new Vector3(0.25f, 0.25f, 0.25f), 1, 1, 1, BoxFaces.All);
            b1.AddBox(-new Vector3(5.0f, 0.0f, 0.0f), 1, 1, 1, BoxFaces.All);
            b1.AddSphere(new Vector3(5f, 0f, 0f), 0.75, 24, 24);
            b1.AddCylinder(new Vector3(0f, -3f, -5f), new Vector3(0f, 3f, -5f), 1.2, 24);
            b1.AddSphere(new Vector3(-5.0f, -5.0f, 5.0f), 4, 24, 64);
            b1.AddCone(new Vector3(6f, -9f, -6f), new Vector3(6f, -1f, -6f), 4f, true, 64);
            this.Model = b1.ToMeshGeometry3D();
            this.ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.ModelMaterial = PhongMaterials.Chrome;

            this.ModelMaterial.NormalMap = LoadFileToMemory(new System.Uri(SelectedNormalTexture, System.UriKind.RelativeOrAbsolute).ToString());

            // ----------------------------------------------
            // floor model3d
            var b2 = new MeshBuilder(true, true, true);
            //b2.AddRectangularMesh(BoxFaces.Left, 10, 10, 10, 10);
            b2.AddBox(new Vector3(0.0f, -5.0f, 0.0f), 15, 1, 15, BoxFaces.All);
            //b2.AddSphere(new Vector3(-5.0f, -5.0f, 5.0f), 4, 24, 64);
            //b2.AddCone(new Vector3(6f, -9f, -6f), new Vector3(6f, -1f, -6f), 4f, true, 64);
            this.Floor = b2.ToMeshGeometry3D();
            this.FloorTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.FloorMaterial = new PhongMaterial
            {
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 100f,
                DiffuseMap = LoadFileToMemory(new System.Uri(SelectedDiffuseTexture, System.UriKind.RelativeOrAbsolute).ToString()),
                NormalMap = ModelMaterial.NormalMap,
                RenderShadowMap = true
            };
            ModelMaterial.DiffuseMap = FloorMaterial.DiffuseMap;
            
            ReflectMaterial = PhongMaterials.PolishedSilver;
            ReflectMaterial.ReflectiveColor = Colors.Silver;
            ReflectMaterial.RenderEnvironmentMap = true;
            InitialObjectTransforms();           
        }

        private void InitialObjectTransforms()
        {
            var b = new MeshBuilder(true);
            b.AddTorus(1, 0.5);
            b.AddTetrahedron(new Vector3(), new Vector3(1, 0, 0), new Vector3(0, 1, 0), 1.1);
            FlyingObject = b.ToMesh();
            var random = new Random();
            Object1Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-5, 5)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-5, 5)), random.NextDouble(2, 10));
            Object2Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object3Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object4Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object5Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object6Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object7Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
            Object8Transform = CreateAnimatedTransform1(new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-10, 10), random.NextDouble(-10, 10)),
                new Vector3D(random.NextDouble(-5, 5), random.NextDouble(-5, 5), random.NextDouble(-10, 10)), random.NextDouble(2, 10));
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
    }
}