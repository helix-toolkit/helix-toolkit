#if false
#elif WINUI
#elif WPF
using System.Windows;
#elif AVALONIA
using Avalonia;
using Avalonia.Controls;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

public class HelixItemsControl : ItemsControl
{
    public HelixItemsControl()
    {
#if false
#elif WINUI
        ManipulationMode = ManipulationModes.None;
#elif WPF
        Focusable = false;
#elif AVALONIA
        Focusable = false;
#else
#error Unknown framework
#endif

        IsHitTestVisible = false;

#if WINUI || WPF
        this.DefaultStyleKey = typeof(HelixItemsControl);
#endif
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return new Size();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size();
    }
}
