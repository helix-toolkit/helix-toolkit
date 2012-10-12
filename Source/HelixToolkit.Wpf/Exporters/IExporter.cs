// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interface for 3D exporters.
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// Exports the specified viewport.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        void Export(Viewport3D viewport);

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">
        /// The visual.
        /// </param>
        void Export(Visual3D visual);

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        void Export(Model3D model);

    }
}