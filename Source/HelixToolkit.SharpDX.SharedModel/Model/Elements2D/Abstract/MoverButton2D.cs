#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

/// <summary>
/// Use to apply style for mover button from Generic.xaml/>
/// </summary>
/// <seealso cref="Button2D" />
public sealed class MoverButton2D : Button2D
{
    static MoverButton2D()
    {
#if WPF
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MoverButton2D), new FrameworkPropertyMetadata(typeof(MoverButton2D)));
#endif
    }
}
