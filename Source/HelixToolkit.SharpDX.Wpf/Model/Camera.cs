namespace HelixToolkit.SharpDX
{
    using System.Windows;

    using global::SharpDX;

    /// <summary>
    /// Provides a base class for cameras.
    /// </summary>
    public abstract class Camera : DependencyObject
    {
        public abstract Matrix CreateViewMatrix();
        public abstract Matrix CreateProjectionMatrix(double aspectRatio);
    }
}