// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PovRayExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
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
    /// See http://www.povray.org
    /// </remarks>
    public class PovRayExporter : Exporter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PovRayExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public PovRayExporter(string path)
        {
            this.writer = new StreamWriter(path, false, Encoding.UTF8);
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public override void Close()
        {
            this.writer.Close();
            base.Close();
        }

        /// <summary>
        /// Exports the camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        protected override void ExportCamera(Camera camera)
        {
            base.ExportCamera(camera);

            // todo...
            // http://www.povray.org/documentation/view/3.6.1/17/
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
        protected override void ExportLight(Light light, Transform3D inheritedTransform)
        {
            base.ExportLight(light, inheritedTransform);

            // todo...
            // http://www.povray.org/documentation/view/3.6.1/34/
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
        protected override void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
            {
                return;
            }

            // http://www.povray.org/documentation/view/3.6.1/293/

            // todo: create textures/material properties from model.Material
            this.writer.WriteLine("mesh2 {");

            this.writer.WriteLine("  vertex_vectors");
            this.writer.WriteLine("  {");
            this.writer.WriteLine("    " + mesh.Positions.Count + ",");

            foreach (var pt in mesh.Positions)
            {
                this.writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "    {0} {1} {2},", pt.X, pt.Y, pt.Z));
            }

            this.writer.WriteLine("  }");

            this.writer.WriteLine("  face_indices");
            this.writer.WriteLine("  {");
            this.writer.WriteLine("    " + mesh.TriangleIndices.Count / 3 + ",");
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                this.writer.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "    {0} {1} {2},",
                        mesh.TriangleIndices[i],
                        mesh.TriangleIndices[i + 1],
                        mesh.TriangleIndices[i + 2]));
            }

            this.writer.WriteLine("  }");

            // todo: add transform from model.Transform and inheritedTransform
            // http://www.povray.org/documentation/view/3.6.1/49/
            this.writer.WriteLine("}"); // mesh2
        }

    }
}