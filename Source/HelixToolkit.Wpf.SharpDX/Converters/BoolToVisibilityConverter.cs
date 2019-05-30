using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HelixToolkit.Wpf.SharpDX.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool v)
            {
                return v ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Visibility v)
            {
                return v == Visibility.Visible ? true : false;
            }
            else
            {
                return true;
            }
        }
    }
}
