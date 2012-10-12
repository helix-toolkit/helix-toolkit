// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Exports a Viewport3D to a .bmp or .png file.
    /// </summary>
    public class BitmapExporter : IExporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public BitmapExporter(string path)
        {
            this.FileName = path;
            this.OversamplingMultiplier = 2;
        }

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the oversampling multiplier.
        /// </summary>
        /// <value>The oversampling multiplier.</value>
        public int OversamplingMultiplier { get; set; }

        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public void Export(Viewport3D viewport)
        {
            int m = this.OversamplingMultiplier;

            var background = this.Background;
            if (background == null)
            {
                background = Brushes.Transparent;
            }

            var bmp = Viewport3DHelper.RenderBitmap(viewport, background, m);
            BitmapEncoder encoder;
            string ext = Path.GetExtension(this.FileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                    var jpg = new JpegBitmapEncoder();
                    jpg.Frames.Add(BitmapFrame.Create(bmp));
                    encoder = jpg;
                    break;
                case ".png":
                    var png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(bmp));
                    encoder = png;
                    break;
                default:
                    throw new InvalidOperationException("Not supported file format.");
            }

            using (Stream stm = File.Create(this.FileName))
            {
                encoder.Save(stm);
            }
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        public void Export(Visual3D visual)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void Export(Model3D model)
        {
            throw new NotImplementedException();
        }

    }
}