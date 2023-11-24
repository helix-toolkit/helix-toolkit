using HelixToolkit.SharpDX.Model.Scene;

namespace HelixToolkit.Wpf.SharpDX;

public sealed class AmbientLight3D : Light3D
{
    protected override SceneNode OnCreateSceneNode()
    {
        return new AmbientLightNode();
    }
}
