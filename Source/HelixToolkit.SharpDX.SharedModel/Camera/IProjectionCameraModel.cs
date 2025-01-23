﻿#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
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