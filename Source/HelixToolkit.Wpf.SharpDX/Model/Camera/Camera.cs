namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

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
        public abstract Matrix CreateViewMatrix();

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        public abstract Matrix CreateProjectionMatrix(double aspectRatio);
    }
}