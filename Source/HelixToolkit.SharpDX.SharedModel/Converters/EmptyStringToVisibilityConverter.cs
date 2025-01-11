#if false
#elif WINUI
using Microsoft.UI.Xaml.Data;
#elif WPF
using System.Globalization;
using System.Windows;
using System.Windows.Data;
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

public sealed class EmptyStringToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref = "EmptyStringToVisibilityConverter" /> class.
    /// </summary>
    public EmptyStringToVisibilityConverter()
    {
        this.Inverted = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this converter is inverted.
    /// </summary>
    public bool Inverted { get; set; }

#if false
#elif WINUI
    public object? Convert(object? value, Type targetType, object? parameter, string language)
#elif WPF
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
#else
#error Unknown framework
#endif
    {
        if (targetType == typeof(Visibility) && value is string s)
        {
            bool isNotNullOrEmpty = !string.IsNullOrEmpty(s);
            if (isNotNullOrEmpty != this.Inverted)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        return null;
    }

#if false
#elif WINUI
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#elif WPF
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
#else
#error Unknown framework
#endif
    {
        throw new NotImplementedException();
    }
}
