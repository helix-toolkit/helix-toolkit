#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface IProjectionCameraModel : ICameraModel
{
    double FarPlaneDistance
    {
        set; get;
    }
    double NearPlaneDistance
    {
        set; get;
    }
}
