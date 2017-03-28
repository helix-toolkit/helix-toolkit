// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for the cameras.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using Matrix = global::SharpDX.Matrix;
    using Matrix3x3 = global::SharpDX.Matrix3x3;
    using Vector3 = global::SharpDX.Vector3;

    /// <summary>
    /// Provides extension methods for the cameras.
    /// </summary>
    public static class CameraExtensions
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
            this Camera camera,
            Point3D newPosition,
            Vector3D newDirection,
            Vector3D newUpDirection,
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
        /// An orthographic camera.
        /// </param>
        /// <param name="newWidth">
        /// The width to animate to.
        /// </param>
        /// <param name="animationTime">
        /// Animation time in milliseconds
        /// </param>
        public static void AnimateWidth(this OrthographicCamera camera, double newWidth, double animationTime)
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
        /// The new look direction.
        /// </param>
        /// <param name="newUpDirection">
        /// The new up direction.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public static void ChangeDirection(this ProjectionCamera camera, Vector3D newLookDir, Vector3D newUpDirection, double animationTime)
        {
            var target = camera.Position + camera.LookDirection;
            double length = camera.LookDirection.Length;
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
        public static void CopyTo(this Camera source, Camera dest)
        {
            var projectionSource = source as ProjectionCamera;
            var projectionDest = dest as ProjectionCamera;
            if (projectionSource == null || projectionDest == null)
            {
                return;
            }

            projectionDest.LookDirection = projectionSource.LookDirection;
            projectionDest.Position = projectionSource.Position;
            projectionDest.UpDirection = projectionSource.UpDirection;

            var psrc = source as PerspectiveCamera;
            var osrc = source as OrthographicCamera;
            var pdest = dest as PerspectiveCamera;
            var odest = dest as OrthographicCamera;
            if (pdest != null)
            {
                projectionDest.NearPlaneDistance = projectionSource.NearPlaneDistance > 0 ? projectionSource.NearPlaneDistance : 1e-1;
                projectionDest.FarPlaneDistance = projectionSource.FarPlaneDistance;

                double fov = 45;
                if (psrc != null)
                {
                    fov = psrc.FieldOfView;
                }

                if (osrc != null)
                {
                    double dist = projectionSource.LookDirection.Length;
                    fov = Math.Atan2(osrc.Width / 2, dist) * (180 / Math.PI);
                }

                pdest.FieldOfView = fov;
            }

            if (odest != null)
            {
                projectionDest.NearPlaneDistance = projectionSource.NearPlaneDistance;
                projectionDest.FarPlaneDistance = projectionSource.FarPlaneDistance;

                double width = 100;
                if (psrc != null)
                {
                    double dist = projectionSource.LookDirection.Length;
                    width = Math.Tan(psrc.FieldOfView / 180 * Math.PI) * 2 * dist;
                }

                if (osrc != null)
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
        public static PerspectiveCamera CreateDefaultCamera()
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
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return default(Vector3D);
            }

            var axis1 = Vector3D.CrossProduct(projectionCamera.LookDirection, projectionCamera.UpDirection);
            var axis2 = Vector3D.CrossProduct(axis1, projectionCamera.LookDirection);
            axis1.Normalize();
            axis2.Normalize();
            double l = projectionCamera.LookDirection.Length;
            double f = l * 0.001;
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
        public static Matrix GetInverseViewProjectionMatrix(this Camera camera, double aspectRatio)
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
        public static Matrix GetProjectionMatrix(this Camera camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
            {
                return perspectiveCamera.CreateProjectionMatrix(aspectRatio);
            }

            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
            {
                return orthographicCamera.CreateProjectionMatrix(aspectRatio);
            }
            throw new HelixToolkitException("Unknown camera type.");
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
        public static Matrix GetViewProjectionMatrix(this Camera camera, double aspectRatio)
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
        public static Matrix GetViewMatrix(this Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera is ProjectionCamera)
            {
                var projcam = camera as ProjectionCamera;
                return Matrix.LookAtRH(
                    projcam.Position.ToVector3(),
                    (projcam.Position + projcam.LookDirection).ToVector3(),
                    projcam.UpDirection.ToVector3());
            }

            throw new HelixToolkitException("Unknown camera type.");
        }

        public static Matrix GetInversedViewMatrix(this Camera camera)
        {
            var viewMatrix = GetViewMatrix(camera);
            return InverseViewMatrix(ref viewMatrix);
        }

        public static Matrix InverseViewMatrix(ref Matrix viewMatrix)
        {
            //var v33Transpose = new Matrix3x3(
            //    viewMatrix.M11, viewMatrix.M21, viewMatrix.M31,
            //    viewMatrix.M12, viewMatrix.M22, viewMatrix.M32,
            //    viewMatrix.M13, viewMatrix.M23, viewMatrix.M33);
            
            //var vpos = viewMatrix.Row4.ToVector3();

            //     vpos = Vector3.Transform(vpos, v33Transpose) * -1;

            var x = viewMatrix.M41 * viewMatrix.M11 + viewMatrix.M42 * viewMatrix.M12 + viewMatrix.M43 * viewMatrix.M13;
            var y = viewMatrix.M41 * viewMatrix.M21 + viewMatrix.M42 * viewMatrix.M22 + viewMatrix.M43 * viewMatrix.M23;
            var z = viewMatrix.M41 * viewMatrix.M31 + viewMatrix.M42 * viewMatrix.M32 + viewMatrix.M43 * viewMatrix.M33;
      
            return new Matrix(
                viewMatrix.M11, viewMatrix.M21, viewMatrix.M31, 0,
                viewMatrix.M12, viewMatrix.M22, viewMatrix.M32, 0,
                viewMatrix.M13, viewMatrix.M23, viewMatrix.M33, 0, -x, -y, -z, 1);
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
        public static void LookAt(this Camera camera, Point3D target, double animationTime)
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
            this Camera camera, Point3D target, Vector3D newLookDirection, double animationTime)
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
        public static void LookAt(this Camera camera, Point3D target, double distance, double animationTime)
        {
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
                return;

            var d = projectionCamera.LookDirection;
            d.Normalize();
            LookAt(projectionCamera, target, d * distance, animationTime);
        }

        /// <summary>
        /// Resets the specified camera.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public static void Reset(this Camera camera)
        {
            var projectionCamera = camera as PerspectiveCamera;
            if (projectionCamera != null)
            {
                Reset(projectionCamera);
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
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

            if (bounds.IsEmpty || diagonal.LengthSquared.Equals(0))
            {
                return;
            }

            ZoomExtents(camera as ProjectionCamera, viewport, bounds, animationTime);
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
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + (diagonal * 0.5);
            double radius = diagonal.Length * 0.5;
            ZoomExtents(projectionCamera, viewport, center, radius, animationTime);
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
            var projectionCamera = camera as ProjectionCamera;
            if (projectionCamera == null)
            {
                return;
            }

            // var target = Camera.Position + Camera.LookDirection;
            if (camera is PerspectiveCamera)
            {
                var pcam = camera as PerspectiveCamera;
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = projectionCamera.LookDirection;
                dir.Normalize();
                LookAt(projectionCamera, center, dir * dist, animationTime);
            }

            if (camera is OrthographicCamera)
            {
                LookAt(projectionCamera, center, projectionCamera.LookDirection, animationTime);
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
        public static void ZoomToRectangle(this Camera camera, Viewport3DX viewport, Rect zoomRectangle)
        {
            var pcam = camera as ProjectionCamera;
            if (pcam == null)
            {
                return;
            }

            var topLeftRay = viewport.UnProjectToRay(zoomRectangle.TopLeft);
            var topRightRay = viewport.UnProjectToRay(zoomRectangle.TopRight);
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
                var distance = pcam.LookDirection.Length;

                // option 1: change distance
                var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
                var newLookDirection = newDistance * w;
                var newPosition = perspectiveCamera.Position + ((distance - newDistance) * w);
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
                var distance = pcam.LookDirection.Length;
                var newTarget = centerRay.PlaneIntersection(oldTarget, w);
                if (newTarget != null)
                {
                    orthographicCamera.LookDirection = w * distance;
                    orthographicCamera.Position = newTarget.Value - orthographicCamera.LookDirection;
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
            var pcam = camera as ProjectionCamera;
            if (pcam == null)
            {
                return;
            }

            var z = pcam.LookDirection;
            z.Normalize();
            var x = Vector3D.CrossProduct(pcam.LookDirection, pcam.UpDirection);
            var y = Vector3D.CrossProduct(x, z);
            y.Normalize();
            x = Vector3D.CrossProduct(z, y);

            // delta *= this.ZoomSensitivity;
            pcam.Position += (x * delta.X) + (y * delta.Y) + (z * delta.Z);
        }
    }
}