using HelixToolkit.SharpDX.Model;
using SharpDX;
using System.Globalization;

namespace HelixToolkit.SharpDX.Cameras;

public abstract class CameraCore : ObservableObject, ICamera
{
    private Vector3 position;
    public Vector3 Position
    {
        set
        {
            Set(ref position, value);
        }
        get
        {
            return position;
        }
    }

    private Vector3 lookDirection;
    public Vector3 LookDirection
    {
        set
        {
            Set(ref lookDirection, value);
        }
        get
        {
            return lookDirection;
        }
    }

    private Vector3 upDirection;
    public Vector3 UpDirection
    {
        set
        {
            Set(ref upDirection, value);
        }
        get
        {
            return upDirection;
        }
    }

    public Vector3 Target
    {
        get
        {
            return position + lookDirection;
        }
    }

    private bool createLeftHandSystem = false;
    /// <summary>
    /// Gets or sets a value indicating whether to create a left hand system.
    /// </summary>
    /// <value>
    /// <c>true</c> if creating a left hand system; otherwise, <c>false</c>.
    /// </value>
    public bool CreateLeftHandSystem
    {
        set
        {
            Set(ref createLeftHandSystem, value);
        }
        get
        {
            return createLeftHandSystem;
        }
    }

    public abstract Matrix CreateProjectionMatrix(float aspectRatio);

    public abstract Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane);

    public abstract Matrix CreateViewMatrix();

    public abstract FrustumCameraParams CreateCameraParams(float aspectRatio);
    public abstract FrustumCameraParams CreateCameraParams(float aspectRatio, float nearPlane, float farPlane);
    public override string ToString()
    {
        var target = Position + LookDirection;
        return string.Format(
                    CultureInfo.InvariantCulture,
                    "LookDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                    LookDirection.X,
                    LookDirection.Y,
                    LookDirection.Z) + "\n"
                    + string.Format(
                    CultureInfo.InvariantCulture,
                    "UpDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                    UpDirection.X,
                    UpDirection.Y,
                    UpDirection.Z) + "\n"
                    + string.Format(
                    CultureInfo.InvariantCulture,
                    "Position:\t\t{0:0.000},{1:0.000},{2:0.000}",
                    Position.X,
                    Position.Y,
                    Position.Z) + "\n"
                    + string.Format(
                    CultureInfo.InvariantCulture,
                    "Target:\t\t{0:0.000},{1:0.000},{2:0.000}",
                    target.X,
                    target.Y,
                    target.Z);
    }


    private Vector3 targetPosition;
    private Vector3 targetLookDirection;
    private Vector3 targetUpDirection;
    private Vector3 oldPosition;
    private Vector3 oldLookDir;
    private Vector3 oldUpDir;
    private float aniTime = 0;
    private float accumTime = 0;
    private long prevTicks = 0;
    /// <summary>
    /// Animates to.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    /// <param name="newDirection">The new direction.</param>
    /// <param name="newUpDirection">The new up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public void AnimateTo(
        Vector3 newPosition,
        Vector3 newDirection,
        Vector3 newUpDirection,
        float animationTime)
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
            targetPosition = newPosition;
            targetLookDirection = newDirection;
            targetUpDirection = newUpDirection;
            oldPosition = Position;
            oldLookDir = LookDirection;
            oldUpDir = UpDirection;
            aniTime = animationTime;
            accumTime = 1;
            prevTicks = System.Diagnostics.Stopwatch.GetTimestamp();
            OnUpdateAnimation(0);
        }
    }
    /// <summary>
    /// Called when [time step] to update camera animation.
    /// </summary>
    /// <returns></returns>
    public virtual bool OnTimeStep()
    {
        var ticks = System.Diagnostics.Stopwatch.GetTimestamp();
        var ellapsed = (float)(ticks - prevTicks) / System.Diagnostics.Stopwatch.Frequency * 1000;
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
            Position = targetPosition;
            LookDirection = targetLookDirection;
            UpDirection = targetUpDirection;
            aniTime = 0;
            return false;
        }
        else
        {
            var l = accumTime / aniTime;
            var nextPos = Vector3.Lerp(oldPosition, targetPosition, l);
            var nextLook = Vector3.Lerp(oldLookDir, targetLookDirection, l);
            var nextUp = Vector3.Lerp(oldUpDir, targetUpDirection, l);
            Position = nextPos;
            LookDirection = nextLook;
            UpDirection = nextUp;
            return true;
        }
    }

    public void StopAnimation()
    {
        aniTime = 0;
    }
}
