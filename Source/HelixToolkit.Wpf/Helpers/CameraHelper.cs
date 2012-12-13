// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides helper methods related to Media3D.Camera.
    /// </summary>
    public static class CameraHelper
    {
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
            ProjectionCamera camera,
            Point3D newPosition,
            Vector3D newDirection,
            Vector3D newUpDirection,
            double animationTime)
        {
            var fromPosition = camera.Position;
            var fromDirection = camera.LookDirection;
            var fromUpDirection = camera.UpDirection;

            camera.Position = newPosition;
            camera.LookDirection = newDirection;
            camera.UpDirection = newUpDirection;

            if (animationTime > 0)
            {
                var a1 = new Point3DAnimation(
                    fromPosition, newPosition, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                    {
                        AccelerationRatio = 0.3,
                        DecelerationRatio = 0.5,
                        FillBehavior = FillBehavior.Stop

                    };
                a1.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.PositionProperty, null); };
                camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

                var a2 = new Vector3DAnimation(
                    fromDirection, newDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                    {
                        AccelerationRatio = 0.3,
                        DecelerationRatio = 0.5,
                        FillBehavior = FillBehavior.Stop
                    };
                a2.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null); };
                camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

                var a3 = new Vector3DAnimation(
                    fromUpDirection, newUpDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                    {
                        AccelerationRatio = 0.3,
                        DecelerationRatio = 0.5,
                        FillBehavior = FillBehavior.Stop
                    };
                a3.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, null); };
                camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);
            }
        }

        /// <summary>
        /// Animates the orthographic width.
        /// </summary>
        /// <param name="camera">
        /// An ortographic camera.
        /// </param>
        /// <param name="newWidth">
        /// The width to animate to.
        /// </param>
        /// <param name="animationTime">
        /// Animation time in milliseconds
        /// </param>
        public static void AnimateWidth(OrthographicCamera camera, double newWidth, double animationTime)
        {
            double fromWidth = camera.Width;

            camera.Width = newWidth;

            if (animationTime > 0)
            {
                var a1 = new DoubleAnimation(
                    fromWidth, newWidth, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                    {
                        AccelerationRatio = 0.3,
                        DecelerationRatio = 0.5,
                        FillBehavior = FillBehavior.Stop
                    };
                camera.BeginAnimation(OrthographicCamera.WidthProperty, a1);
            }
        }

        /// <summary>
        /// Changes the direction of a camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        /// <param name="newLookDir">
        /// The new look dir.
        /// </param>
        /// <param name="newUpDirection">
        /// The new up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ChangeDirection(
            ProjectionCamera camera, Vector3D newLookDir, Vector3D newUpDirection, double animationTime)
        {
            var target = camera.Position + camera.LookDirection;
            double length = camera.LookDirection.Length;
            newLookDir.Normalize();
            LookAt(camera, target, newLookDir * length, newUpDirection, animationTime);
        }

        /// <summary>
        /// Copies the specified camera, converts field of view/width if neccessary.
        /// </summary>
        /// <param name="source">
        /// The source camera.
        /// </param>
        /// <param name="dest">
        /// The destination camera.
        /// </param>
        public static void Copy(ProjectionCamera source, ProjectionCamera dest)
        {
            if (source == null || dest == null)
            {
                return;
            }

            dest.LookDirection = source.LookDirection;
            dest.Position = source.Position;
            dest.UpDirection = source.UpDirection;
            dest.NearPlaneDistance = source.NearPlaneDistance;
            dest.FarPlaneDistance = source.FarPlaneDistance;
            var psrc = source as PerspectiveCamera;
            var osrc = source as OrthographicCamera;
            var pdest = dest as PerspectiveCamera;
            var odest = dest as OrthographicCamera;
            if (pdest != null)
            {
                double fov = 45;
                if (psrc != null)
                {
                    fov = psrc.FieldOfView;
                }

                if (osrc != null)
                {
                    double dist = source.LookDirection.Length;
                    fov = Math.Atan(osrc.Width / 2 / dist) * 180 / Math.PI * 2;
                }

                pdest.FieldOfView = fov;
            }

            if (odest != null)
            {
                double width = 100;
                if (psrc != null)
                {
                    double dist = source.LookDirection.Length;
                    width = Math.Tan(psrc.FieldOfView / 180 * Math.PI / 2) * dist * 2;
                }

                if (osrc != null)
                {
                    width = osrc.Width;
                }

                odest.Width = width;
            }
        }

        /// <summary>
        /// Copy the direction of the source <see cref="Camera"/>. Used for the CoordinateSystem view.
        /// </summary>
        /// <param name="source">
        /// The source camera.
        /// </param>
        /// <param name="dest">
        /// The destination camera.
        /// </param>
        /// <param name="distance">
        /// New length of the LookDirection vector.
        /// </param>
        public static void CopyDirectionOnly(ProjectionCamera source, ProjectionCamera dest, double distance)
        {
            if (source == null || dest == null)
            {
                return;
            }

            Vector3D dir = source.LookDirection;
            dir.Normalize();
            dir *= distance;

            dest.LookDirection = dir;
            dest.Position = new Point3D(-dest.LookDirection.X, -dest.LookDirection.Y, -dest.LookDirection.Z);
            dest.UpDirection = source.UpDirection;
        }

        /// <summary>
        /// Creates a default perspective camera.
        /// </summary>
        /// <returns>A perspective camera.</returns>
        public static PerspectiveCamera CreateDefaultCamera()
        {
            var camera = new PerspectiveCamera();
            Reset(camera);
            return camera;
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
        public static string GetInfo(Camera camera)
        {
            var matrixCamera = camera as MatrixCamera;
            var perspectiveCamera = camera as PerspectiveCamera;
            var projectionCamera = camera as ProjectionCamera;
            var orthographicCamera = camera as OrthographicCamera;
            var sb = new StringBuilder();
            sb.AppendLine(camera.GetType().Name);
            if (projectionCamera != null)
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

            if (perspectiveCamera != null)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", perspectiveCamera.FieldOfView));
            }

            if (orthographicCamera != null)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", orthographicCamera.Width));
            }

            if (matrixCamera != null)
            {
                sb.AppendLine("ProjectionMatrix:");
                sb.AppendLine(matrixCamera.ProjectionMatrix.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine("ViewMatrix:");
                sb.AppendLine(matrixCamera.ViewMatrix.ToString(CultureInfo.InvariantCulture));
            }

            return sb.ToString().Trim();
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
        public static void LookAt(ProjectionCamera camera, Point3D target, double animationTime)
        {
            LookAt(camera, target, camera.LookDirection, animationTime);
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
            ProjectionCamera camera, Point3D target, Vector3D newLookDirection, double animationTime)
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
            ProjectionCamera camera,
            Point3D target,
            Vector3D newLookDirection,
            Vector3D newUpDirection,
            double animationTime)
        {
            Point3D newPosition = target - newLookDirection;

            if (camera is PerspectiveCamera)
            {
                AnimateTo(camera as PerspectiveCamera, newPosition, newLookDirection, newUpDirection, animationTime);
            }

            if (camera is OrthographicCamera)
            {
                AnimateTo(camera as OrthographicCamera, newPosition, newLookDirection, newUpDirection, animationTime);
            }
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
        public static void LookAt(ProjectionCamera camera, Point3D target, double distance, double animationTime)
        {
            Vector3D d = camera.LookDirection;
            d.Normalize();
            LookAt(camera, target, d * distance, animationTime);
        }

        /// <summary>
        /// Resets the specified camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(Camera camera)
        {
            var pcamera = camera as PerspectiveCamera;
            if (pcamera != null)
            {
                Reset(pcamera);
            }

            var ocamera = camera as OrthographicCamera;
            if (ocamera != null)
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
        public static void Reset(PerspectiveCamera camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Point3D(2, 16, 20);
            camera.LookDirection = new Vector3D(-2, -16, -20);
            camera.UpDirection = new Vector3D(0, 0, 1);
            camera.FieldOfView = 45;
            camera.NearPlaneDistance = 0.1;
            camera.FarPlaneDistance = 100000;
        }

        /// <summary>
        /// Resets the specified orthographic camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(OrthographicCamera camera)
        {
            if (camera == null)
            {
                return;
            }

            camera.Position = new Point3D(2, 16, 20);
            camera.LookDirection = new Vector3D(-2, -16, -20);
            camera.UpDirection = new Vector3D(0, 0, 1);
            camera.Width = 40;
            camera.NearPlaneDistance = 0.1;
            camera.FarPlaneDistance = 100000;
        }

        /// <summary>
        /// Zooms to fit the extents of the specified viewport.
        /// </summary>
        /// <param name="actualCamera">
        /// The actual camera.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ZoomExtents(ProjectionCamera actualCamera, Viewport3D viewport, double animationTime = 0)
        {
            var bounds = Visual3DHelper.FindBounds(viewport.Children);
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

            if (bounds.IsEmpty || diagonal.LengthSquared == 0)
            {
                return;
            }

            ZoomExtents(actualCamera, viewport, bounds, animationTime);
        }

        /// <summary>
        /// Zooms to fit the specified bounding rectangle.
        /// </summary>
        /// <param name="actualCamera">
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
            ProjectionCamera actualCamera, Viewport3D viewport, Rect3D bounds, double animationTime = 0)
        {
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + diagonal * 0.5;
            double radius = diagonal.Length * 0.5;
            ZoomExtents(actualCamera, viewport, center, radius, animationTime);
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
            ProjectionCamera camera, Viewport3D viewport, Point3D center, double radius, double animationTime = 0)
        {
            // var target = Camera.Position + Camera.LookDirection;
            if (camera is PerspectiveCamera)
            {
                var pcam = camera as PerspectiveCamera;
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = camera.LookDirection;
                dir.Normalize();
                LookAt(camera, center, dir * dist, animationTime);
            }

            if (camera is OrthographicCamera)
            {
                LookAt(camera, center, camera.LookDirection, animationTime);
                double newWidth = radius * 2;

                if (viewport.ActualWidth > viewport.ActualHeight)
                {
                    newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
                }

                AnimateWidth(camera as OrthographicCamera, newWidth, animationTime);
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
        public static void ZoomToRectangle(ProjectionCamera camera, Viewport3D viewport, Rect zoomRectangle)
        {
            var topLeftRay = Viewport3DHelper.Point2DtoRay3D(viewport, zoomRectangle.TopLeft);
            var topRightRay = Viewport3DHelper.Point2DtoRay3D(viewport, zoomRectangle.TopRight);
            var centerRay = Viewport3DHelper.Point2DtoRay3D(
                viewport,
                new Point(
                    (zoomRectangle.Left + zoomRectangle.Right) * 0.5, (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5));

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
                var distance = camera.LookDirection.Length;

                // option 1: change distance
                var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
                var newLookDirection = newDistance * w;
                var newPosition = perspectiveCamera.Position + (distance - newDistance) * w;
                var newTarget = newPosition + newLookDirection;
                LookAt(camera, newTarget, newLookDirection, 200);

                // option 2: change fov
                //    double newFieldOfView = Math.Acos(Vector3D.DotProduct(u, v));
                //    var newTarget = camera.Position + distance * w;
                //    pcamera.FieldOfView = newFieldOfView * 180 / Math.PI;
                //    LookAt(camera, newTarget, distance * w, 0);
            }

            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
            {
                orthographicCamera.Width *= zoomRectangle.Width / viewport.ActualWidth;
                var oldTarget = camera.Position + camera.LookDirection;
                var distance = camera.LookDirection.Length;
                Point3D newTarget;
                if (centerRay.PlaneIntersection(oldTarget, w, out newTarget))
                {
                    orthographicCamera.LookDirection = w * distance;
                    orthographicCamera.Position = newTarget - orthographicCamera.LookDirection;
                }
            }
        }

    }
}