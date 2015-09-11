using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace HelixToolkit.Wpf.SharpDX
{
    public class RenderTechniqueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((KeyValuePair<string, RenderTechnique>)value).Value;
        }
    }
}
