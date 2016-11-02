// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageExtension.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HippoDemo
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    public class ImageExtension : MarkupExtension
    {
        public string Path { get; set; }

        public ImageExtension(string path)
        {
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var source = new BitmapImage(new Uri(Path, UriKind.Relative));
            return new Image() { Source = source, Height=24 };
        }
    }
}