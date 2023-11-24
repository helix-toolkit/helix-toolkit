using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Wpf.SharpDX.Utilities;

public sealed class Vector4Converter : FromToStringTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }


        if (value is string source)
        {
            var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
            var result = new Vector4(
                Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
            return result;
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is Vector4 val)
        {
            var str = string.Format("{0},{1},{2},{3}", val.X, val.Y, val.Z, val.W);
            return str;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
