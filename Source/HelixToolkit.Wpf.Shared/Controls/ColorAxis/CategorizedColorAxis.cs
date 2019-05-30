// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategorizedColorAxis.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a color axis for categories.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Shapes;

    /// <summary>
    /// Provides a color axis for categories.
    /// </summary>
    public class CategorizedColorAxis : ColorAxis
    {
        /// <summary>
        /// Identifies the <see cref="Categories"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register(
            "Categories",
            typeof(IList<string>),
            typeof(CategorizedColorAxis),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, PropertyChanged));

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public IList<string> Categories
        {
            get
            {
                return (IList<string>)this.GetValue(CategoriesProperty);
            }

            set
            {
                this.SetValue(CategoriesProperty, value);
            }
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        protected override void AddVisuals()
        {
            if (this.Categories == null || this.Categories.Count == 0 || this.ColorScheme == null)
            {
                return;
            }

            base.AddVisuals();

            for (int i = 0; i < this.Categories.Count; i++)
            {
                var text = this.Categories[i];
                var tb = new TextBlock(new Run(text)) { Foreground = this.Foreground };
                tb.Measure(new Size(this.ActualWidth, this.ActualHeight));

                double y = this.ColorArea.Top + (((double)i / this.Categories.Count) * this.ColorArea.Height);
                double y1 = this.ColorArea.Top + (((i + 0.5) / this.Categories.Count) * this.ColorArea.Height);
                double y2 = this.ColorArea.Top + (((i + 1.0) / this.Categories.Count) * this.ColorArea.Height);

                Point p0, p1, p2, p3, p4;
                switch (this.Position)
                {
                    case ColorAxisPosition.Right:
                        p0 = new Point(this.ColorArea.Right, y);
                        p1 = new Point(this.ColorArea.Left - this.TickLength, y);
                        p2 = new Point(
                            this.ColorArea.Left - this.TickLength - this.TextMargin - tb.DesiredSize.Width,
                            y1 - (tb.DesiredSize.Height / 2));
                        p3 = new Point(this.ColorArea.Right, y2);
                        p4 = new Point(this.ColorArea.Left - this.TickLength, y2);
                        break;
                    default:
                        p0 = new Point(this.ColorArea.Left, y);
                        p1 = new Point(this.ColorArea.Right + this.TickLength, y);
                        p2 = new Point(
                            this.ColorArea.Right + this.TickLength + this.TextMargin, y1 - (tb.DesiredSize.Height / 2));
                        p3 = new Point(this.ColorArea.Left, y2);
                        p4 = new Point(this.ColorArea.Right + this.TickLength, y2);
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
                if (i == this.Categories.Count - 1)
                {
                    var l2 = new System.Windows.Shapes.Line
                                 {
                                     X1 = p3.X,
                                     X2 = p4.X,
                                     Y1 = p3.Y,
                                     Y2 = p4.Y,
                                     Stroke = this.BorderBrush,
                                     StrokeThickness = 1,
                                     SnapsToDevicePixels = true
                                 };
                    this.Canvas.Children.Add(l2);
                }

                Canvas.SetLeft(tb, p2.X);
                Canvas.SetTop(tb, p2.Y);
                this.Canvas.Children.Add(tb);
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
            if (this.Categories == null)
            {
                return new[] { string.Empty };
            }

            return this.Categories;
        }
    }
}