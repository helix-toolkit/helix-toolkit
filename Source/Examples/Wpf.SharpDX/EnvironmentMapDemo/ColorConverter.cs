using HelixToolkit.Wpf.SharpDX;
using System;
using System.Windows.Data;

namespace EnvironmentMapDemo;

public class ColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var c = (Color4)value;
        return c.ToColor();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var c = (System.Windows.Media.Color)value;
        return c.ToColor4();
    }
}
