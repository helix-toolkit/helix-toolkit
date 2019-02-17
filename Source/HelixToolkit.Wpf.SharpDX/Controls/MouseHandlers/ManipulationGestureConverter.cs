// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationGestureConverter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Converts a <see cref="ManipulationGesture"/> object to and from other types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Converts a <see cref="ManipulationGesture"/> object to and from other types.
    /// </summary>
    public class ManipulationGestureConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var tc = TypeDescriptor.GetConverter(typeof(ManipulationAction));
                if (tc.ConvertFrom(context, culture, value) is ManipulationAction manipulationAction)
                {
                    return new ManipulationGesture(manipulationAction);
                }
            }

            throw this.GetConvertFromException(value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (context?.Instance is ManipulationGesture manipulationGesture)
                {
                    return Enum.IsDefined(typeof(ManipulationAction), manipulationGesture.ManipulationAction);
                }
            }

            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                if (value == null) return string.Empty;
                if (value is ManipulationGesture manipulationGesture)
                {
                    return "ManipulationGesture";
                }
            }

            throw this.GetConvertToException(value, destinationType);
        }
    }
}