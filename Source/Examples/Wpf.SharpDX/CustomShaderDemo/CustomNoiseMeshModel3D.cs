using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;

namespace CustomShaderDemo;

public class CustomNoiseMeshModel3D : MeshGeometryModel3D
{
    protected override SceneNode OnCreateSceneNode()
    {
        var node = base.OnCreateSceneNode();
        node.OnSetRenderTechnique = (host) => node?.EffectsManager?[CustomShaderNames.NoiseMesh];
        return node;
    }
}
