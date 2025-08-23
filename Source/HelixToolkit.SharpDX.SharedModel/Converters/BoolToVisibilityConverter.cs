#if false
#elif WINUI
using Microsoft.UI.Xaml.Data;
#elif WPF
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#elif AVALONIA
using Avalonia.Data.Converters;
using System.Globalization;
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

public sealed class BoolToVisibilityConverter : IValueConverter
{
#if false
#elif WINUI
    public object? Convert(object? value, Type targetType, object? parameter, string language)
#elif WPF
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#elif AVALONIA
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#else
#error Unknown framework
#endif
    {
#if AVALONIA
        return value;
#else
        if (value is bool v)
        {
            return v ? Visibility.Visible : Visibility.Collapsed;
        }
        else
        {
            return Visibility.Visible;
        }
#endif
    }

#if false
#elif WINUI
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#elif WPF
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#elif AVALONIA
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
#else
#error Unknown framework
#endif
    {
#if AVALONIA
        return value;
#else
        if (value is Visibility v)
        {
            return v == Visibility.Visible;
        }
        else
        {
            return true;
        }
#endif
    }
}
