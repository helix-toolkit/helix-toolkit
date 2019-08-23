// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GradientExtension.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Markupextension for Materials
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows.Markup;

    /// <summary>
    /// Markupextension for Materials
    /// </summary>
    /// <example>
    /// <code>
    /// Material={helix:Gradient Rainbow}
    ///  </code>
    /// </example>
    public class GradientExtension : MarkupExtension
    {
        /// <summary>
        /// The type.
        /// </summary>
        private readonly GradientBrushType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientExtension"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public GradientExtension(GradientBrushType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gradient brush types
        /// </summary>
        public enum GradientBrushType
        {
            /// <summary>
            /// Hue gradient
            /// </summary>
            Hue,

            /// <summary>
            /// Rainbow gradient
            /// </summary>
            Rainbow
        }

        /// <summary>
        /// Returns the gradient brush of the specified type.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// The brush to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            switch (this.type)
            {
                case GradientBrushType.Hue:
                    return GradientBrushes.Hue;
                case GradientBrushType.Rainbow:
                    return GradientBrushes.Rainbow;
                default:
                    return null;
            }
        }

    }
}