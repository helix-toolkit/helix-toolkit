#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    public class DynamicReflectionMap3D : GroupModel3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new DynamicReflectionNode();
        }
    }
}
