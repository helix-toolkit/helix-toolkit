using SharpDX;

namespace HelixToolkit.SharpDX;

public static class IRenderMetricesExtensions
{
    /// <summary>
    /// Un-project 2D screen point onto 3D space by camera.
    /// </summary>
    /// <param name="renderMatrices">The renderMatrices.</param>
    /// <param name="point2d">The point2d.</param>
    /// <param name="ray">The ray.</param>
    /// <returns></returns>
    public static bool UnProject(this IRenderMatrices renderMatrices, Vector2 point2d, out Ray ray)//, out Vector3 pointNear, out Vector3 pointFar)
    {
        if (renderMatrices == null)
        {
            ray = new Ray();
            return false;
        }

        renderMatrices.Update();
        var px = point2d.X;
        var py = point2d.Y;

        var viewInv = renderMatrices.ViewMatrixInv;
        var projMatrix = renderMatrices.ProjectionMatrix;

        var w = renderMatrices.ActualWidth / renderMatrices.DpiScale;
        var h = renderMatrices.ActualHeight / renderMatrices.DpiScale;

        var v = new Vector3
        {
            X = (2 * px / w - 1) / projMatrix.M11,
            Y = -(2 * py / h - 1) / projMatrix.M22,
            Z = 1 / projMatrix.M33
        };
        Vector3Helper.TransformCoordinate(ref v, ref viewInv, out var zf);
        Vector3 zn;
        if (renderMatrices.IsPerspective)
        {
            zn = viewInv.Row4().ToHomogeneousVector3();
        }
        else
        {
            v.Z = 0;
            Vector3Helper.TransformCoordinate(ref v, ref viewInv, out zn);
        }
        var r = zf - zn;
        r = Vector3.Normalize(r);
        ray = new Ray(zn + r * renderMatrices.CameraParams.ZNear, r);
        return true;
    }

    public static Vector2 Project(this IRenderMatrices renderMatrices, Vector3 point)
    {
        renderMatrices.Update();
        var matrix = renderMatrices.ScreenViewProjectionMatrix;
        var pointTransformed = Vector3Helper.TransformCoordinate(point, matrix);
        var pt = new Vector2(pointTransformed.X, pointTransformed.Y) / renderMatrices.DpiScale;
        return pt;
    }
}
