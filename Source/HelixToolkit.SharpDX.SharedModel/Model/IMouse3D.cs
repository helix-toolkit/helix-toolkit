#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public interface IMouse3D
{
    event RoutedEventHandler MouseDown3D;
    event RoutedEventHandler MouseUp3D;
    event RoutedEventHandler MouseMove3D;
}
