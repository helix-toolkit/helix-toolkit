using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media = System.Windows.Media;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace LineShadingDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D? model;

    [ObservableProperty]
    private LineGeometry3D? lines;

    [ObservableProperty]
    private LineGeometry3D? grid;

    [ObservableProperty]
    private double lineThickness;

    public double LineThicknessMaximum => FixedSize ? 10 : 0.05;

    public double LineThicknessTickFrequency => FixedSize ? 1 : 0.005;

    [ObservableProperty]
    private double lineSmoothness;

    [ObservableProperty]
    private bool linesEnabled;

    [ObservableProperty]
    private bool gridEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LineThicknessMaximum))]
    [NotifyPropertyChangedFor(nameof(LineThicknessTickFrequency))]
    private bool fixedSize = true;

    partial void OnFixedSizeChanged(bool value)
    {
        LineThickness = FixedSize ? 2 : 0.005;
    }

    [ObservableProperty]
    private PhongMaterial? material1;

    [ObservableProperty]
    private PhongMaterial? material2;

    [ObservableProperty]
    private PhongMaterial? material3;

    [ObservableProperty]
    private LineMaterial? lineMaterial;

    [ObservableProperty]
    private LineMaterial? gridMaterial;

    [ObservableProperty]
    private Color gridColor;

    [ObservableProperty]
    private Transform3D? model1Transform;

    [ObservableProperty]
    private Transform3D? model2Transform;

    [ObservableProperty]
    private Transform3D? model3Transform;

    [ObservableProperty]
    private Transform3D? model4Transform;

    [ObservableProperty]
    private Transform3D? gridTransform;

    [ObservableProperty]
    private Vector3D directionalLightDirection;

    [ObservableProperty]
    private Color directionalLightColor;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private bool enableArrowHeadTail = false;

    partial void OnEnableArrowHeadTailChanged(bool value)
    {
        var texture = LineMaterial?.Texture;
        var tscale = LineMaterial?.TextureScale ?? 0.4;
        LineMaterial = value ?
            new LineArrowHeadTailMaterial()
            {
                ArrowSize = 0.04,
                Color = Colors.White,
                Texture = texture,
                TextureScale = tscale
            }
            : new LineArrowHeadMaterial()
            {
                ArrowSize = 0.04,
                Color = Colors.White,
                Texture = texture,
                TextureScale = tscale
            };
    }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        this.Title = "Line Shading Demo (HelixToolkitDX)";
        this.SubTitle = string.Empty;

        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(0, 5, 5), LookDirection = new Vector3D(-0, -5, -5), UpDirection = new Vector3D(0, 1, 0) };

        // setup lighting            
        this.AmbientLightColor = Colors.DimGray;
        this.DirectionalLightColor = Colors.White;
        this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // floor plane grid
        this.Grid = LineBuilder.GenerateGrid();
        this.GridColor = Media.Colors.Black;
        this.GridTransform = new TranslateTransform3D(-5, -1, -5);

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0), 0.5f);
        b1.AddBox(new Vector3(0, 0, 0), 1, 0.5f, 2, BoxFaces.All);
        this.Model = b1.ToMeshGeometry3D();

        // lines model3d
        var e1 = new LineBuilder();
        e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
        //this.Lines = e1.ToLineGeometry3D().ToUnshared();
        this.Lines = e1.ToLineGeometry3D(true);
        this.Lines.Colors = new Color4Collection();
        var linesCount = this.Lines.Indices?.Count ?? 0;
        var rnd = new Random();
        while (linesCount-- > 0)
        {
            this.Lines.Colors.Add(rnd.NextColor());
        }

        // lines params
        this.LineThickness = 2;
        this.LineSmoothness = 2.0;
        this.LinesEnabled = true;
        this.GridEnabled = true;

        // model trafos
        this.Model1Transform = new TranslateTransform3D(0, 0, 0);
        this.Model2Transform = new TranslateTransform3D(-2, 0, 0);
        this.Model3Transform = new TranslateTransform3D(+2, 0, 0);
        this.Model4Transform = new TranslateTransform3D(0, 2, 0);
        // model materials
        this.Material1 = PhongMaterials.PolishedGold;
        this.Material2 = PhongMaterials.Copper;
        this.Material3 = PhongMaterials.Glass;
        this.LineMaterial = new LineArrowHeadMaterial() { ArrowSize = 0.04, Color = Colors.White, TextureScale = 0.4 };
        this.GridMaterial = new LineMaterial() { Color = Colors.Red, TextureScale = 0.4 };
        var dash = TextureModel.Create("Dash.png");
        var dotLine = TextureModel.Create("DotLine.png");
        GridMaterial.Texture = dotLine;
        LineMaterial.Texture = dash;
    }
}
