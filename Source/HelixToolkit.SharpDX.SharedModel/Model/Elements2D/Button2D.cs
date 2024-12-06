#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

public class Button2D : Clickable2D
{
    static Button2D()
    {
#if WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Button2D), new FrameworkPropertyMetadata(typeof(Button2D)));
#endif
    }
}
