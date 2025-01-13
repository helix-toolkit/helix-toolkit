using HelixToolkit.SharpDX.Cameras;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
public interface ICameraModel
{
    /// <summary>
    /// 
    /// </summary>
    bool CreateLeftHandSystem
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    Point3D Position
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    Vector3D LookDirection
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    Vector3D UpDirection
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    CameraCore CameraInternal
    {
        get;
    }
    void AnimateTo(
        Point3D newPosition,
        Vector3D newDirection,
        Vector3D newUpDirection,
        double animationTime);
    void StopAnimation();
    bool OnTimeStep();
}
