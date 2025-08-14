using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

public sealed class AmbientLight3D : Light3D
{
    protected override SceneNode OnCreateSceneNode()
    {
        return new AmbientLightNode();
    }
}
