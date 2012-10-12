// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporters.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;

    /// <summary>
    /// Contains a list of all supported exporters.
    /// </summary>
    public static class Exporters
    {
        /// <summary>
        /// Default file export extension.
        /// </summary>
        public static readonly string DefaultExtension = ".png";

        /// <summary>
        /// File filter for all the supported exporters.
        /// </summary>
        public static readonly string Filter =
            "Bitmap Files (*.png;*.jpg)|*.png;*.jpg|XAML Files (*.xaml)|*.xml|Kerkythea Files (*.xml)|*.xml|Wavefront Files (*.obj)|*.obj|Wavefront Files zipped (*.objz)|*.objz|Extensible 3D Graphics Files (*.x3d)|*.x3d|Collada Files (*.dae)|*.dae";

        /// <summary>
        /// Creates an exporter based on the extension of the specified path.
        /// </summary>
        /// <param name="path">
        /// The output path.
        /// </param>
        /// <returns>
        /// An exporter.
        /// </returns>
        public static IExporter Create(string path)
        {
            if (path == null)
            {
                return null;
            }

            string ext = Path.GetExtension(path);
            if (ext == null)
            {
                return null;
            }

            switch (ext.ToLower())
            {
                case ".png":
                case ".jpg":
                    return new BitmapExporter(path);
                case ".obj":
                case ".objz":
                    return new ObjExporter(path);
                case ".xml":
                    return new KerkytheaExporter(path);
                case ".x3d":
                    return new X3DExporter(path);
                case ".dae":
                    return new ColladaExporter(path);
                default:
                    throw new InvalidOperationException("File format not supported.");
            }
        }

    }
}