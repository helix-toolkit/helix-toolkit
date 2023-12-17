using HelixToolkit.SharpDX.Cameras;
using SharpDX;

namespace HelixToolkit.SharpDX;

public static class CameraCoreExtensions
{
    public static BoundingFrustum CreateFrustum(this CameraCore camera, float aspectRatio)
    {
        return new BoundingFrustum(camera.CreateViewMatrix() * camera.CreateProjectionMatrix(aspectRatio));
    }

    // Returns whether or not the given point is the outermost point in the given direction among all points of the bounds
    private static bool IsOutermostPointInDirection(int pointIndex, ref Vector3 direction, Vector3[] corners)
    {
        Vector3 point = corners[pointIndex];
        for (int i = 0; i < corners.Length; i++)
        {
            if (i != pointIndex && Vector3.Dot(direction, corners[i] - point) > 0)
                return false;
        }

        return true;
    }

    // Credit: http://wiki.unity3d.com/index.php/3d_Math_functions
    // Returns the edge points of the closest line segment between 2 lines
    private static void FindClosestPointsOnTwoLines(ref Ray line1, ref Ray line2, out Vector3 closestPointLine1, out Vector3 closestPointLine2)
    {
        Vector3 line1Direction = line1.Direction;
        Vector3 line2Direction = line2.Direction;

        float a = Vector3.Dot(line1Direction, line1Direction);
        float b = Vector3.Dot(line1Direction, line2Direction);
        float e = Vector3.Dot(line2Direction, line2Direction);

        float d = a * e - b * b;

        Vector3 r = line1.Position - line2.Position;
        float c = Vector3.Dot(line1Direction, r);
        float f = Vector3.Dot(line2Direction, r);

        float s = (b * f - c * e) / d;
        float t = (a * f - c * b) / d;

        closestPointLine1 = line1.Position + line1Direction * s;
        closestPointLine2 = line2.Position + line2Direction * t;
    }
    /// <summary>
    /// Ref: https://github.com/yasirkula/UnityRuntimePreviewGenerator/blob/7a3b44b07949f712010b680b9f2c499e5aa2ebc1/Plugins/RuntimePreviewGenerator/RuntimePreviewGenerator.cs#L347
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="aspectRatio"></param>
    /// <param name="boundingBox"></param>
    /// <param name="position"></param>
    /// <param name="lookDir"></param>
    /// <param name="upDir"></param>
    public static void ZoomExtents(this PerspectiveCameraCore camera, float aspectRatio, BoundingBox boundingBox, out Vector3 position, out Vector3 lookDir, out Vector3 upDir)
    {
        var cameraDir = Vector3.Normalize(camera.LookDirection);
        var cameraUp = Vector3.Normalize(camera.UpDirection);
        var cameraRight = Vector3.Cross(cameraDir, cameraUp);
        cameraUp = Vector3.Cross(cameraRight, cameraDir);

        var corners = boundingBox.GetCorners();

        var frustum = camera.CreateFrustum(aspectRatio);
        var leftNormal = -frustum.Left.Normal;
        var rightNormal = -frustum.Right.Normal;
        var topNormal = -frustum.Top.Normal;
        var bottomNormal = -frustum.Bottom.Normal;

        int leftMostPoint = -1, rightMostPoint = -1, topMostPoint = -1, bottomMostPoint = -1;
        for (int i = 0; i < corners.Length; i++)
        {
            if (leftMostPoint < 0 && IsOutermostPointInDirection(i, ref leftNormal, corners))
            {
                leftMostPoint = i;
            }
            if (rightMostPoint < 0 && IsOutermostPointInDirection(i, ref rightNormal, corners))
            {
                rightMostPoint = i;
            }
            if (topMostPoint < 0 && IsOutermostPointInDirection(i, ref topNormal, corners))
            {
                topMostPoint = i;
            }
            if (bottomMostPoint < 0 && IsOutermostPointInDirection(i, ref bottomNormal, corners))
            {
                bottomMostPoint = i;
            }
        }

        var plane1 = PlaneHelper.GetPlane(corners[leftMostPoint], leftNormal);
        var plane2 = PlaneHelper.GetPlane(corners[rightMostPoint], rightNormal);
        Collision.PlaneIntersectsPlane(ref plane1, ref plane2, out var horizontalIntersection);
        plane1 = PlaneHelper.GetPlane(corners[topMostPoint], topNormal);
        plane2 = PlaneHelper.GetPlane(corners[bottomMostPoint], bottomNormal);
        Collision.PlaneIntersectsPlane(ref plane1, ref plane2, out var verticalIntersection);
        FindClosestPointsOnTwoLines(ref horizontalIntersection, ref verticalIntersection, out var closestPointLine1, out var closestPointLine2);
        position = Vector3.Dot(closestPointLine1 - closestPointLine2, cameraDir) < 0 ? closestPointLine1 : closestPointLine2;
        upDir = cameraUp;
        var boundPlane = PlaneHelper.GetPlane(boundingBox.Center, cameraDir);
        var lookRay = new Ray(position, cameraDir);
        boundPlane.Intersects(ref lookRay, out float dist);
        lookDir = cameraDir * dist;
    }
    /// <summary>
    /// Ref: https://github.com/yasirkula/UnityRuntimePreviewGenerator/blob/7a3b44b07949f712010b680b9f2c499e5aa2ebc1/Plugins/RuntimePreviewGenerator/RuntimePreviewGenerator.cs#L347
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="aspectRatio"></param>
    /// <param name="boundingBox"></param>
    /// <param name="position"></param>
    /// <param name="lookDir"></param>
    /// <param name="upDir"></param>
    /// <param name="width"></param>
    public static void ZoomExtents(this OrthographicCameraCore camera, float aspectRatio, BoundingBox boundingBox, out Vector3 position, out Vector3 lookDir, out Vector3 upDir, out float width)
    {
        float minX = float.PositiveInfinity, minY = float.PositiveInfinity, maxX = float.NegativeInfinity, maxY = float.NegativeInfinity;
        var corners = boundingBox.GetCorners();
        var view = camera.CreateViewMatrix();
        foreach (var p in corners)
        {
            var local = Vector3Helper.TransformCoordinate(p, view);
            minX = Math.Min(minX, local.X);
            minY = Math.Min(minY, local.Y);
            maxX = Math.Max(maxX, local.X);
            maxY = Math.Max(maxY, local.Y);
        }
        width = aspectRatio > 1 ? Math.Max((maxX - minX), (maxY - minY) * aspectRatio) : Math.Max((maxX - minX) / aspectRatio, maxY - minY);
        position = boundingBox.Center - camera.LookDirection.Normalized() * width;
        lookDir = camera.LookDirection.Normalized() * width;
        upDir = camera.UpDirection;
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
        if (camera is ProjectionCameraCore pcam)
        {
            if (viewport.UnProject(new Vector2(zoomRectangle.Top, zoomRectangle.Left), out var topLeftRay)
            && viewport.UnProject(new Vector2(zoomRectangle.Top, zoomRectangle.Right), out var topRightRay)
            && viewport.UnProject(
                    new Vector2(
                        (zoomRectangle.Left + zoomRectangle.Right) * 0.5f,
                        (zoomRectangle.Top + zoomRectangle.Bottom) * 0.5f), out var centerRay))
            {
                var u = Vector3.Normalize(topLeftRay.Direction);
                var v = Vector3.Normalize(topRightRay.Direction);
                var w = Vector3.Normalize(centerRay.Direction);
                if (camera is PerspectiveCameraCore perspectiveCamera)
                {
                    var distance = pcam.LookDirection.Length();

                    // option 1: change distance
                    var newDistance = distance * zoomRectangle.Width / viewport.ViewportRectangle.Width;
                    var newLookDirection = (float)newDistance * w;
                    var newPosition = perspectiveCamera.Position + ((distance - (float)newDistance) * w);
                    var newTarget = newPosition + newLookDirection;
                    LookAt(pcam, newTarget, newLookDirection, 200);
                }
                else if (camera is OrthographicCameraCore orthographicCamera)
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

        if (diagonal.LengthSquared().Equals(0))
        {
            return;
        }
        if (camera is PerspectiveCameraCore pCore)
        {
            pCore.ZoomExtents((float)(viewport.ActualWidth / viewport.ActualHeight), bounds, out var pos, out var look, out var up);
            pCore.AnimateTo(pos, look, up, animationTime);
        }
        else if (camera is OrthographicCameraCore oCore)
        {
            oCore.ZoomExtents((float)(viewport.ActualWidth / viewport.ActualHeight), bounds, out var pos, out var look, out var up, out var width);
            oCore.AnimateWidth(width, animationTime);
            oCore.AnimateTo(pos, look, up, animationTime);
        }
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
            var dir = Vector3.Normalize(camera.LookDirection);
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
