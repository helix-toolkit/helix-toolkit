// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX.Utilities
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Core;

    public abstract class FromToStringTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }
    }

    public sealed class Vector2CollectionConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                return Vector2Collection.Parse(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is Vector2Collection)
            {
                var instance = (Vector2Collection)value;

                if (destinationType == typeof(string))
                {
                    return instance.ConvertToString(null, culture);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Vector3CollectionConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                return Vector3Collection.Parse(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is Vector3Collection)
            {
                var instance = (Vector3Collection)value;

                if (destinationType == typeof(string))
                {
                    return instance.ConvertToString(null, culture);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class IntCollectionConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                return IntCollection.Parse(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is IntCollection)
            {
                var instance = (IntCollection)value;

                if (destinationType == typeof(string))
                {
                    return instance.ConvertToString(null, culture);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Color4CollectionConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                return Color4Collection.Parse(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != null && value is Color4Collection)
            {
                var instance = (Color4Collection)value;

                if (destinationType == typeof(string))
                {
                    return instance.ConvertToString(null, culture);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class ColorConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Color(
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Color)
            {
                var val = (Color)value;
                var str = string.Format("{0},{1},{2},{3}", val.R, val.G, val.B, val.A);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Color4Converter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Color4(
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Color4)
            {
                var val = (Color4)value;
                var str = string.Format("{0},{1},{2},{3}", val.Red, val.Green, val.Blue, val.Alpha);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Vector2Converter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Vector2(
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vector2)
            {
                var val = (Vector2)value;
                var str = string.Format("{0},{1}", val.X, val.Y);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Vector3Converter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Vector3(
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vector3)
            {
                var val = (Vector3)value;
                var str = string.Format("{0},{1},{2}", val.X, val.Y, val.Z);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class Vector4Converter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
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

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Vector4)
            {
                var val = (Vector4)value;
                var str = string.Format("{0},{1},{2},{3}", val.X, val.Y, val.Z, val.W);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public sealed class QuaternionConverter : FromToStringTypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(value);
            }

            var source = value as string;

            if (source != null)
            {
                var th = new TokenizerHelper(source, CultureInfo.InvariantCulture);
                var result = new Quaternion(
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture),
                    Convert.ToSingle(th.NextTokenRequired(), CultureInfo.InvariantCulture));
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Quaternion)
            {
                var val = (Quaternion)value;
                var str = string.Format("{0},{1},{2},{3}", val.X, val.Y, val.Z, val.W);
                return str;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
