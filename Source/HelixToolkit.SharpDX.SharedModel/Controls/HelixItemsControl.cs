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

public class HelixItemsControl : ItemsControl
{
    public HelixItemsControl()
    {
#if false
#elif WINUI
        ManipulationMode = ManipulationModes.None;
#elif WPF
        Focusable = false;
#else
#error Unknown framework
#endif
        IsHitTestVisible = false;
        this.DefaultStyleKey = typeof(HelixItemsControl);
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
