// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Model3DProperties.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   A control that contains a <see cref="Viewport3D" /> and a <see cref="CameraController" /> .
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides attached properties for <see cref="Model3D"/>.
    /// </summary>
    public static class Model3DProperties
    {
        /// <summary>
        /// The name property
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached(
            "Name",
            typeof(string),
            typeof(Model3D),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        /// <param name="obj">The model.</param>
        /// <returns>The name.</returns>
        public static string GetName(this Model3D obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        /// <summary>
        /// Sets the name of the model.
        /// </summary>
        /// <param name="obj">The model.</param>
        /// <param name="value">The value.</param>
        public static void SetName(this Model3D obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }
    }
}