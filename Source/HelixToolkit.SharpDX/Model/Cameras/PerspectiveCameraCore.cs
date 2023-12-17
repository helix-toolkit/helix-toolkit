using SharpDX;
using System.Globalization;

namespace HelixToolkit.SharpDX.Cameras;

public class PerspectiveCameraCore : ProjectionCameraCore
{
    public float FieldOfView
    {
        set; get;
    } = 45;

    public override Matrix CreateProjectionMatrix(float aspectRatio)
    {
        return CreateProjectionMatrix(aspectRatio, NearPlaneDistance, FarPlaneDistance);
    }

    public override Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane)
    {
        var fov = this.FieldOfView * Math.PI / 180;
        Matrix projM;
        if (this.CreateLeftHandSystem)
        {
            projM = MatrixHelper.PerspectiveFovLH((float)fov, aspectRatio, nearPlane, farPlane);
        }
        else
        {
            projM = MatrixHelper.PerspectiveFovRH((float)fov, (float)aspectRatio, nearPlane, farPlane);
        }
        if (float.IsNaN(projM.M33) || float.IsNaN(projM.M43))
        {
            projM.M33 = projM.M43 = -1;
        }
        return projM;
    }

    public override FrustumCameraParams CreateCameraParams(float aspectRatio)
    {
        return CreateCameraParams(aspectRatio, NearPlaneDistance, FarPlaneDistance);
    }
    public override FrustumCameraParams CreateCameraParams(float aspectRatio, float nearPlane, float farPlane)
    {
        return new FrustumCameraParams()
        {
            AspectRatio = aspectRatio,
            FOV = FieldOfView / 180f * (float)(Math.PI),
            LookAtDir = LookDirection,
            UpDir = UpDirection,
            Position = Position,
            ZNear = nearPlane,
            ZFar = farPlane
        };
    }
    public override string ToString()
    {
        return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", FieldOfView);
    }
}
