using SharpDX;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Cameras;
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

            var plane1 = new Plane(corners[leftMostPoint], leftNormal);
            var plane2 = new Plane(corners[rightMostPoint], rightNormal);
            PlaneExtensions.PlaneIntersectsPlane(ref plane1, ref plane2, out var horizontalIntersection);
            plane1 = new Plane(corners[topMostPoint], topNormal);
            plane2 = new Plane(corners[bottomMostPoint], bottomNormal);
            PlaneExtensions.PlaneIntersectsPlane(ref plane1, ref plane2, out var verticalIntersection);
            FindClosestPointsOnTwoLines(ref horizontalIntersection, ref verticalIntersection, out var closestPointLine1, out var closestPointLine2);
            position = Vector3.Dot(closestPointLine1 - closestPointLine2, cameraDir) < 0 ? closestPointLine1 : closestPointLine2;
            upDir = cameraUp;
            var boundPlane = new Plane(boundingBox.Center, cameraDir);
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
                var local = Vector3.TransformCoordinate(p, view);
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
    }
}