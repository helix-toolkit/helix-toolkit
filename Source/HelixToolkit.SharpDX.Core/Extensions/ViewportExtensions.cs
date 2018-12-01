using System;
using System.Collections.Generic;
using SharpDX;
using System.Runtime.CompilerServices;
using System.Linq;

namespace HelixToolkit.SharpDX.Core
{
    using Controls;
    using Model.Scene;
    using Cameras;

    public static class ViewportExtensions
    {
        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this ViewportCore viewport, Vector2 point2d)
        {
            var camera = viewport.CameraCore as ProjectionCameraCore;
            if (camera != null)
            {
                var px = (float)point2d.X;
                var py = (float)point2d.Y;

                var viewMatrix = camera.CreateViewMatrix();
                Vector3 v = new Vector3();

                var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
                float w = (float)viewport.ViewportRectangle.Width;
                float h = (float)viewport.ViewportRectangle.Height;
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

        /// <summary>
        /// Un-project a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <param name="position">
        /// plane position
        /// </param>
        /// <param name="normal">
        /// plane normal
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public static Vector3? UnProjectOnPlane(this ViewportCore viewport, Vector2 p, Vector3 position, Vector3 normal)
        {
            var ray = UnProject(viewport, p);
            if (ray == null)
            {
                return null;
            }
            return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="ray"></param>
        /// <returns>The intersection point.</returns>
        public static Vector3? PlaneIntersection(this Ray ray, Vector3 position, Vector3 normal)
        {
            // http://paulbourke.net/geometry/planeline/
            var dn = Vector3.Dot(normal, ray.Direction);
            if (dn == 0)
            {
                return null;
            }

            var u = Vector3.Dot(normal, position - ray.Position) / dn;
            return ray.Position + u * ray.Direction;
        }

        /// <summary>
        /// Projects the specified 3D point to a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Vector2 Project(this ViewportCore viewport, Vector3 point)
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
        public static Matrix GetViewProjectionMatrix(this ViewportCore viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ViewMatrix * viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ViewportRectangle.Width / (float)viewport.ViewportRectangle.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this ViewportCore viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ViewportRectangle.Width / (float)viewport.ViewportRectangle.Height);
        }

        /// <summary>
        /// Gets the total transform for a ViewportCore. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetScreenViewProjectionMatrix(this ViewportCore viewport)
        {
            return GetViewProjectionMatrix(viewport) * GetViewportMatrix(viewport);
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
        public static Matrix GetViewportMatrix(this ViewportCore viewport)
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

        public static readonly HitTestResult[] EmptyHits = new HitTestResult[0];
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
        public static IList<HitTestResult> FindHits(this ViewportCore viewport, Vector2 position)
        {
            if (viewport.CameraCore is ProjectionCameraCore)
            {
                var ray = UnProject(viewport, position);
                var hits = new List<HitTestResult>();

                foreach (var element in viewport.Renderables)
                {
                    element.HitTest(viewport.RenderContext, ray, ref hits);
                }
                hits.Sort();

                return hits;               
            }
            else
            {
                return EmptyHits;
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
        public static bool FindNearest(this ViewportCore viewport, Vector2 position,
            out Vector3 point, out Vector3 normal, out object model)
        {
            point = new Vector3();
            normal = new Vector3();
            model = null;
            if(viewport.CameraCore is ProjectionCameraCore)
            {
                var hits = FindHits(viewport, position);
                if (hits.Count > 0)
                {
                    point = hits[0].PointHit;
                    normal = hits[0].NormalAtHit;
                    model = hits[0].ModelHit;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="delta">The delta.</param>
        public static void ZoomByChangingFieldOfView(this CameraController controller, float delta)
        {
            var viewport = controller.Viewport;
            if(viewport.CameraCore is PerspectiveCameraCore pcamera)
            {
                float fov = pcamera.FieldOfView;
                float d = pcamera.LookDirection.Length();
                float r = d * (float)Math.Tan(0.5f * fov / 180 * Math.PI);

                fov *= 1f + (delta * 0.5f);
                if (fov < controller.MinimumFieldOfView)
                {
                    fov = controller.MinimumFieldOfView;
                }

                if (fov > controller.MaximumFieldOfView)
                {
                    fov = controller.MaximumFieldOfView;
                }

                pcamera.FieldOfView = fov;
                float d2 = r / (float)Math.Tan(0.5f * fov / 180 * Math.PI);
                var newLookDirection = pcamera.LookDirection;
                newLookDirection.Normalize();
                newLookDirection *= (float)d2;
                var target = pcamera.Position + pcamera.LookDirection;
                pcamera.Position = target - newLookDirection;
                pcamera.LookDirection = newLookDirection;
            }
        }

        /// <summary>
        /// Zooms the viewport to the specified rectangle.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void ZoomToRectangle(this ViewportCore viewport, RectangleF rectangle)
        {
            viewport.CameraCore.ZoomToRectangle(viewport, rectangle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point2d"></param>
        /// <returns></returns>
        public static Ray UnProjectToRay(this ViewportCore viewport, Vector2 point2d)
        {
            var r = viewport.UnProject(point2d);
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FindBounds(this ViewportCore viewport)
        {
            if (viewport.RenderHost != null && viewport.RenderHost.IsRendering)
            {
                viewport.RenderHost.UpdateAndRender();
            }
            return FindBoundsInternal(viewport);
        }

        internal static BoundingBox FindBoundsInternal(this ViewportCore viewport)
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
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse(this ViewportCore viewport, Action<SceneNode> action)
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                action(node);
                return true;
            });
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="function">The function. Return true to continue traverse, otherwise stop at current node</param>
        public static void Traverse(this ViewportCore viewport, Func<SceneNode, bool> function) 
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                 return function(node);
            });
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse(this SceneNode element, Action<SceneNode> action)
        {
            var sceneNode = new SceneNode[] { element };
            Traverse(element, action);
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="function">
        /// The action.
        /// </param>
        public static void Traverse(this SceneNode element, Func<SceneNode, bool> function)
        {
            var sceneNode = new SceneNode[] { element };
            Traverse(element, function);
        }
    }
}
