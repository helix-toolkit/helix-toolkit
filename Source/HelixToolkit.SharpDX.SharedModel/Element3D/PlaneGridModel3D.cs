#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;

namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;

    public class PlaneGridModel3D : Element3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new PlaneGridNode();
        }
    }
}
