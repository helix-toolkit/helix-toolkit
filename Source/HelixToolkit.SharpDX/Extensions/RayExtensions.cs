using SharpDX;

namespace HelixToolkit.SharpDX;

public static class RayExtensions
{
    public static Ray UnProject(this Vector2 point2d, ref Matrix view, ref Matrix projection, float nearPlane, float w, float h, bool isPerpective)
    {
        var px = point2d.X;
        var py = point2d.Y;

        var matrix = MatrixHelper.PsudoInvert(ref view);

        var v = new Vector3
        {
            X = (2 * px / w - 1) / projection.M11,
            Y = -(2 * py / h - 1) / projection.M22,
            Z = 1 / projection.M33
        };
        Vector3Helper.TransformCoordinate(ref v, ref matrix, out var zf);
        Vector3 zn;
        if (isPerpective)
        {
            zn = new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }
        else
        {
            v.Z = 0;
            Vector3Helper.TransformCoordinate(ref v, ref matrix, out zn);
        }
        var r = zf - zn;
        r = Vector3.Normalize(r);

        return new Ray(zn + r * nearPlane, r);
    }
}
