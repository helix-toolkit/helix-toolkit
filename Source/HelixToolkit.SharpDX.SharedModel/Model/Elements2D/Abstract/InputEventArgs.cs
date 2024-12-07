#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

#if WINUI
public class InputEventArgs : RoutedEventArgs
{
    public int Timestamp { get; }
}
#endif
