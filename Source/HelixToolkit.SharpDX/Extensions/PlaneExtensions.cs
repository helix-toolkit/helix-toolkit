using SharpDX;

namespace HelixToolkit.SharpDX;

public static class PlaneExtensions
{
    public static bool PlaneIntersectsPlane(ref Plane p1, ref Plane p2, out Ray intersection)
    {
        var dir = Vector3.Cross(p1.Normal, p2.Normal);
        float det = Vector3.Dot(dir, dir);
        if (Math.Abs(det) > float.Epsilon)
        {
            var p = (Vector3.Cross(dir, p2.Normal) * p1.D + Vector3.Cross(p1.Normal, dir) * p2.D) / det;
            intersection = new Ray(p, dir);
            return true;
        }
        intersection = default;
        return false;
    }
}
