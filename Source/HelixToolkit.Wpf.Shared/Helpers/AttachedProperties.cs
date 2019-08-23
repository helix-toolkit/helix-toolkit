// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttachedProperties.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides attached properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// Provides attached properties.
    /// </summary>
    public static class AttachedProperties
    {
        /// <summary>
        /// The name property
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached(
            "Name",
            typeof(string),
            typeof(DependencyObject),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <param name="obj">The model.</param>
        /// <returns>The name.</returns>
        public static string GetName(this DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        /// <summary>
        /// Sets the name of the model.
        /// </summary>
        /// <param name="obj">The model.</param>
        /// <param name="value">The value.</param>
        public static void SetName(this DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }
    }
}