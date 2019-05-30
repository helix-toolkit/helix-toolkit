// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleAdorner.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An adorner showing a rectangle with a crosshair in the middle. This is shown when zooming a rectangle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// An adorner showing a rectangle with a crosshair in the middle. This is shown when zooming a rectangle.
    /// </summary>
    public class RectangleAdorner : Adorner
    {
        /// <summary>
        /// The cross hair size.
        /// </summary>
        private readonly double crossHairSize;

        /// <summary>
        /// The pen.
        /// </summary>
        private readonly Pen pen;

        /// <summary>
        /// The pen 2.
        /// </summary>
        private readonly Pen pen2;

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        /// <param name="color1">
        /// The color1.
        /// </param>
        /// <param name="color2">
        /// The color2.
        /// </param>
        /// <param name="thickness1">
        /// The thickness1.
        /// </param>
        /// <param name="thickness2">
        /// The thickness2.
        /// </param>
        /// <param name="crossHairSize">
        /// Size of the cross hair.
        /// </param>
        public RectangleAdorner(
            UIElement adornedElement,
            Rect rectangle,
            Color color1,
            Color color2,
            double thickness1 = 1.0,
            double thickness2 = 1.0,
            double crossHairSize = 10)
            : this(adornedElement, rectangle, color1, color2, thickness1, thickness2, crossHairSize, DashStyles.Dash)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="rectangle">
        /// The rectangle.
        /// </param>
        /// <param name="color1">
        /// The color1.
        /// </param>
        /// <param name="color2">
        /// The color2.
        /// </param>
        /// <param name="thickness1">
        /// The thickness1.
        /// </param>
        /// <param name="thickness2">
        /// The thickness2.
        /// </param>
        /// <param name="crossHairSize">
        /// Size of the cross hair.
        /// </param>
        /// <param name="dashStyle2">
        /// The dash style2.
        /// </param>
        public RectangleAdorner(
            UIElement adornedElement,
            Rect rectangle,
            Color color1,
            Color color2,
            double thickness1,
            double thickness2,
            double crossHairSize,
            DashStyle dashStyle2)
            : base(adornedElement)
        {
            if (adornedElement == null)
            {
                throw new ArgumentNullException("adornedElement");
            }

            this.Rectangle = rectangle;

            // http://www.wpftutorial.net/DrawOnPhysicalDevicePixels.html
            var ps = PresentationSource.FromVisual(adornedElement);
            if (ps == null)
            {
                return;
            }

            var ct = ps.CompositionTarget;
            if (ct == null)
            {
                return;
            }

            var m = ct.TransformToDevice;
            double dpiFactor = 1 / m.M11;

            this.pen = new Pen(new SolidColorBrush(color1), thickness1 * dpiFactor);
            this.pen2 = new Pen(new SolidColorBrush(color2), thickness2 * dpiFactor);
            this.pen2.DashStyle = dashStyle2;
            this.crossHairSize = crossHairSize;
        }

        /// <summary>
        /// Gets or sets Rectangle.
        /// </summary>
        public Rect Rectangle { get; set; }

        /// <summary>
        /// Called when rendering.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            double halfPenWidth = this.pen.Thickness / 2;

            double mx = (this.Rectangle.Left + this.Rectangle.Right) / 2;
            double my = (this.Rectangle.Top + this.Rectangle.Bottom) / 2;
            mx = (int)mx + halfPenWidth;
            my = (int)my + halfPenWidth;

            var rect = new Rect(
                (int)this.Rectangle.Left + halfPenWidth,
                (int)this.Rectangle.Top + halfPenWidth,
                (int)this.Rectangle.Width,
                (int)this.Rectangle.Height);

            // Create a guidelines set
            /*GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(rect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(rect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(rect.Bottom + halfPenWidth);
            guidelines.GuidelinesX.Add(mx + halfPenWidth);
            guidelines.GuidelinesY.Add(my + halfPenWidth);

            dc.PushGuidelineSet(guidelines);*/
            dc.DrawRectangle(null, this.pen, rect);
            dc.DrawRectangle(null, this.pen2, rect);

            if (this.crossHairSize > 0)
            {
                dc.DrawLine(this.pen, new Point(mx, my - this.crossHairSize), new Point(mx, my + this.crossHairSize));
                dc.DrawLine(this.pen, new Point(mx - this.crossHairSize, my), new Point(mx + this.crossHairSize, my));
                dc.DrawLine(this.pen2, new Point(mx, my - this.crossHairSize), new Point(mx, my + this.crossHairSize));
                dc.DrawLine(this.pen2, new Point(mx - this.crossHairSize, my), new Point(mx + this.crossHairSize, my));
            }

            // dc.Pop();
        }
    }
}