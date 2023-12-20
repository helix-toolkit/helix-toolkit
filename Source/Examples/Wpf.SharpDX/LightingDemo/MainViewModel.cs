using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace LightingDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    public MainViewModel ViewModel => this;

    [ObservableProperty]
    private MeshGeometry3D? model;

    [ObservableProperty]
    private MeshGeometry3D? floor;

    [ObservableProperty]
    private MeshGeometry3D? sphere;

    [ObservableProperty]
    private MeshGeometry3D? flyingObject;

    [ObservableProperty]
    private LineGeometry3D? cubeEdges;

    [ObservableProperty]
    private Transform3D? modelTransform;

    [ObservableProperty]
    private Transform3D? model1Transform;

    [ObservableProperty]
    private Transform3D? floorTransform;

    [ObservableProperty]
    private Transform3D? light1Transform;

    [ObservableProperty]
    private Transform3D? light2Transform;

    [ObservableProperty]
    private Transform3D? light3Transform;

    [ObservableProperty]
    private Transform3D? light4Transform;

    [ObservableProperty]
    private Transform3D? light1DirectionTransform;

    [ObservableProperty]
    private Transform3D? light4DirectionTransform;

    [ObservableProperty]
    private Transform3D? object1Transform;

    [ObservableProperty]
    private Transform3D? object2Transform;

    [ObservableProperty]
    private Transform3D? object3Transform;

    [ObservableProperty]
    private Transform3D? object4Transform;

    [ObservableProperty]
    private Transform3D? object5Transform;

    [ObservableProperty]
    private Transform3D? object6Transform;

    [ObservableProperty]
    private Transform3D? object7Transform;

    [ObservableProperty]
    private Transform3D? object8Transform;

    [ObservableProperty]
    private PhongMaterial modelMaterial = PhongMaterials.Black;

    [ObservableProperty]
    private PhongMaterial reflectMaterial = PhongMaterials.Black;

    [ObservableProperty]
    private PhongMaterial floorMaterial = PhongMaterials.Black;

    [ObservableProperty]
    private PhongMaterial lightModelMaterial = PhongMaterials.Black;

    [ObservableProperty]
    private PhongMaterial objectMaterial = PhongMaterials.Red;

    [ObservableProperty]
    private Vector3D light1Direction;

    [ObservableProperty]
    private Vector3D light4Direction;

    [ObservableProperty]
    private Vector3D lightDirection4;

    [ObservableProperty]
    private Color light1Color;

    [ObservableProperty]
    private Color light2Color;

    [ObservableProperty]
    private Color light3Color;

    [ObservableProperty]
    private Color light4Color;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private Vector3D light2Attenuation;

    [ObservableProperty]
    private Vector3D light3Attenuation;

    [ObservableProperty]
    private Vector3D light4Attenuation;

    [ObservableProperty]
    private bool renderLight1;

    [ObservableProperty]
    private bool renderLight2;

    [ObservableProperty]
    private bool renderLight3;

    [ObservableProperty]
    private bool renderLight4;

    [ObservableProperty]
    private bool renderDiffuseMap = true;

    partial void OnRenderDiffuseMapChanged(bool value)
    {
        ModelMaterial.RenderDiffuseMap = FloorMaterial.RenderDiffuseMap = value;
    }

    [ObservableProperty]
    private bool renderNormalMap = true;

    partial void OnRenderNormalMapChanged(bool value)
    {
        ModelMaterial.RenderNormalMap = FloorMaterial.RenderNormalMap = value;
    }

    public string[] TextureFiles { get; } = new string[] { @"TextureCheckerboard2.jpg", @"TextureCheckerboard3.jpg", @"TextureNoise1.jpg", @"TextureNoise1_dot3.jpg", @"TextureCheckerboard2_dot3.jpg" };

    [ObservableProperty]
    private string selectedDiffuseTexture = @"TextureCheckerboard2.jpg";

    partial void OnSelectedDiffuseTextureChanged(string value)
    {
        ModelMaterial.DiffuseMap = TextureModel.Create(new Uri(value, UriKind.RelativeOrAbsolute).ToString());
        FloorMaterial.DiffuseMap = ModelMaterial.DiffuseMap;
    }

    [ObservableProperty]
    private string selectedNormalTexture = @"TextureCheckerboard2_dot3.jpg";

    partial void OnSelectedNormalTextureChanged(string value)
    {
        ModelMaterial.NormalMap = TextureModel.Create(new Uri(value, UriKind.RelativeOrAbsolute).ToString());
        FloorMaterial.NormalMap = ModelMaterial.NormalMap;
    }

    public Color DiffuseColor
    {
        set
        {
            FloorMaterial.DiffuseColor = ModelMaterial.DiffuseColor = value.ToColor4();
            OnPropertyChanged();
        }
        get
        {
            return ModelMaterial.DiffuseColor.ToColor();
        }
    }


    public Color ReflectiveColor
    {
        set
        {
            FloorMaterial.ReflectiveColor = ModelMaterial.ReflectiveColor = value.ToColor4();
            OnPropertyChanged();
        }
        get
        {
            return ModelMaterial.ReflectiveColor.ToColor();
        }
    }

    public Color EmissiveColor
    {
        set
        {
            FloorMaterial.EmissiveColor = ModelMaterial.EmissiveColor = value.ToColor4();
            OnPropertyChanged();
        }
        get
        {
            return ModelMaterial.EmissiveColor.ToColor();
        }
    }

    [ObservableProperty]
    private MSAALevel mSAA = MSAALevel.Disable;

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
        this.Light4Transform = CreateAnimatedTransform2(-Light4Direction * 2, new Vector3D(0, 1, 0), 24);
        this.Light4DirectionTransform = CreateAnimatedTransform2(-Light4Direction, new Vector3D(1, 0, 0), 12);

        var transformGroup = new Media3D.Transform3DGroup();
        transformGroup.Children.Add(new Media3D.ScaleTransform3D(10, 10, 10));
        transformGroup.Children.Add(new Media3D.TranslateTransform3D(2, -4, 2));
        Model1Transform = transformGroup;
        // ----------------------------------------------
        // light model3d
        var sphere = new MeshBuilder();
        sphere.AddSphere(new Vector3(0, 0, 0), 0.2f);
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
        b1.AddSphere(new Vector3(0.25f, 0.25f, 0.25f), 0.75f, 24, 24);
        b1.AddBox(-new Vector3(0.25f, 0.25f, 0.25f), 1, 1, 1, BoxFaces.All);
        b1.AddBox(-new Vector3(5.0f, 0.0f, 0.0f), 1, 1, 1, BoxFaces.All);
        b1.AddSphere(new Vector3(5f, 0f, 0f), 0.75f, 24, 24);
        b1.AddCylinder(new Vector3(0f, -3f, -5f), new Vector3(0f, 3f, -5f), 1.2f, 24);
        b1.AddSphere(new Vector3(-5.0f, -5.0f, 5.0f), 4, 24, 64);
        b1.AddCone(new Vector3(6f, -9f, -6f), new Vector3(6f, -1f, -6f), 4f, true, 64);
        this.Model = b1.ToMeshGeometry3D();
        this.ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        this.ModelMaterial = PhongMaterials.Chrome;

        this.ModelMaterial.NormalMap = TextureModel.Create(new System.Uri(SelectedNormalTexture, System.UriKind.RelativeOrAbsolute).ToString());

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
            DiffuseMap = TextureModel.Create(new System.Uri(SelectedDiffuseTexture, System.UriKind.RelativeOrAbsolute).ToString()),
            NormalMap = ModelMaterial.NormalMap,
            RenderShadowMap = true
        };
        ModelMaterial.DiffuseMap = FloorMaterial.DiffuseMap;

        ReflectMaterial = PhongMaterials.PolishedSilver;
        ReflectMaterial.ReflectiveColor = HelixToolkit.Maths.Color.Silver;
        ReflectMaterial.RenderEnvironmentMap = true;
        InitialObjectTransforms();
    }

    private void InitialObjectTransforms()
    {
        var b = new MeshBuilder(true);
        b.AddTorus(1, 0.5f);
        b.AddTetrahedron(new Vector3(), new Vector3(1, 0, 0), new Vector3(0, 1, 0), 1.1f);
        FlyingObject = b.ToMeshGeometry3D();
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

    private static Media3D.Transform3D CreateAnimatedTransform1(Vector3D translate, Vector3D axis, double speed = 4)
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

    private static Media3D.Transform3D CreateAnimatedTransform2(Vector3D translate, Vector3D axis, double speed = 4)
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
