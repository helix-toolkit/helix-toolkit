// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerspectiveCamera.cs" company="">
//   
// </copyright>
// <summary>
//   Represents a perspective projection camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.SharpDX
{
    using System.Windows;

    using global::SharpDX;

    /// <summary>
    /// Represents a perspective projection camera.
    /// </summary>
    public class PerspectiveCamera : ProjectionCamera
    {
        /// <summary>
        /// The field of view property
        /// </summary>
        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
            "FieldOfView", typeof(double), typeof(PerspectiveCamera), new PropertyMetadata(45.0));

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        public double FieldOfView
        {
            get
            {
                return (double)this.GetValue(FieldOfViewProperty);
            }

            set
            {
                this.SetValue(FieldOfViewProperty, value);
            }
        }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">
        /// The aspect ratio.
        /// </param>
        /// <returns>
        /// The <see cref="Matrix"/>.
        /// </returns>
        public override Matrix CreateProjectionMatrix(double aspectRatio)
        {
            if (this.CreateLeftHandSystem)
            {
                return global::SharpDX.Matrix.PerspectiveFovLH(
                    (float)this.FieldOfView, 
                    (float)aspectRatio, 
                    (float)this.NearPlaneDistance, 
                    (float)this.FarPlaneDistance);
            }

            return global::SharpDX.Matrix.PerspectiveFovRH(
                (float)this.FieldOfView, (float)aspectRatio, (float)this.NearPlaneDistance, (float)this.FarPlaneDistance);
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new PerspectiveCamera();
        }
    }
}