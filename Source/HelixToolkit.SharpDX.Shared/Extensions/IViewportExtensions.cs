using System;
using System.Collections.Generic;
using System.Text;
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
    /// <summary>
    /// 
    /// </summary>
    public static class IViewportExtensions
    {
        public static readonly HitTestResult[] EmptyHits = new HitTestResult[0];
        /// <summary>
        /// Forces to update transform and bounds.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public static void ForceUpdateTransformsAndBounds(this IViewport3DX viewport)
        {
            viewport.Renderables.ForceUpdateTransformsAndBounds();
        }

        public static bool FindHitsInFrustum(this IViewport3DX viewport, Vector2 pos, ref List<HitTestResult> hits)
        {
            if (viewport.RenderHost == null || !viewport.RenderHost.IsRendering)
            {
                return false;
            }
            hits?.Clear();
            if(UnProject(viewport, pos, out var ray))
            {
                foreach (var element in viewport.RenderHost.PerFrameOpaqueNodesInFrustum)
                {
                    element.HitTest(viewport.RenderHost.RenderContext, ray, ref hits);
                }
                foreach (var element in viewport.RenderHost.PerFrameTransparentNodesInFrustum)
                {
                    element.HitTest(viewport.RenderHost.RenderContext, ray, ref hits);
                }
                hits.Sort();
                return hits.Count > 0;
            }
            else
            {
                return false;
            }
        }

        internal static bool UnProject(this IViewport3DX viewport, Vector2 point2d, out Ray ray)//, out Vector3 pointNear, out Vector3 pointFar)
        {
            if (viewport.RenderHost != null && viewport.CameraCore is ProjectionCameraCore camera)
            {
                var px = point2d.X;
                var py = point2d.Y;

                var viewMatrix = camera.CreateViewMatrix();               

                var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
                float w = viewport.RenderHost.ActualWidth;
                float h = viewport.RenderHost.ActualHeight;
                var aspectRatio = w / h;

                var projMatrix = camera.CreateProjectionMatrix(aspectRatio);
                
                Vector3 v = new Vector3
                {
                    X = (2 * px / w - 1) / projMatrix.M11,
                    Y = -(2 * py / h - 1) / projMatrix.M22,
                    Z = 1 / projMatrix.M33
                };
                Vector3.TransformCoordinate(ref v, ref matrix, out Vector3 zf);
                Vector3 zn;
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

                ray = new Ray(zn + r * camera.NearPlaneDistance, r);
                return true;
            }
            else
            {
                ray = new Ray();
                return false;
            }
        }
    }
}
