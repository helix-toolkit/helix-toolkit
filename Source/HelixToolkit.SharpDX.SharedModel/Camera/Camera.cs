using HelixToolkit.SharpDX.Cameras;
using SharpDX;
using System.Diagnostics;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
/// </summary>
public abstract class Camera : Animatable, ICameraModel
{
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public abstract Point3D Position
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets the look direction.
    /// </summary>
    /// <value>
    /// The look direction.
    /// </value>
    public abstract Vector3D LookDirection
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets up direction.
    /// </summary>
    /// <value>
    /// Up direction.
    /// </value>
    public abstract Vector3D UpDirection
    {
        get; set;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [create left hand system].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [create left hand system]; otherwise, <c>false</c>.
    /// </value>
    public abstract bool CreateLeftHandSystem
    {
        set; get;
    }

    private CameraCore? core;
    /// <summary>
    /// Gets the camera internal.
    /// </summary>
    /// <value>
    /// The camera internal.
    /// </value>
    public CameraCore CameraInternal
    {
        get
        {
            if (core == null)
            {
                core = CreatePortableCameraCore();
                OnCoreCreated(core);
            }
            return core;
        }
    }

    /// <summary>
    /// Creates the view matrix.
    /// </summary>
    /// <returns>A <see cref="Matrix" />.</returns>
    public Matrix CreateViewMatrix()
    {
        return CameraInternal.CreateViewMatrix();
    }

    /// <summary>
    /// Creates the projection matrix.
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio.</param>
    /// <returns>A <see cref="Matrix" />.</returns>
    public Matrix CreateProjectionMatrix(double aspectRatio)
    {
        return CameraInternal.CreateProjectionMatrix((float)aspectRatio);
    }

    private Vector3 targetPosition;
    private Vector3 targetLookDirection;
    private Vector3 targetUpDirection;
    private Vector3 oldPosition;
    private Vector3 oldLookDir;
    private Vector3 oldUpDir;
    private double aniTime = 0;
    private double accumTime = 0;
    private long prevTicks = 0;
    /// <summary>
    /// Creates the portable camera core.
    /// </summary>
    /// <returns></returns>
    protected abstract CameraCore CreatePortableCameraCore();
    /// <summary>
    /// Called when [core created].
    /// </summary>
    protected virtual void OnCoreCreated(CameraCore core)
    {
        core.LookDirection = this.LookDirection.ToVector3();
        core.Position = this.Position.ToVector3();
        core.UpDirection = this.UpDirection.ToVector3();
        core.CreateLeftHandSystem = this.CreateLeftHandSystem;
    }
    /// <summary>
    /// Animates to.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    /// <param name="newDirection">The new direction.</param>
    /// <param name="newUpDirection">The new up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public void AnimateTo(
        Point3D newPosition,
        Vector3D newDirection,
        Vector3D newUpDirection,
        double animationTime)
    {
        if (animationTime == 0)
        {
            Position = newPosition;
            LookDirection = newDirection;
            UpDirection = newUpDirection;
            aniTime = 0;
        }
        else
        {
            targetPosition = newPosition.ToVector3();
            targetLookDirection = newDirection.ToVector3();
            targetUpDirection = newUpDirection.ToVector3();
            oldPosition = CameraInternal.Position;
            oldLookDir = CameraInternal.LookDirection;
            oldUpDir = CameraInternal.UpDirection;
            aniTime = animationTime;
            accumTime = 1;
            prevTicks = Stopwatch.GetTimestamp();
            OnUpdateAnimation(0);
        }
    }
    /// <summary>
    /// Called when [time step] to update camera animation.
    /// </summary>
    /// <returns></returns>
    public virtual bool OnTimeStep()
    {
        var ticks = Stopwatch.GetTimestamp();
        var ellapsed = (float)(ticks - prevTicks) / Stopwatch.Frequency * 1000;
        prevTicks = ticks;
        return OnUpdateAnimation(ellapsed);
    }

    protected virtual bool OnUpdateAnimation(float ellapsed)
    {
        if (aniTime == 0)
        {
            return false;
        }
        accumTime += ellapsed;
        if (accumTime > aniTime)
        {
            Position = targetPosition.ToPoint3D();
            LookDirection = targetLookDirection.ToVector3D();
            UpDirection = targetUpDirection.ToVector3D();
            aniTime = 0;
            return false;
        }
        else
        {
            var l = (float)(accumTime / aniTime);
            var nextPos = Vector3.Lerp(oldPosition, targetPosition, l);
            var nextLook = Vector3.Lerp(oldLookDir, targetLookDirection, l);
            var nextUp = Vector3.Lerp(oldUpDir, targetUpDirection, l);
            Position = nextPos.ToPoint3D();
            LookDirection = nextLook.ToVector3D();
            UpDirection = nextUp.ToVector3D();
            return true;
        }
    }

    public void StopAnimation()
    {
        aniTime = 0;
    }

    public static implicit operator CameraCore?(Camera? camera)
    {
        return camera?.CameraInternal;
    }
}
