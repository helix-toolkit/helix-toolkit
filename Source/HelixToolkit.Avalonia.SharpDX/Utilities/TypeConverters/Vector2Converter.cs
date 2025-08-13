using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Avalonia.SharpDX.Utilities;

public sealed class Vector2Converter : FromToStringTypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(Point))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(Point))
        {
            return true;
        }
        return base.CanConvertTo(context, destinationType);
    }
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }
        if (value is Point)
        {
            var source = (Point)value;
            return new Vector2((float)source.X, (float)source.Y);
        }
        else
        {
            if (value is string source)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Vector2(
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Vector2 val)
        {
            if (destinationType == typeof(Point))
            {
                return new Point(val.X, val.Y);
            }
            else if (destinationType == typeof(string))
            {
                var str = string.Format("{0},{1}", val.X, val.Y);
                return str;
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
