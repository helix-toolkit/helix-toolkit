// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class providing common functionality for exporters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a base class providing common functionality for exporters.
    /// </summary>
    /// <typeparam name="T">The type of the output writer.</typeparam>
    public abstract class Exporter<T> : IExporter
    {
        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="stream">The output stream.</param>
        public virtual void Export(Viewport3D viewport, Stream stream)
        {
            var writer = this.Create(stream);
            this.ExportHeader(writer);
            this.ExportViewport(writer, viewport);

            // Export objects
            viewport.Children.Traverse<GeometryModel3D>((m, t) => this.ExportModel(writer, m, t));

            // Export camera
            this.ExportCamera(writer, viewport.Camera);

            // Export lights
            viewport.Children.Traverse<Light>((m, t) => this.ExportLight(writer, m, t));

            this.Close(writer);
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="stream">The output stream.</param>
        public virtual void Export(Visual3D visual, Stream stream)
        {
            var writer = this.Create(stream);
            this.ExportHeader(writer);
            visual.Traverse<GeometryModel3D>((m, t) => this.ExportModel(writer, m, t));
            this.Close(writer);
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The output stream.</param>
        public virtual void Export(Model3D model, Stream stream)
        {
            var writer = this.Create(stream);
            this.ExportHeader(writer);
            model.Traverse<GeometryModel3D>((m, t) => this.ExportModel(writer, m, t));
            this.Close(writer);
        }

        /// <summary>
        /// Creates the writer for the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The writer.</returns>
        protected abstract T Create(Stream stream);

        /// <summary>
        /// Closes the export writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void Close(T writer)
        {
        }

        /// <summary>
        /// Exports the camera.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="camera">The camera.</param>
        protected virtual void ExportCamera(T writer, Camera camera)
        {
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void ExportHeader(T writer)
        {
        }

        /// <summary>
        /// Exports the light.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="light">The light.</param>
        /// <param name="inheritedTransform">The inherited transform.</param>
        protected virtual void ExportLight(T writer, Light light, Transform3D inheritedTransform)
        {
        }

        /// <summary>
        /// Exports the model.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="model">The model.</param>
        /// <param name="inheritedTransform">The inherited transform.</param>
        protected virtual void ExportModel(T writer, GeometryModel3D model, Transform3D inheritedTransform)
        {
        }

        /// <summary>
        /// Exports the viewport.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="viewport">The viewport.</param>
        protected virtual void ExportViewport(T writer, Viewport3D viewport)
        {
        }

        /// <summary>
        /// Renders the brush to a JPG image.
        /// </summary>
        /// <param name="stm">The output stream.</param>
        /// <param name="brush">The brush to render.</param>
        /// <param name="w">The width of the output image.</param>
        /// <param name="h">The height of the output image.</param>
        /// <param name="qualityLevel">The quality level of the JPG image. E.g. 90.
        /// The value range is 1 (lowest quality) to 100 (highest quality).</param>
        protected void RenderBrush(Stream stm, Brush brush, int w, int h, int qualityLevel)
        {
            this.Encode(this.RenderBrush(brush, w, h), stm, qualityLevel);
        }

        /// <summary>
        /// Renders the brush to a PNG image.
        /// </summary>
        /// <param name="stm">The output stream.</param>
        /// <param name="brush">The brush to render.</param>
        /// <param name="w">The width of the output image.</param>
        /// <param name="h">The height of the output image.</param>
        protected void RenderBrush(Stream stm, Brush brush, int w, int h)
        {
            this.Encode(this.RenderBrush(brush, w, h), stm);
        }

        /// <summary>
        /// Renders the specified brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="w">The width.</param>
        /// <param name="h">The height.</param>
        /// <returns>RenderTargetBitmap.</returns>
        protected RenderTargetBitmap RenderBrush(Brush brush, int w, int h)
        {
            var ib = brush as ImageBrush;
            if (ib != null)
            {
                var bi = ib.ImageSource as BitmapImage;
                if (bi != null)
                {
                    w = bi.PixelWidth;
                    h = bi.PixelHeight;
                }
            }

            var bmp = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            var rect = new Grid
            {
                Background = brush,
                Width = 1,
                Height = 1,
                LayoutTransform = new ScaleTransform(w, h)
            };
            rect.Arrange(new Rect(0, 0, w, h));
            bmp.Render(rect);
            return bmp;
        }

        /// <summary>
        /// Encodes the specified bitmap as a PNG image.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="stm">The output stream.</param>
        protected void Encode(RenderTargetBitmap bmp, Stream stm)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(stm);
        }

        /// <summary>
        /// Encodes the specified bitmap as a Jpeg image.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="stm">The output stream.</param>
        /// <param name="qualityLevel">The jpeg quality level.</param>
        protected void Encode(RenderTargetBitmap bmp, Stream stm, int qualityLevel)
        {
            var encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(stm);
        }
    }
}