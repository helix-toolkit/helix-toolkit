using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf
{
    internal class NearestPointInCamera
    {
        public NearestPointInCamera(Point mouseDownNearestPoint2D, Point3D? mouseDownNearestPoint3D)
        {
            MouseDownNearestPoint2D = mouseDownNearestPoint2D;
            MouseDownNearestPoint3D = mouseDownNearestPoint3D;
        }

        /// <summary>
        /// The 3D point that most closely matches the ray from the camera cast at the current mouse position.
        /// </summary>
        public Point3D? MouseDownNearestPoint3D { get; set; }

        /// <summary>
        /// The 2D coordinate corresponding to said 3D point. This will usually coïncide with the mouse position, but may differ if there is no geometry directly under the mouse postion.
        /// </summary>
        public Point MouseDownNearestPoint2D { get; set; }
    }
}