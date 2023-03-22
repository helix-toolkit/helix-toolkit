// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for the cameras.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

using Matrix = global::SharpDX.Matrix;
using Matrix3x3 = global::SharpDX.Matrix3x3;
using Vector3 = global::SharpDX.Vector3;

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
#if !COREWPF
    using Cameras;
#endif

    /// <summary>
    /// Provides extension methods for the cameras.
    /// </summary>
    public static class CameraExtensions
    {
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
        public static void ChangeDirection(this Camera camera, Vector3D newLookDir, Vector3D newUpDirection, double animationTime)
        {
            var target = camera.Position + camera.LookDirection;
            var length = camera.LookDirection.Length;
            newLookDir.Normalize();
            LookAt(camera, target, newLookDir * length, newUpDirection, animationTime);
        }

        /// <summary>
        /// Copies the specified camera, converts field of view/width if necessary.
        /// </summary>
        /// <param name="source">
        /// The source camera.
        /// </param>
        /// <param name="dest">
        /// The destination camera.
        /// </param>
        public static void CopyTo(this ICameraModel source, Camera dest)
        {
            var projectionSource = source as IProjectionCameraModel;
            var projectionDest = dest as IProjectionCameraModel;
            if (projectionSource == null || projectionDest == null)
            {
                return;
            }

            projectionDest.LookDirection = projectionSource.LookDirection;
            projectionDest.Position = projectionSource.Position;
            projectionDest.UpDirection = projectionSource.UpDirection;

            if (dest is IPerspectiveCameraModel pdest)
            {
                projectionDest.NearPlaneDistance = projectionSource.NearPlaneDistance > 0 ? projectionSource.NearPlaneDistance : 1e-1;
                projectionDest.FarPlaneDistance = projectionSource.FarPlaneDistance;

                double fov = 45;
                if (source is IPerspectiveCameraModel psrc)
                {
                    fov = psrc.FieldOfView;
                }

                if (source is IOrthographicCameraModel osrc)
                {
                    var dist = projectionSource.LookDirection.Length;
                    fov = Math.Atan2(osrc.Width / 2, dist) * (180 / Math.PI);
                }

                pdest.FieldOfView = fov;
            }

            if (dest is IOrthographicCameraModel odest)
            {
                projectionDest.NearPlaneDistance = projectionSource.NearPlaneDistance;
                projectionDest.FarPlaneDistance = projectionSource.FarPlaneDistance;

                double width = 100;
                if (source is IPerspectiveCameraModel psrc)
                {
                    var dist = projectionSource.LookDirection.Length;
                    width = Math.Tan(psrc.FieldOfView / 180 * Math.PI) * 2 * dist;
                }

                if (source is IOrthographicCameraModel osrc)
                {
                    width = osrc.Width;
                }

                odest.Width = width;
            }
        }

        /// <summary>
        /// Creates a default perspective camera.
        /// </summary>
        /// <returns>A perspective camera.</returns>
        public static Camera CreateDefaultCamera()
        {
            var camera = new PerspectiveCamera();
            Reset(camera);
            return camera;
        }

        /// <summary>
        /// Finds the pan vector.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="dx">The delta x.</param>
        /// <param name="dy">The delta y.</param>
        /// <returns>
        /// The <see cref="Vector3D" /> .
        /// </returns>
        public static Vector3D FindPanVector(this Camera camera, double dx, double dy)
        {
            if (!(camera is IProjectionCameraModel projectionCamera))
            {
                return default(Vector3D);
            }

            var axis1 = Vector3D.CrossProduct(projectionCamera.LookDirection, projectionCamera.UpDirection);
            var axis2 = Vector3D.CrossProduct(axis1, projectionCamera.LookDirection);
            axis1.Normalize();
            axis2.Normalize();
            var l = projectionCamera.LookDirection.Length;
            var f = l * 0.001;
            var move = (-axis1 * f * dx) + (axis2 * f * dy);

            // this should be dependent on distance to target?
            return move;
        }

        /// <summary>
        /// Gets an information string about the specified camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <returns>
        /// The get info.
        /// </returns>
        public static string GetInfo(this Camera camera)
        {
            var sb = new StringBuilder();
            sb.AppendLine(camera.GetType().Name);
            if (camera is IProjectionCameraModel projectionCamera)
            {
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "LookDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                        projectionCamera.LookDirection.X,
                        projectionCamera.LookDirection.Y,
                        projectionCamera.LookDirection.Z));
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "UpDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                        projectionCamera.UpDirection.X,
                        projectionCamera.UpDirection.Y,
                        projectionCamera.UpDirection.Z));
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Position:\t\t{0:0.000},{1:0.000},{2:0.000}",
                        projectionCamera.Position.X,
                        projectionCamera.Position.Y,
                        projectionCamera.Position.Z));
                var target = projectionCamera.Position + projectionCamera.LookDirection;
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Target:\t\t{0:0.000},{1:0.000},{2:0.000}",
                        target.X,
                        target.Y,
                        target.Z));
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture, "NearPlaneDist:\t{0}", projectionCamera.NearPlaneDistance));
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "FarPlaneDist:\t{0}", projectionCamera.FarPlaneDistance));
            }

            if (camera is PerspectiveCamera perspectiveCamera)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", perspectiveCamera.FieldOfView));
            }

            if (camera is OrthographicCamera orthographicCamera)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", orthographicCamera.Width));
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Gets the inverse camera transform.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="aspectRatio">
        /// The aspect ratio.
        /// </param>
        /// <returns>
        /// The inverse transform.
        /// </returns>
        public static Matrix3D GetInverseViewProjectionMatrix3D(this Camera camera, double aspectRatio)
        {
            return GetInverseViewProjectionMatrix(camera, aspectRatio).ToMatrix3D();
        }

        /// <summary>
        /// Gets the inverse camera transform.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="aspectRatio">
        /// The aspect ratio.
        /// </param>
        /// <returns>
        /// The inverse transform.
        /// </returns>
        public static Matrix GetInverseViewProjectionMatrix(this CameraCore camera, double aspectRatio)
        {
            var m = GetViewProjectionMatrix(camera, aspectRatio);
            m.Invert();
            return m;
        }

        /// <summary>
        /// Gets the projection matrix for the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The projection matrix.</returns>
        public static Matrix3D GetProjectionMatrix3D(this Camera camera, double aspectRatio)
        {
            return GetProjectionMatrix(camera, aspectRatio).ToMatrix3D();
        }

        /// <summary>
        /// Gets the projection matrix for the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The projection matrix.</returns>
        public static Matrix GetProjectionMatrix(this CameraCore camera, double aspectRatio)
        {
            return camera.CreateProjectionMatrix((float)aspectRatio);
        }

        /// <summary>
        /// Get the combined view and projection transform
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The total view and projection transform.</returns>
        public static Matrix3D GetViewProjectionMatrix3D(this Camera camera, double aspectRatio)
        {
            return GetViewProjectionMatrix(camera, aspectRatio).ToMatrix3D();
        }

        /// <summary>
        /// Get the combined view and projection transform
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The total view and projection transform.</returns>
        public static Matrix GetViewProjectionMatrix(this CameraCore camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }
            return GetViewMatrix(camera) * GetProjectionMatrix(camera, aspectRatio);
        }

        /// <summary>
        /// Obtains the view transform matrix for a camera. (see page 327)
        /// </summary>
        /// <param name="camera">
        /// Camera to obtain the ViewMatrix for
        /// </param>
        /// <returns>
        /// A Matrix object with the camera view transform matrix, or a Matrix with all zeros if the "camera" is null.
        /// </returns>
        public static Matrix3D GetViewMatrix3D(this Camera camera)
        {
            return GetViewMatrix(camera).ToMatrix3D();
        }

        /// <summary>
        /// Obtains the view transform matrix for a camera. (see page 327)
        /// </summary>
        /// <param name="camera">
        /// Camera to obtain the ViewMatrix for
        /// </param>
        /// <returns>
        /// A Matrix object with the camera view transform matrix, or a Matrix with all zeros if the "camera" is null.
        /// </returns>
        public static Matrix GetViewMatrix(this CameraCore camera)
        {
            return camera.CreateViewMatrix();
        }

        public static Matrix3D GetInversedViewMatrix(this Camera camera)
        {
            var viewMatrix = GetViewMatrix(camera);
            return MatrixExtensions.PsudoInvert(ref viewMatrix).ToMatrix3D();
        }

        public static Matrix GetInversedViewMatrix(this CameraCore camera)
        {
            var viewMatrix = GetViewMatrix(camera);
            return MatrixExtensions.PsudoInvert(ref viewMatrix);
        }
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
        public static void LookAt(this ICameraModel camera, Point3D target, double animationTime)
        {
            LookAt(camera, target, camera.LookDirection, animationTime);
        }

        public static void LookAt(this ICameraModel camera, Vector3 target, double animationTime)
        {
            LookAt(camera, target.ToPoint3D(), camera.LookDirection, animationTime);
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
            this ICameraModel camera, Point3D target, Vector3D newLookDirection, double animationTime)
        {
            LookAt(camera, target, newLookDirection, camera.UpDirection, animationTime);
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
            this ICameraModel camera,
            Point3D target,
            Vector3D newLookDirection,
            Vector3D newUpDirection,
            double animationTime)
        {
            var newPosition = target - newLookDirection;
            camera.AnimateTo(newPosition, newLookDirection, newUpDirection, animationTime);
        }

        /// <summary>
        /// Set the camera target point and camera distance.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="target">
        /// The target point.
        /// </param>
        /// <param name="distance">
        /// The distance to the camera.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void LookAt(this Camera camera, Point3D target, double distance, double animationTime)
        {
            var d = camera.LookDirection;
            d.Normalize();
            LookAt(camera, target, d * distance, animationTime);
        }

        /// <summary>
        /// Resets the specified camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this Camera camera)
        {
            if (camera is PerspectiveCamera pCamera)
            {
                Reset(pCamera);
            }
            else if (camera is OrthographicCamera ocamera)
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
        public static void Reset(this PerspectiveCamera camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Point3D(20, 10, 40);
            camera.LookDirection = new Vector3D(-20, -10, -40);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 45;
            camera.NearPlaneDistance = 0.1;
            camera.FarPlaneDistance = 1000;
        }

        /// <summary>
        /// Resets the specified orthographic camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this OrthographicCamera camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Point3D(20, 10, 40);
            camera.LookDirection = new Vector3D(-20, -10, -40);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.Width = 40;
            camera.NearPlaneDistance = 0.1;
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
            this Camera camera, Viewport3DX viewport, double animationTime = 0)
        {
            var bounds = viewport.FindBounds();
            var diagonal = bounds.Maximum - bounds.Minimum;

            if (diagonal.LengthSquared().Equals(0))
            {
                return;
            }

            ZoomExtents(camera, viewport, new Rect3D(bounds.Minimum.ToPoint3D(), (bounds.Maximum - bounds.Minimum).ToSize3D()), animationTime);
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
            this Camera camera, Viewport3DX viewport, Rect3D bounds, double animationTime = 0)
        {
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + (diagonal * 0.5);
            var radius = diagonal.Length * 0.5;
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
            this Camera camera, Viewport3DX viewport, Point3D center, double radius, double animationTime = 0)
        {
            // var target = Camera.Position + Camera.LookDirection;
            if (camera is IPerspectiveCameraModel pcam)
            {
                var disth = radius / Math.Tan(0.75 * pcam.FieldOfView * Math.PI / 180);
                var vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                var distv = radius / Math.Tan(0.75 * vfov * Math.PI / 180);

                var dist = Math.Max(disth, distv);
                var dir = camera.LookDirection;
                dir.Normalize();
                LookAt(camera, center, dir * dist, animationTime);
            }
            else if (camera is IOrthographicCameraModel orth)
            {
                orth.LookAt(center, 0);
                var newWidth = radius * 2;
                if (viewport.ActualWidth > viewport.ActualHeight)
                {
                    newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
                }
                orth.AnimateWidth(newWidth, animationTime);
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
        public static void ZoomToRectangle(this Camera camera, Viewport3DX viewport, Rect zoomRectangle)
        {
            if (viewport.UnProject(zoomRectangle.TopLeft.ToVector2(), out var topLeftRay)
                && viewport.UnProject(zoomRectangle.TopRight.ToVector2(), out var topRightRay)
                && viewport.UnProject(new global::SharpDX.Vector2(
                        (float)(zoomRectangle.Left + zoomRectangle.Right) * 0.5f,
                        (float)(zoomRectangle.Top + zoomRectangle.Bottom) * 0.5f), out var centerRay))
            {
                var u = Vector3.Normalize(topLeftRay.Direction);
                var v = Vector3.Normalize(topRightRay.Direction);
                var w = Vector3.Normalize(centerRay.Direction);
                if (camera is IPerspectiveCameraModel perspectiveCamera)
                {
                    var distance = camera.LookDirection.Length;

                    // option 1: change distance
                    var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
                    var newLookDirection = (float)newDistance * w;
                    var newPosition = camera.CameraInternal.Position + ((float)(distance - newDistance) * w);
                    var newTarget = newPosition + newLookDirection;
                    LookAt(camera, newTarget.ToPoint3D(), newLookDirection.ToVector3D(), 200);

                    // option 2: change fov
                    // double newFieldOfView = Math.Acos(Vector3D.DotProduct(u, v));
                    // var newTarget = camera.Position + distance * w;
                    // pcamera.FieldOfView = newFieldOfView * 180 / Math.PI;
                    // LookAt(camera, newTarget, distance * w, 0);
                }
                else if (camera is IOrthographicCameraModel orthographicCamera)
                {
                    orthographicCamera.Width *= zoomRectangle.Width / viewport.ActualWidth;
                    var oldTarget = camera.CameraInternal.Position + camera.CameraInternal.LookDirection;
                    var distance = camera.CameraInternal.LookDirection.Length();
                    if (centerRay.PlaneIntersection(oldTarget, w, out var newTarget))
                    {
                        LookAt(orthographicCamera, newTarget.ToPoint3D(), 200);
                    }
                }
            }
        }

        /// <summary>
        /// Moves the camera position.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="delta">The delta.</param>
        public static void MoveCameraPosition(this Camera camera, Vector3D delta)
        {
            var z = Vector3.Normalize(camera.CameraInternal.LookDirection);
            var x = Vector3.Cross(z, camera.CameraInternal.UpDirection);
            var y = Vector3.Normalize(Vector3.Cross(x, z));
            x = Vector3.Cross(z, y);

            // delta *= this.ZoomSensitivity;
            camera.Position += ((x * (float)delta.X) + (y * (float)delta.Y) + (z * (float)delta.Z)).ToVector3D();
        }
    }
}