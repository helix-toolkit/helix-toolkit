using SharpDX;
using System.Globalization;

namespace HelixToolkit.SharpDX.Cameras;

public abstract class ProjectionCameraCore : CameraCore
{
    private float farPlane = 100;
    /// <summary>
    /// Gets or sets the far plane distance.
    /// </summary>
    /// <value>
    /// The far plane distance.
    /// </value>
    public float FarPlaneDistance
    {
        set
        {
            Set(ref farPlane, value);
        }
        get
        {
            return farPlane;
        }
    }

    private float nearPlane = 0.001f;
    /// <summary>
    /// Gets or sets the near plane distance.
    /// </summary>
    /// <value>
    /// The near plane distance.
    /// </value>
    public float NearPlaneDistance
    {
        set
        {
            Set(ref nearPlane, value);
        }
        get
        {
            return nearPlane;
        }
    }

    public override Matrix CreateViewMatrix()
    {
        return CreateLeftHandSystem ? MatrixHelper.LookAtLH(this.Position, this.Position + this.LookDirection, this.UpDirection)
            : MatrixHelper.LookAtRH(this.Position, this.Position + this.LookDirection, this.UpDirection);
    }

    public override string ToString()
    {
        return base.ToString() + "\n" +
            string.Format(
                    CultureInfo.InvariantCulture, "NearPlaneDist:\t{0}", NearPlaneDistance) + "\n"
                    + string.Format(CultureInfo.InvariantCulture, "FarPlaneDist:\t{0}", FarPlaneDistance);
    }
}
