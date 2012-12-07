// --------------------------------------------------------------------------------------------------------------------
// <copyright file="X3DExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Media.Media3D;
    using System.Xml;

    /// <summary>
    /// Exports the 3D visual tree to an X3D file.
    /// </summary>
    /// <remarks>
    /// http://en.wikipedia.org/wiki/X3D
    /// http://en.wikipedia.org/wiki/Web3D
    /// Validation
    /// http://www.w3.org/People/mimasa/test/schemas/SCHEMA/x3d-3.0.xsd
    /// http://www.web3d.org/x3d/tools/schematron/X3dSchematron.html
    /// </remarks>
    public class X3DExporter : Exporter
    {
        /// <summary>
        /// The writer.
        /// </summary>
        private readonly XmlTextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="X3DExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public X3DExporter(string path)
        {
            this.Metadata = new Dictionary<string, string>();

            this.writer = new XmlTextWriter(path, Encoding.UTF8);
            this.writer.Formatting = Formatting.Indented;
            this.writer.WriteStartDocument(false);
            this.writer.WriteDocType(
                "X3D", "ISO//Web3D//DTD X3D 3.0//EN", "http://www.web3d.org/specifications/x3d-3.1.dtd", null);
            this.writer.WriteStartElement("X3D");
            this.writer.WriteAttributeString("profile", "Immersive");
            this.writer.WriteAttributeString("version", "3.1");
            this.writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema-instance");
            this.writer.WriteAttributeString(
                "xsd:noNamespaceSchemaLocation", "http://www.web3d.org/specifications/x3d-3.1.xsd");
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            set
            {
                this.SetMetadata("title", value);
            }
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        public override void Close()
        {
            this.writer.WriteEndElement(); // Scene
            this.writer.WriteEndElement(); // X3D
            this.writer.WriteEndDocument();
            this.writer.Close();
            base.Close();
        }

        /// <summary>
        /// Exports the header.
        /// </summary>
        protected override void ExportHeader()
        {
            // http://www.w3.org/TR/SVG/struct.html#SVGElement
            this.writer.WriteStartElement("head");
            foreach (var kvp in this.Metadata)
            {
                this.writer.WriteStartElement("meta");
                this.writer.WriteAttributeString("name", kvp.Key);
                this.writer.WriteAttributeString("value", kvp.Value);
                this.writer.WriteEndElement(); // meta
            }

            this.writer.WriteEndElement(); // head
            this.writer.WriteStartElement("Scene");
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
            var indices = new StringBuilder();
            foreach (int i in mesh.TriangleIndices)
            {
                indices.Append(i + " ");
            }

            var points = new StringBuilder();
            foreach (var pt in mesh.Positions)
            {
                points.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", pt.X, pt.Y, pt.Z);
            }

            this.writer.WriteStartElement("Transform");
            this.writer.WriteStartElement("Shape");
            this.writer.WriteStartElement("IndexedFaceSet");
            this.writer.WriteAttributeString("coordIndex", indices.ToString());
            this.writer.WriteStartElement("Coordinate");
            this.writer.WriteAttributeString("point", points.ToString());
            this.writer.WriteEndElement();
            this.writer.WriteEndElement(); // IndexedFaceSet
            this.writer.WriteStartElement("Appearance");

            this.writer.WriteStartElement("Material");
            this.writer.WriteAttributeString("diffuseColor", "0.8 0.8 0.2");
            this.writer.WriteAttributeString("specularColor", "0.5 0.5 0.5");
            this.writer.WriteEndElement();
            this.writer.WriteEndElement(); // Appearance

            this.writer.WriteEndElement(); // Shape
            this.writer.WriteEndElement(); // Transform
        }

        /// <summary>
        /// The set metadata.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private void SetMetadata(string key, string value)
        {
            if (this.Metadata.ContainsKey(key))
            {
                this.Metadata[key] = value;
            }
            else
            {
                this.Metadata.Add(key, value);
            }
        }

        /* Example
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE X3D PUBLIC "ISO//Web3D//DTD X3D 3.0//EN" "http://www.web3d.org/specifications/x3d-3.0.dtd">
        <X3D profile='Immersive' version='3.0' xmlns:xsd='http://www.w3.org/2001/XMLSchema-instance' xsd:noNamespaceSchemaLocation=' http://www.web3d.org/specifications/x3d-3.0.xsd '>
        <head>
        <meta name='title' content='Pyramid.x3d'/>
        <meta name='creator' content='MV4204 class'/>
        <meta name='created' content='8 July 2000'/>
        <meta name='modified' content='27 December 2003'/>
        <meta name='description' content='Constructing a Pyramid geometric primitive in order to show use of points and coordinate indices.'/>
        <meta name='identifier' content=' http://www.web3d.org/x3d/content/examples/Basic/course/Pyramid.x3d '/>
        <meta name='generator' content='X3D-Edit 3.2, https://savage.nps.edu/X3D-Edit'/>
        <meta name='license' content='../license.html'/>
        </head>
        <Scene>
        <Viewpoint description='Pyramid' orientation='0 1 0 .2' position='4 0 25' fieldOfView='0.7854'/>
        <Transform translation='0 -5 0'>
        <Shape>
        <IndexedFaceSet coordIndex='0 1 2 -1 1 3 2 -1 2 3 0 -1 3 1 0'>
        <Coordinate point='0 0 0 10 0 0 5 0 8.3 5 8.3 2.8'/>
        </IndexedFaceSet>
        <Appearance>
        <Material diffuseColor='0.8 0.8 0.2' specularColor='0 0 0.5'/>
        </Appearance>
        </Shape>
        </Transform>
        </Scene>
        </X3D>
        */
    }
}