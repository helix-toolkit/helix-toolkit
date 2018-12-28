using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
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
    }
}
