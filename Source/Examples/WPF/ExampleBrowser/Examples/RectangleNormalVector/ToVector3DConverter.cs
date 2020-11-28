using System;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace RectangleNormalVector
{
    public class ToVector3DConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, 
            object parameter, System.Globalization.CultureInfo culture)
        {
            var result = new Vector3D {X = (double) values[0], Y = (double) values[1], Z = (double) values[2]};
            return result;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, 
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
