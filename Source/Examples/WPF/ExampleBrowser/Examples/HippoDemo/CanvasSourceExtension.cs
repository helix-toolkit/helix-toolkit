// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CanvasSourceExtension.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HippoDemo
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

    public class CanvasSourceExtension : MarkupExtension
    {
        public string Path { get; set; }

        public double Height { get; set; }
        public CanvasSourceExtension(string path, double height)
        {
            Path = path;
            Height = height;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var canvas = Application.LoadComponent(new Uri(Path, UriKind.Relative)) as Canvas;
            double scale = Height / canvas.Height;
            canvas.LayoutTransform = new ScaleTransform(scale, scale);
            return canvas;
        }
    }
}