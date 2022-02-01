

#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene2D;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Model.Scene2D;
#endif
    namespace Elements2D
    {
        internal sealed class Overlay : Panel2D
        {
            protected override SceneNode2D OnCreateSceneNode()
            {
                return new OverlayNode2D();
            }
        }
    }
}
