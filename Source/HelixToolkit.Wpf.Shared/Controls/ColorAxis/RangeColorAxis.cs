// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeColorAxis.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a color axis for a numeric value range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Shapes;

    /// <summary>
    /// Provides a color axis for a numeric value range.
    /// </summary>
    public class RangeColorAxis : ColorAxis
    {
        /// <summary>
        /// Identifies the <see cref="FormatProvider"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatProviderProperty = DependencyProperty.Register(
            "FormatProvider", typeof(IFormatProvider), typeof(RangeColorAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FormatString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(
            "FormatString", typeof(string), typeof(RangeColorAxis), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(100.0));

        /// <summary>
        /// Identifies the <see cref="MaximumTextureCoordinate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumTextureCoordinateProperty =
            DependencyProperty.Register(
                "MaximumTextureCoordinate", typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="MinimumTextureCoordinate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumTextureCoordinateProperty =
            DependencyProperty.Register(
                "MinimumTextureCoordinate", typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="Step"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
            "Step", typeof(double), typeof(RangeColorAxis), new UIPropertyMetadata(10.0));

        /// <summary>
        /// Gets or sets the format provider.
        /// </summary>
        /// <value>The format provider.</value>
        public IFormatProvider FormatProvider
        {
            get
            {
                return (IFormatProvider)this.GetValue(FormatProviderProperty);
            }

            set
            {
                this.SetValue(FormatProviderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString
        {
            get
            {
                return (string)this.GetValue(FormatStringProperty);
            }

            set
            {
                this.SetValue(FormatStringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public double Maximum
        {
            get
            {
                return (double)this.GetValue(MaximumProperty);
            }

            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum texture coordinate.
        /// </summary>
        /// <value>The maximum texture coordinate.</value>
        public double MaximumTextureCoordinate
        {
            get
            {
                return (double)this.GetValue(MaximumTextureCoordinateProperty);
            }

            set
            {
                this.SetValue(MaximumTextureCoordinateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public double Minimum
        {
            get
            {
                return (double)this.GetValue(MinimumProperty);
            }

            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum texture coordinate.
        /// </summary>
        /// <value>The minimum texture coordinate.</value>
        public double MinimumTextureCoordinate
        {
            get
            {
                return (double)this.GetValue(MinimumTextureCoordinateProperty);
            }

            set
            {
                this.SetValue(MinimumTextureCoordinateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>The step.</value>
        public double Step
        {
            get
            {
                return (double)this.GetValue(StepProperty);
            }

            set
            {
                this.SetValue(StepProperty, value);
            }
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        protected override void AddVisuals()
        {
            if (this.Maximum <= this.Minimum || this.Step <= 0 || this.ColorScheme == null)
            {
                return;
            }

            base.AddVisuals();

            double miny = this.ColorArea.Bottom - (this.MinimumTextureCoordinate * this.ColorArea.Height);
            double maxy = this.ColorArea.Bottom - (this.MaximumTextureCoordinate * this.ColorArea.Height);
            Func<double, double> transform =
                v => miny + ((v - this.Minimum) / (this.Maximum - this.Minimum) * (maxy - miny));

            double p = double.MinValue;
            double ymax = transform(this.Maximum);
            foreach (var v in this.GetTickValues())
            {
                var text = v.ToString(this.FormatString, this.FormatProvider);
                var tb = new TextBlock(new Run(text)) { Foreground = this.Foreground };
                tb.Measure(new Size(this.ActualWidth, this.ActualHeight));

                double y = transform(v);

                Point p0, p1, p2;
                switch (this.Position)
                {
                    case ColorAxisPosition.Right:
                        p0 = new Point(this.ColorArea.Right, y);
                        p1 = new Point(this.ColorArea.Left - this.TickLength, y);
                        p2 = new Point(
                            this.ColorArea.Left - this.TickLength - this.TextMargin - tb.DesiredSize.Width,
                            y - (tb.DesiredSize.Height / 2));
                        break;
                    default:
                        p0 = new Point(this.ColorArea.Left, y);
                        p1 = new Point(this.ColorArea.Right + this.TickLength, y);
                        p2 = new Point(
                            this.ColorArea.Right + this.TickLength + this.TextMargin, y - (tb.DesiredSize.Height / 2));
                        break;
                }

                var l = new System.Windows.Shapes.Line
                            {
                                X1 = p0.X,
                                X2 = p1.X,
                                Y1 = p0.Y,
                                Y2 = p1.Y,
                                Stroke = this.Foreground,
                                StrokeThickness = 1,
                                SnapsToDevicePixels = true
                            };
                this.Canvas.Children.Add(l);

                double h = tb.DesiredSize.Height * 0.7;
                if (v < this.Maximum && Math.Abs(y - ymax) < h)
                {
                    continue;
                }

                if (Math.Abs(y - p) < h)
                {
                    continue;
                }

                Canvas.SetLeft(tb, p2.X);
                Canvas.SetTop(tb, p2.Y);
                this.Canvas.Children.Add(tb);
                p = y;
            }
        }

        /// <summary>
        /// Gets the tick labels.
        /// </summary>
        /// <returns>
        /// The labels.
        /// </returns>
        protected override IEnumerable<string> GetTickLabels()
        {
            return this.GetTickValues().Select(v => v.ToString(this.FormatString, this.FormatProvider));
        }

        /// <summary>
        /// Gets the tick values.
        /// </summary>
        /// <returns>The tick values</returns>
        private IEnumerable<double> GetTickValues()
        {
            yield return this.Minimum;
            double x = Math.Floor(this.Minimum / this.Step) * this.Step;
            while (x < this.Maximum)
            {
                if (x > this.Minimum)
                {
                    yield return x;
                }

                x += this.Step;
            }

            yield return this.Maximum;
        }
    }
}