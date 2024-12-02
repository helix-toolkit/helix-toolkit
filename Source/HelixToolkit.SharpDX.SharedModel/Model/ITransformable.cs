using HelixToolkit.SharpDX;
#if WINUI
using Microsoft.UI.Xaml.Media.Media3D;
#else
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface ITransformable : ITransform
{
    Transform3D Transform
    {
        get; set;
    }
}
