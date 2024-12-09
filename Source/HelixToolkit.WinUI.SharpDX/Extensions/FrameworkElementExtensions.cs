using HelixToolkit.WinUI.SharpDX.Elements2D;

namespace HelixToolkit.WinUI.SharpDX;

internal static class FrameworkElementExtensions
{
    internal static void AddLogicalChild(this UIElement element, object child)
    {
    }

    internal static void RemoveLogicalChild(this UIElement element, object child)
    {
    }

    internal static void RaiseEvent(this FrameworkElement element, RoutedEventArgs e)
    {
        if (element is Element2D element2D)
        {
            if (e is Mouse2DEventArgs args)
            {
                if (args.RoutedEvent == Element2D.MouseEnter2DEvent)
                {
                    element2D.OnMouseEnter2D(args);
                }
                else if (args.RoutedEvent == Element2D.MouseLeave2DEvent)
                {
                    element2D.OnMouseLeave2D(args);
                }
            }
        }
    }
}
