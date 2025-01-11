using HelixToolkit.SharpDX;
#if false
#elif WINUI
using Microsoft.UI.Xaml.Media.Media3D;
#elif WPF
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public interface ITransformable : ITransform
{
    Transform3D Transform
    {
        get; set;
    }
}
