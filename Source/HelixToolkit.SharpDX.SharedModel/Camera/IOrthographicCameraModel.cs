#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public interface IOrthographicCameraModel : IProjectionCameraModel
{
    double Width
    {
        set; get;
    }
    void AnimateWidth(double newWidth, double animationTime);
}
