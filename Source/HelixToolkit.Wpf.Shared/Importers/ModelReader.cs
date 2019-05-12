// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReader.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Class ModelReader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.IO;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

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
            this.DefaultMaterial = Materials.Gold;
            this.Dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the model should be frozen.
        /// </summary>
        /// <value><c>true</c> if model should be frozen; otherwise, <c>false</c>.</value>
        public bool Freeze { get; set; }

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
        /// <returns>The model.</returns>
        public virtual Model3DGroup Read(string path)
        {
            this.Directory = Path.GetDirectoryName(path);
            using (var s = File.OpenRead(path))
            {
                return this.Read(s);
            }
        }

        /// <summary>
        /// Reads the model from the specified stream.
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns>The model.</returns>
        public abstract Model3DGroup Read(Stream s);

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