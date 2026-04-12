#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
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
