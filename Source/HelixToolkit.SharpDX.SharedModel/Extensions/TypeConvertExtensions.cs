using Scene2D = HelixToolkit.SharpDX.Model.Scene2D;
#if WINUI
using UIHorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using UIVerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;
using UIVisibility = Microsoft.UI.Xaml.Visibility;
using UIThickness = Microsoft.UI.Xaml.Thickness;
using UIOrientation = Microsoft.UI.Xaml.Controls.Orientation;
#else
using UIHorizontalAlignment = System.Windows.HorizontalAlignment;
using UIVerticalAlignment = System.Windows.VerticalAlignment;
using UIVisibility = System.Windows.Visibility;
using UIThickness = System.Windows.Thickness;
using UIOrientation = System.Windows.Controls.Orientation;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Extensions;
#else
namespace HelixToolkit.Wpf.SharpDX.Extensions;
#endif

public static class TypeConvertExtensions
{
    public static Scene2D.Visibility ToD2DVisibility(this UIVisibility v)
    {
        return v switch
        {
            UIVisibility.Collapsed => Scene2D.Visibility.Collapsed,
#if WPF
            UIVisibility.Hidden => Scene2D.Visibility.Hidden,
#endif
            UIVisibility.Visible => Scene2D.Visibility.Visible,
            _ => Scene2D.Visibility.Visible,
        };
    }

    public static Scene2D.HorizontalAlignment ToD2DHorizontalAlignment(this UIHorizontalAlignment v)
    {
        return v switch
        {
            UIHorizontalAlignment.Center => Scene2D.HorizontalAlignment.Center,
            UIHorizontalAlignment.Left => Scene2D.HorizontalAlignment.Left,
            UIHorizontalAlignment.Right => Scene2D.HorizontalAlignment.Right,
            UIHorizontalAlignment.Stretch => Scene2D.HorizontalAlignment.Stretch,
            _ => Scene2D.HorizontalAlignment.Center,
        };
    }

    public static Scene2D.VerticalAlignment ToD2DVerticalAlignment(this UIVerticalAlignment v)
    {
        return v switch
        {
            UIVerticalAlignment.Center => Scene2D.VerticalAlignment.Center,
            UIVerticalAlignment.Top => Scene2D.VerticalAlignment.Top,
            UIVerticalAlignment.Bottom => Scene2D.VerticalAlignment.Bottom,
            UIVerticalAlignment.Stretch => Scene2D.VerticalAlignment.Stretch,
            _ => Scene2D.VerticalAlignment.Center,
        };
    }

    public static Scene2D.Thickness ToD2DThickness(this UIThickness t)
    {
        return new Scene2D.Thickness((float)t.Left, (float)t.Right, (float)t.Top, (float)t.Bottom);
    }

    public static Scene2D.Orientation ToD2DOrientation(this UIOrientation o)
    {
        return o == UIOrientation.Horizontal ? Scene2D.Orientation.Horizontal : Scene2D.Orientation.Vertical;
    }
}
