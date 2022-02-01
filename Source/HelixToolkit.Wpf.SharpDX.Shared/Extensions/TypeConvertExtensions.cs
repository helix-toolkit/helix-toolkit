
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene2D;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Model.Scene2D;
#endif
    namespace Extensions
    {
        public static class TypeConvertExtensions
        {
            public static Visibility ToD2DVisibility(this System.Windows.Visibility v)
            {
                switch (v)
                {
                    case System.Windows.Visibility.Collapsed:
                        return Visibility.Collapsed;
                    case System.Windows.Visibility.Hidden:
                        return Visibility.Hidden;
                    case System.Windows.Visibility.Visible:
                        return Visibility.Visible;
                    default:
                        return Visibility.Visible;

                }
            }

            public static HorizontalAlignment ToD2DHorizontalAlignment(this System.Windows.HorizontalAlignment v)
            {
                switch (v)
                {
                    case System.Windows.HorizontalAlignment.Center:
                        return HorizontalAlignment.Center;
                    case System.Windows.HorizontalAlignment.Left:
                        return HorizontalAlignment.Left;
                    case System.Windows.HorizontalAlignment.Right:
                        return HorizontalAlignment.Right;
                    case System.Windows.HorizontalAlignment.Stretch:
                        return HorizontalAlignment.Stretch;
                    default:
                        return HorizontalAlignment.Center;
                }
            }

            public static VerticalAlignment ToD2DVerticalAlignment(this System.Windows.VerticalAlignment v)
            {
                switch (v)
                {
                    case System.Windows.VerticalAlignment.Center:
                        return VerticalAlignment.Center;
                    case System.Windows.VerticalAlignment.Top:
                        return VerticalAlignment.Top;
                    case System.Windows.VerticalAlignment.Bottom:
                        return VerticalAlignment.Bottom;
                    case System.Windows.VerticalAlignment.Stretch:
                        return VerticalAlignment.Stretch;
                    default:
                        return VerticalAlignment.Center;
                }
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
    }
}
