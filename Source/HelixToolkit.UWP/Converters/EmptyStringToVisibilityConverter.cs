/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HelixToolkit.UWP.Converters
{
    public sealed class EmptyStringToVisibilityConverter : IValueConverter
    {        /// <summary>
             /// Initializes a new instance of the <see cref = "NotNullToVisibilityConverter" /> class.
             /// </summary>
        public EmptyStringToVisibilityConverter()
        {
            this.Inverted = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this converter is inverted.
        /// </summary>
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(Visibility) && value is string s)
            {
                bool isNotNullOrEmpty = !string.IsNullOrEmpty(s);
                if (isNotNullOrEmpty != this.Inverted)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
