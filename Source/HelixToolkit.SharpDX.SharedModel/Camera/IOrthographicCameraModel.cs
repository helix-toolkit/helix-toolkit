#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface IOrthographicCameraModel : IProjectionCameraModel
{
    double Width
    {
        set; get;
    }
    void AnimateWidth(double newWidth, double animationTime);
}
