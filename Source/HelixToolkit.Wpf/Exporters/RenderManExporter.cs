// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
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
    public class RenderManExporter : Exporter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public RenderManExporter(string path)
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

            // todo
        }

    }
}