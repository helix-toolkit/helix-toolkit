// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VrmlExporter.cs" company="Helix 3D Toolkit">
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
    /// Exports the 3D visual tree to a VRML97 (2.0) file.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/Vrml
    /// http://en.wikipedia.org/wiki/Web3D
    /// VRML plugin/browser detector:
    /// http://cic.nist.gov/vrml/vbdetect.html
    /// Links
    /// http://openvrml.org/
    /// </remarks>
    public class VrmlExporter : Exporter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VrmlExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public VrmlExporter(string path, string title = null)
        {
            this.writer = new StreamWriter(path, false, Encoding.UTF8);
            this.writer.WriteLine("# VRML V2.0 utf8");
            if (title != null)
            {
                this.writer.WriteLine("# " + title);
            }
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

            // writer.WriteLine("Transform {");
            // todo: add transform from model.Transform and inheritedTransform
            this.writer.WriteLine("Shape {");

            this.writer.WriteLine("  appearance Appearance {");

            // todo: set material properties from model.Material
            this.writer.WriteLine("    material Material {");
            this.writer.WriteLine("      diffuseColor 0.8 0.8 0.2");
            this.writer.WriteLine("      specularColor 0.5 0.5 0.5");
            this.writer.WriteLine("    }");
            this.writer.WriteLine("  }"); // Appearance

            this.writer.WriteLine("  geometry IndexedFaceSet {");
            this.writer.WriteLine("    coord Coordinate {");
            this.writer.WriteLine("      point [");

            foreach (var pt in mesh.Positions)
            {
                this.writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2},", pt.X, pt.Y, pt.Z));
            }

            this.writer.WriteLine("      ]");
            this.writer.WriteLine("    }");

            this.writer.WriteLine("    coordIndex [");
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                this.writer.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} {1} {2},",
                        mesh.TriangleIndices[i],
                        mesh.TriangleIndices[i + 1],
                        mesh.TriangleIndices[i + 2]));
            }

            this.writer.WriteLine("    ]");
            this.writer.WriteLine("  }"); // IndexedFaceSet

            this.writer.WriteLine("}"); // Shape

            // writer.WriteLine("}"); // Transform
        }

    }
}