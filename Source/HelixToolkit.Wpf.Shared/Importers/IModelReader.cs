// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interface for model readers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.IO;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Interface for model readers.
    /// </summary>
    public interface IModelReader
    {
        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The model.</returns>
        Model3DGroup Read(string path);

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns>The model.</returns>
        Model3DGroup Read(Stream s);
    }
}