#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Controls;
#else
namespace HelixToolkit.Wpf.SharpDX.Controls;
#endif

public class HelixItemsControl : ItemsControl
{
    public HelixItemsControl()
    {
#if WINUI
        ManipulationMode = ManipulationModes.None;
#else
        Focusable = false;
#endif

        Visibility = Visibility.Collapsed;
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
