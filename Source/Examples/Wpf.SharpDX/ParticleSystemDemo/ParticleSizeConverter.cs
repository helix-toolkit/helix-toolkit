﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ParticleSystemDemo;

public class ParticleSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((Size)value).Width * 100;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var v = ((double)value) / 100;
        return new Size(v, v);
    }
}
