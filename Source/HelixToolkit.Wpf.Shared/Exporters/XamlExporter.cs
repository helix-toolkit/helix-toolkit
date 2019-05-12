// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Exports a Viewport3D or 3D model to XAML.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media.Media3D;
    using System.Xml;

    /// <summary>
    /// Exports a Viewport3D or 3D model to XAML.
    /// </summary>
    public class XamlExporter : Exporter<XmlWriter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XamlExporter" /> class.
        /// </summary>
        public XamlExporter()
        {
            this.CreateResourceDictionary = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create a resource dictionary.
        /// </summary>
        /// <value>
        ///  <c>true</c> if a resource dictionary should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateResourceDictionary { get; set; }

        /// <summary>
        /// Wraps the specified object in a resource dictionary.
        /// </summary>
        /// <param name="obj">
        /// The object to be wrapped.
        /// </param>
        /// <returns>
        /// A resource dictionary.
        /// </returns>
        public static ResourceDictionary WrapInResourceDictionary(object obj)
        {
            var rd = new ResourceDictionary();
            var list = obj as IEnumerable;
            if (list != null)
            {
                int i = 1;
                foreach (var o in list)
                {
                    rd.Add("Model" + i, o);
                    i++;
                }
            }
            else
            {
                rd.Add("Model", obj);
            }

            return rd;
        }

        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="stream">The stream.</param>
        public override void Export(Viewport3D viewport, Stream stream)
        {
            var writer = this.Create(stream);
            object obj = viewport;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, writer);
            this.Close(writer);
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="stream">The stream.</param>
        public override void Export(Visual3D visual, Stream stream)
        {
            var writer = this.Create(stream);
            object obj = visual;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, writer);
            this.Close(writer);
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public override void Export(Model3D model, Stream stream)
        {
            var writer = this.Create(stream);
            object obj = model;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, writer);
            this.Close(writer);
        }

        /// <summary>
        /// Creates a new <see cref="XmlWriter" /> on the specified stream.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <returns>A <see cref="XmlWriter"/>.</returns>
        protected override XmlWriter Create(Stream stream)
        {
            return new XmlTextWriter(stream, Encoding.UTF8) { Formatting = Formatting.Indented };
        }

        /// <summary>
        /// Closes this exporter.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Close(XmlWriter writer)
        {
            writer.Close();
        }
    }
}