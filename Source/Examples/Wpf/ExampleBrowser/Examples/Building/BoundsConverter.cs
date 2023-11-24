using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System;
using HelixToolkit.Wpf;

namespace Building;

[ValueConversion(typeof(Visual3D), typeof(Rect3D))]
public sealed class BoundsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visual3D visual ? visual.FindBounds(Transform3D.Identity) : Rect3D.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
