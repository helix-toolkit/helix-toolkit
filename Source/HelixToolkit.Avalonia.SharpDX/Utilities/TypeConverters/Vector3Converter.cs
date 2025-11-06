using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Avalonia.SharpDX.Utilities;

public sealed class Vector3Converter : FromToStringTypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }

        if (value is string source)
        {
            var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
            var result = new Vector3(
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
            return result;
        }


        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Vector3 val)
        {
            if (destinationType == typeof(string))
            {
                var str = string.Format("{0},{1},{2}", val.X, val.Y, val.Z);
                return str;
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
