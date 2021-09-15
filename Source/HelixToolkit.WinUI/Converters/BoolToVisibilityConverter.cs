/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace HelixToolkit.WinUI.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool v)
            {
                return v ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is Visibility v)
            {
                return v == Visibility.Visible ? true : false;
            }
            else
            {
                return true;
            }
        }
    }
}
