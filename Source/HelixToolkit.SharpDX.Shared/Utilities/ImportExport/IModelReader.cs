// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interface for model readers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.IO;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    using Mesh3DGroup = System.Collections.Generic.List<Object3D>;
    public struct ModelInfo
    {
        public MeshFaces Faces { get; set; }
        public bool Normals { get; set; }
        public bool Tangents { get; set; }
    }

    /// <summary>
    /// Interface for model readers.
    /// </summary>
    public interface IModelReader
    {
        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        Mesh3DGroup Read(string path, ModelInfo info = default(ModelInfo));

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">
        /// The stream.
        /// </param>
        /// <param name="info">
        /// The model info.
        /// </param>
        /// <returns>
        /// The model.
        /// </returns>
        Mesh3DGroup Read(Stream s, ModelInfo info = default(ModelInfo));

    }
}