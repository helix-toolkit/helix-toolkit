// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageBrushExtension.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Markupextension for Image brushes
    /// </summary>
    /// <example>
    /// <code>
    /// Fill={helix:ImageBrush images\\myimage.png}
    ///   </code>
    /// </example>
    public class ImageBrushExtension : MarkupExtension
    {
        #region Constants and Fields

        /// <summary>
        /// The uri.
        /// </summary>
        private readonly string uri;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBrushExtension"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        public ImageBrushExtension(string uri)
        {
            this.uri = uri;
            this.UriKind = UriKind.RelativeOrAbsolute;            
        }

        #endregion

        /// <summary>
        /// Gets or sets the kind of the URI.
        /// </summary>
        /// <value>The kind of the URI.</value>
        public UriKind UriKind { get; set; }

        #region Public Methods

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// Object that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //var fullPath = Path.GetFullPath(this.uri);
            //if (!File.Exists(fullPath))
            //{
            //    return null;
            //}

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(this.uri, this.UriKind);
            image.EndInit();
            return new ImageBrush(image);
        }

        #endregion
    }
}