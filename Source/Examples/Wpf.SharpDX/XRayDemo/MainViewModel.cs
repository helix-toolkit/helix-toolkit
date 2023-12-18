using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace XRayDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    public MainViewModel ViewModel => this;

    [ObservableProperty]
    private MeshGeometry3D model;

    [ObservableProperty]
    private MeshGeometry3D floor;

    [ObservableProperty]
    private MeshGeometry3D? carModel;

    [ObservableProperty]
    private PhongMaterial modelMaterial;

    [ObservableProperty]
    private PhongMaterial floorMaterial;

    [ObservableProperty]
    private PhongMaterial? lightModelMaterial;

    [ObservableProperty]
    private Transform3D modelTransform;

    [ObservableProperty]
    private Transform3D screenSpacedScale = new Media3D.ScaleTransform3D(0.1, 0.1, 0.1);

    [ObservableProperty]
    private Vector3D light1Direction;

    [ObservableProperty]
    private Color light1Color;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private Vector3D camLookDir = new(-100, -100, -100);

    partial void OnCamLookDirChanged(Vector3D value)
    {
        Light1Direction = value;
    }

    [ObservableProperty]
    private Matrix[] instances;

    [ObservableProperty]
    private Matrix[] outlineInstances;

    [ObservableProperty]
    private BlendStateDescription blendDescription;

    [ObservableProperty]
    private DepthStencilStateDescription depthStencilDescription;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        // ----------------------------------------------
        // titles
        this.Title = "Lighting Demo";
        this.SubTitle = "WPF & SharpDX";

        // ----------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(100, 100, 100), LookDirection = new Vector3D(-100, -100, -100), UpDirection = new Vector3D(0, 1, 0) };
        // ----------------------------------------------
        // setup scene
        this.AmbientLightColor = Colors.DimGray;
        this.Light1Color = Colors.LightGray;


        this.Light1Direction = new Vector3D(-100, -100, -100);
        SetupCameraBindings(Camera);
        // ----------------------------------------------
        // ----------------------------------------------
        // scene model3d
        this.ModelMaterial = PhongMaterials.Silver;

        // ----------------------------------------------
        // floor model3d
        var b2 = new MeshBuilder(true, true, true);
        b2.AddBox(new Vector3(0.0f, 0, 0.0f), 150, 1, 150, BoxFaces.All);
        b2.AddBox(new Vector3(0, 25, 70), 150, 50, 20);
        b2.AddBox(new Vector3(0, 25, -70), 150, 50, 20);
        this.Floor = b2.ToMeshGeometry3D();
        this.FloorMaterial = PhongMaterials.Bisque;
        this.FloorMaterial.DiffuseMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute).ToString());
        this.FloorMaterial.NormalMap = TextureModel.Create(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute).ToString());

        MeshGeometry3D[] caritems = MainViewModel.Load3ds("leone.3DBuilder.obj")
            .Select(x => x.Geometry as MeshGeometry3D)
            .Where(t => t is not null)
            .Cast<MeshGeometry3D>()
            .ToArray();

        var scale = new Vector3(1f);

        foreach (var item in caritems)
        {
            if (item?.Positions is null)
            {
                continue;
            }

            for (int i = 0; i < item.Positions.Count; ++i)
            {
                item.Positions[i] = item.Positions[i] * scale;
            }
        }

        Model = MeshGeometry3D.Merge(caritems);

        ModelTransform = new Media3D.RotateTransform3D() { Rotation = new Media3D.AxisAngleRotation3D(new Vector3D(1, 0, 0), -90) };

        Instances = new Matrix[6];
        for (int i = 0; i < Instances.Length; ++i)
        {
            Instances[i] = Matrix.CreateTranslation(new Vector3(15 * i - 30, 15 * (i % 2) - 30, 0));
        }

        OutlineInstances = new Matrix[6];
        for (int i = 0; i < Instances.Length; ++i)
        {
            OutlineInstances[i] = Matrix.CreateTranslation(new Vector3(15 * i - 30, 15 * (i % 2), 0));
        }

        var blendDesc = new BlendStateDescription();
        blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
        {
            IsBlendEnabled = true,
            BlendOperation = BlendOperation.Add,
            AlphaBlendOperation = BlendOperation.Add,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.One,
            SourceAlphaBlend = BlendOption.Zero,
            DestinationAlphaBlend = BlendOption.One,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };
        BlendDescription = blendDesc;
        DepthStencilDescription = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthComparison = Comparison.LessEqual,
            DepthWriteMask = DepthWriteMask.Zero
        };
    }

    public static List<Object3D> Load3ds(string path)
    {
        var reader = new ObjReader();
        var list = reader.Read(path);
        return list ?? new();
    }

    public void SetupCameraBindings(Camera camera)
    {
        if (camera is ProjectionCamera)
        {
            SetBinding("CamLookDir", camera, ProjectionCamera.LookDirectionProperty, this);
        }
    }

    private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
    {
        var binding = new Binding(path)
        {
            Source = viewModel,
            Mode = mode
        };

        BindingOperations.SetBinding(dobj, property, binding);
    }
}
