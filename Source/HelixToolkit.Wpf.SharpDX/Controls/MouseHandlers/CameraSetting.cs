// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraSetting.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents camera settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    /// <summary>
    /// Represents camera settings.
    /// </summary>
    public struct CameraSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraSetting"/> class.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public CameraSetting(Camera camera)
        {
            this.Position = camera.Position;
            this.LookDirection = camera.LookDirection;
            this.UpDirection = camera.UpDirection;
            if(camera is IProjectionCameraModel c)
            {
                this.NearPlaneDistance = c.NearPlaneDistance;
                this.FarPlaneDistance = c.FarPlaneDistance;
                this.FieldOfView = 45;
                this.Width = 100;
                if (camera is IPerspectiveCameraModel pcamera)
                {
                    this.FieldOfView = pcamera.FieldOfView;
                }
                else if (camera is IOrthographicCameraModel ocamera)
                {
                    this.Width = ocamera.Width;
                }
            }
            else
            {
                this.NearPlaneDistance = 0;
                this.FarPlaneDistance = 0;
                this.FieldOfView = 45;
                this.Width = 100;
            }
        }

        /// <summary>
        /// Gets or sets FarPlaneDistance.
        /// </summary>
        public double FarPlaneDistance { get; set; }

        /// <summary>
        /// Gets or sets FieldOfView.
        /// </summary>
        public double FieldOfView { get; set; }

        /// <summary>
        /// Gets or sets LookDirection.
        /// </summary>
        public Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets NearPlaneDistance.
        /// </summary>
        public double NearPlaneDistance { get; set; }

        /// <summary>
        /// Gets or sets Position.
        /// </summary>
        public Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets UpDirection.
        /// </summary>
        public Vector3D UpDirection { get; set; }

        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Sets the properties of the specified camera to the settings stored in this object.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public void UpdateCamera(Camera camera)
        {
            camera.Position = this.Position;
            camera.LookDirection = this.LookDirection;
            camera.UpDirection = this.UpDirection;
            if(camera is IProjectionCameraModel c)
            {
                c.NearPlaneDistance = this.NearPlaneDistance;
                c.FarPlaneDistance = this.FarPlaneDistance;
                if (camera is IPerspectiveCameraModel perspectiveCamera)
                {
                    perspectiveCamera.FieldOfView = this.FieldOfView;
                }
                else if (camera is IOrthographicCameraModel orthographicCamera)
                {
                    orthographicCamera.Width = this.Width;
                }
            }
        }
    }
}