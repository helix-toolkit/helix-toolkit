using SharpDX;
using System;

namespace HelixToolkit.UWP
{
    using Cameras;

    public static class ViewportExtensions
    {        
        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Vector2 point2d)//, out Vector3 pointNear, out Vector3 pointFar)
        {
            var camera = viewport.CameraCore as ProjectionCameraCore;
            if (camera != null)
            {
                var px = point2d.X;
                var py = point2d.Y;

                var viewMatrix = camera.CreateViewMatrix();
                Vector3 v = new Vector3();

                var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
                float w = (float)viewport.ActualWidth;
                float h = (float)viewport.ActualHeight;
                var aspectRatio = w / h;

                var projMatrix = camera.CreateProjectionMatrix(aspectRatio);
                Vector3 zn, zf;
                v.X = (2 * px / w - 1) / projMatrix.M11;
                v.Y = -(2 * py / h - 1) / projMatrix.M22;
                v.Z = 1 / projMatrix.M33;
                Vector3.TransformCoordinate(ref v, ref matrix, out zf);

                if (camera is PerspectiveCameraCore)
                {
                    zn = camera.Position;
                }
                else
                {
                    v.Z = 0;
                    Vector3.TransformCoordinate(ref v, ref matrix, out zn);
                }
                Vector3 r = zf - zn;
                r.Normalize();

                return new Ray(zn + r * camera.NearPlaneDistance, r);
            }
            throw new HelixToolkitException("Unproject camera error.");
        }
    }
}
