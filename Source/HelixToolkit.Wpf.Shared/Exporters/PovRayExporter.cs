// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PovRayExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports the 3D visual tree to a PovRay input file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Exports the 3D visual tree to a PovRay input file.
    /// </summary>
    /// <remarks>
    /// See <a href="http://www.povray.org">povray.org</a>.
    /// </remarks>
    public class PovRayExporter : Exporter<StreamWriter>
    {
        /// <summary>
        /// Creates the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>StreamWriter.</returns>
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
            // http://www.povray.org/documentation/view/3.6.1/17/
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
            // http://www.povray.org/documentation/view/3.6.1/34/
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

            // http://www.povray.org/documentation/view/3.6.1/293/

            // todo: create textures/material properties from model.Material
            writer.WriteLine("mesh2 {");

            writer.WriteLine("  vertex_vectors");
            writer.WriteLine("  {");
            writer.WriteLine("    " + mesh.Positions.Count + ",");

            foreach (var pt in mesh.Positions)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "    {0} {1} {2},", pt.X, pt.Y, pt.Z));
            }

            writer.WriteLine("  }");

            writer.WriteLine("  face_indices");
            writer.WriteLine("  {");
            writer.WriteLine("    " + (mesh.TriangleIndices.Count / 3) + ",");
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                writer.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "    {0} {1} {2},",
                        mesh.TriangleIndices[i],
                        mesh.TriangleIndices[i + 1],
                        mesh.TriangleIndices[i + 2]));
            }

            writer.WriteLine("  }");

            // todo: add transform from model.Transform and inheritedTransform
            // http://www.povray.org/documentation/view/3.6.1/49/
            writer.WriteLine("}"); // mesh2
        }
    }
}