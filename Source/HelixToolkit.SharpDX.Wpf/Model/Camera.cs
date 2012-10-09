namespace HelixToolkit.SharpDX
{
    using System.Windows;
    using System.Windows.Media.Media3D;
    using global::SharpDX;

    /// <summary>
    /// Provides a base class for cameras.
    /// </summary>
    public abstract class Camera : DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>        
        public abstract Matrix CreateViewMatrix();

        /// <summary>
        /// 
        /// </summary>
        public abstract Matrix CreateProjectionMatrix(double aspectRatio);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Camera), new PropertyMetadata(Transform3D.Identity, TransformChanged));
                           
        /// <summary>
        /// 
        /// </summary>
        protected static void TransformChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((Camera)obj).OnTransformChanged(args);           
        }

        /// <summary>
        /// 
        /// </summary>        
        protected virtual void OnTransformChanged(DependencyPropertyChangedEventArgs args)
        { }

        /// <summary>
        /// Gets or sets the Transform3D applied to the camera.
        /// Returns: Transform3D applied to the camera.
        /// </summary>
        public Transform3D Transform { get { return (Transform3D)GetValue(TransformProperty); } set { SetValue(TransformProperty, value); } }
    }
}