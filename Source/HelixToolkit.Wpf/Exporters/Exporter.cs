// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// An abstract base class providing common functionality for exporters.
    /// </summary>
    public abstract class Exporter : IExporter, IDisposable
    {
        /// <summary>
        /// The disposed flag.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Renders the brush to an image.
        /// </summary>
        /// <param name="path">
        /// The output path. If the path extension is .png, a PNG image is generated, otherwise a JPEG image.
        /// </param>
        /// <param name="brush">
        /// The brush to render.
        /// </param>
        /// <param name="w">
        /// The width of the output image.
        /// </param>
        /// <param name="h">
        /// The height of the output image.
        /// </param>
        /// <param name="qualityLevel">
        /// The quality level of the image (only used if an JPEG image is exported). 
        /// The value range is 1 (lowest quality) to 100 (highest quality). 
        /// </param>
        public static void RenderBrush(string path, Brush brush, int w, int h, int qualityLevel = 90)
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

            var ext = (Path.GetExtension(path) ?? string.Empty).ToLower();

            BitmapEncoder encoder;
            if (ext == ".png")
            {
                encoder = new PngBitmapEncoder();
            }
            else
            {
                encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stm = File.Create(path))
            {
                encoder.Save(stm);
            }
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Exports the specified viewport.
        /// Exports model, camera and lights.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public virtual void Export(Viewport3D viewport)
        {
            this.ExportHeader();
            this.ExportViewport(viewport);

            // Export objects
            viewport.Children.Traverse<GeometryModel3D>(this.ExportModel);

            // Export camera
            this.ExportCamera(viewport.Camera);

            // Export lights
            viewport.Children.Traverse<Light>(this.ExportLight);
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        public void Export(Visual3D visual)
        {
            this.ExportHeader();
            visual.Traverse<GeometryModel3D>(this.ExportModel);
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void Export(Model3D model)
        {
            this.ExportHeader();
            model.Traverse<GeometryModel3D>(this.ExportModel);
        }

        /// <summary>
        /// Exports the camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        protected virtual void ExportCamera(Camera camera)
        {
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        protected virtual void ExportHeader()
        {
        }

        /// <summary>
        /// Exports the light.
        /// </summary>
        /// <param name="light">
        /// The light.
        /// </param>
        /// <param name="inheritedTransform">
        /// The inherited transform.
        /// </param>
        protected virtual void ExportLight(Light light, Transform3D inheritedTransform)
        {
        }

        /// <summary>
        /// Exports the model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="inheritedTransform">
        /// The inherited transform.
        /// </param>
        protected virtual void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
        }

        /// <summary>
        /// Exports the viewport.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        protected virtual void ExportViewport(Viewport3D viewport)
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            this.disposed = true;
        }
    }
}