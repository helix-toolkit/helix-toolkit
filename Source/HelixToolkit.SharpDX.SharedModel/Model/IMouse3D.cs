#if false
#elif WINUI
#elif WPF
using System.Windows;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public interface IMouse3D
{
#if false
#elif WINUI
    event EventHandler<MouseDown3DEventArgs>? Mouse3DDown;
    event EventHandler<MouseUp3DEventArgs>? Mouse3DUp;
    event EventHandler<MouseMove3DEventArgs>? Mouse3DMove;
#elif WPF
    event RoutedEventHandler MouseDown3D;
    event RoutedEventHandler MouseUp3D;
    event RoutedEventHandler MouseMove3D;
#else
#error Unknown framework
#endif
}
