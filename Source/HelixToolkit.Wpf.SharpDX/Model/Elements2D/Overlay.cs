

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;
    internal sealed class Overlay : Panel2D
    {
        protected override SceneNode2D OnCreateSceneNode()
        {
            return new OverlayNode2D();
        }
    }
}
