using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX;

public static class ViewportCommands
{
    public static RoutedCommand Zoom
    {
        get
        {
            return zoom;
        }
    }
    public static RoutedCommand ZoomExtents
    {
        get
        {
            return zoomExtents;
        }
    }
    public static RoutedCommand ZoomRectangle
    {
        get
        {
            return zoomRectangle;
        }
    }
    public static RoutedCommand Pan
    {
        get
        {
            return pan;
        }
    }
    public static RoutedCommand Rotate
    {
        get
        {
            return rotate;
        }
    }
    public static RoutedCommand SetTarget
    {
        get
        {
            return setTarget;
        }
    }
    public static RoutedCommand Reset
    {
        get
        {
            return reset;
        }
    }
    public static RoutedCommand ChangeFieldOfView
    {
        get
        {
            return changeFieldOfView;
        }
    }
    public static RoutedCommand BackView
    {
        get
        {
            return backView;
        }
    }
    public static RoutedCommand FrontView
    {
        get
        {
            return frontView;
        }
    }
    public static RoutedCommand TopView
    {
        get
        {
            return topView;
        }
    }
    public static RoutedCommand BottomView
    {
        get
        {
            return bottomView;
        }
    }
    public static RoutedCommand LeftView
    {
        get
        {
            return leftView;
        }
    }
    public static RoutedCommand RightView
    {
        get
        {
            return rightView;
        }
    }

    private static readonly RoutedCommand zoom = new();
    private static readonly RoutedCommand zoomExtents = new();
    private static readonly RoutedCommand zoomRectangle = new();
    private static readonly RoutedCommand pan = new();
    private static readonly RoutedCommand rotate = new();
    private static readonly RoutedCommand setTarget = new();
    private static readonly RoutedCommand reset = new();
    private static readonly RoutedCommand changeFieldOfView = new();
    private static readonly RoutedCommand backView = new();
    private static readonly RoutedCommand frontView = new();
    private static readonly RoutedCommand topView = new();
    private static readonly RoutedCommand bottomView = new();
    private static readonly RoutedCommand leftView = new();
    private static readonly RoutedCommand rightView = new();
}
