using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Wpf.SharpDX.Utilities;

public sealed class Color4Converter : FromToStringTypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(System.Windows.Media.Color))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(System.Windows.Media.Color))
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
        if (value is System.Windows.Media.Color color)
        {
            return color.ToColor4();
        }
        else if (value is string source)
        {
            var sepChar = TokenizerHelper.GetNumericListSeparator(CultureInfo.InvariantCulture);
            if (source.Contains(sepChar.ToString()))
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Color4(
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            try
            {
                var obj = System.Windows.Media.ColorConverter.ConvertFromString(source);
                if (obj is System.Windows.Media.Color objColor)
                {
                    return objColor.ToColor4();
                }
            }
            catch (Exception) { }
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Color4 val)
        {
            if (destinationType == typeof(System.Windows.Media.Color))
            {
                return val.ToColor();
            }
            else if (destinationType == typeof(string))
            {
                var str = string.Format("{0},{1},{2},{3}", val.Red, val.Green, val.Blue, val.Alpha);
                return str;
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
