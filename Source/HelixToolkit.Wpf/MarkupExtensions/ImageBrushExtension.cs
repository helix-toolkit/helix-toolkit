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
        /// The path.
        /// </summary>
        private readonly string path;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBrushExtension"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public ImageBrushExtension(string path)
        {
            this.path = path;
        }

        #endregion

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
            var fullPath = Path.GetFullPath(this.path);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fullPath);
            image.EndInit();
            return new ImageBrush(image);
        }

        #endregion
    }
}