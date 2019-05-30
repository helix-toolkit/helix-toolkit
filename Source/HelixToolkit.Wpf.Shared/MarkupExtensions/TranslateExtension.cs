// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateExtension.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A markup extension creating a translation transform.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Markup;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A markup extension creating a translation transform.
    /// </summary>
    public class TranslateExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="dx">
        /// The dx.
        /// </param>
        /// <param name="dy">
        /// The dy.
        /// </param>
        /// <param name="dz">
        /// The dz.
        /// </param>
        public TranslateExtension(double dx, double dy, double dz)
        {
            this.Offset = new Vector3D(dx, dy, dz);
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector3D Offset { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TranslateTransform3D(this.Offset);
        }

    }
}