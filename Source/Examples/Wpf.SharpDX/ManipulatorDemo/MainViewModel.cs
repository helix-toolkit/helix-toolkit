using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace ManipulatorDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D? model;

    [ObservableProperty]
    private MeshGeometry3D? model2;

    [ObservableProperty]
    private LineGeometry3D? lines;

    [ObservableProperty]
    private LineGeometry3D? grid;

    [ObservableProperty]
    private PhongMaterial? material1;

    [ObservableProperty]
    private PhongMaterial? material2;

    [ObservableProperty]
    private PhongMaterial? material3;

    [ObservableProperty]
    private Color gridColor;

    [ObservableProperty]
    private Transform3D? model1Transform;

    [ObservableProperty]
    private Transform3D? model2Transform;

    [ObservableProperty]
    private Transform3D? model3Transform;

    [ObservableProperty]
    private Transform3D? gridTransform;

    [ObservableProperty]
    private Vector3D directionalLightDirection;

    [ObservableProperty]
    private Color directionalLightColor;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private Element3D? target;

    [ObservableProperty]
    private Vector3 centerOffset;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        this.Title = "Manipulator Demo";
        this.SubTitle = string.Empty;

        // camera setup
        this.Camera = new OrthographicCamera
        {
            Position = new Point3D(0, 0, 5),
            LookDirection = new Vector3D(0, 0, -5),
            UpDirection = new Vector3D(0, 1, 0)
        };

        // setup lighting            
        this.AmbientLightColor = Colors.DimGray;
        this.DirectionalLightColor = Colors.White;
        this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // floor plane grid
        this.Grid = LineBuilder.GenerateGrid();
        this.GridColor = Colors.Black;
        this.GridTransform = new TranslateTransform3D(-5, -1, -5);

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0), 0.5f);
        b1.AddBox(new Vector3(0, 0, 0), 1, 0.5f, 1.5f, BoxFaces.All);
        this.Model = b1.ToMeshGeometry3D();
        var m1 = MainViewModel.Load3ds("suzanne.3ds");
        this.Model2 = m1[0].Geometry as MeshGeometry3D;
        //Manully set an offset for test
        if (Model2?.Positions is not null)
        {
            for (int i = 0; i < Model2.Positions.Count; ++i)
            {
                Model2.Positions[i] = Model2.Positions[i] + new Vector3(2, 3, 4);
            }
        }
        Model2?.UpdateBounds();

        // lines model3d
        var e1 = new LineBuilder();
        e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 1.5);
        this.Lines = e1.ToLineGeometry3D();

        // model trafos
        this.Model1Transform = new TranslateTransform3D(0, 0, 0);
        this.Model2Transform = new TranslateTransform3D(-3, 0, 0);
        this.Model3Transform = new TranslateTransform3D(+3, 0, 0);

        // model materials
        this.Material1 = PhongMaterials.Orange;
        this.Material2 = PhongMaterials.Orange;
        this.Material3 = PhongMaterials.Red;

        var dr = Colors.DarkRed;
        Console.WriteLine(dr);
    }

    [RelayCommand]
    private void ResetTransforms()
    {
        this.Model1Transform = new TranslateTransform3D(0, 0, 0);
        this.Model2Transform = new TranslateTransform3D(-3, 0, 0);
        this.Model3Transform = new TranslateTransform3D(+3, 0, 0);
    }

    public void OnMouseDown3DHandler(object? sender, MouseDown3DEventArgs e)
    {
        if (e.HitTestResult?.ModelHit is MeshGeometryModel3D m && (m.Geometry == Model || m.Geometry == Model2))
        {
            Target = null;
            CenterOffset = m.Geometry?.Bound.Center ?? Vector3.Zero; // Must update this before updating target
            Target = e.HitTestResult.ModelHit as Element3D;
        }
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

        list ??= new List<Object3D>();
        return list;
    }
}
