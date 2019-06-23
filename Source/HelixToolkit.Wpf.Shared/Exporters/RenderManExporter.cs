// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports the 3D visual tree to a RenderMan input file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Text;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Exports the 3D visual tree to a RenderMan input file.
    /// </summary>
    /// <remarks>
    /// See https://renderman.pixar.com/products/rispec/rispec_pdf/RISpec3_2.pdf
    /// </remarks>
    public class RenderManExporter : Exporter<StreamWriter>
    {
        /// <summary>
        /// Creates the writer for the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The output writer.</returns>
        protected override StreamWriter Create(Stream stream)
        {
            return new StreamWriter(stream, Encoding.UTF8);
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Close(StreamWriter writer)
        {
            writer.Close();
        }

        /// <summary>
        /// Exports the camera.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="camera">The camera.</param>
        protected override void ExportCamera(StreamWriter writer, Camera camera)
        {
            base.ExportCamera(writer, camera);

            // todo...
        }

        /// <summary>
        /// Exports the light.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="light">The light.</param>
        /// <param name="inheritedTransform">The inherited transform.</param>
        protected override void ExportLight(StreamWriter writer, Light light, Transform3D inheritedTransform)
        {
            base.ExportLight(writer, light, inheritedTransform);

            // todo...
        }

        /// <summary>
        /// Exports the model.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="model">The model.</param>
        /// <param name="inheritedTransform">The inherited transform.</param>
        protected override void ExportModel(StreamWriter writer, GeometryModel3D model, Transform3D inheritedTransform)
        {
            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
            {
                return;
            }

            // todo
        }
    }
}