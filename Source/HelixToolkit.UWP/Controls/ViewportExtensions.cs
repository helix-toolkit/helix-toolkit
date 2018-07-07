using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Matrix = System.Numerics.Matrix4x4;
using HelixToolkit.Mathematics;

namespace HelixToolkit.UWP
{
    using Cameras;
    using HelixToolkit.UWP.Model.Scene;
    using Windows.Foundation;

    public static class ViewportExtensions
    {
        public static Ray UnProject(this Viewport3DX viewport, Point point2d)
        {
            return UnProject(viewport, point2d.ToVector2());
        }
        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Vector2 point2d)
        {
            var camera = viewport.CameraCore as ProjectionCameraCore;
            if (camera != null)
            {
                var px = (float)point2d.X;
                var py = (float)point2d.Y;

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
                Vector3Helper.TransformCoordinate(ref v, ref matrix, out zf);

                if (camera is PerspectiveCameraCore)
                {
                    zn = camera.Position;
                }
                else
                {
                    v.Z = 0;
                    Vector3Helper.TransformCoordinate(ref v, ref matrix, out zn);
                }
                Vector3 r = Vector3.Normalize(zf - zn);

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
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Point p, Vector3 position, Vector3 normal)
        {
            var ray = UnProject(viewport, p);
            if (ray == null)
            {
                return null;
            }
            return ray.PlaneIntersection(position, normal);
        }
        /// <summary>
        /// Uns the project on plane.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="p">The p.</param>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Vector2 p, Vector3 position, Vector3 normal)
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
        public static Point Project(this Viewport3DX viewport, Vector3 point)
        {
            var matrix = GetScreenViewProjectionMatrix(viewport);
            var pointTransformed = Vector3.Transform(point, matrix);
            var pt = new Point((int)pointTransformed.X, (int)pointTransformed.Y);
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
        public static Matrix GetViewProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ViewMatrix * viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ActualWidth / (float)viewport.ActualHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.CreateProjectionMatrix((float)viewport.ActualWidth / (float)viewport.ActualHeight);
        }        
        
        /// <summary>
        /// Gets the total transform for a Viewport3DX. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetScreenViewProjectionMatrix(this Viewport3DX viewport)
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
        public static Matrix GetViewportMatrix(this Viewport3DX viewport)
        {
            return new Matrix(
                (float)(viewport.ActualWidth / 2),
                0,
                0,
                0,
                0,
                (float)(-viewport.ActualHeight / 2),
                0,
                0,
                0,
                0,
                1,
                0,
                (float)((viewport.ActualWidth - 1) / 2),
                (float)((viewport.ActualHeight - 1) / 2),
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
        public static IList<HitTestResult> FindHits(this Viewport3DX viewport, Point position)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return EmptyHits;
            }

            var ray = UnProject(viewport, position);
            var hits = new List<HitTestResult>();

            foreach (var element in viewport.Renderables)
            {
                element.HitTest(viewport.RenderContext, ray, ref hits);
            }
            hits.Sort();

            return hits;
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
        public static bool FindNearest(this Viewport3DX viewport, Point position,
            out Vector3 point, out Vector3 normal, out Element3D model)
        {
            point = new Vector3();
            normal = new Vector3();
            model = null;

            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return false;
            }

            var hits = FindHits(viewport, position);
            if (hits.Count > 0)
            {
                point = hits[0].PointHit;
                normal = hits[0].NormalAtHit;
                model = hits[0].ModelHit as Element3D;
                return true;
            }
            else
            {
                // check for nearest points in the scene
                // TODO!!
                return false;
            }
        }

        /// <summary>
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="delta">
        /// The relative change in fov.
        /// </param>
        public static void ZoomByChangingFieldOfView(this Viewport3DX viewport, double delta)
        {
            var pcamera = viewport.Camera as PerspectiveCamera;
            if (pcamera == null || !viewport.IsChangeFieldOfViewEnabled)
            {
                return;
            }

            double fov = pcamera.FieldOfView;
            double d = pcamera.LookDirection.Length();
            double r = d * Math.Tan(0.5 * fov / 180 * Math.PI);

            fov *= 1 + (delta * 0.5);
            if (fov < viewport.MinimumFieldOfView)
            {
                fov = viewport.MinimumFieldOfView;
            }

            if (fov > viewport.MaximumFieldOfView)
            {
                fov = viewport.MaximumFieldOfView;
            }

            pcamera.FieldOfView = fov;
            double d2 = r / Math.Tan(0.5 * fov / 180 * Math.PI);
            var newLookDirection = Vector3.Normalize(pcamera.LookDirection);
            newLookDirection *= (float)d2;
            var target = pcamera.Position + pcamera.LookDirection;
            pcamera.Position = target - newLookDirection;
            pcamera.LookDirection = newLookDirection;
        }

        /// <summary>
        /// Zooms the viewport to the specified rectangle.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="rectangle">The rectangle.</param>
        public static void ZoomToRectangle(this Viewport3DX viewport, Rect rectangle)
        {
            var pcam = viewport.Camera as ProjectionCamera;
            if (pcam != null)
            {
                pcam.ZoomToRectangle(viewport, rectangle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point2d"></param>
        /// <returns></returns>
        public static Ray UnProjectToRay(this Viewport3DX viewport, Point point2d)
        {
            var r = viewport.UnProject(point2d);
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static BoundingBox FindBounds(this Viewport3DX viewport)
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
                        bounds = BoundingBox.Merge(bounds, r.BoundsWithTransform);
                    }
                }
            }
            return bounds;
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Viewport3DX viewport, Action<T> action) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                if (node.WrapperSource is T element)
                {
                    action(element);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewport">The viewport.</param>
        /// <param name="function">The function. Return true to continue traverse, otherwise stop at current node</param>
        public static void Traverse<T>(this Viewport3DX viewport, Func<T, bool> function) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                if (node.WrapperSource is T element)
                {
                    return function(element);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Element3D element, Action<T> action) where T : Element3D
        {
            var sceneNode = new SceneNode[] { element.SceneNode };
            Traverse(element, action);
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="function">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Element3D element, Func<T, bool> function) where T : Element3D
        {
            var sceneNode = new SceneNode[] { element.SceneNode };
            Traverse(element, function);
        }
    }
}
