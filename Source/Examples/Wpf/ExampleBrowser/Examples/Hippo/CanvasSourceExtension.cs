using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System;
using System.Windows;

namespace Hippo;

public sealed class CanvasSourceExtension : MarkupExtension
{
    public string Path { get; set; }

    public double Height { get; set; }

    public CanvasSourceExtension(string path, double height)
    {
        Path = path;
        Height = height;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (Application.LoadComponent(new Uri(Path, UriKind.RelativeOrAbsolute)) is not Canvas canvas)
        {
            return null;
        }

        double scale = Height / canvas.Height;
        canvas.LayoutTransform = new ScaleTransform(scale, scale);
        return canvas;
    }
}
