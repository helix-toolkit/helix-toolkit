using HelixToolkit.SharpDX.Model.Scene2D;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

internal sealed class Overlay : Panel2D
{
    protected override SceneNode2D OnCreateSceneNode()
    {
        return new OverlayNode2D();
    }
}
