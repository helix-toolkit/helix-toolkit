﻿using Scene2D = HelixToolkit.SharpDX.Model.Scene2D;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Extensions;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Extensions;
#else
#error Unknown framework
#endif

public static class TypeConvertExtensions
{
    public static Scene2D.Visibility ToD2DVisibility(this UIVisibility v)
    {
        return v switch
        {
            UIVisibility.Collapsed => Scene2D.Visibility.Collapsed,
#if false
#elif WINUI
#elif WPF
            UIVisibility.Hidden => Scene2D.Visibility.Hidden,
#else
#error Unknown framework
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
