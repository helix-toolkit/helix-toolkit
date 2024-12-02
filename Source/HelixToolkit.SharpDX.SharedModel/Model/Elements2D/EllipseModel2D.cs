using HelixToolkit.SharpDX.Model.Scene2D;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public class EllipseModel2D : ShapeModel2D
{
    protected override SceneNode2D OnCreateSceneNode()
    {
        return new EllipseNode2D();
    }
}
