using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX;
using SharpDX.Direct3D11;
using System.Threading;
using HelixToolkit;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System;
using HelixToolkit.SharpDX.Model;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SwapChainRenderingDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    public MainViewModel ViewModel => this;

    [ObservableProperty]
    private ObservableElement3DCollection landerModels = new();

    [ObservableProperty]
    private MeshGeometry3D? floor;

    [ObservableProperty]
    private MeshGeometry3D? sphere;

    [ObservableProperty]
    private LineGeometry3D? cubeEdges;

    [ObservableProperty]
    private Transform3D modelTransform;

    [ObservableProperty]
    private Transform3D? floorTransform;

    [ObservableProperty]
    private Transform3D light1Transform;

    [ObservableProperty]
    private Transform3D light2Transform;

    [ObservableProperty]
    private Transform3D light3Transform;

    [ObservableProperty]
    private Transform3D light4Transform;

    [ObservableProperty]
    private Transform3D light1DirectionTransform;

    [ObservableProperty]
    private Transform3D light4DirectionTransform;

    [ObservableProperty]
    private PhongMaterial? modelMaterial;

    [ObservableProperty]
    private PhongMaterial? floorMaterial;

    [ObservableProperty]
    private PhongMaterial lightModelMaterial;

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

    [ObservableProperty]
    private bool renderNormalMap = true;

    public string SelectedDiffuseTexture => @"TextureCheckerboard2.jpg";

    public string SelectedNormalTexture => @"TextureCheckerboard2_dot3.jpg";

    public System.Windows.Media.Color DiffuseColor
    {
        set
        {
            if (FloorMaterial is not null)
            {
                FloorMaterial.DiffuseColor = value.ToColor4();
            }

            if (ModelMaterial is not null)
            {
                ModelMaterial.DiffuseColor = value.ToColor4();
            }
        }
        get
        {
            return ModelMaterial?.DiffuseColor.ToColor() ?? Colors.Black;
        }
    }


    public System.Windows.Media.Color ReflectiveColor
    {
        set
        {
            if (FloorMaterial is not null)
            {
                FloorMaterial.ReflectiveColor = value.ToColor4();
            }

            if (ModelMaterial is not null)
            {
                ModelMaterial.ReflectiveColor = value.ToColor4();
            }
        }
        get
        {
            return ModelMaterial?.ReflectiveColor.ToColor() ?? Colors.Black;
        }
    }

    public System.Windows.Media.Color EmissiveColor
    {
        set
        {
            if (FloorMaterial is not null)
            {
                FloorMaterial.EmissiveColor = value.ToColor4();
            }

            if (ModelMaterial is not null)
            {
                ModelMaterial.EmissiveColor = value.ToColor4();
            }
        }
        get
        {
            return ModelMaterial?.EmissiveColor.ToColor() ?? Colors.Black;
        }
    }

    public Camera Camera2 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

    public Camera Camera3 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

    public Camera Camera4 { get; } = new PerspectiveCamera { Position = new Point3D(8, 9, 7), LookDirection = new Vector3D(-5, -12, -5), UpDirection = new Vector3D(0, 1, 0) };

    [ObservableProperty]
    private FillMode fillMode = FillMode.Solid;

    [ObservableProperty]
    private int numberOfTriangles = 0;

    [ObservableProperty]
    private int numberOfVertices = 0;

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
    {
        FillMode = value ? FillMode.Wireframe : FillMode.Solid;
    }

    [ObservableProperty]
    private LineGeometry3D? lineGeo;

    private readonly SynchronizationContext? context = SynchronizationContext.Current;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();


        // ----------------------------------------------
        // titles
        this.Title = "SwapChain Top Surface Rendering Demo";
        this.SubTitle = "WPF & SharpDX";

        // ----------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(100, 100, 100), LookDirection = new Vector3D(-100, -100, -100), UpDirection = new Vector3D(0, 1, 0) };

        // ----------------------------------------------
        // setup scene
        this.AmbientLightColor = Colors.Gray;

        this.Light1Color = Colors.LightGray;
        this.Light2Color = Colors.Red;
        this.Light3Color = Colors.LightYellow;
        this.Light4Color = Colors.LightBlue;

        this.Light2Attenuation = new Vector3D(0.1f, 0.05f, 0.010f);
        this.Light3Attenuation = new Vector3D(0.1f, 0.01f, 0.005f);
        this.Light4Attenuation = new Vector3D(0.1f, 0.02f, 0.0f);

        this.Light1Direction = new Vector3D(0, -10, -10);
        this.Light1Transform = new TranslateTransform3D(-Light1Direction);
        this.Light1DirectionTransform = CreateAnimatedTransform2(-Light1Direction, new Vector3D(0, 1, -1), 36);

        this.Light2Transform = CreateAnimatedTransform1(new Vector3D(-100, 50, 0), new Vector3D(0, 0, 1), 3);
        this.Light3Transform = CreateAnimatedTransform1(new Vector3D(0, 50, 100), new Vector3D(0, 1, 0), 5);

        this.Light4Direction = new Vector3D(0, -100, 0);
        this.Light4Transform = new TranslateTransform3D(-Light4Direction);
        this.Light4DirectionTransform = CreateAnimatedTransform2(-Light4Direction, new Vector3D(1, 0, 0), 48);

        // ----------------------------------------------
        // light model3d
        var sphere = new MeshBuilder();
        sphere.AddSphere(new Vector3(0, 0, 0).ToVector(), 4);
        Sphere = sphere.ToMesh().ToMeshGeometry3D();
        this.LightModelMaterial = new PhongMaterial
        {
            AmbientColor = Colors.Gray.ToColor4(),
            DiffuseColor = Colors.Gray.ToColor4(),
            EmissiveColor = Colors.Yellow.ToColor4(),
            SpecularColor = Colors.Black.ToColor4(),
        };

        Task.Run(() => { LoadFloor(); });
        Task.Run(() => { LoadLander(); });

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
    }

    private void LoadLander()
    {
        foreach (var obj in MainViewModel.Load3ds("Car.3ds"))
        {
            obj.Geometry?.UpdateOctree();
            Task.Delay(10).Wait();
            context?.Post((o) =>
            {
                var model = new MeshGeometryModel3D() { Geometry = obj.Geometry };
                if (obj.Material is PhongMaterialCore p)
                {
                    model.Material = p.ConvertToPhongMaterial();
                }
                LanderModels.Add(model);
                if (obj.Geometry?.Indices is not null)
                {
                    NumberOfTriangles += obj.Geometry.Indices.Count / 3;
                }
                if (obj.Geometry?.Positions is not null)
                {
                    NumberOfVertices += obj.Geometry.Positions.Count;
                }
                OnPropertyChanged(nameof(NumberOfTriangles));
                OnPropertyChanged(nameof(NumberOfVertices));
            }, null);
        }
    }

    private void LoadFloor()
    {
        var models = MainViewModel.Load3ds("wall12.obj").Select(x => x.Geometry as MeshGeometry3D).ToArray();
        foreach (var model in models)
        {
            model?.UpdateOctree();
        }

        context?.Post((o) =>
        {
            Floor = models[0];
            this.FloorTransform = new Media3D.TranslateTransform3D(0, 0, 0);
            this.FloorMaterial = new PhongMaterial
            {
                AmbientColor = Colors.Gray.ToColor4(),
                DiffuseColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f),
                SpecularColor = Colors.White.ToColor4(),
                SpecularShininess = 100f
            };
            if (Floor?.Indices is not null)
            {
                NumberOfTriangles += Floor.Indices.Count / 3;
            }
            if (Floor?.Positions is not null)
            {
                NumberOfVertices += Floor.Positions.Count;
            }
            OnPropertyChanged(nameof(NumberOfTriangles));
            OnPropertyChanged(nameof(NumberOfVertices));
            OnPropertyChanged(nameof(Floor));
            OnPropertyChanged(nameof(FloorMaterial));
        }, null);
    }

    public static List<Object3D> Load3ds(string path)
    {
        List<Object3D>? list = null;

        if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
        {
            var reader = new ObjReader();
            list = reader.Read(path);
        }
        else if (path.EndsWith(".3ds", StringComparison.CurrentCultureIgnoreCase))
        {
            var reader = new StudioReader();
            list = reader.Read(path);
        }

        return list ?? new();
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
        if (sender is not Viewport3DX viewport)
        {
            return;
        }

        var point = e.GetPosition(viewport);
        var watch = Stopwatch.StartNew();
        var hitTests = viewport.FindHits(point);
        watch.Stop();
        Console.WriteLine("Hit test time =" + watch.ElapsedMilliseconds);

        if (hitTests.Count > 0)
        {
            var lineBuilder = new LineBuilder();
            foreach (var hit in hitTests)
            {
                lineBuilder.AddLine(hit.PointHit, (hit.PointHit + hit.NormalAtHit * 10));
            }
            LineGeo = lineBuilder.ToLineGeometry3D();
        }
    }
}
