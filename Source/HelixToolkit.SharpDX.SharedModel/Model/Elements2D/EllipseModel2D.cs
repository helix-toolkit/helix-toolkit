using HelixToolkit.SharpDX.Model.Scene2D;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public class EllipseModel2D : ShapeModel2D
{
    protected override SceneNode2D OnCreateSceneNode()
    {
        return new EllipseNode2D();
    }
}
