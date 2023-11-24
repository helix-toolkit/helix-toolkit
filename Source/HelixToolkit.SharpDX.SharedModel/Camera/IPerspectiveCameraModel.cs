#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface IPerspectiveCameraModel
{
    double FieldOfView
    {
        set; get;
    }
}
