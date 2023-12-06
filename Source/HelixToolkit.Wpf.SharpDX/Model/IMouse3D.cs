using System.Windows;

namespace HelixToolkit.Wpf.SharpDX;

public interface IMouse3D
{
    event RoutedEventHandler MouseDown3D;
    event RoutedEventHandler MouseUp3D;
    event RoutedEventHandler MouseMove3D;
}
