// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorAxis.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   The base class for color axes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// The base class for color axes.
    /// </summary>
    [TemplatePart(Name = "PART_Canvas", Type = typeof(Canvas))]
    public abstract class ColorAxis : Control
    {
        /// <summary>
        /// Identifies the <see cref="BarWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register(
            "BarWidth", typeof(double), typeof(ColorAxis), new UIPropertyMetadata(20.0));

        /// <summary>
        /// Identifies the <see cref="ColorScheme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorSchemeProperty = DependencyProperty.Register(
            "ColorScheme", typeof(Brush), typeof(ColorAxis), new UIPropertyMetadata(null, PropertyChanged));

        /// <summary>
        /// Identifies the <see cref="FlipColorScheme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FlipColorSchemeProperty = DependencyProperty.Register(
            "FlipColorScheme", typeof(bool), typeof(ColorAxis), new UIPropertyMetadata(false, PropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(ColorAxisPosition), typeof(ColorAxis), new UIPropertyMetadata(ColorAxisPosition.Left));

        /// <summary>
        /// Identifies the <see cref="TextMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(
            "TextMargin", typeof(double), typeof(ColorAxis), new UIPropertyMetadata(2.0));

        /// <summary>
        /// Identifies the <see cref="TickLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty = DependencyProperty.Register(
            "TickLength", typeof(double), typeof(ColorAxis), new UIPropertyMetadata(3.0));

        /// <summary>
        /// Initializes static members of the <see cref="ColorAxis" /> class.
        /// </summary>
        static ColorAxis()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ColorAxis), new FrameworkPropertyMetadata(typeof(ColorAxis)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorAxis" /> class.
        /// </summary>
        protected ColorAxis()
        {
            this.SizeChanged += (s, e) => this.UpdateVisuals();
            this.Loaded += (s, e) => this.UpdateVisuals();
        }

        /// <summary>
        /// Gets or sets the width of the color bar rectangle.
        /// </summary>
        /// <value>The width.</value>
        public double BarWidth
        {
            get
            {
                return (double)this.GetValue(BarWidthProperty);
            }

            set
            {
                this.SetValue(BarWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color scheme.
        /// </summary>
        /// <value>The color scheme.</value>
        public Brush ColorScheme
        {
            get
            {
                return (Brush)this.GetValue(ColorSchemeProperty);
            }

            set
            {
                this.SetValue(ColorSchemeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color scheme direction, if true inverts the color normal color brush direction.
        /// </summary>
        /// <value>A boolean indicating inverted color direction when true.</value>
        public bool FlipColorScheme
        {
            get
            {
                return (bool)this.GetValue(FlipColorSchemeProperty);
            }

            set
            {
                this.SetValue(FlipColorSchemeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public ColorAxisPosition Position
        {
            get
            {
                return (ColorAxisPosition)this.GetValue(PositionProperty);
            }

            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text margin.
        /// </summary>
        /// <value>The text margin.</value>
        public double TextMargin
        {
            get
            {
                return (double)this.GetValue(TextMarginProperty);
            }

            set
            {
                this.SetValue(TextMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the tick.
        /// </summary>
        /// <value>The length of the tick.</value>
        public double TickLength
        {
            get
            {
                return (double)this.GetValue(TickLengthProperty);
            }

            set
            {
                this.SetValue(TickLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        protected Canvas Canvas { get; private set; }

        /// <summary>
        /// Gets the color rectangle area.
        /// </summary>
        protected Rect ColorArea { get; private set; }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see
        ///         cref="M:System.Windows.FrameworkElement.ApplyTemplate" />
        /// .
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Canvas = (Canvas)this.GetTemplateChild("PART_Canvas");
        }

        /// <summary>
        /// Handles changes in properties.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorAxis)d).UpdateVisuals();
        }

        /// <summary>
        /// Adds the visuals.
        /// </summary>
        protected virtual void AddVisuals()
        {
            switch (this.Position)
            {
                case ColorAxisPosition.Left:
                    this.ColorArea = new Rect(
                        this.Padding.Left,
                        this.Padding.Top,
                        this.BarWidth,
                        this.ActualHeight - this.Padding.Bottom - this.Padding.Top);
                    break;
                case ColorAxisPosition.Right:
                    this.ColorArea = new Rect(
                        this.ActualWidth - this.Padding.Right - this.BarWidth,
                        this.Padding.Top,
                        this.BarWidth,
                        this.ActualHeight - this.Padding.Bottom - this.Padding.Top);
                    break;
            }

            var r = new Rectangle
                              {
                                  Fill = this.ColorScheme,
                                  Width = this.ColorArea.Width,
                                  Height = this.ColorArea.Height
                              };

            if (this.FlipColorScheme)
            {
                r.LayoutTransform = new RotateTransform(180);
            }

            Canvas.SetLeft(r, this.ColorArea.Left);
            Canvas.SetTop(r, this.ColorArea.Top);

            this.Canvas.Children.Add(r);

            this.Canvas.Children.Add(
                new System.Windows.Shapes.Line
                    {
                        Stroke = this.Foreground,
                        StrokeThickness = 1,
                        SnapsToDevicePixels = true,
                        X1 = this.ColorArea.Left,
                        Y1 = this.ColorArea.Top,
                        X2 = this.ColorArea.Left,
                        Y2 = this.ColorArea.Bottom
                    });
            this.Canvas.Children.Add(
                new System.Windows.Shapes.Line
                    {
                        Stroke = this.Foreground,
                        StrokeThickness = 1,
                        SnapsToDevicePixels = true,
                        X1 = this.ColorArea.Right,
                        Y1 = this.ColorArea.Top,
                        X2 = this.ColorArea.Right,
                        Y2 = this.ColorArea.Bottom
                    });
        }

        /// <summary>
        /// Gets the tick labels.
        /// </summary>
        /// <returns>The labels.</returns>
        protected abstract IEnumerable<string> GetTickLabels();

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.Canvas"/> in anticipation of arranging them during the
        ///     <see cref="M:System.Windows.Controls.Canvas.ArrangeOverride(System.Windows.Size)"/>
        /// pass.
        /// </summary>
        /// <param name="constraint">
        /// An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> that represents the size that is required to arrange child content.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);

            var maxWidth = this.GetTickLabels().Max(
                c =>
                {
                    var tb = new TextBlock(new Run(c));
                    tb.Measure(constraint);
                    return tb.DesiredSize.Width;
                });
            size.Width = maxWidth + this.BarWidth + this.TickLength + this.Padding.Left + this.Padding.Right
                         + this.TextMargin;

            return size;
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        protected void UpdateVisuals()
        {
            if (Canvas == null)
            {
                return;
            }

            this.Canvas.Children.Clear();
            this.AddVisuals();
        }
    }
}