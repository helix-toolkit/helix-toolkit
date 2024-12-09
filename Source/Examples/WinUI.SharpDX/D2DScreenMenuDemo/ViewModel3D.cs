using CommunityToolkit.Mvvm.ComponentModel;
using System.Numerics;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.WinUI.SharpDX;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using Vector3D = System.Numerics.Vector3;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;
using System.Reflection;
using System.IO;

namespace D2DScreenMenuDemo;

public partial class ViewModel3D : ObservableObject
{
    [ObservableProperty]
    private MeshGeometry3D model = new();

    [ObservableProperty]
    private PhongMaterial modelMaterial = PhongMaterials.White;

    [ObservableProperty]
    private Vector3D light1Direction = new(1, -1, -1);

    [ObservableProperty]
    private Color light1Color = Colors.Blue;

    [ObservableProperty]
    private Vector3D light2Direction = new(-1, -1, -1);

    [ObservableProperty]
    private Color light2Color = Colors.Red;

    [ObservableProperty]
    private Vector3D light3Direction = new(-1, -1, 1);

    [ObservableProperty]
    private Color light3Color = Colors.Green;

    private const string NormalTexture = @"TextureCheckerboard2_dot3.jpg";

    private const string Texture = @"TextureCheckerboard2.jpg";

    public ViewModel3D()
    {
        string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var builder = new MeshBuilder(true, true, true);
        builder.AddBox(new Vector3(0, 2.5f, 0), 5, 5, 5);
        builder.AddBox(new Vector3(0, 0, 0), 10, 0.1f, 10);
        Model = builder.ToMeshGeometry3D();
        var diffuseMap = TextureModel.Create(Path.Combine(directory, Texture));
        var normalMap = TextureModel.Create(Path.Combine(directory, NormalTexture));
        ModelMaterial.DiffuseMap = diffuseMap;
        ModelMaterial.NormalMap = normalMap;
    }
}
