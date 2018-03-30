using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class Button2D : Clickable2D
    {
        static Button2D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Button2D), new FrameworkPropertyMetadata(typeof(Button2D)));
        }
    }
}