using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX.Extensions
{
    public static class TypeConvertExtensions
    {
        public static Model.Scene2D.Visibility ToD2DVisibility(this System.Windows.Visibility v)
        {
            switch (v)
            {
                case System.Windows.Visibility.Collapsed:
                    return Model.Scene2D.Visibility.Collapsed;
                case System.Windows.Visibility.Hidden:
                    return Model.Scene2D.Visibility.Hidden;
                case System.Windows.Visibility.Visible:
                    return Model.Scene2D.Visibility.Visible;
                default:
                    return Model.Scene2D.Visibility.Visible;

            }
        }

        public static Model.Scene2D.HorizontalAlignment ToD2DHorizontalAlignment(this System.Windows.HorizontalAlignment v)
        {
            switch (v)
            {
                case System.Windows.HorizontalAlignment.Center:
                    return Model.Scene2D.HorizontalAlignment.Center;
                case System.Windows.HorizontalAlignment.Left:
                    return Model.Scene2D.HorizontalAlignment.Left;
                case System.Windows.HorizontalAlignment.Right:
                    return Model.Scene2D.HorizontalAlignment.Right;
                case System.Windows.HorizontalAlignment.Stretch:
                    return Model.Scene2D.HorizontalAlignment.Stretch;
                default:
                    return Model.Scene2D.HorizontalAlignment.Center;
            }
        }

        public static Model.Scene2D.VerticalAlignment ToD2DVerticalAlignment(this System.Windows.VerticalAlignment v)
        {
            switch (v)
            {
                case System.Windows.VerticalAlignment.Center:
                    return Model.Scene2D.VerticalAlignment.Center;
                case System.Windows.VerticalAlignment.Top:
                    return Model.Scene2D.VerticalAlignment.Top;
                case System.Windows.VerticalAlignment.Bottom:
                    return Model.Scene2D.VerticalAlignment.Bottom;
                case System.Windows.VerticalAlignment.Stretch:
                    return Model.Scene2D.VerticalAlignment.Stretch;
                default:
                    return Model.Scene2D.VerticalAlignment.Center;
            }
        }

        public static Model.Scene2D.Thickness ToD2DThickness(this System.Windows.Thickness t)
        {
            return new Model.Scene2D.Thickness((float)t.Left, (float)t.Right, (float)t.Top, (float)t.Bottom);
        }

        public static Model.Scene2D.Orientation ToD2DOrientation(this System.Windows.Controls.Orientation o)
        {
            return o == System.Windows.Controls.Orientation.Horizontal ? Model.Scene2D.Orientation.Horizontal : Model.Scene2D.Orientation.Vertical;
        }
    }
}
