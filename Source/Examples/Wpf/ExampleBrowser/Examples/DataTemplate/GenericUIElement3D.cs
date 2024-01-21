using DependencyPropertyGenerator;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DataTemplate;

[DependencyProperty<Color>("Color")]
[DependencyProperty<Material>("Material")]
public partial class GenericUIElement3D : UIElement3D
{
    protected GeometryModel3D Model { get; set; }

    public GenericUIElement3D()
    {
        Model = new GeometryModel3D();
        BindingOperations.SetBinding(Model, GeometryModel3D.MaterialProperty, new Binding(nameof(Material)) { Source = this });
        Visual3DModel = Model;
        SetGeometry();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        MessageBox.Show("OnMouseDown raised. " + Color.ToString());
    }

    private void SetGeometry()
    {
        var meshBuilder = new MeshBuilder(false, false);
        meshBuilder.AddBox(new System.Numerics.Vector3(0, 0, 0), 0.5f, 0.5f, 0.5f);
        Model.Geometry = meshBuilder.ToMesh().ToWndMeshGeometry3D();
    }

    partial void OnColorChanged()
    {
        Material = MaterialHelper.CreateMaterial(Color);
    }
}
