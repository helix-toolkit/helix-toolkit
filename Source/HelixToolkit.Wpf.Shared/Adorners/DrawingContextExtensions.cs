// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawingContextExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Extension methods for DrawingContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Extension methods for DrawingContext.
    /// </summary>
    public static class DrawingContextExtensions
    {
        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="radiusX">The radius X.</param>
        /// <param name="radiusY">The radius Y.</param>
        public static void DrawArc(
            this DrawingContext dc,
            Brush brush,
            Pen pen,
            Point start,
            Point end,
            SweepDirection direction,
            double radiusX,
            double radiusY)
        {
            // http://blogs.vertigo.com/personal/ralph/Blog/archive/2007/02/09/wpf-drawing-arcs.aspx
            // setup the geometry object
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = start;

            // add the arc to the geometry
            figure.Segments.Add(new ArcSegment(end, new Size(radiusX, radiusY), 0, false, direction, true));

            // draw the arc
            dc.DrawGeometry(brush, pen, geometry);
        }

        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="pen">
        /// The pen.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="startAngle">
        /// The start angle.
        /// </param>
        /// <param name="endAngle">
        /// The end angle.
        /// </param>
        /// <param name="direction">
        /// The direction.
        /// </param>
        /// <param name="radiusX">
        /// The radius X.
        /// </param>
        /// <param name="radiusY">
        /// The radius Y.
        /// </param>
        public static void DrawArc(
            this DrawingContext dc,
            Brush brush,
            Pen pen,
            Point position,
            double startAngle,
            double endAngle,
            SweepDirection direction,
            double radiusX,
            double radiusY)
        {
            double startRadians = startAngle / 180 * Math.PI;
            double endRadians = endAngle / 180 * Math.PI;
            var start = position + new Vector(Math.Cos(startRadians) * radiusX, -Math.Sin(startRadians) * radiusY);
            var end = position + new Vector(Math.Cos(endRadians) * radiusX, -Math.Sin(endRadians) * radiusY);
            dc.DrawArc(brush, pen, start, end, direction, radiusX, radiusY);
        }

        /// <summary>
        /// Draws the arc.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="pen">
        /// The pen.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="startAngle">
        /// The start angle.
        /// </param>
        /// <param name="endAngle">
        /// The end angle.
        /// </param>
        /// <param name="radiusX">
        /// The radius X.
        /// </param>
        /// <param name="radiusY">
        /// The radius Y.
        /// </param>
        public static void DrawArc(
            this DrawingContext dc,
            Brush brush,
            Pen pen,
            Point position,
            double startAngle,
            double endAngle,
            double radiusX,
            double radiusY)
        {
            DrawArc(dc, brush, pen, position, startAngle, endAngle, SweepDirection.Counterclockwise, radiusX, radiusY);
        }
    }
}