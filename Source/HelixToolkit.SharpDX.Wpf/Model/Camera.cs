namespace HelixToolkit.SharpDX
{
    using System.Windows;
    using System.Windows.Media.Animation;

    using global::SharpDX;

    /// <summary>
    /// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
    /// </summary>
    public abstract class Camera : Animatable
    {
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