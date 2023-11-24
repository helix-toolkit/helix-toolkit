using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Windows.Data;

namespace DeferredShadingDemo;

public class ColorVectorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return ((Color4)value).ToColor();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return ((System.Windows.Media.Color)value).ToColor4();
    }
}
