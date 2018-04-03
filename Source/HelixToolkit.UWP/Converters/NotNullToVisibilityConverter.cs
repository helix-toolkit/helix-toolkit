/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace HelixToolkit.UWP.Converters
{
    /// <summary>
    /// A not-null reference to Visibility value converter.
    /// </summary>
    public sealed class NotNullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "NotNullToVisibilityConverter" /> class.
        /// </summary>
        public NotNullToVisibilityConverter()
        {
            this.Inverted = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this converter is inverted.
        /// </summary>
        public bool Inverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(Visibility))
            {
                bool isNotNull = value != null;
                if (isNotNull != this.Inverted)
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