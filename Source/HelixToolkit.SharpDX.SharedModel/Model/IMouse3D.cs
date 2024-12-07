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
#if WINUI
    event EventHandler<MouseDown3DEventArgs>? Mouse3DDown;
    event EventHandler<MouseUp3DEventArgs>? Mouse3DUp;
    event EventHandler<MouseMove3DEventArgs>? Mouse3DMove;
#else
    event RoutedEventHandler MouseDown3D;
    event RoutedEventHandler MouseUp3D;
    event RoutedEventHandler MouseMove3D;
#endif
}
