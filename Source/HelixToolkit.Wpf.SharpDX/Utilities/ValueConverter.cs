namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

    public static class ValueConverter
    {
        public static readonly Vector3Converter Vector3ConverterInstance = new Vector3Converter();
        public static readonly DoubleVectorRounder DoubleVectorRounderInstance = new DoubleVectorRounder();
        public static readonly TransformToPositionConverter TransformToPositionConverterInstance = new TransformToPositionConverter();
        public static readonly PositionToTransformConverter PositionToTransformConverterInstance = new PositionToTransformConverter();
    }

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = (global::SharpDX.Color4)value;
            return c.ToColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var c = (System.Windows.Media.Color)value;
            return c.ToColor4();
        }
    }


    public class MaterialConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return PhongMaterials.GetMaterial((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((PhongMaterial)value).Name;
        }
    }

    public class Vector3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pos = ((Vector3)value).ToPoint3D();
            return pos;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pos = (System.Windows.Media.Media3D.Point3D)value;
            return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
        }
    }

    public class DoubleVectorRounder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var p = (System.Windows.Media.Media3D.Vector3D)value;
            return new System.Windows.Media.Media3D.Vector3D(Math.Round(p.X, 2), Math.Round(p.Y, 2), Math.Round(p.Z, 2));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var p = (System.Windows.Media.Media3D.Vector3D)value;
            return new System.Windows.Media.Media3D.Vector3D(Math.Round(p.X, 2), Math.Round(p.Y, 2), Math.Round(p.Z, 2));
        }
    }

    public class TransformToPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var trafo = ((Transform3D)value).Value;
            var pos = new System.Windows.Media.Media3D.Point3D(trafo.OffsetX, trafo.OffsetY, trafo.OffsetZ);
            return pos;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pos = (System.Windows.Media.Media3D.Point3D)value;
            return new TranslateTransform3D(pos.X, pos.Y, pos.Z);
        }
    }

    public class PositionToTransformConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var pos = (Vector3)value;
            return new TranslateTransform3D(pos.X, pos.Y, pos.Z);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var trafo = ((Transform3D)value).Value;
            var pos = new System.Windows.Media.Media3D.Point3D(trafo.OffsetX, trafo.OffsetY, trafo.OffsetZ);
            return pos.ToVector3();
        }
    }


    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if(value is int)
            //    Console.WriteLine();
            //if(value is double)
            //    Console.WriteLine();
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class EmptyStringToZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null || string.IsNullOrEmpty(value.ToString())
                ? 0
                : value;
        }
    }

    public class StringToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Source to Target
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //string str;
            //if (value is string)
            //    str = value == null || string.IsNullOrEmpty((string)value) ? "0.0" : (string)value;
            //else if(value is int)
            //    return (int)value;
            //else
            //    return (double)value;


            //return double.Parse(str, culture);
            return value.ToString();
        }

        /// <summary>
        /// Target to Source
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return double.Parse(value.ToString(), culture);
        }
    }

    public class ObjectToDouble : IValueConverter
    {
        /// <summary>
        /// Source to Target
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value;
        }

        /// <summary>
        /// Target to Source
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value;
        }
    }

    public class ObjectToInt : IValueConverter
    {
        /// <summary>
        /// Source to Target
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }

        /// <summary>
        /// Target to Source
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }
    }

    public class ValueRounderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (Point3D)value;
            return new Point3D(Math.Round(input.X, 2), Math.Round(input.Y, 2), Math.Round(input.Z, 2));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return Point3D.Parse(value.ToString());
            return value;
        }
    }

    public class DoubleRounderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (double)value;
            return Math.Round(input, 2);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (double)value;
            return Math.Round(input, 2);
        }
    }

    public class Vector3RounderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            var input = (Vector3)value;
            return new Vector3((float)Math.Round(input.X, 2), (float)Math.Round(input.Y, 2), (float)Math.Round(input.Z, 2));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (Vector3)value;
            return new Vector3((float)Math.Round(input.X, 2), (float)Math.Round(input.Y, 2), (float)Math.Round(input.Z, 2));
        }
    }
}