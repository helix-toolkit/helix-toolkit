using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Point = Windows.Foundation.Point;

namespace HelixToolkit.UWP
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Set the camera target point without changing the look direction.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void LookAt(this Camera camera, Vector3 target, double animationTime)
        {
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            LookAt(camera, target, projectionCamera.LookDirection, animationTime);
        }

        /// <summary>
        /// Set the camera target point and look direction
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="newLookDirection">
        /// The new look direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void LookAt(
            this Camera camera, Vector3 target, Vector3 newLookDirection, double animationTime)
        {
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            LookAt(camera, target, newLookDirection, projectionCamera.UpDirection, animationTime);
        }

        /// <summary>
        /// Set the camera target point and directions
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="newLookDirection">
        /// The new look direction.
        /// </param>
        /// <param name="newUpDirection">
        /// The new up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void LookAt(
            this Camera camera,
            Vector3 target,
            Vector3 newLookDirection,
            Vector3 newUpDirection,
            double animationTime)
        {
            Vector3 newPosition = target - newLookDirection;

            if (camera is PerspectiveCamera persp)
            {
                AnimateTo(persp, newPosition, newLookDirection, newUpDirection, animationTime);
            }
            else if (camera is OrthographicCamera orth)
            {
                AnimateTo(orth, newPosition, newLookDirection, newUpDirection, animationTime);
            }
        }

        /// <summary>
        /// Animates the camera position and directions.
        /// </summary>
        /// <param name="camera">
        /// The camera to animate.
        /// </param>
        /// <param name="newPosition">
        /// The position to animate to.
        /// </param>
        /// <param name="newDirection">
        /// The direction to animate to.
        /// </param>
        /// <param name="newUpDirection">
        /// The up direction to animate to.
        /// </param>
        /// <param name="animationTime">
        /// Animation time in milliseconds.
        /// </param>
        public static void AnimateTo(
            this Camera camera,
            Vector3 newPosition,
            Vector3 newDirection,
            Vector3 newUpDirection,
            double animationTime)
        {
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            var fromPosition = projectionCamera.Position;
            var fromDirection = projectionCamera.LookDirection;
            var fromUpDirection = projectionCamera.UpDirection;

            projectionCamera.Position = newPosition;
            projectionCamera.LookDirection = newDirection;
            projectionCamera.UpDirection = newUpDirection;

            //if (animationTime > 0)
            //{
            //    var a1 = new Point3DAnimation(
            //        fromPosition, newPosition, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };

            //    a1.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.PositionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

            //    var a2 = new Vector3DAnimation(
            //        fromDirection, newDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };
            //    a2.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

            //    var a3 = new Vector3DAnimation(
            //        fromUpDirection, newUpDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };
            //    a3.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);
            //}
        }

        /// <summary>
        /// Zooms the camera to the specified rectangle.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="zoomRectangle">
        /// The zoom rectangle.
        /// </param>
        public static void ZoomToRectangle(this Camera camera, Viewport3DX viewport, Rect zoomRectangle)
        {
            var pcam = camera as ProjectionCamera;
            if (pcam == null)
            {
                return;
            }

            var topLeftRay = viewport.UnProjectToRay(new Point(zoomRectangle.Top, zoomRectangle.Left));
            var topRightRay = viewport.UnProjectToRay(new Point(zoomRectangle.Top, zoomRectangle.Right));
            var centerRay =
                viewport.UnProjectToRay(
                    new Point(
                        (zoomRectangle.Left + zoomRectangle.Right) * 0.5,
                        (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5));

            if (topLeftRay == null || topRightRay == null || centerRay == null)
            {
                // could not invert camera matrix
                return;
            }

            var u = topLeftRay.Direction;
            var v = topRightRay.Direction;
            var w = centerRay.Direction;
            u.Normalize();
            v.Normalize();
            w.Normalize();
            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
            {
                var distance = pcam.LookDirection.Length();

                // option 1: change distance
                var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
                var newLookDirection = (float)newDistance * w;
                var newPosition = perspectiveCamera.Position + ((distance - (float)newDistance) * w);
                var newTarget = newPosition + newLookDirection;
                LookAt(pcam, newTarget, newLookDirection, 200);

                // option 2: change fov
                // double newFieldOfView = Math.Acos(Vector3D.DotProduct(u, v));
                // var newTarget = camera.Position + distance * w;
                // pcamera.FieldOfView = newFieldOfView * 180 / Math.PI;
                // LookAt(camera, newTarget, distance * w, 0);
            }

            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
            {
                orthographicCamera.Width *= zoomRectangle.Width / viewport.ActualWidth;
                var oldTarget = pcam.Position + pcam.LookDirection;
                var distance = pcam.LookDirection.Length();
                var newTarget = centerRay.PlaneIntersection(oldTarget, w);
                if (newTarget != null)
                {
                    orthographicCamera.LookDirection = w * distance;
                    orthographicCamera.Position = newTarget.Value - orthographicCamera.LookDirection;
                }
            }
        }
    }
}
