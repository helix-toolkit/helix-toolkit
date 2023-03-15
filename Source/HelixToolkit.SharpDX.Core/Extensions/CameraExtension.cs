using SharpDX;
using System;
namespace HelixToolkit.SharpDX.Core
{
    using Controls;   
    using Cameras;
    public static class CameraExtension
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
        public static void LookAt(this CameraCore camera, Vector3 target, float animationTime)
        {
            if (camera is ProjectionCameraCore projectionCamera)
            {
                LookAt(camera, target, projectionCamera.LookDirection, animationTime);
            }            
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
            this CameraCore camera, Vector3 target, Vector3 newLookDirection, float animationTime)
        {
            if (camera is ProjectionCameraCore projectionCamera)
            {
                LookAt(camera, target, newLookDirection, projectionCamera.UpDirection, animationTime);
            }
        }
        /// <summary>
        /// Looks at.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="target">The target.</param>
        /// <param name="newLookDirection">The new look direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void LookAt(
            this CameraCore camera,
            Vector3 target,
            Vector3 newLookDirection,
            Vector3 newUpDirection,
            float animationTime)
        {
            Vector3 newPosition = target - newLookDirection;
            camera.AnimateTo(newPosition, newLookDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Changes the direction of a camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="newLookDir">
        /// The new look direction.
        /// </param>
        /// <param name="newUpDirection">
        /// The new up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ChangeDirection(this CameraCore camera, Vector3 newLookDir, Vector3 newUpDirection, float animationTime)
        {
            var target = camera.Position + camera.LookDirection;
            var length = camera.LookDirection.Length();
            LookAt(camera, target, Vector3.Normalize(newLookDir) * length, newUpDirection, animationTime);
        }

        /// <summary>
        /// Animates the orthographic width.
        /// </summary>
        /// <param name="camera">
        /// An orthographic camera.
        /// </param>
        /// <param name="newWidth">
        /// The width to animate to.
        /// </param>
        /// <param name="animationTime">
        /// Animation time in milliseconds
        /// </param>
        public static void AnimateWidth(this OrthographicCameraCore camera, float newWidth, float animationTime)
        {
            if (animationTime > 0)
            {
                camera.AnimateWidth(newWidth, animationTime);
            }
            else
            {
                camera.Width = newWidth;
            }
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
        public static void ZoomToRectangle(this CameraCore camera, ViewportCore viewport, RectangleF zoomRectangle)
        {
            if(camera is ProjectionCameraCore pcam)
            {
                if(viewport.UnProject(new Vector2(zoomRectangle.Top, zoomRectangle.Left), out var topLeftRay)
                && viewport.UnProject(new Vector2(zoomRectangle.Top, zoomRectangle.Right), out var topRightRay)
                && viewport.UnProject(
                        new Vector2(
                            (zoomRectangle.Left + zoomRectangle.Right) * 0.5f,
                            (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5f), out var centerRay))
                {
                    var u = topLeftRay.Direction;
                    var v = topRightRay.Direction;
                    var w = centerRay.Direction;
                    u.Normalize();
                    v.Normalize();
                    w.Normalize();
                    if(camera is PerspectiveCameraCore perspectiveCamera)
                    {
                        var distance = pcam.LookDirection.Length();

                        // option 1: change distance
                        var newDistance = distance * zoomRectangle.Width / viewport.ViewportRectangle.Width;
                        var newLookDirection = (float)newDistance * w;
                        var newPosition = perspectiveCamera.Position + ((distance - (float)newDistance) * w);
                        var newTarget = newPosition + newLookDirection;
                        LookAt(pcam, newTarget, newLookDirection, 200);
                    }
                    else if(camera is OrthographicCameraCore orthographicCamera)
                    {
                        orthographicCamera.Width *= zoomRectangle.Width / viewport.ViewportRectangle.Width;
                        var oldTarget = pcam.Position + pcam.LookDirection;
                        var distance = pcam.LookDirection.Length();
                        
                        if (centerRay.PlaneIntersection(oldTarget, w, out var newTarget))
                        {
                            orthographicCamera.LookDirection = w * distance;
                            orthographicCamera.Position = newTarget - orthographicCamera.LookDirection;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the specified camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this CameraCore camera)
        {
            if (camera is PerspectiveCameraCore projectionCamera)
            {
                Reset(projectionCamera);
            }         
            else if (camera is OrthographicCameraCore ocamera)
            {
                Reset(ocamera);
            }
        }
        /// <summary>
        /// Resets the specified perspective camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this PerspectiveCameraCore camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Vector3(20, 10, 40);
            camera.LookDirection = new Vector3(-20, -10, -40);
            camera.UpDirection = new Vector3(0, 1, 0);
            camera.FieldOfView = 45;
            camera.NearPlaneDistance = 0.1f;
            camera.FarPlaneDistance = 1000;
        }

        /// <summary>
        /// Resets the specified orthographic camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this OrthographicCameraCore camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Vector3(20, 10, 40);
            camera.LookDirection = new Vector3(-20, -10, -40);
            camera.UpDirection = new Vector3(0, 1, 0);
            camera.Width = 40;
            camera.NearPlaneDistance = 0.1f;
            camera.FarPlaneDistance = 1000;
        }

        /// <summary>
        /// Zooms to fit the extents of the specified viewport.
        /// </summary>
        /// <param name="camera">
        /// The actual camera.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ZoomExtents(
            this CameraCore camera, ViewportCore viewport, float animationTime = 0)
        {
            var bounds = viewport.FindBoundsInternal();
            var diagonal = bounds.Maximum-bounds.Minimum;

            if (diagonal.LengthSquared().Equals(0))
            {
                return;
            }

            ZoomExtents(camera, viewport, bounds, animationTime);
        }

        /// <summary>
        /// Zooms to fit the specified bounding rectangle.
        /// </summary>
        /// <param name="camera">
        /// The actual camera.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="bounds">
        /// The bounding rectangle.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ZoomExtents(
            this CameraCore camera, ViewportCore viewport, BoundingBox bounds, float animationTime = 0)
        {
            var diagonal = bounds.Maximum - bounds.Minimum;
            var center = bounds.Center;
            float radius = diagonal.Length() * 0.5f;
            ZoomExtents(camera, viewport, center, radius, animationTime);
        }

        /// <summary>
        /// Zooms to fit the specified sphere.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="center">
        /// The center of the sphere.
        /// </param>
        /// <param name="radius">
        /// The radius of the sphere.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ZoomExtents(
            this CameraCore camera, ViewportCore viewport, Vector3 center, float radius, float animationTime = 0)
        {
            // var target = Camera.Position + Camera.LookDirection;
            if (camera is PerspectiveCameraCore pcam)
            {
                float disth = radius / (float)Math.Tan(0.75 * pcam.FieldOfView * Math.PI / 180);
                float vfov = pcam.FieldOfView / viewport.ViewportRectangle.Width * viewport.ViewportRectangle.Height;
                float distv = radius / (float)Math.Tan(0.75 * vfov * Math.PI / 180);

                float dist = Math.Max(disth, distv);
                var dir = camera.LookDirection;
                dir.Normalize();
                LookAt(camera, center, dir * dist, animationTime);
            }
            else if (camera is OrthographicCameraCore orth)
            {
                orth.LookAt(center, 0);
                var newWidth = radius * 2;
                if (viewport.ActualWidth > viewport.ActualHeight)
                {
                    newWidth = radius * 2 * (float)(viewport.ActualWidth / viewport.ActualHeight);
                }
                orth.AnimateWidth(newWidth, animationTime);
            }
        }
    }
}
