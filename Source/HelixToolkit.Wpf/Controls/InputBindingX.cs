// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputBindingX.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
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
        /// The gesture property.
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