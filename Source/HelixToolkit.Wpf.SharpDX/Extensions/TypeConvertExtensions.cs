using HelixToolkit.SharpDX.Model.Scene2D;

namespace HelixToolkit.Wpf.SharpDX.Extensions;

public static class TypeConvertExtensions
{
    public static Visibility ToD2DVisibility(this System.Windows.Visibility v)
    {
        return v switch
        {
            System.Windows.Visibility.Collapsed => Visibility.Collapsed,
            System.Windows.Visibility.Hidden => Visibility.Hidden,
            System.Windows.Visibility.Visible => Visibility.Visible,
            _ => Visibility.Visible,
        };
    }

    public static HorizontalAlignment ToD2DHorizontalAlignment(this System.Windows.HorizontalAlignment v)
    {
        return v switch
        {
            System.Windows.HorizontalAlignment.Center => HorizontalAlignment.Center,
            System.Windows.HorizontalAlignment.Left => HorizontalAlignment.Left,
            System.Windows.HorizontalAlignment.Right => HorizontalAlignment.Right,
            System.Windows.HorizontalAlignment.Stretch => HorizontalAlignment.Stretch,
            _ => HorizontalAlignment.Center,
        };
    }

    public static VerticalAlignment ToD2DVerticalAlignment(this System.Windows.VerticalAlignment v)
    {
        return v switch
        {
            System.Windows.VerticalAlignment.Center => VerticalAlignment.Center,
            System.Windows.VerticalAlignment.Top => VerticalAlignment.Top,
            System.Windows.VerticalAlignment.Bottom => VerticalAlignment.Bottom,
            System.Windows.VerticalAlignment.Stretch => VerticalAlignment.Stretch,
            _ => VerticalAlignment.Center,
        };
    }

    public static Thickness ToD2DThickness(this System.Windows.Thickness t)
    {
        return new Thickness((float)t.Left, (float)t.Right, (float)t.Top, (float)t.Bottom);
    }

    public static Orientation ToD2DOrientation(this System.Windows.Controls.Orientation o)
    {
        return o == System.Windows.Controls.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical;
    }
}
