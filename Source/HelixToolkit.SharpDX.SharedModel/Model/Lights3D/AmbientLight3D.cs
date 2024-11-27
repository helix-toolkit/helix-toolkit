using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public sealed class AmbientLight3D : Light3D
{
    protected override SceneNode OnCreateSceneNode()
    {
        return new AmbientLightNode();
    }
}
