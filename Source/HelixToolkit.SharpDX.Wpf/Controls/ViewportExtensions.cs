namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    using MatrixCamera = HelixToolkit.SharpDX.MatrixCamera;
    using OrthographicCamera = HelixToolkit.SharpDX.OrthographicCamera;
    using PerspectiveCamera = HelixToolkit.SharpDX.PerspectiveCamera;
    using ProjectionCamera = HelixToolkit.SharpDX.ProjectionCamera;

    /// <summary>
    /// Provides extension methods for <see cref="Viewport3DX" />.
    /// </summary>
    public static class ViewportExtensions
    {
        /// <summary>
        /// Copies the specified viewport to the clipboard.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="m">The oversampling multiplier.</param>
        public static void Copy(this Viewport3DX view, int m = 1)
        {
            Clipboard.SetImage(RenderBitmap(view, Brushes.White, m));
        }

        /// <summary>
        /// Copies the specified viewport to the clipboard.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="background">The background.</param>
        /// <param name="m">The oversampling multiplier.</param>
        public static void Copy(this Viewport3DX view, double width, double height, Brush background, int m = 1)
        {
            Clipboard.SetImage(RenderBitmap(view, width, height, background));
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static Rect3D FindBounds(this Viewport3DX viewport)
        {
            // TODO
            return Rect3D.Empty;
        }

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
        public static IList<HitResult> FindHits(Viewport3DX viewport, Point position)
        {
            // var camera = viewport.Camera as ProjectionCamera;
            // if (camera == null)
            // {
            // return null;
            // }

            // var result = new List<HitResult>();
            // HitTestResultCallback callback = hit =>
            // {
            // var rayHit = hit as RayMeshGeometry3DHitTestResult;
            // if (rayHit != null)
            // {
            // if (rayHit.MeshHit != null)
            // {
            // var p = GetGlobalHitPosition(rayHit, viewport);
            // var nn = GetNormalHit(rayHit);
            // var n = nn.HasValue ? nn.Value : new Vector3(0, 0, 1);

            // result.Add(
            // new HitResult
            // {
            // Distance = (camera.Position - p).Length,
            // RayHit = rayHit,
            // Normal = n,
            // Position = p
            // });
            // }
            // }

            // return HitTestResultBehavior.Continue;
            // };

            // var hitParams = new PointHitTestParameters(position);
            // VisualTreeHelper.HitTest(viewport, null, callback, hitParams);

            // return result.OrderBy(k => k.Distance).ToList();
            return null;
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
        /// <param name="visual">
        /// The visual.
        /// </param>
        /// <returns>
        /// The find nearest.
        /// </returns>
        public static bool FindNearest(
            this Viewport3DX viewport,
            Point position,
            out Point3D point,
            out Vector3D normal,
            out DependencyObject visual)
        {
            // TODO
            var camera = viewport.Camera as ProjectionCamera;
            // if (camera == null)
            {
                point = new Point3D();
                normal = new Vector3D();
                visual = null;
                return false;
            }

            // var hitParams = new PointHitTestParameters(position);

            // double minimumDistance = double.MaxValue;
            // var nearestPoint = new Vector3();
            // var nearestNormal = new Vector3();
            // DependencyObject nearestObject = null;

            // VisualTreeHelper.HitTest(
            // viewport,
            // null,
            // delegate(HitTestResult hit)
            // {
            // var rayHit = hit as RayMeshGeometry3DHitTestResult;
            // if (rayHit != null)
            // {
            // var mesh = rayHit.MeshHit;
            // if (mesh != null)
            // {
            // var p1 = mesh.Positions[rayHit.VertexIndex1];
            // var p2 = mesh.Positions[rayHit.VertexIndex2];
            // var p3 = mesh.Positions[rayHit.VertexIndex3];
            // double x = p1.X * rayHit.VertexWeight1 + p2.X * rayHit.VertexWeight2
            // + p3.X * rayHit.VertexWeight3;
            // double y = p1.Y * rayHit.VertexWeight1 + p2.Y * rayHit.VertexWeight2
            // + p3.Y * rayHit.VertexWeight3;
            // double z = p1.Z * rayHit.VertexWeight1 + p2.Z * rayHit.VertexWeight2
            // + p3.Z * rayHit.VertexWeight3;

            // // point in local coordinates
            // var p = new Vector3(x, y, z);

            // // transform to global coordinates

            // // first transform the Model3D hierarchy
            // var t2 = GetTransform(rayHit.VisualHit, rayHit.ModelHit);
            // if (t2 != null)
            // {
            // p = t2.Transform(p);
            // }

            // // then transform the Visual3D hierarchy up to the Viewport3DX ancestor
            // var t = GetTransform(viewport, rayHit.VisualHit);
            // if (t != null)
            // {
            // p = t.Transform(p);
            // }

            // double distance = (camera.Position - p).LengthSquared;
            // if (distance < minimumDistance)
            // {
            // minimumDistance = distance;
            // nearestPoint = p;
            // nearestNormal = Vector3D.CrossProductProduct(p2 - p1, p3 - p1);
            // nearestObject = hit.VisualHit;
            // }
            // }
            // }

            // return HitTestResultBehavior.Continue;
            // },
            // hitParams);

            // point = nearestPoint;
            // visual = nearestObject;
            // normal = nearestNormal;

            // if (minimumDistance >= double.MaxValue)
            // {
            // return false;
            // }

            // normal.Normalize();
            // return true;
        }

        /// <summary>
        /// Find the coordinates of the nearest point given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <returns>The nearest point, or null if no point was found.</returns>
        public static Point3D? FindNearestPoint(this Viewport3DX viewport, Point position)
        {
            Point3D p;
            Vector3D n;
            DependencyObject obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
            {
                return p;
            }

            return null;
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
        public static Matrix3D GetCameraTransform(this Viewport3DX viewport)
        {
            return viewport.Camera.GetTotalTransform(viewport.ActualWidth / viewport.ActualHeight);
        }

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates).
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="position">
        /// A 2D point.
        /// </param>
        /// <returns>
        /// The ray.
        /// </returns>
        public static Ray3D GetRay(this Viewport3DX viewport, Point position)
        {
            Point3D point1, point2;
            bool ok = UnProject(viewport, position, out point1, out point2);
            if (!ok)
            {
                return null;
            }

            return new Ray3D { Origin = point1, Direction = point2 - point1 };
        }

        /// <summary>
        /// Gets the total number of triangles in the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total number of triangles</returns>
        public static int GetTotalNumberOfTriangles(this Viewport3DX viewport)
        {
            return 0;

            // int count = 0;
            // Visual3DHelper.Traverse<GeometryModel3D>(
            // viewport.Children,
            // (m, t) =>
            // {
            // var geometry = m.Geometry as MeshGeometry3D;
            // if (geometry != null && geometry.TriangleIndices != null)
            // {
            // count += geometry.TriangleIndices.Count / 3;
            // }
            // });
            // return count;
        }

        /// <summary>
        /// Gets the total transform for a Viewport3DX.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>
        public static Matrix3D GetTotalTransform(this Viewport3DX viewport)
        {
            return GetCameraTransform(viewport) * GetViewportTransform(viewport);
        }

        /// <summary>
        /// Gets the viewport transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        public static Matrix3D GetViewportTransform(this Viewport3DX viewport)
        {
            return new Matrix3D(
                viewport.ActualWidth / 2,
                0,
                0,
                0,
                0,
                -viewport.ActualHeight / 2,
                0,
                0,
                0,
                0,
                1,
                0,
                viewport.ActualWidth / 2,
                viewport.ActualHeight / 2,
                0,
                1);
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="pointIn">The input point.</param>
        /// <param name="pointNear">The point at the near clipping plane.</param>
        /// <param name="pointFar">The point at the far clipping plane.</param>
        /// <returns>True if the points were found.</returns>
        public static bool UnProject(this Viewport3DX viewport, Point pointIn, out Point3D pointNear, out Point3D pointFar)
        {
            pointNear = new Point3D();
            pointFar = new Point3D();

            var pointIn3D = new Point3D(pointIn.X, pointIn.Y, 0);
            var matrixViewport = GetViewportTransform(viewport);
            var matrixCamera = GetCameraTransform(viewport);

            if (!matrixViewport.HasInverse)
            {
                return false;
            }

            if (!matrixCamera.HasInverse)
            {
                return false;
            }

            matrixViewport.Invert();
            matrixCamera.Invert();

            var pointNormalized = matrixViewport.Transform(pointIn3D);
            pointNormalized.Z = 0.01f;
            pointNear = matrixCamera.Transform(pointNormalized);
            pointNormalized.Z = 0.99f;
            pointFar = matrixCamera.Transform(pointNormalized);

            return true;
        }

        /// <summary>
        /// Un-projects the specified 2D screen point to a ray.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="pointIn">The point.</param>
        /// <returns>The ray.</returns>
        public static Ray3D UnProjectToRay(this Viewport3DX viewport, Point pointIn)
        {
            Point3D pointNear, pointFar;
            if (!UnProject(viewport, pointIn, out pointNear, out pointFar))
            {
                return null;
            }

            return new Ray3D(pointNear, pointFar - pointNear);
        }

        /// <summary>
        /// Projects the specified 3D point to a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Point Project(this Viewport3DX viewport, Point3D point)
        {
            var matrix = GetTotalTransform(viewport);
            var pointTransformed = matrix.Transform(point);
            var pt = new Point(pointTransformed.X, pointTransformed.Y);
            return pt;
        }

        /// <summary>
        /// Prints the specified viewport.
        /// </summary>
        /// <param name="vp">
        /// The viewport.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public static void Print(this Viewport3DX vp, string description)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                dlg.PrintVisual(vp, description);
            }
        }

        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="background">The background.</param>
        /// <param name="m">The oversampling multiplier.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(this Viewport3DX view, Brush background, int m = 1)
        {
            var target = new WriteableBitmap(
                (int)view.ActualWidth * m, (int)view.ActualHeight * m, 96, 96, PixelFormats.Pbgra32, null);

            var originalCamera = view.Camera;
            var vm = originalCamera.GetViewMatrix();
            double ar = view.ActualWidth / view.ActualHeight;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // change the camera viewport and scaling
                    var pm = originalCamera.GetProjectionMatrix(ar);
                    if (originalCamera is OrthographicCamera)
                    {
                        pm.OffsetX = m - 1 - (i * 2);
                        pm.OffsetY = -(m - 1 - (j * 2));
                    }

                    if (originalCamera is PerspectiveCamera)
                    {
                        pm.M31 = -(m - 1 - (i * 2));
                        pm.M32 = m - 1 - (j * 2);
                    }

                    pm.M11 *= m;
                    pm.M22 *= m;

                    var mc = new MatrixCamera(vm, pm);
                    view.Camera = mc;

                    var partialBitmap = new RenderTargetBitmap(
                        (int)view.ActualWidth, (int)view.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                    // render background
                    var backgroundRectangle = new Rectangle
                        {
                            Width = partialBitmap.Width,
                            Height = partialBitmap.Height,
                            Fill = background
                        };
                    backgroundRectangle.Arrange(new Rect(0, 0, backgroundRectangle.Width, backgroundRectangle.Height));
                    partialBitmap.Render(backgroundRectangle);

                    // render 3d
                    partialBitmap.Render(view);

                    // copy to the target bitmap
                    CopyBitmap(partialBitmap, target, (int)(i * view.ActualWidth), (int)(j * view.ActualHeight));
                }
            }

            // restore the camera
            view.Camera = originalCamera;
            return target;
        }

        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="background">The background.</param>
        /// <param name="m">The oversampling multiplier.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(
            this Viewport3DX view, double width, double height, Brush background, int m = 1)
        {
            double w = view.Width;
            double h = view.Height;
            ResizeAndArrange(view, width, height);
            var rtb = RenderBitmap(view, background, m);
            ResizeAndArrange(view, w, h);
            return rtb;
        }

        /// <summary>
        /// Resizes and arranges the viewport.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        public static void ResizeAndArrange(this Viewport3DX view, double width, double height)
        {
            view.Width = width;
            view.Height = height;
            if (double.IsNaN(width) || double.IsNaN(height))
            {
                return;
            }

            view.Measure(new Size(width, height));
            view.Arrange(new Rect(0, 0, width, height));
        }

        /// <summary>
        /// Saves the viewport to a file.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="background">The background brush.</param>
        /// <param name="m">The oversampling multiplier.</param>
        public static void SaveBitmap(this Viewport3DX view, string fileName, Brush background = null, int m = 1)
        {
            // var exporter = new BitmapExporter(fileName) { Background = background, OversamplingMultiplier = m };
            // exporter.Export(view);
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
        public static Point3D? UnProject(this Viewport3DX viewport, Point p, Point3D position, Vector3D normal)
        {
            var ray = GetRay(viewport, p);
            if (ray == null)
            {
                return null;
            }

            return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// Un-projects a point from the screen (2D) to a point on the plane trough the camera target point.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public static Point3D? UnProject(this Viewport3DX viewport, Point p)
        {
            var pc = viewport.Camera as ProjectionCamera;
            if (pc == null)
            {
                return null;
            }

            return UnProject(viewport, p, pc.Position + pc.LookDirection, pc.LookDirection);
        }

        /// <summary>
        /// Zooms to the extents of the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, double animationTime = 0)
        {
            var bounds = viewport.FindBounds();
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

            if (bounds.IsEmpty || diagonal.LengthSquared.Equals(0))
            {
                return;
            }

            ZoomExtents(viewport, bounds, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding box.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="bounds">The bounding rectangle.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, Rect3D bounds, double animationTime = 0)
        {
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + (diagonal * 0.5);
            double radius = diagonal.Length * 0.5;
            ZoomExtents(viewport, center, radius, animationTime);
        }

        /// <summary>
        /// Zooms to the extents of the specified bounding sphere.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, Point3D center, double radius, double animationTime = 0)
        {
            var camera = viewport.Camera;
            var pcam = camera as PerspectiveCamera;
            if (pcam != null)
            {
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = pcam.LookDirection;
                dir.Normalize();
                pcam.LookAt(center, dir * dist, animationTime);
            }

            var ocam = camera as OrthographicCamera;
            if (ocam != null)
            {
                ocam.LookAt(center, ocam.LookDirection, animationTime);
                double newWidth = radius * 2;

                if (viewport.ActualWidth > viewport.ActualHeight)
                {
                    newWidth = radius * 2 * viewport.ActualWidth / viewport.ActualHeight;
                }

                ocam.AnimateWidth(newWidth, animationTime);
            }
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
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
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
            double d = pcamera.LookDirection.Length;
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
            newLookDirection *= d2;
            var target = pcamera.Position + pcamera.LookDirection;
            pcamera.Position = target - newLookDirection;
            pcamera.LookDirection = newLookDirection;
        }

        /// <summary>
        /// Copies the source bitmap to the specified position in the target bitmap.
        /// </summary>
        /// <param name="source">The source bitmap.</param>
        /// <param name="target">The target bitmap.</param>
        /// <param name="offsetx">The x offset.</param>
        /// <param name="offsety">The y offset.</param>
        private static void CopyBitmap(BitmapSource source, WriteableBitmap target, int offsetx, int offsety)
        {
            // Calculate stride of source
            int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

            // Create data array to hold source pixel data
            var data = new byte[stride * source.PixelHeight];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(new Int32Rect(offsetx, offsety, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }

        /// <summary>
        /// Provides a hit test result.
        /// </summary>
        public struct HitResult
        {
            // TODO
        }
    }
}