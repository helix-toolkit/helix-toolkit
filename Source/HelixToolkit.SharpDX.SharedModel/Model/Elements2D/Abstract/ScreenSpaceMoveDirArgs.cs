#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public sealed class ScreenSpaceMoveDirArgs : EventArgs
{
    public readonly ScreenSpaceMoveDirection Direction;

    public ScreenSpaceMoveDirArgs(ScreenSpaceMoveDirection direction)
    {
        Direction = direction;
    }
}
