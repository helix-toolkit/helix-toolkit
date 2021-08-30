using SharpDX;
#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    public static class RayExtensions
    {
        /// <summary>
        /// Planes the intersection with a ray.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <param name="position">The plane position.</param>
        /// <param name="normal">The plane normal.</param>
        /// <param name="intersection">The intersection point.</param>
        /// <returns>Return true if intersect, return false if ray is not intersect with the plane</returns>
        public static bool PlaneIntersection(this Ray ray, Vector3 position, Vector3 normal, out Vector3 intersection)
        {
            // http://paulbourke.net/geometry/planeline/
            var dn = Vector3.Dot(normal, ray.Direction);
            if (dn == 0)
            {
                intersection = new Vector3();
                return false;
            }

            var u = Vector3.Dot(normal, position - ray.Position) / dn;
            intersection = ray.Position + u * ray.Direction;
            return true;
        }

        public static Ray UnProject(this Vector2 point2d, ref Matrix view, ref Matrix projection, float nearPlane, float w, float h, bool isPerpective)
        {
            var px = point2d.X;
            var py = point2d.Y;

            var matrix = MatrixExtensions.PsudoInvert(ref view);

            Vector3 v = new Vector3
            {
                X = (2 * px / w - 1) / projection.M11,
                Y = -(2 * py / h - 1) / projection.M22,
                Z = 1 / projection.M33
            };
            Vector3.TransformCoordinate(ref v, ref matrix, out Vector3 zf);
            Vector3 zn;
            if (isPerpective)
            {
                zn = new Vector3(matrix.M41, matrix.M42, matrix.M43);
            }
            else
            {
                v.Z = 0;
                Vector3.TransformCoordinate(ref v, ref matrix, out zn);
            }
            Vector3 r = zf - zn;
            r.Normalize();

            return new Ray(zn + r * nearPlane, r);
        }
    }
}
