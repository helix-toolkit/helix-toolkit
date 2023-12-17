namespace HelixToolkit.WinUI.SharpDX;

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
    public CameraSetting(ProjectionCamera camera)
    {
        this.Position = camera.Position;
        this.LookDirection = camera.LookDirection;
        this.UpDirection = camera.UpDirection;
        this.NearPlaneDistance = camera.NearPlaneDistance;
        this.FarPlaneDistance = camera.FarPlaneDistance;
        var pcamera = camera as PerspectiveCamera;
        if (pcamera != null)
        {
            this.FieldOfView = pcamera.FieldOfView;
        }
        else
        {
            this.FieldOfView = 45;
        }

        var ocamera = camera as OrthographicCamera;
        if (ocamera != null)
        {
            this.Width = ocamera.Width;
        }
        else
        {
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
    public Vector3 LookDirection { get; set; }

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
    public Vector3 UpDirection { get; set; }

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
    public void UpdateCamera(ProjectionCamera? camera)
    {
        if (camera is null)
        {
            return;
        }

        camera.Position = this.Position;
        camera.LookDirection = this.LookDirection;
        camera.UpDirection = this.UpDirection;
        camera.NearPlaneDistance = this.NearPlaneDistance;
        camera.FarPlaneDistance = this.FarPlaneDistance;

        if (camera is PerspectiveCamera perspectiveCamera)
        {
            perspectiveCamera.FieldOfView = this.FieldOfView;
        }

        if (camera is OrthographicCamera orthographicCamera)
        {
            orthographicCamera.Width = this.Width;
        }
    }
}
