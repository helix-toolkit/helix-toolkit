using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MeshSimplificationDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    public MainViewModel ViewModel => this;

    [ObservableProperty]
    private MeshGeometry3D? model;

    partial void OnModelChanged(MeshGeometry3D? value)
    {
        NumberOfTriangles = value?.Indices is null ? 0 : value.Indices.Count / 3;
        NumberOfVertices = value?.Positions is null ? 0 : value.Positions.Count;
    }

    [ObservableProperty]
    private PhongMaterial? modelMaterial;

    [ObservableProperty]
    private PhongMaterial? lightModelMaterial;

    [ObservableProperty]
    private Transform3D? modelTransform;

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

    private MeshSimplification simHelper;

    [ObservableProperty]
    private bool busy = false;

    [ObservableProperty]
    private bool showWireframe = true;

    partial void OnShowWireframeChanged(bool value)
    {
        FillMode = value ? FillMode.Wireframe : FillMode.Solid;
    }

    [ObservableProperty]
    private FillMode fillMode = FillMode.Wireframe;

    [ObservableProperty]
    private int numberOfTriangles = 0;

    [ObservableProperty]
    private int numberOfVertices = 0;

    private MeshGeometry3D? orgMesh;

    [ObservableProperty]
    private bool lossless = false;

    [ObservableProperty]
    private long calculationTime = 0;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        // ----------------------------------------------
        // titles
        this.Title = "Mesh Simplification Demo";
        this.SubTitle = "WPF & SharpDX";

        // ----------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera
        {
            Position = new Point3D(100, 100, 100),
            LookDirection = new Vector3D(-100, -100, -100),
            UpDirection = new Vector3D(0, 1, 0)
        };
        // ----------------------------------------------
        // setup scene
        this.AmbientLightColor = Colors.DimGray;
        this.Light1Color = Colors.Gray;


        this.Light1Direction = new Vector3D(-100, -100, -100);
        SetupCameraBindings(Camera);
        // ----------------------------------------------
        // ----------------------------------------------
        // scene model3d
        this.ModelMaterial = PhongMaterials.Silver;

        var models = Load3ds("wall12.obj").Select(x => x.Geometry as MeshGeometry3D).ToArray();
        //var scale = new Vector3(1f);

        //foreach (var item in caritems)
        //{
        //    for (int i = 0; i < item.Positions.Count; ++i)
        //    {
        //        item.Positions[i] = item.Positions[i] * scale;
        //    }

        //}
        Model = models[0];
        orgMesh = Model;

        //ModelTransform = new Media3D.RotateTransform3D() { Rotation = new Media3D.AxisAngleRotation3D(new Vector3D(1, 0, 0), -90) };

        simHelper = new MeshSimplification(Model?.ToWndMeshGeometry3D());
    }

    public List<Object3D> Load3ds(string path)
    {
        var reader = new ObjReader();
        var list = reader.Read(path);
        return list ?? new List<Object3D>();
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

    private bool CanSimplify(object? obj)
    {
        return !Busy;
    }

    [RelayCommand(CanExecute = nameof(CanSimplify))]
    private void Simplify(object? obj)
    {
        if (!CanSimplify(null))
        {
            return;
        }

        Busy = true;
        int size = Model?.Indices is null ? 0 : Model.Indices.Count / 3 / 2;
        CalculationTime = 0;
        Task.Factory.StartNew(() =>
        {
            var sw = Stopwatch.StartNew();
            var model = simHelper.Simplify(size, 7, true, Lossless);
            sw.Stop();
            CalculationTime = sw.ElapsedMilliseconds;
            model.Normals = new Vector3Collection(model.CalculateNormals());
            return model;
        }).ContinueWith(x =>
        {
            Busy = false;
            Model = x.Result.ToMeshGeometry3D();
            CommandManager.InvalidateRequerySuggested();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    [RelayCommand(CanExecute = nameof(CanSimplify))]
    private void Reset(object? obj)
    {
        Model = orgMesh;
        simHelper = new MeshSimplification(Model?.ToWndMeshGeometry3D());
    }
}
