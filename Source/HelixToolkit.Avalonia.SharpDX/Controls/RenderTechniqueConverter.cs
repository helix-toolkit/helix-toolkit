using Avalonia.Data.Converters;
using HelixToolkit.SharpDX;
using System.Globalization;

namespace HelixToolkit.Avalonia.SharpDX;

public class RenderTechniqueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        return ((KeyValuePair<string, IRenderTechnique>)value).Value;
    }
}
