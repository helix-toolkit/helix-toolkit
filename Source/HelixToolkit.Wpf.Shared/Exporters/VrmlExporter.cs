// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VrmlExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports the 3D visual tree to a VRML97 (2.0) file.
// </summary>
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
    /// See <a href="http://en.wikipedia.org/wiki/Vrml">Wikipedia</a>, <a href="http://en.wikipedia.org/wiki/Web3D">Web3D</a>,
    /// <a href="http://cic.nist.gov/vrml/vbdetect.html">VRML plugin/browser detector</a>,
    /// and <a href="http://openvrml.org/">openvrml.org</a>.
    /// </remarks>
    public class VrmlExporter : Exporter<StreamWriter>
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Creates the writer for the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The writer.</returns>
        protected override StreamWriter Create(Stream stream)
        {
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.WriteLine("# VRML V2.0 utf8");
            if (this.Title != null)
            {
                writer.WriteLine("# " + this.Title);
            }

            return writer;
        }

        /// <summary>
        /// Closes the export writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Close(StreamWriter writer)
        {
            writer.Close();
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

            // writer.WriteLine("Transform {");
            // todo: add transform from model.Transform and inheritedTransform
            writer.WriteLine("Shape {");

            writer.WriteLine("  appearance Appearance {");

            // todo: set material properties from model.Material
            writer.WriteLine("    material Material {");
            writer.WriteLine("      diffuseColor 0.8 0.8 0.2");
            writer.WriteLine("      specularColor 0.5 0.5 0.5");
            writer.WriteLine("    }");
            writer.WriteLine("  }"); // Appearance

            writer.WriteLine("  geometry IndexedFaceSet {");
            writer.WriteLine("    coord Coordinate {");
            writer.WriteLine("      point [");

            foreach (var pt in mesh.Positions)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2},", pt.X, pt.Y, pt.Z));
            }

            writer.WriteLine("      ]");
            writer.WriteLine("    }");

            writer.WriteLine("    coordIndex [");
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                writer.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0} {1} {2},",
                        mesh.TriangleIndices[i],
                        mesh.TriangleIndices[i + 1],
                        mesh.TriangleIndices[i + 2]));
            }

            writer.WriteLine("    ]");
            writer.WriteLine("  }"); // IndexedFaceSet

            writer.WriteLine("}"); // Shape

            // writer.WriteLine("}"); // Transform
        }
    }
}