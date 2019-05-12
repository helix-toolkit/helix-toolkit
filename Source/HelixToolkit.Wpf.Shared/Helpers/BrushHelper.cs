// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BrushHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides methods that creates brushes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Provides methods that creates brushes.
    /// </summary>
    public static class BrushHelper
    {
        /// <summary>
        /// Creates a copy of a brush with the specified opacity.
        /// </summary>
        /// <param name="brush">
        /// The brush to copy.
        /// </param>
        /// <param name="opacity">
        /// The opacity.
        /// </param>
        /// <returns>
        /// </returns>
        public static Brush ChangeOpacity(Brush brush, double opacity)
        {
            brush = brush.Clone();
            brush.Opacity = opacity;
            return brush;
        }

        /// <summary>
        /// Creates a gradient brush from the given colors.
        /// </summary>
        /// <param name="colors">
        /// The colors.
        /// </param>
        /// <returns>
        /// A LinearGradientBrush.
        /// </returns>
        public static LinearGradientBrush CreateGradientBrush(params Color[] colors)
        {
            return CreateGradientBrush(colors.ToList());
        }

        /// <summary>
        /// Creates a gradient brush from a list of colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="horizontal">if set to <c>true</c> [horizontal].</param>
        /// <returns>A LinearGradientBrush.</returns>
        public static LinearGradientBrush CreateGradientBrush(IList<Color> colors, bool horizontal = true)
        {
            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = horizontal ? new Point(1, 0) : new Point(0, 1) };
            int n = colors.Count;
            for (int i = 0; i < n; i++)
            {
                var gs = new GradientStop(colors[i], (double)i / (n - 1));
                brush.GradientStops.Add(gs);
            }

            return brush;
        }

        /// <summary>
        /// Creates a gray brush.
        /// </summary>
        /// <param name="intensity">
        /// The intensity of the gray color.
        /// </param>
        /// <returns>
        /// </returns>
        public static SolidColorBrush CreateGrayBrush(double intensity)
        {
            var b = (byte)(255 * intensity);
            return new SolidColorBrush(Color.FromArgb(255, b, b, b));
        }

        // http://en.wikipedia.org/wiki/HSL_and_HSV
        /// <summary>
        /// Creates a HSV brush.
        /// </summary>
        /// <param name="alpha">The opacity (0-1).</param>
        /// <param name="horizontal">if set to <c>true</c> [horizontal].</param>
        /// <returns>LinearGradientBrush.</returns>
        public static LinearGradientBrush CreateHsvBrush(double alpha = 1, bool horizontal = true)
        {
            var a = (byte)(alpha * 255);
            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = horizontal ? new Point(1, 0) : new Point(0, 1) };
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0xff, 0x00, 0x00), 0.00));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0xff, 0xff, 0x00), 0.17));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0x00, 0xff, 0x00), 0.33));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0x00, 0xff, 0xff), 0.50));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0x00, 0x00, 0xff), 0.67));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0xff, 0x00, 0xff), 0.84));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(a, 0xff, 0x00, 0x00), 1.00));
            return brush;
        }

        /// <summary>
        /// Creates a rainbow brush.
        /// </summary>
        /// <returns>
        /// A rainbow brush.
        /// </returns>
        public static LinearGradientBrush CreateRainbowBrush(bool horizontal = true)
        {
            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = horizontal ? new Point(1, 0) : new Point(0, 1) };
            brush.GradientStops.Add(new GradientStop(Colors.Red, 0.00));
            brush.GradientStops.Add(new GradientStop(Colors.Orange, 0.17));
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.33));
            brush.GradientStops.Add(new GradientStop(Colors.Green, 0.50));
            brush.GradientStops.Add(new GradientStop(Colors.Blue, 0.67));
            brush.GradientStops.Add(new GradientStop(Colors.Indigo, 0.84));
            brush.GradientStops.Add(new GradientStop(Colors.Violet, 1.00));
            return brush;
        }

        /// <summary>
        /// Creates a 'stepped' gradient brush from a list of colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="horizontal">if set to <c>true</c> [horizontal].</param>
        /// <returns>A gradientbrush.</returns>
        public static LinearGradientBrush CreateSteppedGradientBrush(IList<Color> colors, bool horizontal = true)
        {
            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = horizontal ? new Point(1, 0) : new Point(0, 1) };
            int n = colors.Count;
            for (int i = 0; i < n; i++)
            {
                var gs0 = new GradientStop(colors[i], (double)i / n);
                var gs1 = new GradientStop(colors[i], (double)(i + 1) / n);
                brush.GradientStops.Add(gs0);
                brush.GradientStops.Add(gs1);
            }

            return brush;
        }

        /// <summary>
        /// Creates the stepped gradient brush (same number of steps as the number of stops in the gradient).
        /// </summary>
        /// <param name="gradient">
        /// The gradient.
        /// </param>
        /// <returns>
        /// </returns>
        public static LinearGradientBrush CreateSteppedGradientBrush(LinearGradientBrush gradient)
        {
            var brush = new LinearGradientBrush { StartPoint = gradient.StartPoint, EndPoint = gradient.EndPoint };
            for (int i = 0; i + 1 < gradient.GradientStops.Count; i++)
            {
                var gs0 = gradient.GradientStops[i].Clone();
                var gs1 = gs0.Clone();
                gs1.Offset = gradient.GradientStops[i + 1].Offset;
                brush.GradientStops.Add(gs0);
                brush.GradientStops.Add(gs1);
            }

            return brush;
        }

        /// <summary>
        /// Creates the stepped gradient brush (any number of steps).
        /// </summary>
        /// <param name="gradient">
        /// The gradient.
        /// </param>
        /// <param name="steps">
        /// The number of steps.
        /// </param>
        /// <returns>
        /// </returns>
        public static LinearGradientBrush CreateSteppedGradientBrush(LinearGradientBrush gradient, int steps)
        {
            var brush = new LinearGradientBrush { StartPoint = gradient.StartPoint, EndPoint = gradient.EndPoint };
            int n = gradient.GradientStops.Count;
            for (int i = 0; i < steps; i++)
            {
                double index = 1.0 * i / (steps - 1) * (n - 1);
                var i0 = (int)index;
                double f = index - i0;
                int i1 = i0 + 1;
                if (i1 >= n)
                {
                    i1 = n - 1;
                }

                var c0 = gradient.GradientStops[i0].Color;
                var c1 = gradient.GradientStops[i1].Color;
                var gs0 = new GradientStop();
                var gs1 = new GradientStop();
                gs0.Color = ColorHelper.Interpolate(c0, c1, f);
                gs1.Color = gs0.Color;
                gs0.Offset = 1.0 * i / steps;
                gs1.Offset = 1.0 * (i + 1) / steps;
                brush.GradientStops.Add(gs0);
                brush.GradientStops.Add(gs1);
            }

            return brush;
        }

        /// <summary>
        /// Creates a stepped HSV brush.
        /// </summary>
        /// <param name="nSteps">
        /// The number of steps.
        /// </param>
        /// <returns>
        /// </returns>
        public static LinearGradientBrush CreateSteppedHsvBrush(int nSteps)
        {
            var colors = new List<Color>();
            for (int i = 0; i < nSteps; i++)
            {
                double hue = (double)i / (nSteps - 1);
                colors.Add(ColorHelper.HsvToColor(hue, 1, 1));
            }

            return CreateSteppedGradientBrush(colors);
        }

    }
}