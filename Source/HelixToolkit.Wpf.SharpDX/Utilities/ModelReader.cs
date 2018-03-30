// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Class ModelReader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.IO;
    using System.Windows.Threading;
    using Mesh3DGroup = System.Collections.Generic.List<Object3D>;
    /// <summary>
    /// Class ModelReader.
    /// </summary>
    public abstract class ModelReader : IModelReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelReader"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        protected ModelReader(Dispatcher dispatcher = null)
        {
            this.DefaultMaterial = PhongMaterials.Gold;
            this.Dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public Material DefaultMaterial { get; set; }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <value>The dispatcher.</value>
        public Dispatcher Dispatcher { get; private set; }

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

        /// <summary>
        /// Invokes the specified action on the dispatcher.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void Dispatch(Action action)
        {
            if (this.Dispatcher == null)
            {
                action();
                return;
            }

            this.Dispatcher.Invoke(action);
        }
    }
}