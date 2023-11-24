using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.SharpDX.Utilities;

public sealed class Color4CollectionConverter : FromToStringTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }

        if (value is string source)
        {
            return Color4Collection.Parse(source);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != null && value is Color4Collection instance)
        {
            if (destinationType == typeof(string))
            {
                return instance.ConvertToString(null, culture);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType!);
    }
}
