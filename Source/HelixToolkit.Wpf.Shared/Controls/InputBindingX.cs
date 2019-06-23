// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputBindingX.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Input binding supporting binding the Gezture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Input binding supporting binding the Gezture.
    /// </summary>
    public class InputBindingX : InputBinding
    {
        /// <summary>
        /// Gets or sets the gesture.
        /// </summary>
        /// <value>The gesture.</value>
        public InputGesture Gezture
        {
            get { return (InputGesture)GetValue(GeztureProperty); }
            set { SetValue(GeztureProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Gezture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeztureProperty =
            DependencyProperty.Register("Gezture", typeof(InputGesture), typeof(InputBindingX), new UIPropertyMetadata(null, GeztureChanged));

        /// <summary>
        /// Geztures the changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void GeztureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((InputBindingX)d).OnGeztureChanged();
        }

        /// <summary>
        /// Called when [gezture changed].
        /// </summary>
        protected virtual void OnGeztureChanged()
        {
            this.Gesture = Gezture;
        }
    }
}