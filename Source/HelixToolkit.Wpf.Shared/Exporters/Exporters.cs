// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporters.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Contains a list of all supported exporters.
// </summary>
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
            "Bitmap Files (*.png;*.jpg)|*.png;*.jpg|XAML Files (*.xaml)|*.xaml|Kerkythea Files (*.xml)|*.xml|Wavefront Files (*.obj)|*.obj|Wavefront Files zipped (*.objz)|*.objz|Extensible 3D Graphics Files (*.x3d)|*.x3d|Collada Files (*.dae)|*.dae|STereoLithography (*.stl)|*.stl";

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
            switch (ext.ToLower())
            {
                case ".png":
                case ".jpg":
                    return new BitmapExporter();
                case ".obj":
                case ".objz":
                    return new ObjExporter();
                case ".xaml":
                    return new XamlExporter();
                case ".xml":
                    return new KerkytheaExporter();
                case ".x3d":
                    return new X3DExporter();
                case ".dae":
                    return new ColladaExporter();
                case ".stl":
                    return new StlExporter();
                default:
                    throw new InvalidOperationException("File format not supported.");
            }
        }
    }
}