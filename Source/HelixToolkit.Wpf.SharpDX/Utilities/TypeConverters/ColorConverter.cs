using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Wpf.SharpDX.Utilities;

public sealed class ColorConverter : FromToStringTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }
        if (value is string source)
        {
            try
            {
                var c = System.Windows.Media.ColorConverter.ConvertFromString(source);
                if (c != null)
                {
                    var color = (System.Windows.Media.Color)c;
                    return new Color(color.R, color.G, color.B, color.A);
                }
            }
            catch (FormatException) { }
            var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
            var result = new Color(
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
            return result;
        }
        else if (value is System.Windows.Media.Color color)
        {
            return (Color)color.ToColor4();
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Color val)
        {
            if (destinationType == typeof(string))
            {
                var str = string.Format("{0},{1},{2},{3}", val.R, val.G, val.B, val.A);
                return str;
            }
            else if (destinationType == typeof(System.Windows.Media.Color))
            {
                return System.Windows.Media.Color.FromArgb(val.A, val.R, val.G, val.B);
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
