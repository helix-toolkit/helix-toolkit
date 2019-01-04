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
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public static class IViewportExtensions
    {
        public static readonly HitTestResult[] EmptyHits = new HitTestResult[0];

        [ThreadStatic]
        private static readonly Stack<IEnumerator<SceneNode>> stackCache = new Stack<IEnumerator<SceneNode>>();
        /// <summary>
        /// Forces to update transform and bounds.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public static void ForceUpdateTransformsAndBounds(this IViewport3DX viewport)
        {
            viewport.Renderables.ForceUpdateTransformsAndBounds();
        }
        /// <summary>
        /// Finds the hits in camera view frustum only.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="pos">The position.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Finds the hits for a given 2D viewport position.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <returns>
        /// List of hits, sorted with the nearest hit first.
        /// </returns>
        public static IList<HitTestResult> FindHits(this IViewport3DX viewport, Vector2 position)
        {
            var hits = new List<HitTestResult>();
            if (FindHits(viewport, position, ref hits))
            {
                return hits;
            }
            else
            {
                return EmptyHits;
            }
        }

        /// <summary>
        /// Finds the hits.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        public static bool FindHits(this IViewport3DX viewport, Vector2 position, ref List<HitTestResult> hits)
        {
            hits?.Clear();
            if (viewport.CameraCore is ProjectionCameraCore && viewport.RenderHost != null)
            {
                if(!viewport.UnProject(position, out var ray))
                {
                    return false;
                }
                if (hits == null)
                {
                    hits = new List<HitTestResult>();
                }

                foreach (var element in viewport.Renderables)
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

        /// <summary>
        /// Finds the nearest point and its normal.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The find nearest.
        /// </returns>
        public static bool FindNearest(this IViewport3DX viewport, Vector2 position,
            out Vector3 point, out Vector3 normal, out object model)
        {
            point = new Vector3();
            normal = new Vector3();
            model = null;
            if (viewport.CameraCore is ProjectionCameraCore)
            {
                var hits = new List<HitTestResult>();
                if(viewport.FindHitsInFrustum(position, ref hits) && hits.Count > 0)
                {
                    point = hits[0].PointHit;
                    normal = hits[0].NormalAtHit;
                    model = hits[0].ModelHit;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Un-project 2D screen point onto 3D space by camera.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The point2d.</param>
        /// <param name="ray">The ray.</param>
        /// <returns></returns>
        public static bool UnProject(this IViewport3DX viewport, Vector2 point2d, out Ray ray)//, out Vector3 pointNear, out Vector3 pointFar)
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

        /// <summary>
        /// Uns the project 2D point onto a 3D plane.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="p">The p.</param>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="intersection">The intersection.</param>
        /// <returns></returns>
        public static bool UnProjectOnPlane(this IViewport3DX viewport, Vector2 p, Vector3 position, Vector3 normal, out Vector3 intersection)
        {
            if (viewport.UnProject(p, out Ray ray))
            {
                return ray.PlaneIntersection(position, normal, out intersection);
            }
            else
            {
                intersection = Vector3.Zero;
                return false;
            }
        }
        /// <summary>
        /// Uns the project on plane.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="p">The p.</param>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this IViewport3DX viewport, Vector2 p, Vector3 position, Vector3 normal)
        {
            if(viewport.UnProjectOnPlane(p, position, normal, out var intersection))
            {
                return intersection;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the viewport transform aka the screen-space transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewportMatrix(this IViewport3DX viewport)
        {
            return new Matrix(
                viewport.ViewportRectangle.Width / 2f,
                0,
                0,
                0,
                0,
                -viewport.ViewportRectangle.Height / 2f,
                0,
                0,
                0,
                0,
                1,
                0,
                (viewport.ViewportRectangle.Width - 1) / 2f,
                (viewport.ViewportRectangle.Height - 1) / 2f,
                0,
                1);
        }

        /// <summary>
        /// Gets the total transform for a ViewportCore. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetScreenViewProjectionMatrix(this IViewport3DX viewport)
        {
            return GetViewProjectionMatrix(viewport) * GetViewportMatrix(viewport);
        }

        /// <summary>
        /// Projects the specified 3D point to a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Vector2 Project(this IViewport3DX viewport, Vector3 point)
        {
            var matrix = GetScreenViewProjectionMatrix(viewport);
            var pointTransformed = Vector3.Transform(point, matrix);
            var pt = new Vector2((int)pointTransformed.X, (int)pointTransformed.Y);
            return pt;
        }

        /// <summary>
        /// Gets the camera transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>
        /// The camera transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewProjectionMatrix(this IViewport3DX viewport)
        {
            return viewport.RenderHost != null && viewport.RenderHost.RenderContext != null ? 
                viewport.RenderHost.RenderContext.ViewMatrix * viewport.RenderHost.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ViewportRectangle.Width / (float)viewport.ViewportRectangle.Height);
        }
        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this IViewport3DX viewport)
        {
            return viewport.RenderHost != null && viewport.RenderHost.RenderContext != null ?
                viewport.RenderHost.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ViewportRectangle.Width / (float)viewport.ViewportRectangle.Height);
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse(this IViewport3DX viewport, Action<SceneNode> action)
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                action(node);
                return true;
            }, stackCache);
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="function">The function. Return true to continue traverse, otherwise stop at current node</param>
        public static void Traverse(this IViewport3DX viewport, Func<SceneNode, bool> function)
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                return function(node);
            }, stackCache);
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FindBounds(this IViewport3DX viewport)
        {
            if (viewport.RenderHost != null && viewport.RenderHost.IsRendering)
            {
                viewport.RenderHost.UpdateAndRender();
            }
            return FindBoundsInternal(viewport);
        }

        internal static BoundingBox FindBoundsInternal(this IViewport3DX viewport)
        {
            var maxVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var firstModel = viewport.Renderables.PreorderDFT((r) =>
            {
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }).Where(x =>
            {
                if (x is IBoundable b)
                {
                    return b.HasBound && b.BoundsWithTransform.Maximum != b.BoundsWithTransform.Minimum
                    && b.BoundsWithTransform.Maximum != Vector3.Zero && b.BoundsWithTransform.Maximum != maxVector;
                }
                else
                {
                    return false;
                }
            }).FirstOrDefault();
            if (firstModel == null)
            {
                return new BoundingBox();
            }
            var bounds = firstModel.BoundsWithTransform;

            foreach (var renderable in viewport.Renderables.PreorderDFT((r) =>
            {
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }))
            {
                if (renderable is IBoundable r)
                {
                    if (r.HasBound && r.BoundsWithTransform.Maximum != maxVector)
                    {
                        bounds = global::SharpDX.BoundingBox.Merge(bounds, r.BoundsWithTransform);
                    }
                }
            }
            return bounds;
        }

        /// <summary>
        /// Renders to bitmap stream.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public static System.IO.MemoryStream RenderToBitmapStream(this IViewport3DX view)
        {
            if (view.RenderHost != null && view.RenderHost.IsRendering)
            {
                view.RenderHost.UpdateAndRender();              
                if (view.RenderHost != null && view.RenderHost.IsRendering)
                {
                    var memoryStream = new System.IO.MemoryStream();
                    Utilities.ScreenCapture.SaveWICTextureToBitmapStream(view.RenderHost.EffectsManager, 
                        view.RenderHost.RenderBuffer.BackBuffer.Resource as global::SharpDX.Direct3D11.Texture2D, memoryStream);
                    return memoryStream;
                }
            }
            return null;
        }
    }
}
