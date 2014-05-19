// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
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
        void Export(Viewport3DX viewport);

        /// <summary>
        /// Exports the specified model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        void Export(Model3D model);
    }
}