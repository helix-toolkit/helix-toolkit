#if WINUI
using Microsoft.UI.Xaml.Data;
#else
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public sealed class BoolToVisibilityConverter : IValueConverter
{
#if WINUI
    public object? Convert(object? value, Type targetType, object? parameter, string language)
#else
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#endif
    {
        if (value is bool v)
        {
            return v ? Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            return Visibility.Visible;
        }
    }

#if WINUI
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#else
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
    {
        if (value is Visibility v)
        {
            return v == Visibility.Visible;
        }
        else
        {
            return true;
        }
    }
}
