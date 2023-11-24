using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.SharpDX;

public sealed class StreamToTextureModelConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(Stream);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(TextureModel);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is Stream st)
        {
            return new TextureModel(st);
        }
        else
        {
            return null;
        }
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return null;
    }
}
