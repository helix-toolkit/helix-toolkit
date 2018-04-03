/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using Cameras;
    using Windows.UI.Xaml;

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
                (d, e) =>
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

        protected override CameraCore CreatePortableCameraCore()
        {
            return new PerspectiveCameraCore()
            {
                CreateLeftHandSystem = this.CreateLeftHandSystem,
                FarPlaneDistance = (float)this.FarPlaneDistance,
                FieldOfView = (float)this.FieldOfView,
                LookDirection = this.LookDirection,
                NearPlaneDistance = (float)this.NearPlaneDistance,
                Position = this.Position,
                UpDirection = this.UpDirection
            };
        }
    }
}
