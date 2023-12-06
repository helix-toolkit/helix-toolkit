namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public sealed class ScreenSpaceMoveDirArgs : EventArgs
{
    public readonly ScreenSpaceMoveDirection Direction;

    public ScreenSpaceMoveDirArgs(ScreenSpaceMoveDirection direction)
    {
        Direction = direction;
    }
}
