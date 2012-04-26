// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media.Media3D;
    using System.Xml;

    /// <summary>
    /// Exports a Viewport3D or 3D model to XAML.
    /// </summary>
    public class XamlExporter : IExporter, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The xw.
        /// </summary>
        private readonly XmlTextWriter xw;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlExporter"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public XamlExporter(string path)
        {
            this.CreateResourceDictionary = true;
            this.xw = new XmlTextWriter(path, Encoding.UTF8) { Formatting = Formatting.Indented };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether [create resource dictionary].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create resource dictionary]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateResourceDictionary { get; set; }

        #endregion

        #region Public Methods

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
        /// Closes this exporter.
        /// </summary>
        public void Close()
        {
            this.xw.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        public void Export(Viewport3D viewport)
        {
            object obj = viewport;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, this.xw);
        }

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        public void Export(Visual3D visual)
        {
            object obj = visual;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, this.xw);
        }

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public void Export(Model3D model)
        {
            object obj = model;
            if (this.CreateResourceDictionary)
            {
                obj = WrapInResourceDictionary(obj);
            }

            XamlWriter.Save(obj, this.xw);
        }

        #endregion
    }
}