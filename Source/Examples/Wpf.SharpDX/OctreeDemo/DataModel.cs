using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Media3D = System.Windows.Media.Media3D;

namespace OctreeDemo;

public partial class DataModel : ObservableObject
{
    [ObservableProperty]
    private MeshGeometry3D? model = null;

    public readonly Media3D.ScaleTransform3D scaleTransform = new();
    public readonly Media3D.TranslateTransform3D translateTransform = new();
    public Media3D.Transform3DGroup DynamicTransform { get; private set; } = new Media3D.Transform3DGroup();

    [ObservableProperty]
    private PhongMaterial material = PhongMaterials.Red;

    [ObservableProperty]
    private bool highlight = false;

    partial void OnHighlightChanged(bool value)
    {
        if (value)
        {
            //orgMaterial = material;
            Material.EmissiveColor = Color.Yellow;
        }
        else
        {
            Material.EmissiveColor = Color.Transparent;
            //Material = orgMaterial;
        }
    }

    public DataModel()
    {
        DynamicTransform.Children.Add(scaleTransform);
        DynamicTransform.Children.Add(translateTransform);
    }
}
