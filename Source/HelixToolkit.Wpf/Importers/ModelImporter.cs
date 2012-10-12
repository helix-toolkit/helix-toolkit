// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelImporter.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Imports a model from a file.
    /// </summary>
    public static class ModelImporter
    {
        /// <summary>
        /// Loads a model from the specified path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// A model.
        /// </returns>
        public static Model3DGroup Load(string path)
        {
            if (path == null)
            {
                return null;
            }

            Model3DGroup model = null;
            string ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".3ds":
                    {
                        var r = new StudioReader();
                        model = r.Read(path);
                        break;
                    }

                case ".lwo":
                    {
                        var r = new LwoReader();
                        model = r.Read(path);

                        break;
                    }

                case ".obj":
                    {
                        var r = new ObjReader();
                        model = r.Read(path);
                        break;
                    }

                case ".objz":
                    {
                        var r = new ObjReader();
                        model = r.ReadZ(path);
                        break;
                    }

                case ".stl":
                    {
                        var r = new StLReader();
                        model = r.Read(path);
                        break;
                    }

                case ".off":
                    {
                        var r = new OffReader();
                        model = r.Read(path);
                        break;
                    }

                default:
                    throw new InvalidOperationException("File format not supported.");
            }

            return model;
        }

    }
}