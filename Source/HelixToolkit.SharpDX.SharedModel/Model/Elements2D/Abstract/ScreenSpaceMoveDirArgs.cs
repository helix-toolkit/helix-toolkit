#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public sealed class ScreenSpaceMoveDirArgs : EventArgs
{
    public readonly ScreenSpaceMoveDirection Direction;

    public ScreenSpaceMoveDirArgs(ScreenSpaceMoveDirection direction)
    {
        Direction = direction;
    }
}
