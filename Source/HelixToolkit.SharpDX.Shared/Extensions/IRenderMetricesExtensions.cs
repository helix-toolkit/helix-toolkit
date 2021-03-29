using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    using Cameras;
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
            renderMatrices.Update();
            var px = point2d.X;
            var py = point2d.Y;
            
            var viewInv = renderMatrices.ViewMatrixInv;
            var projMatrix = renderMatrices.ProjectionMatrix;

            float w = renderMatrices.ActualWidth / renderMatrices.DpiScale;
            float h = renderMatrices.ActualHeight / renderMatrices.DpiScale;

            Vector3 v = new Vector3
            {
                X = (2 * px / w - 1) / projMatrix.M11,
                Y = -(2 * py / h - 1) / projMatrix.M22,
                Z = 1 / projMatrix.M33
            };
            Vector3.TransformCoordinate(ref v, ref viewInv, out Vector3 zf);
            Vector3 zn;
            if (renderMatrices.IsPerspective)
            {
                zn = viewInv.Row4.ToVector3();
            }
            else
            {
                v.Z = 0;
                Vector3.TransformCoordinate(ref v, ref viewInv, out zn);
            }
            Vector3 r = zf - zn;
            r.Normalize();
            ray = new Ray(zn + r * renderMatrices.CameraParams.ZNear, r);
            return true;
        }

        public static Vector2 Project(this IRenderMatrices renderMatrices, Vector3 point)
        {
            renderMatrices.Update();
            var matrix = renderMatrices.ScreenViewProjectionMatrix;
            var pointTransformed = Vector3.TransformCoordinate(point, matrix);
            var pt = new Vector2(pointTransformed.X, pointTransformed.Y) / renderMatrices.DpiScale;
            return pt;
        }
    }
}
