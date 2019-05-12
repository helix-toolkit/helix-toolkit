// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnaglyphView3D.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An anaglyph viewer control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    /// An anaglyph viewer control.
    /// </summary>
    /// <remarks>
    /// Petzold's anaglyph space station (using opacity)
    /// http://www.charlespetzold.com/3D/
    /// Greg Schechter multi input shader effects (for the AnaglyphEffect)
    /// http://blogs.msdn.com/greg_schechter/archive/2008/09/27/a-more-useful-multi-input-effect.aspx
    /// Barcinski and Jean-Jean: Making of Part III - Anaglyph
    /// http://blog.barcinski-jeanjean.com/2008/10/17/making-of-part-iii-anaglyph/
    /// </remarks>
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public partial class AnaglyphView3D : StereoControl
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(AnaglyphView3D),
                new UIPropertyMetadata(0.0, HorizontalOffsetChanged));

        /// <summary>
        /// Identifies the <see cref="Method"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MethodProperty = DependencyProperty.Register(
            "Method", typeof(AnaglyphMethod), typeof(AnaglyphView3D), new UIPropertyMetadata(AnaglyphMethod.Gray));

        /// <summary>
        /// Initializes a new instance of the <see cref = "AnaglyphView3D" /> class.
        /// </summary>
        public AnaglyphView3D()
        {
            this.InitializeComponent();
            this.BindViewports(this.LeftView, this.RightView);
        }

        /// <summary>
        /// Gets or sets the horizontal offset.
        /// </summary>
        /// <value>The horizontal offset.</value>
        public double HorizontalOffset
        {
            get
            {
                return (double)this.GetValue(HorizontalOffsetProperty);
            }

            set
            {
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public AnaglyphMethod Method
        {
            get
            {
                return (AnaglyphMethod)this.GetValue(MethodProperty);
            }

            set
            {
                this.SetValue(MethodProperty, value);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Left:
                    this.HorizontalOffset -= 0.001f;
                    break;
                case Key.Right:
                    this.HorizontalOffset += 0.001f;
                    break;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.
        /// </param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        /// <summary>
        /// The horizontal offset changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnaglyphView3D)d).OnHorizontalOffsetChanged();
        }

        /// <summary>
        /// The on horizontal offset changed.
        /// </summary>
        private void OnHorizontalOffsetChanged()
        {
            // RightView.Margin=new Thickness(HorizontalOffset,0,-HorizontalOffset,0);
        }

    }
}