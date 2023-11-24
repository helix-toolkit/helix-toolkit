using HelixToolkit.SharpDX.Cameras;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Represents a perspective projection camera.
/// </summary>
public class PerspectiveCamera : ProjectionCamera, IPerspectiveCameraModel
{
    /// <summary>
    /// The field of view property
    /// </summary>
    public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
        "FieldOfView", typeof(double), typeof(PerspectiveCamera), new PropertyMetadata(45.0,
            (d, e) =>
            {
                if (d is Camera { CameraInternal: PerspectiveCameraCore core })
                {
                    core.FieldOfView = (float)(double)e.NewValue;
                }
            }));

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

    protected override CameraCore CreatePortableCameraCore()
    {
        return new PerspectiveCameraCore();
    }

    protected override void OnCoreCreated(CameraCore core)
    {
        base.OnCoreCreated(core);

        if (core is not PerspectiveCameraCore camera)
        {
            return;
        }

        camera.FarPlaneDistance = (float)this.FarPlaneDistance;
        camera.FieldOfView = (float)this.FieldOfView;
        camera.NearPlaneDistance = (float)this.NearPlaneDistance;
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new PerspectiveCamera();
    }
#endif
}
