using SharpDX;
using System.Globalization;

namespace HelixToolkit.SharpDX.Cameras;

public class OrthographicCameraCore : ProjectionCameraCore
{
    private float width = 100;
    public float Width
    {
        set
        {
            Set(ref width, value);
        }
        get
        {
            return width;
        }
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
            FOV = (float)Math.PI / 2,
            LookAtDir = LookDirection,
            UpDir = UpDirection,
            Position = Position,
            ZNear = nearPlane,
            ZFar = farPlane
        };
    }

    public override Matrix CreateProjectionMatrix(float aspectRatio)
    {
        return CreateProjectionMatrix(aspectRatio, NearPlaneDistance, FarPlaneDistance);
    }

    public override Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane)
    {
        return this.CreateLeftHandSystem ?
            Matrix.OrthoLH(this.Width, (float)(this.Width / aspectRatio), nearPlane, Math.Min(1e15f, farPlane))
            : Matrix.OrthoRH(this.Width, (float)(this.Width / aspectRatio), nearPlane, Math.Min(1e15f, farPlane));
    }


    public override string ToString()
    {
        return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", Width);
    }

    private float oldWidth;
    private float targetWidth;
    private float accumTime;
    private float aniTime;

    public void AnimateWidth(float newWidth, float animationTime)
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
        bool res = base.OnUpdateAnimation(ellapsed);
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
        var dir = LookDirection;
        var target = Target;
        var dist = dir.Length();
        var newDist = dist * ratio;
        dir.Normalize();
        var position = (target - dir * (float)newDist);
        var lookDir = dir * (float)newDist;
        Position = position;
        LookDirection = lookDir;
    }
}
