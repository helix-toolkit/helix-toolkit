// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerspectiveCamera.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a perspective projection camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using HelixToolkit.Wpf.SharpDX.Cameras;

    /// <summary>
    /// Represents a perspective projection camera.
    /// </summary>
    public class PerspectiveCamera : ProjectionCamera
    {
        /// <summary>
        /// The field of view property
        /// </summary>
        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
            "FieldOfView", typeof(double), typeof(PerspectiveCamera), new PropertyMetadata(45.0, 
                (d,e)=>
                {
                    ((d as Camera).CameraInternal as PerspectiveCameraCore).FieldOfView = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        public double FieldOfView
        {
            get { return (double)this.GetValue(FieldOfViewProperty); }
            set { this.SetValue(FieldOfViewProperty, value); }
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

        protected override CameraCore CreatePortableCameraCore()
        {
            return new PerspectiveCameraCore()
            {
                CreateLeftHandSystem = this.CreateLeftHandSystem,
                FarPlaneDistance = (float)this.FarPlaneDistance,
                FieldOfView = (float)this.FieldOfView,
                LookDirection = this.LookDirection.ToVector3(),
                NearPlaneDistance = (float)this.NearPlaneDistance,
                Position = this.Position.ToVector3(),
                UpDirection = this.UpDirection.ToVector3()
            };
        }
    }
}