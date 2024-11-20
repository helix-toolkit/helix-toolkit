using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using System.ComponentModel;
using System.Globalization;

namespace HelixToolkit.Wpf.SharpDX.Utilities;

public sealed class Vector3Converter : FromToStringTypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(System.Windows.Media.Media3D.Vector3D) || sourceType == typeof(System.Windows.Media.Media3D.Point3D))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(System.Windows.Media.Media3D.Vector3D) || destinationType == typeof(System.Windows.Media.Media3D.Point3D))
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
        if (value is System.Windows.Media.Media3D.Vector3D)
        {
            var source = (System.Windows.Media.Media3D.Vector3D)value;
            return new Vector3((float)source.X, (float)source.Y, (float)source.Z);
        }
        else if (value is System.Windows.Media.Media3D.Point3D)
        {
            var source = (System.Windows.Media.Media3D.Point3D)value;
            return new Vector3((float)source.X, (float)source.Y, (float)source.Z);
        }
        else
        {
            if (value is string source)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Vector3(
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    NumericHelpers.ParseSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }
        }


        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Vector3 val)
        {
            if (destinationType == typeof(System.Windows.Media.Media3D.Vector3D))
            {
                return new System.Windows.Media.Media3D.Vector3D(val.X, val.Y, val.Z);
            }
            else if (destinationType == typeof(System.Windows.Media.Media3D.Point3D))
            {
                return new System.Windows.Media.Media3D.Point3D(val.X, val.Y, val.Z);
            }
            else if (destinationType == typeof(string))
            {
                var str = string.Format("{0},{1},{2}", val.X, val.Y, val.Z);
                return str;
            }
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
