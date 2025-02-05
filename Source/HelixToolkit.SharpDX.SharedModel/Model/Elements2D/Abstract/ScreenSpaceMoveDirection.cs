#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
public enum ScreenSpaceMoveDirection
{
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom
};
