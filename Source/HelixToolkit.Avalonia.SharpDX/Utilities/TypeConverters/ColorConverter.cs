using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Avalonia.SharpDX.Utilities;

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
                if (UIColor.TryParse(source, out UIColor color))
                {
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
        else if (value is UIColor color)
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
            else if (destinationType == typeof(UIColor))
            {
                return UIColor.FromArgb(val.A, val.R, val.G, val.B);
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
