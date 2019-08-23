// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides helper methods to generate xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Xml;

    /// <summary>
    /// Provides helper methods to generate xaml.
    /// </summary>
    public class XamlHelper
    {
        /// <summary>
        /// Gets the xaml for the specified viewport.
        /// </summary>
        /// <param name="view">
        /// The viewport.
        /// </param>
        /// <returns>
        /// The get xaml.
        /// </returns>
        public static string GetXaml(Viewport3D view)
        {
            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
                XamlWriter.Save(view, xw);

                string xaml = sb.ToString();
                xaml =
                    xaml.Replace(
                        string.Format("<Viewport3D Height=\"{0}\" Width=\"{1}\" ", view.ActualHeight, view.ActualWidth),
                        "<Viewport3D ");

                return xaml;
            }
        }

        /// <summary>
        /// Gets the xaml for the specified object.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The get xaml.
        /// </returns>
        public static string GetXaml(object obj)
        {
            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                var xw = new XmlTextWriter(tw) { Formatting = Formatting.Indented };
                XamlWriter.Save(obj, xw);
            }

            return sb.ToString();
        }

    }
}