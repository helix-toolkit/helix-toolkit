using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// Provides extension methods for <see cref="Camera"/> derived classes.
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
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void AnimateTo(
        this ProjectionCamera? camera,
        Point3D newPosition,
        Vector3D newDirection,
        Vector3D newUpDirection,
        double animationTime)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (animationTime < 0)
        {
            throw new ArgumentException("Animation time value must be greater than or equal to zero or indefinite", nameof(animationTime));
        }

        var fromPosition = camera.Position;
        var fromDirection = camera.LookDirection;
        var fromUpDirection = camera.UpDirection;

        camera.Position = newPosition;
        camera.LookDirection = newDirection;
        camera.UpDirection = newUpDirection;

        var a1 = new Point3DAnimation(fromPosition, newPosition, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
            AccelerationRatio = 0.3,
            DecelerationRatio = 0.5,
            FillBehavior = FillBehavior.Stop
        };
        a1.Completed += (s, a) => camera.BeginAnimation(ProjectionCamera.PositionProperty, null);
        camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

        var a2 = new Vector3DAnimation(fromDirection, newDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
            AccelerationRatio = 0.3,
            DecelerationRatio = 0.5,
            FillBehavior = FillBehavior.Stop
        };
        a2.Completed += (s, a) => camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null);
        camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

        var a3 = new Vector3DAnimation(fromUpDirection, newUpDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
            AccelerationRatio = 0.3,
            DecelerationRatio = 0.5,
            FillBehavior = FillBehavior.Stop
        };
        a3.Completed += (s, a) => camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, null);
        camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);
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
    /// Animation time in milliseconds.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void AnimateWidth(this OrthographicCamera? camera, double newWidth, double animationTime)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (animationTime < 0)
        {
            throw new ArgumentException("Animation time value must be greater than or equal to zero or indefinite", nameof(animationTime));
        }

        double fromWidth = camera.Width;
        camera.Width = newWidth;
        var a1 = new DoubleAnimation(fromWidth, newWidth, new Duration(TimeSpan.FromMilliseconds(animationTime)))
        {
            AccelerationRatio = 0.3,
            DecelerationRatio = 0.5,
            FillBehavior = FillBehavior.Stop
        };
        camera.BeginAnimation(OrthographicCamera.WidthProperty, a1);
    }

    /// <summary>
    /// Changes the direction of a camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void ChangeDirection(
        this ProjectionCamera? camera,
        Vector3D newLookDirection,
        Vector3D newUpDirection,
        double animationTime)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        var target = camera.Position + camera.LookDirection;
        var length = camera.LookDirection.Length;
        newLookDirection.Normalize();
        LookAt(camera, target, newLookDirection * length, newUpDirection, animationTime);
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
    /// <param name="copyNearFarPlaneDistances">
    /// Copy near and far plane distances if set to <c>true</c>.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CopyTo(this ProjectionCamera? source, ProjectionCamera? dest, bool copyNearFarPlaneDistances = true)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (dest is null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        dest.LookDirection = source.LookDirection;
        dest.Position = source.Position;
        dest.UpDirection = source.UpDirection;

        if (copyNearFarPlaneDistances)
        {
            dest.NearPlaneDistance = source.NearPlaneDistance;
            dest.FarPlaneDistance = source.FarPlaneDistance;
        }

        var psrc = source as PerspectiveCamera;
        var osrc = source as OrthographicCamera;

        if (dest is PerspectiveCamera pdest)
        {
            double fov = 45;
            if (psrc != null)
            {
                fov = psrc.FieldOfView;
            }
            else if (osrc != null)
            {
                // https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media3D/PerspectiveCamera.cs
                // Pythagorean theorem
                double dist = source.LookDirection.Length;
                fov = Math.Atan(osrc.Width / 2 / dist) * 180 / Math.PI * 2;
            }

            pdest.FieldOfView = fov;
        }
        else if (dest is OrthographicCamera odest)
        {
            double width = 100;
            if (osrc != null)
            {
                width = osrc.Width;
            }
            else if (psrc != null)
            {
                // https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media3D/OrthographicCamera.cs
                // Pythagorean theorem
                double dist = source.LookDirection.Length;
                width = Math.Tan(psrc.FieldOfView / 2 / 180 * Math.PI) * dist * 2;
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void CopyDirectionOnly(this ProjectionCamera? source, ProjectionCamera? dest, double distance)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (dest is null)
        {
            throw new ArgumentNullException(nameof(dest));
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
    /// <returns>
    /// A perspective camera.
    /// </returns>
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
    public static string GetInfo(this Camera camera)
    {
        var sb = new StringBuilder();
        sb.AppendLine(camera.GetType().Name);

        if (camera is MatrixCamera matrixCamera)
        {
            sb.AppendLine("ProjectionMatrix:");
            sb.AppendLine(matrixCamera.ProjectionMatrix.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine("ViewMatrix:");
            sb.AppendLine(matrixCamera.ViewMatrix.ToString(CultureInfo.InvariantCulture));
        }
        else if (camera is ProjectionCamera projectionCamera)
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

            if (camera is PerspectiveCamera perspectiveCamera)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", perspectiveCamera.FieldOfView));
            }
            else if (camera is OrthographicCamera orthographicCamera)
            {
                sb.AppendLine(
                    string.Format(CultureInfo.InvariantCulture, "Width:\t\t{0:0.###}", orthographicCamera.Width));
            }
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
    public static void LookAt(this ProjectionCamera camera, Point3D target, double animationTime)
    {
        LookAt(camera, target, camera.LookDirection, animationTime);
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void LookAt(this ProjectionCamera? camera, Point3D target, double distance, double animationTime)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        Vector3D d = camera.LookDirection;
        d.Normalize();
        LookAt(camera, target, d * distance, animationTime);
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
    public static void LookAt(this ProjectionCamera camera, Point3D target, Vector3D newLookDirection, double animationTime)
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void LookAt(
        this ProjectionCamera? camera,
        Point3D target,
        Vector3D newLookDirection,
        Vector3D newUpDirection,
        double animationTime)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        Point3D newPosition = target - newLookDirection;

        if (camera is PerspectiveCamera perspectiveCamera)
        {
            AnimateTo(perspectiveCamera, newPosition, newLookDirection, newUpDirection, animationTime);
        }
        else if (camera is OrthographicCamera orthographicCamera)
        {
            AnimateTo(orthographicCamera, newPosition, newLookDirection, newUpDirection, animationTime);
        }
    }

    /// <summary>
    /// Resets the specified camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    public static void Reset(this Camera? camera)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }
        if (camera is not ProjectionCamera projectionCamera)
        {
            throw new NotSupportedException(nameof(camera));
        }
        projectionCamera.Position = new Point3D(2, 16, 20);
        projectionCamera.LookDirection = new Vector3D(-2, -16, -20);
        projectionCamera.UpDirection = new Vector3D(0, 0, 1);
        projectionCamera.NearPlaneDistance = 0.1;
        projectionCamera.FarPlaneDistance = double.PositiveInfinity;
        if (camera is PerspectiveCamera pcamera)
        {
            pcamera.FieldOfView = 45;
        }
        else if (camera is OrthographicCamera ocamera)
        {
            ocamera.Width = 40;
        }
    }

    /// <summary>
    /// Obtains the view transform matrix for a camera. (see page 327)
    /// </summary>
    /// <param name="camera">
    /// Camera to obtain the ViewMatrix for
    /// </param>
    /// <returns>
    /// A Matrix3D object with the camera view transform matrix, or a Matrix3D with all zeros if the "camera" is null.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HelixToolkitException"></exception>
    public static Matrix3D GetViewMatrix(this Camera? camera)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (camera is MatrixCamera matrixCamera)
        {
            return matrixCamera.ViewMatrix;
        }
        else if (camera is ProjectionCamera projectionCamera)
        {
            var zaxis = -projectionCamera.LookDirection;
            zaxis.Normalize();

            var xaxis = Vector3D.CrossProduct(projectionCamera.UpDirection, zaxis);
            xaxis.Normalize();

            var yaxis = Vector3D.CrossProduct(zaxis, xaxis);
            var pos = (Vector3D)projectionCamera.Position;

            return new Matrix3D(
                xaxis.X,
                yaxis.X,
                zaxis.X,
                0,
                xaxis.Y,
                yaxis.Y,
                zaxis.Y,
                0,
                xaxis.Z,
                yaxis.Z,
                zaxis.Z,
                0,
                -Vector3D.DotProduct(xaxis, pos),
                -Vector3D.DotProduct(yaxis, pos),
                -Vector3D.DotProduct(zaxis, pos),
                1);
        }

        throw new HelixToolkitException("Unknown camera type.");
    }

    /// <summary>
    /// Gets the projection matrix for the specified camera.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="aspectRatio">
    /// The aspect ratio.
    /// </param>
    /// <returns>
    /// The projection matrix.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HelixToolkitException"></exception>
    public static Matrix3D GetProjectionMatrix(this Camera? camera, double aspectRatio)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (camera is MatrixCamera matrixCamera)
        {
            return matrixCamera.ProjectionMatrix;
        }
        else if (camera is PerspectiveCamera perspectiveCamera)
        {
            // The angle-to-radian formula is a little off because only
            // half the angle enters the calculation.
            double xscale = 1 / Math.Tan(Math.PI * perspectiveCamera.FieldOfView / 360);
            double yscale = xscale * aspectRatio;
            double znear = perspectiveCamera.NearPlaneDistance;
            double zfar = perspectiveCamera.FarPlaneDistance;
            double zscale = double.IsPositiveInfinity(zfar) ? -1 : (zfar / (znear - zfar));
            double zoffset = znear * zscale;

            return new Matrix3D(xscale, 0, 0, 0, 0, yscale, 0, 0, 0, 0, zscale, -1, 0, 0, zoffset, 0);
        }
        else if (camera is OrthographicCamera orthographicCamera)
        {
            double xscale = 2.0 / orthographicCamera.Width;
            double yscale = xscale * aspectRatio;
            double znear = orthographicCamera.NearPlaneDistance;
            double zfar = orthographicCamera.FarPlaneDistance;

            if (double.IsPositiveInfinity(zfar))
            {
                zfar = znear * 1e5;
            }

            double dzinv = 1.0 / (znear - zfar);
            return new Matrix3D(xscale, 0, 0, 0, 0, yscale, 0, 0, 0, 0, dzinv, 0, 0, 0, znear * dzinv, 1);
        }

        throw new HelixToolkitException("Unknown camera type.");
    }

    /// <summary>
    /// Gets the combined view and projection transform.
    /// </summary>
    /// <param name="camera">
    /// The camera.
    /// </param>
    /// <param name="aspectRatio">
    /// The aspect ratio.
    /// </param>
    /// <returns>
    /// The total view and projection transform.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HelixToolkitException"></exception>
    public static Matrix3D GetTotalTransform(this Camera? camera, double aspectRatio)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        var matrix = Matrix3D.Identity;

        if (camera.Transform != null)
        {
            var cameraTransform = camera.Transform.Value;

            if (!cameraTransform.HasInverse)
            {
                throw new HelixToolkitException("Camera transform has no inverse.");
            }

            cameraTransform.Invert();
            matrix.Append(cameraTransform);
        }

        matrix.Append(GetViewMatrix(camera));
        matrix.Append(GetProjectionMatrix(camera, aspectRatio));
        return matrix;
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
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="HelixToolkitException"></exception>
    public static Matrix3D GetInverseTransform(this Camera? camera, double aspectRatio)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        var matrix = GetTotalTransform(camera, aspectRatio);

        if (!matrix.HasInverse)
        {
            throw new HelixToolkitException("Camera transform has no inverse.");
        }

        matrix.Invert();
        return matrix;
    }

    /// <summary>
    /// Fits the current scene in the current view.
    /// </summary>
    /// <param name="camera">The actual camera.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(this ProjectionCamera? camera, Viewport3D viewport, double animationTime = 0)
    {
        if (camera is PerspectiveCamera perspectiveCamera)
        {
            FitView(camera, viewport, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        }
        else if (camera is OrthographicCamera orthoCamera)
        {
            FitView(camera, viewport, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
        }
    }

    /// <summary>
    /// Fits the current scene in the current view.
    /// </summary>
    /// <param name="camera">
    /// The actual camera.
    /// </param>
    /// <param name="viewport">
    /// The viewport.
    /// </param>
    /// <param name="lookDirection">
    /// The look direction.
    /// </param>
    /// <param name="upDirection">
    /// The up direction.
    /// </param>
    /// <param name="animationTime">
    /// The animation time.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void FitView(
        this ProjectionCamera? camera,
        Viewport3D? viewport,
        Vector3D lookDirection,
        Vector3D upDirection,
        double animationTime = 0)
    {
        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        var bounds = Visual3DHelper.FindBounds(viewport.Children);
        var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

        if (bounds.IsEmpty || diagonal.LengthSquared < double.Epsilon)
        {
            return;
        }

        FitView(camera, viewport, bounds, lookDirection, upDirection, animationTime);
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void ZoomExtents(this ProjectionCamera? camera, Viewport3D? viewport, double animationTime = 0)
    {
        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        var bounds = Visual3DHelper.FindBounds(viewport.Children);
        var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

        if (bounds.IsEmpty || diagonal.LengthSquared < double.Epsilon)
        {
            return;
        }

        ZoomExtents(camera, viewport, bounds, animationTime);
    }

    /// <summary>
    /// Zooms to fit the specified bounding rectangle.
    /// </summary>
    /// <param name="camera">
    /// The camera to change.
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
    public static void ZoomExtents(this ProjectionCamera? camera, Viewport3D? viewport, Rect3D bounds, double animationTime = 0)
    {
        if (camera is PerspectiveCamera perspectiveCamera)
        {
            FitView(camera, viewport, bounds, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        }
        else if (camera is OrthographicCamera orthoCamera)
        {
            FitView(camera, viewport, bounds, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
        }
    }

    /// <summary>
    /// Fits the specified bounding rectangle in the current view.
    /// </summary>
    /// <param name="camera">The camera to change.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="bounds">The bounding rectangle.</param>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">The up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    public static void FitView(
        this ProjectionCamera? camera,
        Viewport3D? viewport,
        Rect3D bounds,
        Vector3D lookDirection,
        Vector3D upDirection,
        double animationTime = 0)
    {
        var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
        var center = bounds.Location + (diagonal * 0.5);
        double radius = diagonal.Length * 0.5;
        FitView(camera, viewport, center, radius, lookDirection, upDirection, animationTime);
    }

    /// <summary>
    /// Zooms to fit the specified sphere.
    /// </summary>
    /// <param name="camera">
    /// The camera to change.
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
        ProjectionCamera? camera,
        Viewport3D? viewport,
        Point3D center,
        double radius,
        double animationTime = 0)
    {
        if (camera is PerspectiveCamera perspectiveCamera)
        {
            FitView(camera, viewport, center, radius, perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, animationTime);
        }
        else if (camera is OrthographicCamera orthoCamera)
        {
            FitView(camera, viewport, center, radius, orthoCamera.LookDirection, orthoCamera.UpDirection, animationTime);
        }
    }

    /// <summary>
    /// Fits the specified bounding sphere to the view.
    /// </summary>
    /// <param name="camera">The camera to change.</param>
    /// <param name="viewport">The viewport.</param>
    /// <param name="center">The center of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">The up direction.</param>
    /// <param name="animationTime">The animation time.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void FitView(
        ProjectionCamera? camera,
        Viewport3D? viewport,
        Point3D center,
        double radius,
        Vector3D lookDirection,
        Vector3D upDirection,
        double animationTime = 0)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        if (camera is PerspectiveCamera perspectiveCamera)
        {
            double disth = radius / Math.Tan(0.5 * perspectiveCamera.FieldOfView * Math.PI / 180);
            double vfov = perspectiveCamera.FieldOfView;

            if (viewport.ActualWidth > 0 && viewport.ActualHeight > 0)
            {
                vfov *= viewport.ActualHeight / viewport.ActualWidth;
            }

            double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);
            double dist = Math.Max(disth, distv);
            var dir = lookDirection;
            dir.Normalize();
            LookAt(perspectiveCamera, center, dir * dist, upDirection, animationTime);
        }
        else if (camera is OrthographicCamera orthographicCamera)
        {
            LookAt(orthographicCamera, center, lookDirection, upDirection, animationTime);
            double newWidth = radius * 2;

            if (viewport.ActualWidth > viewport.ActualHeight)
            {
                newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
            }

            AnimateWidth(orthographicCamera, newWidth, animationTime);
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
    /// <exception cref="ArgumentNullException"></exception>
    public static void ZoomToRectangle(this ProjectionCamera? camera, Viewport3D? viewport, Rect zoomRectangle)
    {
        if (camera is null)
        {
            throw new ArgumentNullException(nameof(camera));
        }

        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        var topLeftRay = Viewport3DHelper.Point2DtoRay3D(viewport, zoomRectangle.TopLeft);
        var topRightRay = Viewport3DHelper.Point2DtoRay3D(viewport, zoomRectangle.TopRight);
        var centerRay = Viewport3DHelper.Point2DtoRay3D(
            viewport,
            new Point(
                (zoomRectangle.Left + zoomRectangle.Right) * 0.5,
                (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5));

        if (topLeftRay is null || topRightRay is null || centerRay is null)
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

        if (camera is PerspectiveCamera perspectiveCamera)
        {
            var distance = camera.LookDirection.Length;

            // option 1: change distance
            var newDistance = distance * zoomRectangle.Width / viewport.ActualWidth;
            var newLookDirection = newDistance * w;
            var newPosition = perspectiveCamera.Position + ((distance - newDistance) * w);
            var newTarget = newPosition + newLookDirection;
            LookAt(camera, newTarget, newLookDirection, 200);

            // option 2: change fov
            //    double newFieldOfView = Math.Acos(Vector3D.DotProduct(u, v));
            //    var newTarget = camera.Position + distance * w;
            //    pcamera.FieldOfView = newFieldOfView * 180 / Math.PI;
            //    LookAt(camera, newTarget, distance * w, 0);
        }
        else if (camera is OrthographicCamera orthographicCamera)
        {
            orthographicCamera.Width *= zoomRectangle.Width / viewport.ActualWidth;
            var oldTarget = camera.Position + camera.LookDirection;
            var distance = camera.LookDirection.Length;
            if (centerRay.PlaneIntersection(oldTarget, w, out Point3D newTarget))
            {
                orthographicCamera.LookDirection = w * distance;
                orthographicCamera.Position = newTarget - orthographicCamera.LookDirection;
            }
        }
    }
}
