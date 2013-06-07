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
    public class ModelImporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelImporter"/> class.
        /// </summary>
        public ModelImporter()
        {
            this.DefaultMaterial = Materials.Blue;
        }

        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public Material DefaultMaterial { get; set; }

        /// <summary>
        /// Loads a model from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// A model.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">File format not supported.</exception>
        public Model3DGroup Load(string path)
        {
            if (path == null)
            {
                return null;
            }

            Model3DGroup model;
            var ext = Path.GetExtension(path);
            if (ext != null)
            {
                ext = ext.ToLower();
            }

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
                        var r = new LwoReader { DefaultMaterial = this.DefaultMaterial };
                        model = r.Read(path);

                        break;
                    }

                case ".obj":
                    {
                        var r = new ObjReader { DefaultMaterial = this.DefaultMaterial };
                        model = r.Read(path);
                        break;
                    }

                case ".objz":
                    {
                        var r = new ObjReader { DefaultMaterial = this.DefaultMaterial };
                        model = r.ReadZ(path);
                        break;
                    }

                case ".stl":
                    {
                        var r = new StLReader { DefaultMaterial = this.DefaultMaterial };
                        model = r.Read(path);
                        break;
                    }

                case ".off":
                    {
                        var r = new OffReader { DefaultMaterial = this.DefaultMaterial };
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