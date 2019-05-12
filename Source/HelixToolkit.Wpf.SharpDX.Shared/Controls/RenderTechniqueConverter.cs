using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

#if COREWPF
namespace HelixToolkit.SharpDX.Core.Wpf
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public class RenderTechniqueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((KeyValuePair<string, IRenderTechnique>)value).Value;
        }
    }
}
