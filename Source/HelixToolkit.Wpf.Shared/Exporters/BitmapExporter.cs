// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports a Viewport3D to a .bmp, .png or .jpg file.
// </summary>
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
    /// Exports a <see cref="Viewport3D"/> to a .bmp, .png or .jpg file.
    /// </summary>
    public class BitmapExporter : IExporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapExporter"/> class.
        /// </summary>
        public BitmapExporter()
        {
            this.OversamplingMultiplier = 2;
            this.Format = OutputFormat.Png;
        }

        /// <summary>
        /// Specifies the output format.
        /// </summary>
        public enum OutputFormat
        {
            /// <summary>
            /// Output to PNG.
            /// </summary>
            Png,

            /// <summary>
            /// Output to JPEG.
            /// </summary>
            Jpg,

            /// <summary>
            /// Output to Bitmap.
            /// </summary>
            Bmp
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
        public OutputFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the oversampling multiplier.
        /// </summary>
        /// <value>The oversampling multiplier.</value>
        public int OversamplingMultiplier { get; set; }

        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="stream">The output stream.</param>
        /// <exception cref="System.InvalidOperationException">Not supported file format.</exception>
        public void Export(Viewport3D viewport, Stream stream)
        {
            var background = this.Background ?? Brushes.Transparent;

            var bmp = viewport.RenderBitmap(background, this.OversamplingMultiplier);
            BitmapEncoder encoder;
            switch (this.Format)
            {
                case OutputFormat.Jpg:
                    encoder = new JpegBitmapEncoder();
                    break;
                case OutputFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    break;
                case OutputFormat.Png:
                    encoder = new PngBitmapEncoder();
                    break;
                default:
                    throw new InvalidOperationException("Not supported file format.");
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(stream);
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="stream">The output stream.</param>
        /// <exception cref="System.NotImplementedException">Cannot export a visual to a bitmap.</exception>
        public void Export(Visual3D visual, Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The output stream.</param>
        /// <exception cref="System.NotImplementedException">Cannot export a model to a bitmap.</exception>
        public void Export(Model3D model, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}