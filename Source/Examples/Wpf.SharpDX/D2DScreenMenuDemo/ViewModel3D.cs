using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

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
        var builder = new MeshBuilder(true, true, true);
        builder.AddBox(new Vector3(0, 2.5f, 0), 5, 5, 5);
        builder.AddBox(new Vector3(0, 0, 0), 10, 0.1f, 10);
        Model = builder.ToMeshGeometry3D();
        var diffuseMap = TextureModel.Create(new System.Uri(Texture, System.UriKind.RelativeOrAbsolute).ToString());
        var normalMap = TextureModel.Create(new System.Uri(NormalTexture, System.UriKind.RelativeOrAbsolute).ToString());
        ModelMaterial.DiffuseMap = diffuseMap;
        ModelMaterial.NormalMap = normalMap;
    }
}
