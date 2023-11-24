using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicCodeSurfaceDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private double parameterW = 1;

    [ObservableProperty]
    private int meshSizeU = 120;

    [ObservableProperty]
    private int meshSizeV = 120;

    [ObservableProperty]
    private Material? material;

    public string[] Materials { private set; get; } = Array.Empty<string>();

    public string[] Models { private set; get; } = Array.Empty<string>();

    private List<Uri> sourceCodeUri { get; } = new();
    private readonly Dictionary<string, string> fileDict = new();
    private readonly Dictionary<string, Material> materialDict = new();

    [ObservableProperty]
    private string selectedModel = string.Empty;

    partial void OnSelectedModelChanged(string value)
    {
        LoadSourceCode();
    }

    [ObservableProperty]
    private string selectedMaterial = string.Empty;

    partial void OnSelectedMaterialChanged(string value)
    {
        Material = materialDict[value];
    }

    [ObservableProperty]
    private string sourceCode = string.Empty;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new OrthographicCamera()
        {
            Position = new System.Windows.Media.Media3D.Point3D(0, 0, -10),
            LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 10),
            UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
            FarPlaneDistance = 2000,
            NearPlaneDistance = 1
        };
        //foreach (string m in Models)
        //{
        //    var uri = new Uri(String.Format("pack://application:,,/Expressions/{0}.txt", m));
        //    sourceCodeUri.Add(uri);
        //}

        var dir = "Expressions";
        if (!Directory.Exists(dir))
            return;
        string[] files = Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            fileDict.Add(Path.GetFileNameWithoutExtension(file), Path.GetFullPath(file));
        }
        Models = fileDict.Keys.ToArray();

        materialDict.Add("Normal", new NormalMaterial());
        materialDict.Add("Position", new PositionColorMaterial());
        materialDict.Add("Copper", PhongMaterials.Copper);
        materialDict.Add("Chrome", PhongMaterials.Chrome);
        materialDict.Add("BlackRubber", PhongMaterials.BlackRubber);
        materialDict.Add("Pearl", PhongMaterials.Pearl);
        materialDict.Add("PolishedBronze", PhongMaterials.PolishedBronze);
        materialDict.Add("ColorStripe", new ColorStripeMaterial() { ColorStripeX = GetGradients(Color.Red, Color.Green, Color.Blue, 48).ToArray() });
        materialDict.Add("Diffuse", DiffuseMaterials.Orange);
        Materials = materialDict.Keys.ToArray();
        SelectedMaterial = "Normal";
    }

    void LoadSourceCode()
    {
        if (fileDict.TryGetValue(SelectedModel, out string? filePath))
        {
            using var reader = File.OpenRead(filePath);
            using var strReader = new StreamReader(reader);
            SourceCode = strReader.ReadToEnd();
        }
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
}
