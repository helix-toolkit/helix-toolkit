// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrthographicCamera.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents an orthographic projection camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using HelixToolkit.Wpf.SharpDX.Cameras;

    /// <summary>
    /// Represents an orthographic projection camera.
    /// </summary>
    public class OrthographicCamera : ProjectionCamera
    {
        /// <summary>
        /// The width property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(OrthographicCamera), new PropertyMetadata(10.0, (d,e)=> 
            {
                ((d as Camera).CameraInternal as OrthographicCameraCore).Width = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        public OrthographicCamera()
        {
            // default values for near-far must be different for ortho:
            NearPlaneDistance = -10.0;
            FarPlaneDistance = 100.0;
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

        protected override CameraCore CreatePortableCameraCore()
        {
            return new OrthographicCameraCore()
            {
                CreateLeftHandSystem = this.CreateLeftHandSystem,
                FarPlaneDistance = (float)this.FarPlaneDistance,
                LookDirection = this.LookDirection.ToVector3(),
                NearPlaneDistance = (float)this.NearPlaneDistance,
                Position = this.Position.ToVector3(),
                UpDirection = this.UpDirection.ToVector3(),
                Width = (float)this.Width
            };
        }
    }
}