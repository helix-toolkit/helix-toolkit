using CommunityToolkit.Mvvm.ComponentModel;
using Fractal.Fractals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Fractal;

public sealed class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Level = 2;
    }

    public FractalBase? Fractal { get; set; }
    public GeometryModel3D? Model { get; set; }

    public IEnumerable<FractalType> FractalTypes => Enum<FractalType>.GetValues();

    private FractalType _type;
    public FractalType Type
    {
        get { return _type; }
        set
        {
            _type = value;
            UpdateModel();
            OnPropertyChanged(nameof(Type));
        }
    }

    private int _level;
    public int Level
    {
        get { return _level; }
        set
        {
            if (value == _level) return;
            if (Type == FractalType.MengerSponge && value > 4)
                _level = 4;
            else
                _level = value;
            UpdateModel();
            OnPropertyChanged(nameof(Level));
        }
    }

    public int[] Levels
    {
        get
        {
            return new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }
    }

    public int TriangleCount => Fractal?.TriangleCount ?? 0;

    public Cursor CurrentCursor { get { return IsBusy ? Cursors.Wait : Cursors.Arrow; } }

    private bool _isBusy;
    public bool IsBusy
    {
        get { return _isBusy; }
        set
        {
            _isBusy = value;
            OnPropertyChanged(nameof(CurrentCursor));
        }
    }

    private void UpdateModel()
    {
        IsBusy = true;
        Fractal = FractalFactory(Type);
        Fractal!.Level = Level;
        Model = Fractal.Generate();

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(Fractal));
        OnPropertyChanged(nameof(Level));
        OnPropertyChanged(nameof(TriangleCount));
        IsBusy = false;
    }

    public static FractalBase? FractalFactory(FractalType type)
    {
        return type switch
        {
            FractalType.SierpinskiPyramid => new SierpinskiPyramid(),
            FractalType.MengerSponge => new MengerSponge(),
            FractalType.Plant => new Plant(),
            FractalType.MandelbrotMountain => new MandelbrotMountain(),
            _ => null,
        };
    }
}

public enum FractalType
{
    MengerSponge,
    SierpinskiPyramid,
    Plant,
    MandelbrotMountain
}

public static class Enum<T>
{
    public static IEnumerable<T> GetValues()
    {
        foreach (T value in Enum.GetValues(typeof(T)))
            yield return value;
    }
}

public class SelectionList<T> : List<T>
{
    public T? SelectedItem { get; set; }
    public List<T>? Items { get; set; }

}

public class SelectionItem<T>
{
    public T? Item { get; set; }
    public bool IsSelected { get; set; }
}

public class ValueToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return parameter == null;
        }

        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
        {
            return parameter;
        }

        return DependencyProperty.UnsetValue;
    }
}

public class MultiValueToBoolConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values[0].Equals(values[1]);
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if ((bool)value)
        {
            return new object[] { 2 };
        }

        return null;
    }
}

// http://www.scottlogic.co.uk/blog/colin/2010/07/a-universal-value-converter-for-wpf/
public class UniversalValueConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // obtain the converter for the target type
        TypeConverter converter = TypeDescriptor.GetConverter(targetType);

        try
        {
            // determine if the supplied value is of a suitable type
            if (converter.CanConvertFrom(value.GetType()))
            {
                // return the converted value
                return converter.ConvertFrom(value);
            }
            else
            {
                // try to convert from the string representation
                return converter.ConvertFrom(value.ToString() ?? string.Empty);
            }
        }
        catch (Exception)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
