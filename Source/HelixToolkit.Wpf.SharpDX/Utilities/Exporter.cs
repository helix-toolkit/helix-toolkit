// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An abstract base class providing common functionality for exporters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
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
        /// Renders the brush.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="w">
        /// The w.
        /// </param>
        /// <param name="h">
        /// The h.
        /// </param>
        public static void RenderBrush(string path, Brush brush, int w, int h)
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

            var encoder = new PngBitmapEncoder();
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
        public virtual void Export(Viewport3DX viewport)
        {
            this.ExportHeader();
            this.ExportViewport(viewport);

            // Export objects
            viewport.Traverse<GeometryModel3D>(this.ExportModel);

            // Export camera
            //this.ExportCamera(viewport.Camera);

            // Export lights
            //viewport.Children.Traverse<Light>(this.ExportLight);
        }


        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void Export(Element3D model)
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
        protected virtual void ExportViewport(Viewport3DX viewport)
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