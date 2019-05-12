// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetSymbolAdorner.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A Target symbol adorner. This is shown in the HelixViewport3D when manipulating the camera with the mouse.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// A Target symbol adorner. This is shown in the HelixViewport3D when manipulating the camera with the mouse.
    /// </summary>
    /// <remarks>
    /// Inspired by Google Earth...
    /// </remarks>
    public class TargetSymbolAdorner : Adorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetSymbolAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        public TargetSymbolAdorner(UIElement adornedElement, Point position)
            : base(adornedElement)
        {
            this.Position = position;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position { get; set; }

        /// <summary>
        /// Called when rendering.
        /// </summary>
        /// <param name="dc">
        /// The drawing context.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        protected override void OnRender(DrawingContext dc)
        {
            var lightBrush = new SolidColorBrush(Colors.LightGray);
            var darkBrush = new SolidColorBrush(Colors.Black);
            lightBrush.Opacity = 0.4;
            darkBrush.Opacity = 0.1;

            double t1 = 6; // thickness of dark circle pen
            double t2 = 2; // thickness of light pen (circle, arcs, segments)
            double d = 0; // distance from light circle to segments
            double l = 10; // length of segments
            double r = 20.0; // radius of light circle

            var r1 = r - (t1 + t2) / 2;
            double r2 = r + l;
            double r3 = r + t2 / 2 + d;
            double r4 = (r + r2) / 2;

            var darkPen = new Pen(darkBrush, t1);
            var lightPen = new Pen(lightBrush, t2);

            dc.DrawEllipse(null, lightPen, this.Position, r, r);
            dc.DrawEllipse(null, darkPen, this.Position, r1, r1);
            dc.DrawArc(null, lightPen, this.Position, 10, 80, r4, r4);
            dc.DrawArc(null, lightPen, this.Position, 100, 170, r4, r4);
            dc.DrawArc(null, lightPen, this.Position, 190, 260, r4, r4);
            dc.DrawArc(null, lightPen, this.Position, 280, 350, r4, r4);

            dc.DrawLine(
                lightPen,
                new Point(this.Position.X, this.Position.Y - r2),
                new Point(this.Position.X, this.Position.Y - r3));
            dc.DrawLine(
                lightPen,
                new Point(this.Position.X, this.Position.Y + r2),
                new Point(this.Position.X, this.Position.Y + r3));
            dc.DrawLine(
                lightPen,
                new Point(this.Position.X - r2, this.Position.Y),
                new Point(this.Position.X - r3, this.Position.Y));
            dc.DrawLine(
                lightPen,
                new Point(this.Position.X + r2, this.Position.Y),
                new Point(this.Position.X + r3, this.Position.Y));
        }
    }
}