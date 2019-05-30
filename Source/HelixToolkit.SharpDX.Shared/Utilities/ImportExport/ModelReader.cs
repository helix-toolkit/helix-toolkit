// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Class ModelReader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.IO;
using SharpDX;

#if !NETFX_CORE
using System.Windows.Threading;
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Mesh3DGroup = System.Collections.Generic.List<Object3D>;
    using Model;
    /// <summary>
    /// Class ModelReader.
    /// </summary>
    public abstract class ModelReader : IModelReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelReader"/> class.
        /// </summary>
        protected ModelReader()
        {
            this.DefaultMaterial = new PhongMaterialCore
                {
                    Name = "Gold",
                    AmbientColor = new Color4(0.24725f, 0.1995f, 0.0745f, 1.0f),
                    DiffuseColor = new Color4(0.75164f, 0.60648f, 0.22648f, 1.0f),
                    SpecularColor = new Color4(0.628281f, 0.555802f, 0.366065f, 1.0f),
                    EmissiveColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f),
                    SpecularShininess = 51.2f,
                };
        }
        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public MaterialCore DefaultMaterial { get; set; }

        /// <summary>
        /// Gets or sets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets the texture path.
        /// </summary>
        /// <value>The texture path.</value>
        public string TexturePath
        {
            get
            {
                return this.Directory;
            }

            set
            {
                this.Directory = value;
            }
        }

        /// <summary>
        /// Reads the model from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="info"></param>
        /// <returns>The model.</returns>
        public virtual Mesh3DGroup Read(string path, ModelInfo info = default(ModelInfo))
        {
            this.Directory = Path.GetDirectoryName(path);
            using (var s = File.OpenRead(path))
            {
                return this.Read(s, info);
            }
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <param name="info"></param>
        /// <returns>The model.</returns>
        public abstract Mesh3DGroup Read(Stream s, ModelInfo info = default(ModelInfo));
    }
}