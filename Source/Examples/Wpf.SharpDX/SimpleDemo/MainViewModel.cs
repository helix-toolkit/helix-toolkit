using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace SimpleDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private MeshGeometry3D model;

    [ObservableProperty]
    private MeshGeometry3D textModel;

    [ObservableProperty]
    private LineGeometry3D lines;

    [ObservableProperty]
    private LineGeometry3D grid;

    [ObservableProperty]
    private PointGeometry3D points;

    [ObservableProperty]
    private BillboardText3D text;

    [ObservableProperty]
    private BillboardSingleText3D billboard1Model;

    [ObservableProperty]
    private BillboardSingleText3D billboard2Model;

    [ObservableProperty]
    private BillboardSingleText3D billboard3Model;

    [ObservableProperty]
    private BillboardSingleImage3D billboardImageModel;

    [ObservableProperty]
    private PhongMaterial redMaterial;

    [ObservableProperty]
    private PhongMaterial greenMaterial;

    [ObservableProperty]
    private PhongMaterial blueMaterial;

    [ObservableProperty]
    private Color gridColor;

    [ObservableProperty]
    private Transform3D model1Transform;

    [ObservableProperty]
    private Transform3D model2Transform;

    [ObservableProperty]
    private Transform3D model3Transform;

    [ObservableProperty]
    private Transform3D model4Transform;

    [ObservableProperty]
    private Transform3D gridTransform;

    [ObservableProperty]
    private Vector3D directionalLightDirection;

    [ObservableProperty]
    private Color directionalLightColor;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private Vector3D upDirection = new(0, 1, 0);

    public Stream? BackgroundTexture { get; }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        // titles
        Title = "Simple Demo";
        SubTitle = "WPF & SharpDX";

        // camera setup
        Camera = new OrthographicCamera
        {
            Position = new Point3D(3, 3, 5),
            LookDirection = new Vector3D(-3, -3, -5),
            UpDirection = new Vector3D(0, 1, 0),
            FarPlaneDistance = 50000
        };

        // setup lighting            
        AmbientLightColor = Colors.DimGray;
        DirectionalLightColor = Colors.White;
        DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // floor plane grid
        grid = LineBuilder.GenerateGrid(new Vector3(0, 1, 0), -5, 5, -5, 5);
        GridColor = Colors.Black;
        gridTransform = new Media3D.TranslateTransform3D(0, -3, 0);

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0), 0.5f);
        b1.AddBox(new Vector3(0, 0, 0), 1, 0.5f, 2, BoxFaces.All);

        var meshGeometry = b1.ToMeshGeometry3D();
        meshGeometry.Colors = meshGeometry.TextureCoordinates is null 
            ? null 
            : new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4(1f, 1f)));
        model = meshGeometry;

        // lines model3d
        var e1 = new LineBuilder();
        e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
        lines = e1.ToLineGeometry3D();

        var textBuilder = new MeshBuilder();
        textBuilder.ExtrudeText("HelixToolkit.SharpDX", "Arial", System.Windows.FontStyles.Normal, System.Windows.FontWeights.Bold,
            14, new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        textModel = textBuilder.ToMeshGeometry3D();

        // model trafos
        model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
        model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
        model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);
        model4Transform = new Media3D.TranslateTransform3D(-8, 0, -5);

        // model materials
        redMaterial = PhongMaterials.Red;
        greenMaterial = PhongMaterials.Green;
        blueMaterial = PhongMaterials.Blue;
        //var diffColor = this.RedMaterial.DiffuseColor;
        //diffColor.Alpha = 0.5f;
        //this.RedMaterial.DiffuseColor = diffColor;   

        points = new PointGeometry3D();
        var ptPos = new Vector3Collection();
        var ptIdx = new IntCollection();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    ptIdx.Add(ptPos.Count);
                    ptPos.Add(new Vector3(x, y, z));
                }
            }
        }

        Points.Positions = ptPos;
        Points.Indices = ptIdx;

        text = new BillboardText3D();
        int numRows = 11;
        int numColumns = 11;
        string[] texts = new string[]
        {
                "HelixToolkit",
                "abcde",
                "random",
                "SharpDX",
                "DirectX"
        };
        float angle = 0;
        for (var i = 0; i < numRows; i++)
        {
            for (var j = 0; j < numColumns; j++)
            {
                angle += (float)Math.PI / 10;
                Text.TextInfo.Add(new TextInfo(texts[(i + j) % texts.Length], new Vector3((i - numRows / 2), 0.0f, (j - numColumns / 2)))
                {
                    Foreground = new Color4((float)i / numRows, 1 - (float)i / numRows, (float)(numColumns - j) / numColumns, 1f),
                    Background = new Color4(1 - (float)i / numRows, (float)(numColumns - j) / numColumns, (float)i / numRows, 0.8f),
                    Scale = Math.Max(0.01f, (float)i / numRows * 0.02f),
                    Angle = angle
                });
            }
        }

        billboard1Model = new BillboardSingleText3D()
        {
            TextInfo = new TextInfo("Model 1", new Vector3(0, 1, 0)) { Angle = 0 },
            FontColor = Colors.Blue.ToColor4(),
            FontSize = 12,
            BackgroundColor = Colors.Plum.ToColor4(),
            FontStyle = SharpDX.DirectWrite.FontStyle.Italic,
            Padding = new(2),
        };

        var background = Colors.Blue;
        background.A = (byte)120;
        billboard2Model = new BillboardSingleText3D()
        {
            TextInfo = new TextInfo("Model 2", new Vector3(2, 1, 0)) { Angle = -(float)Math.PI / 3 },
            FontSize = 12,
            FontColor = Colors.Green.ToColor4(),
            BackgroundColor = background.ToColor4(),
            FontWeight = SharpDX.DirectWrite.FontWeight.Bold,
            Padding = new(2),
        };
        background = Colors.Purple;
        background.A = (byte)50;
        billboard3Model = new BillboardSingleText3D(2, 0.8f)
        {
            TextInfo = new TextInfo("Model 3", new Vector3(-2, 1, 0)) { Angle = -(float)Math.PI / 6 },
            FontSize = 12,
            FontColor = Colors.Red.ToColor4(),
            BackgroundColor = background.ToColor4(),
            FontFamily = "Times New Roman",
            FontStyle = SharpDX.DirectWrite.FontStyle.Italic,
            Padding = new(2),
        };

        //BillboardImageModel = new BillboardSingleImage3D(CreateBitmapSample()) { MaskColor = Color.Black };
        billboardImageModel = new BillboardSingleImage3D(CreatePNGSample(), 1, 1)
        {
            Angle = -(float)Math.PI / 5,
            Center = new Vector3(2, 2, 0)
        };

        BackgroundTexture =
            BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
            new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
            {
                    new(){ Color = Colors.White.ToRawColor4(), Position = 0f },
                    new(){ Color = Colors.DarkGray.ToRawColor4(), Position = 1f }
            });
    }

    [RelayCommand]
    private void UpX()
    {
        UpDirection = new Vector3D(1, 0, 0);
    }

    [RelayCommand]
    private void UpY()
    {
        UpDirection = new Vector3D(0, 1, 0);
    }

    [RelayCommand]
    private void UpZ()
    {
        UpDirection = new Vector3D(0, 0, 1);
    }

    private BitmapSource CreateBitmapSample()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        //Read the texture description           
        var texDescriptionStream = assembly.GetManifestResourceStream("SimpleDemo.Sample.png");
        var decoder = new PngBitmapDecoder(texDescriptionStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
        return decoder.Frames[0];
    }

    private Stream? CreatePNGSample()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();

        //Read the texture description           
        var texDescriptionStream = assembly.GetManifestResourceStream("SimpleDemo.Sample.png");

        if (texDescriptionStream is not null)
        {
            texDescriptionStream.Position = 0;
        }

        return texDescriptionStream;
    }
}
