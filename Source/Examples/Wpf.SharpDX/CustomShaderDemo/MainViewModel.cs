using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomShaderDemo.Materials;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace CustomShaderDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    public MeshGeometry3D Model { get; private set; }
    public MeshGeometry3D SphereModel { get; private set; }
    public LineGeometry3D AxisModel { get; private set; }
    public BillboardText3D AxisLabel { private set; get; }
    public ColorStripeMaterial ModelMaterial { get; private set; } = new ColorStripeMaterial();
    public PhongMaterial SphereMaterial { private set; get; } = PhongMaterials.Copper;

    public PointGeometry3D PointModel { private set; get; }

    public CustomPointMaterial CustomPointMaterial { get; }

    public Transform3D PointTransform { get; } = new TranslateTransform3D(10, 0, 0);

    [ObservableProperty]
    private Color startColor;

    partial void OnStartColorChanged(Color value)
    {
        ColorGradient = new Color4Collection(GetGradients(value.ToColor4(), MidColor.ToColor4(), EndColor.ToColor4(), 100));
    }

    [ObservableProperty]
    private Color midColor;

    partial void OnMidColorChanged(Color value)
    {
        ColorGradient = new Color4Collection(GetGradients(StartColor.ToColor4(), value.ToColor4(), EndColor.ToColor4(), 100));
    }

    [ObservableProperty]
    private Color endColor;

    partial void OnEndColorChanged(Color value)
    {
        ColorGradient = new Color4Collection(GetGradients(StartColor.ToColor4(), MidColor.ToColor4(), value.ToColor4(), 100));
    }

    [ObservableProperty]
    private Color4Collection colorGradient = new();

    partial void OnColorGradientChanged(Color4Collection value)
    {
        ModelMaterial.ColorStripeX = value;
    }

    [ObservableProperty]
    private FillMode fillMode = FillMode.Solid;

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
    {
        FillMode = value ? FillMode.Wireframe : FillMode.Solid;
    }

    private const int Width = 100;
    private const int Height = 100;

    public MainViewModel()
    {
        // titles
        Title = "Simple Demo";
        SubTitle = "WPF & SharpDX";

        // camera setup
        Camera = new PerspectiveCamera
        {
            Position = new Point3D(-6, 8, 23),
            LookDirection = new Vector3D(11, -4, -23),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 5000
        };

        EffectsManager = new CustomEffectsManager();

        var builder = new MeshBuilder(true);
        Vector3[] points = new Vector3[Width * Height];
        for (int i = 0; i < Width; ++i)
        {
            for (int j = 0; j < Height; ++j)
            {
                points[i * Width + j] = new Vector3(i / 10f, 0, j / 10f);
            }
        }
        builder.AddRectangularMesh(points.ToList(), Width);
        Model = builder.ToMeshGeometry3D();

        if (Model.Normals is not null)
        {
            for (int i = 0; i < Model.Normals.Count; ++i)
            {
                Model.Normals[i] = new Vector3(0, Math.Abs(Model.Normals[i].Y), 0);
            }
        }

        StartColor = Colors.Blue;
        MidColor = Colors.Green;
        EndColor = Colors.Red;

        var lineBuilder = new LineBuilder();
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
        lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));

        AxisModel = lineBuilder.ToLineGeometry3D();
        AxisModel.Colors = new Color4Collection(AxisModel.Positions?.Count ?? 0)
        {
            Colors.Red.ToColor4(),
            Colors.Red.ToColor4(),
            Colors.Green.ToColor4(),
            Colors.Green.ToColor4(),
            Colors.Blue.ToColor4(),
            Colors.Blue.ToColor4()
        };

        AxisLabel = new BillboardText3D();
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(11, 0, 0), Text = "X", Foreground = Colors.Red.ToColor4() });
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 11, 0), Text = "Y", Foreground = Colors.Green.ToColor4() });
        AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 0, 11), Text = "Z", Foreground = Colors.Blue.ToColor4() });

        builder = new MeshBuilder(true);
        builder.AddSphere(new Vector3(-15, 0, 0), 5);
        SphereModel = builder.ToMeshGeometry3D();

        GenerateNoise();

        PointModel = new PointGeometry3D()
        {
            Positions = SphereModel.Positions
        };

        CustomPointMaterial = new CustomPointMaterial() { Color = Colors.White };
    }

    public static IEnumerable<Color4> GetGradients(Color4 start, Color4 mid, Color4 end, int steps)
    {
        return GetGradients(start, mid, steps / 2).Concat(GetGradients(mid, end, steps / 2));
    }

    public static IEnumerable<Color4> GetGradients(Color4 start, Color4 end, int steps)
    {
        float stepA = ((end.Alpha - start.Alpha) / (steps - 1));
        float stepR = ((end.Red - start.Red) / (steps - 1));
        float stepG = ((end.Green - start.Green) / (steps - 1));
        float stepB = ((end.Blue - start.Blue) / (steps - 1));

        for (int i = 0; i < steps; i++)
        {
            yield return new Color4((start.Red + (stepR * i)),
                                        (start.Green + (stepG * i)),
                                        (start.Blue + (stepB * i)),
                                        (start.Alpha + (stepA * i)));
        }
    }

    [RelayCommand]
    private void GenerateNoise()
    {
        Noise2d.GenerateNoiseMap(Width, Height, 8, out float[] noise);
        Vector2Collection collection = new(Width * Height);

        for (int i = 0; i < Width; ++i)
        {
            for (int j = 0; j < Height; ++j)
            {
                collection.Add(new Vector2(Math.Abs(noise[Width * i + j]), 0));
            }
        }

        Model.TextureCoordinates = collection;
    }
}
