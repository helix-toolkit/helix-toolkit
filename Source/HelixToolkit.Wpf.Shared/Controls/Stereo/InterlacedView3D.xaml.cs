// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterlacedView3D.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An interlaced viewer control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Threading;

    // TODO: used 
    /// <summary>
    /// An interlaced viewer control.
    /// </summary>
    [ContentProperty("Children")]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public partial class InterlacedView3D : StereoControl
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(InterlacedView3D),
                new UIPropertyMetadata(0.0, HorizontalOffsetChanged));

        /// <summary>
        /// Identifies the <see cref="EvenLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EvenLeftProperty = DependencyProperty.Register(
            "EvenLeft", typeof(bool), typeof(InterlacedView3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Initializes a new instance of the <see cref = "InterlacedView3D" /> class.
        /// </summary>
        public InterlacedView3D()
        {
            this.InitializeComponent();
            this.BindViewports(this.LeftView, this.RightView);

            var dt = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            dt.Tick += (s, e) => this.UpdateEvenLeft();
            dt.Start();
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
        /// Updates the <see cref="EvenLeft"/> property based on the vertical position of the control.
        /// </summary>
        public void UpdateEvenLeft()
        {
            if (this.IsLoaded && this.IsVisible)
            {
                int y = (int)this.PointToScreen(default(Point)).Y;
                this.EvenLeft = y % 2 == 0;
            }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public bool EvenLeft
        {
            get
            {
                return (bool)this.GetValue(EvenLeftProperty);
            }

            set
            {
                this.SetValue(EvenLeftProperty, value);
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
            ((InterlacedView3D)d).OnHorizontalOffsetChanged();
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