// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrthographicCamera.cs" company="">
//   
// </copyright>
// <summary>
//   Represents an orthographic projection camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.SharpDX
{
    using System.Windows;

    using global::SharpDX;

    /// <summary>
    /// Represents an orthographic projection camera.
    /// </summary>
    public class OrthographicCamera : ProjectionCamera
    {
        /// <summary>
        /// The width property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(OrthographicCamera), new PropertyMetadata(50.0));

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }

            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">
        /// The aspect ratio.
        /// </param>
        /// <returns>
        /// A <see cref="Matrix"/>.
        /// </returns>
        public override Matrix CreateProjectionMatrix(double aspectRatio)
        {
            if (this.CreateLeftHandSystem)
            {
                return Matrix.OrthoLH(
                    (float)this.Width, 
                    (float)(this.Width * aspectRatio), 
                    (float)this.NearPlaneDistance, 
                    (float)this.FarPlaneDistance);
            }

            return Matrix.OrthoRH(
                (float)this.Width, 
                (float)(this.Width * aspectRatio), 
                (float)this.NearPlaneDistance, 
                (float)this.FarPlaneDistance);
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new OrthographicCamera();
        }
    }
}