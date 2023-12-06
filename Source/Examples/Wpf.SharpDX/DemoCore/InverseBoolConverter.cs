using System.Globalization;
using System.Windows.Data;

namespace DemoCore;

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool v)
        {
            return !v;
        }
        else
        {
            return true;
        }
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool v)
        {
            return !v;
        }
        else
        {
            return true;
        }
    }
}
