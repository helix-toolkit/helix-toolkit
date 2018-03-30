// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Camera.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Cameras;

    /// <summary>
    /// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
    /// </summary>
    public abstract class Camera : Animatable
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public abstract Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        public abstract Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public abstract Vector3D UpDirection { get; set; }


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

        protected abstract CameraCore CreatePortableCameraCore();

        public static implicit operator CameraCore(Camera camera)
        {
            return camera == null ? null : camera.CameraInternal;
        }
    }
}