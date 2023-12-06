using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Controls;

namespace Hippo;

public sealed class ImageExtension : MarkupExtension
{
    public string Path { get; set; }

    public ImageExtension(string path)
    {
        Path = path;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var source = new BitmapImage(new Uri(Path, UriKind.RelativeOrAbsolute));
        return new Image() { Source = source, Height = 24 };
    }
}
