using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit;

public sealed class IntCollectionConverter : FromToStringTypeConverter
{
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
        {
            throw GetConvertFromException(value);
        }

        if (value is string source)
        {
            return IntCollection.Parse(source);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType != null && value is IntCollection instance)
        {
            if (destinationType == typeof(string))
            {
                return instance.ConvertToString(null, culture);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType!);
    }
}
