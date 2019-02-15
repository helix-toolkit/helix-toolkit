// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManipulationActionConverter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Converts a <see cref="ManipulationAction"/> object to and from other types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Converts a <see cref="ManipulationAction"/> object to and from other types.
    /// </summary>
    public class ManipulationActionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) &&
                context?.Instance is ManipulationAction manipulationAction)
            {
                return Enum.IsDefined(typeof(ManipulationAction), manipulationAction);
            }

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string manipulationActionToken)
            {
                manipulationActionToken = manipulationActionToken.Trim();
                var result = ManipulationAction.None;
                if (manipulationActionToken != string.Empty && 
                    !Enum.TryParse(manipulationActionToken, true, out result))
                {
                    throw this.GetConvertFromException(value);
                }

                return result;
            }

            return this.GetConvertFromException(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string) && value is ManipulationAction manipulationAction)
            {
                return manipulationAction.ToString();
            }

            throw this.GetConvertToException(value, destinationType);
        }
    }

    public static class ManipulationActionExtensions
    {
        public static int FingerCount(this ManipulationAction manipulationAction)
        {
            switch (manipulationAction)
            {
                    case ManipulationAction.Pan:
                        return 1;
                    case ManipulationAction.Pinch:
                    case ManipulationAction.TwoFingerPan:
                        return 2;
                    case ManipulationAction.ThreeFingerPan:
                        return 3;
            }

            return 0;
        }
    }
}