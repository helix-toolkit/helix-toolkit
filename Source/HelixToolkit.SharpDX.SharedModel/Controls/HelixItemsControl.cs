#if WINUI
#else
using System.Windows;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
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
