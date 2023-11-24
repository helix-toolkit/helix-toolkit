using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System;

namespace ViewMatrix;

public sealed class MatrixConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var m = (Matrix3D)value;
        var sb = new StringBuilder();
        sb.AppendFormat("{0,8:0.000} {1,8:0.000} {2,8:0.000} {3,8:0.000}\n", m.M11, m.M12, m.M13, m.M14);
        sb.AppendFormat("{0,8:0.000} {1,8:0.000} {2,8:0.000} {3,8:0.000}\n", m.M21, m.M22, m.M23, m.M24);
        sb.AppendFormat("{0,8:0.000} {1,8:0.000} {2,8:0.000} {3,8:0.000}\n", m.M31, m.M32, m.M33, m.M34);
        sb.AppendFormat("{0,8:0.000} {1,8:0.000} {2,8:0.000} {3,8:0.000}\n", m.OffsetX, m.OffsetY, m.OffsetZ, m.M44);
        return sb.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
