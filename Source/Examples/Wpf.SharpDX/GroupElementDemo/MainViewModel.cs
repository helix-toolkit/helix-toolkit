using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace GroupElementDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    private readonly Random rnd = new();

    public LineGeometry3D AxisModel { get; private set; }
    public BillboardText3D AxisLabel { private set; get; }

    public MeshGeometry3D SphereModel { private set; get; }

    public MeshGeometry3D BoxModel { private set; get; }

    public MeshGeometry3D ConeModel { private set; get; }

    public PhongMaterial RedMaterial { get; } = PhongMaterials.Red;
    public PhongMaterial BlueMaterial { get; } = PhongMaterials.Blue;
    public PhongMaterial GreenMaterial { get; } = PhongMaterials.Green;

    public Transform3D GroupModel3DTransform { private set; get; } = new Media3D.TranslateTransform3D(5, 0, 0);
    public Transform3D ItemsModel3DTransform { private set; get; } = new Media3D.TranslateTransform3D(0, 0, 5);

    public Transform3D Transform1 { get; } = new Media3D.TranslateTransform3D(0, 0, 0);

    public Transform3D Transform2 { get; } = new Media3D.TranslateTransform3D(-2, 0, 0);

    public Transform3D Transform3 { get; } = new Media3D.TranslateTransform3D(-4, 0, 0);

    public Transform3D Transform4 { get; } = new Media3D.TranslateTransform3D(-6, 0, 0);

    public ObservableElement3DCollection GroupModelSource { private set; get; } = new();
    public ObservableElement3DCollection TransparentGroupModelSource { private set; get; } = new();
    public ObservableCollection<MeshDataModel> ItemsSource { private set; get; } = new();

    private readonly PhongMaterialCollection materialCollection = new();

    public MainViewModel()
    {
        //    RenderTechniquesManager = new DefaultRenderTechniquesManager();           
        EffectsManager = new DefaultEffectsManager();

        // ----------------------------------------------
        // titles
        this.Title = "GroupElement Test";
        this.SubTitle = "WPF & SharpDX";

        // ----------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(10, 2, 10), LookDirection = new Vector3D(-10, -2, -10), UpDirection = new Vector3D(0, 1, 0) };

        var lineBuilder = new LineBuilder();
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));
        AxisModel = lineBuilder.ToLineGeometry3D();
        AxisModel.Colors = AxisModel.Positions is null ? new Color4Collection() : new Color4Collection(AxisModel.Positions.Count);
        AxisModel.Colors.Add(Colors.Red.ToColor4());
        AxisModel.Colors.Add(Colors.Red.ToColor4());
        AxisModel.Colors.Add(Colors.Green.ToColor4());
        AxisModel.Colors.Add(Colors.Green.ToColor4());
        AxisModel.Colors.Add(Colors.Blue.ToColor4());
        AxisModel.Colors.Add(Colors.Blue.ToColor4());

        AxisLabel = new BillboardText3D();
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(11, 0, 0), Text = "X", Foreground = Colors.Red.ToColor4() });
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 11, 0), Text = "Y", Foreground = Colors.Green.ToColor4() });
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 0, 11), Text = "Z", Foreground = Colors.Blue.ToColor4() });

        var meshBuilder = new MeshBuilder(true);
        meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5f);
        SphereModel = meshBuilder.ToMeshGeometry3D();
        meshBuilder = new MeshBuilder(true);
        meshBuilder.AddBox(Vector3.Zero, 0.5f, 0.5f, 0.5f);
        BoxModel = meshBuilder.ToMeshGeometry3D();
        meshBuilder = new MeshBuilder(true);
        meshBuilder.AddCone(Vector3.Zero, new Vector3(0, 2, 0), 1, true, 24);
        ConeModel = meshBuilder.ToMeshGeometry3D();
    }

    [RelayCommand]
    private void AddGroupModel()
    {
        var model = new MeshGeometryModel3D
        {
            Geometry = SphereModel,
            Material = BlueMaterial,
            Transform = new Media3D.TranslateTransform3D(0, (GroupModelSource.Count + 1) * 2, 0)
        };

        GroupModelSource.Add(model);
    }

    [RelayCommand]
    private void RemoveGroupModel()
    {
        if (GroupModelSource.Count > 0)
        {
            GroupModelSource.RemoveAt(GroupModelSource.Count - 1);
        }
    }

    [RelayCommand]
    private void AddItemsModel()
    {
        var model = new MeshDataModel
        {
            Geometry = SphereModel,
            Material = GreenMaterial,
            Transform = new Media3D.TranslateTransform3D(0, -ItemsSource.Count * 2, 0)
        };

        ItemsSource.Add(model);
    }

    [RelayCommand]
    private void RemoveItemsModel()
    {
        if (ItemsSource.Count > 0)
        {
            ItemsSource.RemoveAt(ItemsSource.Count - 1);
        }
    }

    [RelayCommand]
    private void AnimateGroupModel()
    {
        GroupModel3DTransform = CreateAnimatedTransform1(new Media3D.Transform3DGroup(), new Vector3D(5, 0, 0), new Vector3D(0, 1, 0));
        OnPropertyChanged(nameof(GroupModel3DTransform));
    }

    [RelayCommand]
    private void AnimateItemsModel()
    {
        ItemsModel3DTransform = CreateAnimatedTransform1(new Media3D.Transform3DGroup(), new Vector3D(0, 0, 5), new Vector3D(0, 0, 1));
        OnPropertyChanged(nameof(ItemsModel3DTransform));
    }

    private static Media3D.Transform3D CreateAnimatedTransform1(Media3D.Transform3DGroup transformGroup,
        Media3D.Vector3D center, Media3D.Vector3D axis, double speed = 4)
    {
        var rotateAnimation1 = new Rotation3DAnimation
        {
            RepeatBehavior = RepeatBehavior.Forever,
            By = new Media3D.AxisAngleRotation3D(axis, 240),
            Duration = TimeSpan.FromSeconds(speed / 4),
            IsCumulative = true,
        };

        var rotateTransform1 = new Media3D.RotateTransform3D
        {
            CenterX = 0,
            CenterY = 0,
            CenterZ = 0
        };

        rotateTransform1.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation1);

        transformGroup.Children.Add(rotateTransform1);
        transformGroup.Children.Add(new TranslateTransform3D(center));

        return transformGroup;
    }

    [RelayCommand]
    private void AddTransparentGroupModel()
    {
        var model = new MeshGeometryModel3D();
        int val = rnd.Next(0, 2);
        switch (val)
        {
            case 0:
                model.Geometry = SphereModel;
                break;
            case 1:
                model.Geometry = BoxModel;
                break;
            case 2:
                model.Geometry = ConeModel;
                break;
        }
        val = rnd.Next(0, materialCollection.Count - 1);
        var material = materialCollection[val];
        var diffuse = material.DiffuseColor;
        diffuse.Alpha = (float)rnd.Next(20, 60) / 100f;
        material.DiffuseColor = diffuse;
        model.Material = material;
        model.Transform = new Media3D.TranslateTransform3D((float)rnd.Next(10, 100) / 10, (float)rnd.Next(10, 100) / 10, (float)rnd.Next(10, 100) / 10);
        model.IsTransparent = true;
        TransparentGroupModelSource.Add(model);
    }

    [RelayCommand]
    private void RemoveTransparentGroupModel()
    {
        if (TransparentGroupModelSource.Count > 0)
        {
            TransparentGroupModelSource.RemoveAt(TransparentGroupModelSource.Count - 1);
        }
    }

    [RelayCommand]
    private void ClearGroupModel()
    {
        GroupModelSource.Clear();
    }

    [RelayCommand]
    private void ClearItemsModel()
    {
        ItemsSource.Clear();
    }

    [RelayCommand]
    private void ReplaceGroupSource()
    {
        GroupModelSource = new ObservableElement3DCollection();
        OnPropertyChanged(nameof(GroupModelSource));
    }

    [RelayCommand]
    private void ReplaceItemsModelSource()
    {
        ItemsSource = new ObservableCollection<MeshDataModel>();
        OnPropertyChanged(nameof(ItemsSource));
    }
}
