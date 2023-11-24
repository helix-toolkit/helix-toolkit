using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace ShadowMapDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D model;

    [ObservableProperty]
    private MeshGeometry3D? lightCameraModel;

    [ObservableProperty]
    private MeshGeometry3D plane;

    [ObservableProperty]
    private LineGeometry3D lines;

    [ObservableProperty]
    private LineGeometry3D? grid;

    [ObservableProperty]
    private Matrix[] instances;

    [ObservableProperty]
    private PhongMaterial redMaterial;

    [ObservableProperty]
    private PhongMaterial greenMaterial;

    [ObservableProperty]
    private PhongMaterial blueMaterial;

    [ObservableProperty]
    private PhongMaterial grayMaterial;

    [ObservableProperty]
    private PhongMaterial lightCameraMaterial = new() { EmissiveColor = Color.Yellow };

    [ObservableProperty]
    private Media.Color gridColor;

    [ObservableProperty]
    private Media3D.Transform3D model1Transform;

    [ObservableProperty]
    private Media3D.Transform3D model2Transform;

    [ObservableProperty]
    private Media3D.Transform3D model3Transform;

    [ObservableProperty]
    private Media3D.Transform3D? gridTransform;

    [ObservableProperty]
    private Media3D.Transform3D planeTransform;

    [ObservableProperty]
    private Media3D.Transform3DGroup lightCameraTransform = new();

    [ObservableProperty]
    private Media3D.Transform3D? lightDirectionTransform;

    //[ObservableProperty]
    //private Vector3 directionalLightDirection;

    [ObservableProperty]
    private Media.Color directionalLightColor;

    [ObservableProperty]
    private Color4 ambientLightColor;

    [ObservableProperty]
    private Size shadowMapResolution;

    [ObservableProperty]
    private double xValue;

    partial void OnXValueChanged(double value)
    {
        Console.WriteLine("x: {0}", value);
        //this.DirectionalLightDirection = new Vector3D(value, -10, -10);
        this.LightDirectionTransform = new Media3D.TranslateTransform3D(value, -10, 10);
    }

    [ObservableProperty]
    private ProjectionCamera camera1;

    //[ObservableProperty]
    //private Camera camera2;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Title = "Shadow Map Demo";
        SubTitle = "WPF & SharpDX";

        // setup lighting            
        this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
        this.DirectionalLightColor = Media.Colors.White;
        //this.DirectionalLightDirection = new Vector3(-1, -1, -1);
        // this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
        this.ShadowMapResolution = new Size(2048, 2048);

        // camera setup
        this.Camera = new PerspectiveCamera
        {
            Position = new Point3D(0, 1, 1),
            LookDirection = new Vector3D(0, -1, -1),
            UpDirection = new Vector3D(0, 1, 0)
        };

        Camera1 = new PerspectiveCamera
        {
            Position = new Point3D(0, 5, 0),
            LookDirection = new Vector3D(0, -1, 0),
            UpDirection = new Vector3D(1, 0, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 1,
            FieldOfView = 45
        };

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0).ToVector(), 0.5f);
        b1.AddBox(new Vector3(0, 0, 0).ToVector(), 1, 0.25f, 2, BoxFaces.All);
        Model = b1.ToMesh().ToMeshGeometry3D();
        Instances = new[] { Matrix.Translation(0, 0, -1.5f), Matrix.Translation(0, 0, 1.5f) };

        var b2 = new MeshBuilder();
        b2.AddBox(new Vector3(0, 0, 0).ToVector(), 10, 0, 10, BoxFaces.PositiveY);
        Plane = b2.ToMesh().ToMeshGeometry3D();
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
        //b3.AddBox(new Vector3().ToVector(), 0.3f, 0.3f, 0.3f, BoxFaces.All);
        //b3.AddCone(new Vector3(0, 0.3f, 0).ToVector(), new Vector3(0, 0f, 0).ToVector(), 0.2f, true, 24);
        //LightCameraModel = b3.ToMesh().ToMeshGeometry3D();
        //LightCameraTransform.Children.Add(new Media3D.RotateTransform3D(new Media3D.AxisAngleRotation3D(new Vector3D(1, 0, 0), -135)));
        //LightCameraTransform.Children.Add(new Media3D.TranslateTransform3D(0, 3, 3));
        //UpdateCamera();
    }

    //private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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
