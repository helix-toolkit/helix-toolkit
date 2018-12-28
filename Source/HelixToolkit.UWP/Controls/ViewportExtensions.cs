using SharpDX;
using System;
using System.Collections.Generic;

namespace HelixToolkit.UWP
{
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
            if(viewport.UnProject(point2d, out var ray))
            {
                return ray;
            }
            else
            {
                return new Ray();
            }
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
            return viewport.UnProjectOnPlane(p.ToVector2(), position, normal);
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
            return viewport.FindHits(position.ToVector2());
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
        /// <param name="node"></param>
        /// <returns>
        /// The find nearest.
        /// </returns>
        public static bool FindNearest(this Viewport3DX viewport, Point position,
            out Vector3 point, out Vector3 normal, out Element3D model, out SceneNode node)
        {
            model = null;
            node = null;
            if(viewport.FindNearest(position.ToVector2(), out point, out normal, out var m))
            {
                if(m is Element3D ele)
                {
                    model = ele;
                    node = ele.SceneNode;
                }
                else if(m is SceneNode nd)
                {
                    node = nd;
                    model = null;
                }
                return true;
            }
            return false;
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
            if (!(viewport.Camera is PerspectiveCamera pcamera) || !viewport.IsChangeFieldOfViewEnabled)
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
            var newLookDirection = pcamera.LookDirection;
            newLookDirection.Normalize();
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
            if (viewport.Camera is ProjectionCamera pcam)
            {
                pcam.ZoomToRectangle(viewport, rectangle);
            }
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
