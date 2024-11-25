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

/// <summary>
/// A not-null reference to Visibility value converter.
/// </summary>
#if WPF
[ValueConversion(typeof(object), typeof(Visibility))]
#endif
public sealed class NotNullToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref = "NotNullToVisibilityConverter" /> class.
    /// </summary>
    public NotNullToVisibilityConverter()
    {
        this.Inverted = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this converter is inverted.
    /// </summary>
    public bool Inverted { get; set; }

#if WINUI
    public object? Convert(object? value, Type targetType, object? parameter, string language)
#else
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
#endif
    {
        if (targetType == typeof(Visibility))
        {
            bool isNotNull = value != null;
            if (isNotNull != this.Inverted)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        return null;
    }

#if WINUI
    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
#else
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
#endif
    {
        throw new NotImplementedException();
    }
}
