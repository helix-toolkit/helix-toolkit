// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CubeVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// A visual element that displays a cube.
    /// </summary>
    public class CubeVisual3D : BoxVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="SideLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SideLengthProperty = DependencyProperty.Register(
            "SideLength", typeof(double), typeof(CubeVisual3D), new UIPropertyMetadata(1.0, SideLengthChanged));

        /// <summary>
        /// Gets or sets the length of the cube sides.
        /// </summary>
        /// <value>The length of the sides.</value>
        public double SideLength
        {
            get
            {
                return (double)this.GetValue(SideLengthProperty);
            }

            set
            {
                this.SetValue(SideLengthProperty, value);
            }
        }

        /// <summary>
        /// The side length changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void SideLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CubeVisual3D)d).OnSideLengthChanged();
        }

        /// <summary>
        /// The on side length changed.
        /// </summary>
        private void OnSideLengthChanged()
        {
            this.BeginEdit();
            this.Length = this.Height = this.Width = this.SideLength;
            this.EndEdit();
        }

    }
}