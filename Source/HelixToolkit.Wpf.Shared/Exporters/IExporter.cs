// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExporter.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Specifies functionality to export 3D models.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Specifies functionality to export 3D models.
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// Exports the specified viewport (including model, camera and lights).
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="stream">The output stream.</param>
        void Export(Viewport3D viewport, Stream stream);

        /// <summary>
        /// Exports the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="stream">The output stream.</param>
        void Export(Visual3D visual, Stream stream);

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The output stream.</param>
        void Export(Model3D model, Stream stream);
    }
}