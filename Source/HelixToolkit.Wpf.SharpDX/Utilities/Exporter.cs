/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

namespace HelixToolkit.Wpf.SharpDX
{
    using Model.Scene;
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// An abstract base class providing common functionality for exporters.
    /// </summary>
    public abstract class Exporter : IExporter, IDisposable
    {
        /// <summary>
        /// Traverses the Viewport3DX tree and invokes the specified action on each SceneNode of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private static void Traverse<T>(Viewport3DX viewport, Action<T, Transform3D> action) where T : SceneNode
        {
            //foreach (var element in viewport.Renderables)
            //{
            //    if (element is T node)
            //    {
            //        if (element.WrapperSource is Element3D e)
            //        {
            //            action(node, e.Transform);
            //        }
            //    }

            //    Traverse(element, action);
            //}

            foreach (var element in viewport.Items)
            {
                Traverse(element, action);
            }
        }

        /// <summary>
        /// Traverses the SceneNode tree and invokes the specified action on each SceneNode of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private static void Traverse<T>(SceneNode model, Action<T, Transform3D> action) where T : SceneNode
        {
            if (model is T)
            {
                if (model.WrapperSource is Element3D m)
                {
                    action((T)model, m.Transform);
                }
            }
            foreach (var element in model.Items)
            {
                Traverse(element, action);
            }
        }

        /// <summary>
        /// The disposed flag.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Renders the brush.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="brush">
        /// The brush.
        /// </param>
        public static void RenderBrush(string path, Stream brush)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(brush));

            using (Stream stm = File.Create(path))
            {
                encoder.Save(stm);
            }
        }

        /// <summary>
        /// Exports the specified viewport.
        /// Exports model, camera and lights.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public void Export(Viewport3DX viewport)
        {
            this.ExportHeader();
            this.ExportViewport(viewport);

            // Export objects
            Traverse<MeshNode>(viewport, this.ExportModel);

            // Export camera
            this.ExportCamera(viewport.Camera);

            // Export lights
            Traverse<LightNode>(viewport, this.ExportLight);
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void Export(SceneNode model)
        {
            this.ExportHeader();
            Traverse<MeshNode>(model, this.ExportModel);
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        protected virtual void ExportHeader()
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
        /// Exports the model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        protected virtual void ExportModel(MeshNode model, Transform3D transform)
        {
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
        /// Exports the light.
        /// </summary>
        /// <param name="light">
        /// The light.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        protected virtual void ExportLight(LightNode light, Transform3D transform)
        {
        }
    }
}
