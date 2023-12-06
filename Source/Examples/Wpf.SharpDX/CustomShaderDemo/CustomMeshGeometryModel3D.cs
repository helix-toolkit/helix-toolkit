using DependencyPropertyGenerator;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;

namespace CustomShaderDemo;

[DependencyProperty<double>("HeightScale", DefaultValue = 5.0)]
public partial class CustomMeshGeometryModel3D : MeshGeometryModel3D
{
    partial void OnHeightScaleChanged(double newValue)
    {
        if (SceneNode is CustomMeshNode node)
        {
            node.HeightScale = (float)newValue;
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new CustomMeshNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        base.AssignDefaultValuesToSceneNode(core);

        if (core is CustomMeshNode node)
        {
            node.HeightScale = (float)HeightScale;
        }
    }
}
