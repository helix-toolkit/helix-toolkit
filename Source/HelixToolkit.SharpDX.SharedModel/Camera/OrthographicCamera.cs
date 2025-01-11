using HelixToolkit.SharpDX.Cameras;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Represents an orthographic projection camera.
/// </summary>
public class OrthographicCamera : ProjectionCamera, IOrthographicCameraModel
{
    /// <summary>
    /// The width property
    /// </summary>
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
        "Width", typeof(double), typeof(OrthographicCamera), new PropertyMetadata(10.0, (d, e) =>
        {
            if (d is Camera { CameraInternal: OrthographicCameraCore core })
            {
                core.Width = (float)(double)e.NewValue;
            }
        }));

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

    private double oldWidth;
    private double targetWidth;
    private double accumTime;
    private double aniTime;

    public OrthographicCamera()
    {
        // default values for near-far must be different for ortho:
        NearPlaneDistance = 0.001;
        FarPlaneDistance = 100.0;
    }

    protected override CameraCore CreatePortableCameraCore()
    {
        return new OrthographicCameraCore();
    }

    protected override void OnCoreCreated(CameraCore core)
    {
        base.OnCoreCreated(core);

        if (core is not OrthographicCameraCore camera)
        {
            return;
        }

        camera.FarPlaneDistance = (float)this.FarPlaneDistance;
        camera.NearPlaneDistance = (float)this.NearPlaneDistance;
        camera.Width = (float)this.Width;
    }

    public void AnimateWidth(double newWidth, double animationTime)
    {
        if (animationTime == 0)
        {
            UpdateCameraPositionByWidth(newWidth);
            Width = newWidth;
        }
        else
        {
            oldWidth = Width;
            this.targetWidth = newWidth;
            accumTime = 1;
            aniTime = animationTime;
            OnUpdateAnimation(0);
        }
    }

    protected override bool OnUpdateAnimation(float ellapsed)
    {
        var res = base.OnUpdateAnimation(ellapsed);
        if (aniTime == 0)
        {
            return res;
        }
        accumTime += ellapsed;
        if (accumTime > aniTime)
        {
            UpdateCameraPositionByWidth(targetWidth);
            Width = targetWidth;
            aniTime = 0;
            return res;
        }
        else
        {
            var newWidth = oldWidth + (targetWidth - oldWidth) * (accumTime / aniTime);
            UpdateCameraPositionByWidth(newWidth);
            Width = newWidth;
            return true;
        }
    }

    private void UpdateCameraPositionByWidth(double newWidth)
    {
        var ratio = newWidth / Width;
        var dir = LookDirection.ToVector3();
        var target = Target.ToVector3();
        var dist = dir.Length();
        var newDist = dist * ratio;
        dir = Vector3.Normalize(dir);
        var position = (target - dir * (float)newDist);
        var lookDir = dir * (float)newDist;
        Position = position.ToPoint3D();
        LookDirection = lookDir.ToVector3D();
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new OrthographicCamera();
    }
#else
#error Unknown framework
#endif
}
