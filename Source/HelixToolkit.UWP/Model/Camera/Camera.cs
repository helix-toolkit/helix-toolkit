/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.UWP
{
    using global::SharpDX;
    using Cameras;
    using Windows.UI.Xaml;
    using Windows.UI.Composition;

    /// <summary>
    /// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
    /// </summary>
    public abstract class Camera : DependencyObject
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        public abstract Vector3 LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public abstract Vector3 UpDirection { get; set; }

        private CameraCore core;
        public CameraCore CameraInternal
        {
            get
            {
                if (core == null)
                {
                    core = CreatePortableCameraCore();
                }
                return core;
            }
        }

        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateViewMatrix() { return CameraInternal.CreateViewMatrix(); }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateProjectionMatrix(double aspectRatio) { return CameraInternal.CreateProjectionMatrix((float)aspectRatio); }

        protected abstract CameraCore CreatePortableCameraCore();

        public static implicit operator CameraCore(Camera camera)
        {
            return camera == null ? null : camera.CameraInternal;
        }
    }
}
