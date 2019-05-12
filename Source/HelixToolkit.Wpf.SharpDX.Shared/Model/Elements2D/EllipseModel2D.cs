#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene2D;
namespace HelixToolkit.SharpDX.Core.Wpf
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF
    using Model.Scene2D;
#endif
    namespace Elements2D
    {
        public class EllipseModel2D : ShapeModel2D
        {
            protected override SceneNode2D OnCreateSceneNode()
            {
                return new EllipseNode2D();
            }
        }
    }

}
