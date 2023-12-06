using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// An enumerate to boolean converter.
/// </summary>
/// <example>
/// <code>
/// IsChecked="{Binding MyProperty, Converter={StaticResource EnumToBooleanConverter}, ConverterParameter=Param1}"
///  </code>
/// </example>
[ValueConversion(typeof(Enum), typeof(bool))]
public sealed class EnumToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value produced by the binding source.
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return DependencyProperty.UnsetValue;
        }

        string checkValue = value.ToString()!;
        string targetValue = parameter.ToString()!;
        return checkValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">
    /// The value that is produced by the binding target.
    /// </param>
    /// <param name="targetType">
    /// The type to convert to.
    /// </param>
    /// <param name="parameter">
    /// The converter parameter to use.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the converter.
    /// </param>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return DependencyProperty.UnsetValue;
        }

        try
        {
            bool boolValue = System.Convert.ToBoolean(value, culture);
            if (boolValue)
            {
                return Enum.Parse(targetType, parameter.ToString()!);
            }
        }
        catch (ArgumentException)
        {
        }
        catch (FormatException)
        {
        }

        return DependencyProperty.UnsetValue;
    }
}
