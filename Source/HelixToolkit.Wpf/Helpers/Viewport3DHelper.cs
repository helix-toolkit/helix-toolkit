// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Viewport3DHelper.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    /// <summary>
    /// Helper methods for Viewport3D.
    /// </summary>
    /// <remarks>
    /// See Charles Petzold's book "3D programming for Windows" and Eric Sink's "Twelve Days of WPF 3D"
    /// http://www.ericsink.com/wpf3d/index.html
    /// </remarks>
    public static class Viewport3DHelper
    {
        /// <summary>
        /// Copies the specified viewport to the clipboard.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="m">The oversampling multiplier.</param>
        public static void Copy(Viewport3D view, int m = 1)
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
        public static void Copy(Viewport3D view, double width, double height, Brush background, int m = 1)
        {
            Clipboard.SetImage(RenderBitmap(view, width, height, background));
        }

        /// <summary>
        /// Copies the viewport as xaml to the clipboard.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        public static void CopyXaml(Viewport3D view)
        {
            Clipboard.SetText(XamlWriter.Save(view));
        }

        /// <summary>
        /// Exports the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="background">The background.</param>
        public static void Export(Viewport3D view, string fileName, Brush background = null)
        {
            string ext = System.IO.Path.GetExtension(fileName);
            if (ext != null)
            {
                ext = ext.ToLower();
            }

            switch (ext)
            {
                case ".jpg":
                case ".png":
                    SaveBitmap(view, fileName, background, 2);
                    break;
                case ".xaml":
                    ExportXaml(view, fileName);
                    break;
                case ".xml":
                    ExportKerkythea(view, fileName, background);
                    break;
                case ".obj":
                    ExportObj(view, fileName);
                    break;
                case ".x3d":
                    ExportX3D(view, fileName);
                    break;
                case ".dae":
                    ExportCollada(view, fileName);
                    break;
                default:
                    throw new HelixToolkitException("Not supported file format.");
            }
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
        public static IList<HitResult> FindHits(Viewport3D viewport, Point position)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return null;
            }

            var result = new List<HitResult>();
            HitTestResultCallback callback = hit =>
                {
                    var rayHit = hit as RayMeshGeometry3DHitTestResult;
                    if (rayHit != null)
                    {
                        if (rayHit.MeshHit != null)
                        {
                            var p = GetGlobalHitPosition(rayHit, viewport);
                            var nn = GetNormalHit(rayHit);
                            var n = nn.HasValue ? nn.Value : new Vector3D(0, 0, 1);

                            result.Add(
                                new HitResult
                                    {
                                        Distance = (camera.Position - p).Length,
                                        RayHit = rayHit,
                                        Normal = n,
                                        Position = p
                                    });
                        }
                    }

                    return HitTestResultBehavior.Continue;
                };

            var hitParams = new PointHitTestParameters(position);
            VisualTreeHelper.HitTest(viewport, null, callback, hitParams);

            return result.OrderBy(k => k.Distance).ToList();
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
            Viewport3D viewport, Point position, out Point3D point, out Vector3D normal, out DependencyObject visual)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                point = new Point3D();
                normal = new Vector3D();
                visual = null;
                return false;
            }

            var hitParams = new PointHitTestParameters(position);

            double minimumDistance = double.MaxValue;
            var nearestPoint = new Point3D();
            var nearestNormal = new Vector3D();
            DependencyObject nearestObject = null;

            VisualTreeHelper.HitTest(
                viewport,
                null,
                delegate(HitTestResult hit)
                {
                    var rayHit = hit as RayMeshGeometry3DHitTestResult;
                    if (rayHit != null)
                    {
                        var mesh = rayHit.MeshHit;
                        if (mesh != null)
                        {
                            var p1 = mesh.Positions[rayHit.VertexIndex1];
                            var p2 = mesh.Positions[rayHit.VertexIndex2];
                            var p3 = mesh.Positions[rayHit.VertexIndex3];
                            double x = p1.X * rayHit.VertexWeight1 + p2.X * rayHit.VertexWeight2
                                       + p3.X * rayHit.VertexWeight3;
                            double y = p1.Y * rayHit.VertexWeight1 + p2.Y * rayHit.VertexWeight2
                                       + p3.Y * rayHit.VertexWeight3;
                            double z = p1.Z * rayHit.VertexWeight1 + p2.Z * rayHit.VertexWeight2
                                       + p3.Z * rayHit.VertexWeight3;

                            // point in local coordinates
                            var p = new Point3D(x, y, z);

                            // transform to global coordinates

                            // first transform the Model3D hierarchy
                            var t2 = GetTransform(rayHit.VisualHit, rayHit.ModelHit);
                            if (t2 != null)
                            {
                                p = t2.Transform(p);
                            }

                            // then transform the Visual3D hierarchy up to the Viewport3D ancestor
                            var t = GetTransform(viewport, rayHit.VisualHit);
                            if (t != null)
                            {
                                p = t.Transform(p);
                            }

                            double distance = (camera.Position - p).LengthSquared;
                            if (distance < minimumDistance)
                            {
                                minimumDistance = distance;
                                nearestPoint = p;
                                nearestNormal = Vector3D.CrossProduct(p2 - p1, p3 - p1);
                                nearestObject = hit.VisualHit;
                            }
                        }
                    }

                    return HitTestResultBehavior.Continue;
                },
                hitParams);

            point = nearestPoint;
            visual = nearestObject;
            normal = nearestNormal;

            if (minimumDistance >= double.MaxValue)
            {
                return false;
            }

            normal.Normalize();
            return true;
        }

        /// <summary>
        /// Find the coordinates of the nearest point given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <returns>The nearest point, or null if no point was found.</returns>
        public static Point3D? FindNearestPoint(Viewport3D viewport, Point position)
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
        /// Find the Visual3D that is nearest given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <returns>
        /// The nearest visual, or null if no visual was found.
        /// </returns>
        public static Visual3D FindNearestVisual(Viewport3D viewport, Point position)
        {
            Point3D p;
            Vector3D n;
            DependencyObject obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
            {
                return obj as Visual3D;
            }

            return null;
        }

        /// <summary>
        /// Gets the camera transform.
        /// </summary>
        /// <param name="viewport3DVisual">The viewport visual.</param>
        /// <returns>The camera transform.</returns>
        public static Matrix3D GetCameraTransform(Viewport3DVisual viewport3DVisual)
        {
            return GetTotalTransform(viewport3DVisual.Camera, viewport3DVisual.Viewport.Size.Width / viewport3DVisual.Viewport.Size.Height);
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
        public static Matrix3D GetCameraTransform(Viewport3D viewport)
        {
            return GetTotalTransform(viewport.Camera, viewport.ActualWidth / viewport.ActualHeight);
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
        public static Matrix3D GetInverseTransform(Camera camera, double aspectRatio)
        {
            var m = GetTotalTransform(camera, aspectRatio);

            if (!m.HasInverse)
            {
                throw new HelixToolkitException("Camera transform has no inverse.");
            }

            m.Invert();
            return m;
        }

        /// <summary>
        /// Get all lights in the Viewport3D.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The lights.</returns>
        public static IEnumerable<Light> GetLights(Viewport3D viewport)
        {
            var models = SearchFor<Light>(viewport.Children);
            return models.Select(m => m as Light);
        }

        /// <summary>
        /// Gets the projection matrix for the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The projection matrix.</returns>
        public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera != null)
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

            var orthographicCamera = camera as OrthographicCamera;
            if (orthographicCamera != null)
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

                var m = new Matrix3D(xscale, 0, 0, 0, 0, yscale, 0, 0, 0, 0, dzinv, 0, 0, 0, znear * dzinv, 1);
                return m;
            }

            var matrixCamera = camera as MatrixCamera;
            if (matrixCamera != null)
            {
                return matrixCamera.ProjectionMatrix;
            }

            throw new HelixToolkitException("Unknown camera type.");
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
        public static Ray3D GetRay(Viewport3D viewport, Point position)
        {
            Point3D point1, point2;
            bool ok = Point2DtoPoint3D(viewport, position, out point1, out point2);
            if (!ok)
            {
                return null;
            }

            return new Ray3D { Origin = point1, Direction = point2 - point1 };
        }

        /// <summary>
        /// Get the combined view and projection transform
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>The total view and projection transform.</returns>
        public static Matrix3D GetTotalTransform(Camera camera, double aspectRatio)
        {
            var m = Matrix3D.Identity;

            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera.Transform != null)
            {
                var cameraTransform = camera.Transform.Value;

                if (!cameraTransform.HasInverse)
                {
                    throw new HelixToolkitException("Camera transform has no inverse.");
                }

                cameraTransform.Invert();
                m.Append(cameraTransform);
            }

            m.Append(GetViewMatrix(camera));
            m.Append(GetProjectionMatrix(camera, aspectRatio));
            return m;
        }

        /// <summary>
        /// Gets the total transform.
        /// </summary>
        /// <param name="viewport3DVisual">The viewport3DVisual.</param>
        /// <returns>The total transform.</returns>
        public static Matrix3D GetTotalTransform(Viewport3DVisual viewport3DVisual)
        {
            var m = GetCameraTransform(viewport3DVisual);
            m.Append(GetViewportTransform(viewport3DVisual));
            return m;
        }

        /// <summary>
        /// Gets the total transform for a Viewport3D.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>
        public static Matrix3D GetTotalTransform(Viewport3D viewport)
        {
            var matx = GetCameraTransform(viewport);
            matx.Append(GetViewportTransform(viewport));
            return matx;
        }

        /// <summary>
        /// Get the total transform of a Visual3D
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="visual">The visual.</param>
        /// <returns>The transform.</returns>
        public static GeneralTransform3D GetTransform(Viewport3D viewport, Visual3D visual)
        {
            if (visual == null)
            {
                return null;
            }

            foreach (var ancestor in viewport.Children)
            {
                if (visual.IsDescendantOf(ancestor))
                {
                    var g = new GeneralTransform3DGroup();

                    // this includes the visual.Transform
                    var ta = visual.TransformToAncestor(ancestor);
                    if (ta != null)
                    {
                        g.Children.Add(ta);
                    }

                    // add the transform of the top-level ancestor
                    g.Children.Add(ancestor.Transform);

                    return g;
                }
            }

            return visual.Transform;
        }

        /// <summary>
        /// Gets the transform from the specified Visual3D to the Model3D.
        /// </summary>
        /// <param name="visual">The source visual.</param>
        /// <param name="model">The target model.</param>
        /// <returns>The transform.</returns>
        public static GeneralTransform3D GetTransform(Visual3D visual, Model3D model)
        {
            var mv = visual as ModelVisual3D;
            if (mv != null)
            {
                return GetTransform(mv.Content, model, Transform3D.Identity);
            }

            return null;
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
        public static Matrix3D GetViewMatrix(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera is MatrixCamera)
            {
                return (camera as MatrixCamera).ViewMatrix;
            }

            if (camera is ProjectionCamera)
            {
                // Reflector on: ProjectionCamera.CreateViewMatrix
                var projcam = camera as ProjectionCamera;

                var zaxis = -projcam.LookDirection;
                zaxis.Normalize();

                var xaxis = Vector3D.CrossProduct(projcam.UpDirection, zaxis);
                xaxis.Normalize();

                var yaxis = Vector3D.CrossProduct(zaxis, xaxis);
                var pos = (Vector3D)projcam.Position;

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
        /// Gets the viewport for the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <returns>The parent viewport.</returns>
        public static Viewport3D GetViewport(Visual3D visual)
        {
            DependencyObject parent = visual;
            while (parent != null)
            {
                var vp = parent as Viewport3DVisual;
                if (vp != null)
                {
                    return vp.Parent as Viewport3D;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Gets the viewport transform.
        /// </summary>
        /// <param name="vis">The viewport3DVisual.</param>
        /// <returns>The transform.</returns>
        public static Matrix3D GetViewportTransform(Viewport3DVisual vis)
        {
            return new Matrix3D(
                vis.Viewport.Width / 2,
                0,
                0,
                0,
                0,
                -vis.Viewport.Height / 2,
                0,
                0,
                0,
                0,
                1,
                0,
                vis.Viewport.X + vis.Viewport.Width / 2,
                vis.Viewport.Y + vis.Viewport.Height / 2,
                0,
                1);
        }

        /// <summary>
        /// Gets the viewport transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        public static Matrix3D GetViewportTransform(Viewport3D viewport)
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
        /// Transforms a Point2D to a Point3D.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="pointIn">The pt in.</param>
        /// <param name="pointNear">The point near.</param>
        /// <param name="pointFar">The point far.</param>
        /// <returns>The point 2 dto point 3 d.</returns>
        public static bool Point2DtoPoint3D(Viewport3D viewport, Point pointIn, out Point3D pointNear, out Point3D pointFar)
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
            pointNormalized.Z = 0.01;
            pointNear = matrixCamera.Transform(pointNormalized);
            pointNormalized.Z = 0.99;
            pointFar = matrixCamera.Transform(pointNormalized);

            return true;
        }

        /// <summary>
        /// Transforms a 2D point to a ray.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="pointIn">The point.</param>
        /// <returns>The ray.</returns>
        public static Ray3D Point2DtoRay3D(Viewport3D viewport, Point pointIn)
        {
            Point3D pointNear, pointFar;
            if (!Point2DtoPoint3D(viewport, pointIn, out pointNear, out pointFar))
            {
                return null;
            }

            return new Ray3D(pointNear, pointFar);
        }

        /// <summary>
        /// Transforms the Point3D to a Point2D.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Point Point3DtoPoint2D(Viewport3D viewport, Point3D point)
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
        public static void Print(Viewport3D vp, string description)
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
        public static BitmapSource RenderBitmap(Viewport3D view, Brush background, int m = 1)
        {
            var target = new WriteableBitmap((int)view.ActualWidth * m, (int)view.ActualHeight * m, 96, 96, PixelFormats.Pbgra32, null);

            var originalCamera = view.Camera;
            var vm = GetViewMatrix(originalCamera);
            double ar = view.ActualWidth / view.ActualHeight;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // change the camera viewport and scaling
                    var pm = GetProjectionMatrix(originalCamera, ar);
                    if (originalCamera is OrthographicCamera)
                    {
                        pm.OffsetX = m - 1 - i * 2;
                        pm.OffsetY = -(m - 1 - j * 2);
                    }

                    if (originalCamera is PerspectiveCamera)
                    {
                        pm.M31 = -(m - 1 - i * 2);
                        pm.M32 = m - 1 - j * 2;
                    }

                    pm.M11 *= m;
                    pm.M22 *= m;

                    var mc = new MatrixCamera(vm, pm);
                    view.Camera = mc;

                    var partialBitmap = new RenderTargetBitmap((int)view.ActualWidth, (int)view.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                    // render background
                    var backgroundRectangle = new Rectangle { Width = partialBitmap.Width, Height = partialBitmap.Height, Fill = background };
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
        public static BitmapSource RenderBitmap(Viewport3D view, double width, double height, Brush background, int m = 1)
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
        public static void ResizeAndArrange(Viewport3D view, double width, double height)
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
        public static void SaveBitmap(Viewport3D view, string fileName, Brush background = null, int m = 1)
        {
            var exporter = new BitmapExporter(fileName) { Background = background, OversamplingMultiplier = m };
            exporter.Export(view);
        }

        /// <summary>
        /// Recursive search in a Visual3D collection for objects of given type T
        /// </summary>
        /// <typeparam name="T">The type to search for.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>A list of models.</returns>
        public static IList<Model3D> SearchFor<T>(IEnumerable<Visual3D> collection)
        {
            var output = new List<Model3D>();
            SearchFor(collection, typeof(T), output);
            return output;
        }

        /// <summary>
        /// Un projects a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <param name="position">
        /// A point in the plane.
        /// </param>
        /// <param name="normal">
        /// The plane normal.
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        /// <remarks>
        /// Maps window coordinates to object coordinates like gluUnProject.
        /// </remarks>
        public static Point3D? UnProject(Viewport3D viewport, Point p, Point3D position, Vector3D normal)
        {
            var ray = GetRay(viewport, p);
            if (ray == null)
            {
                return null;
            }

            Point3D i;
            return ray.PlaneIntersection(position, normal, out i) ? (Point3D?)i : null;
        }

        /// <summary>
        /// Un projects a point from the screen (2D) to a point on the plane trough the camera target point.
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
        public static Point3D? UnProject(Viewport3D viewport, Point p)
        {
            var pc = viewport.Camera as ProjectionCamera;
            if (pc == null)
            {
                return null;
            }

            return UnProject(viewport, p, pc.Position + pc.LookDirection, pc.LookDirection);
        }

        /// <summary>
        /// Gets the total number of triangles in the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total number of triangles</returns>
        public static int GetTotalNumberOfTriangles(Viewport3D viewport)
        {
            int count = 0;
            Visual3DHelper.Traverse<GeometryModel3D>(
                viewport.Children,
                (m, t) =>
                {
                    var geometry = m.Geometry as MeshGeometry3D;
                    if (geometry != null && geometry.TriangleIndices != null)
                    {
                        count += geometry.TriangleIndices.Count / 3;
                    }
                });
            return count;
        }
        /// <summary>
        /// Copies the bitmap.
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
            byte[] data = new byte[stride * source.PixelHeight];

            // Copy source image pixels to the data array
            source.CopyPixels(data, stride, 0);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(new Int32Rect(offsetx, offsety, source.PixelWidth, source.PixelHeight), data, stride, 0);
        }

        /// <summary>
        /// Exports to kerkythea.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="background">The background.</param>
        private static void ExportKerkythea(Viewport3D view, string fileName, Brush background)
        {
            ExportKerkythea(view, fileName, background, (int)view.ActualWidth, (int)view.ActualHeight);
        }

        /// <summary>
        /// Exports to kerkythea.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="background">
        /// The background.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        private static void ExportKerkythea(Viewport3D view, string fileName, Brush background, int width, int height)
        {
            var scb = background as SolidColorBrush;
            var backgroundColor = scb != null ? scb.Color : Colors.White;
            using (var e = new KerkytheaExporter(fileName) { Width = width, Height = height, BackgroundColor = backgroundColor })
            {
                e.Export(view);
            }
        }

        /// <summary>
        /// Exports to obj.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        private static void ExportObj(Viewport3D view, string fileName)
        {
            using (var e = new ObjExporter(fileName))
            {
                e.Export(view);
            }
        }

        /// <summary>
        /// Exports to X3D.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        private static void ExportX3D(Viewport3D view, string fileName)
        {
            using (var e = new X3DExporter(fileName))
            {
                e.Export(view);
            }
        }

        /// <summary>
        /// Exports to COLLADA.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        private static void ExportCollada(Viewport3D view, string fileName)
        {
            using (var e = new ColladaExporter(fileName))
            {
                e.Export(view);
            }
        }

        /// <summary>
        /// Exports to xaml.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        private static void ExportXaml(Viewport3D view, string fileName)
        {
            using (var e = new XamlExporter(fileName))
            {
                e.Export(view);
            }
        }

        /// <summary>
        /// Gets the hit position transformed to global (viewport) coordinates.
        /// </summary>
        /// <param name="rayHit">
        /// The hit structure.
        /// </param>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>
        /// The 3D position of the hit.
        /// </returns>
        private static Point3D GetGlobalHitPosition(RayMeshGeometry3DHitTestResult rayHit, Viewport3D viewport)
        {
            var p = rayHit.PointHit;

            // first transform the Model3D hierarchy
            var t2 = GetTransform(rayHit.VisualHit, rayHit.ModelHit);
            if (t2 != null)
            {
                p = t2.Transform(p);
            }

            // then transform the Visual3D hierarchy up to the Viewport3D ancestor
            var t = GetTransform(viewport, rayHit.VisualHit);
            if (t != null)
            {
                p = t.Transform(p);
            }

            return p;
        }

        /// <summary>
        /// Gets the normal for a hit test result.
        /// </summary>
        /// <param name="rayHit">
        /// The ray hit.
        /// </param>
        /// <returns>
        /// The normal.
        /// </returns>
        private static Vector3D? GetNormalHit(RayMeshGeometry3DHitTestResult rayHit)
        {
            if ((rayHit.MeshHit.Normals == null) || (rayHit.MeshHit.Normals.Count < 1))
            {
                return null;
            }

            return rayHit.MeshHit.Normals[rayHit.VertexIndex1] * rayHit.VertexWeight1
                   + rayHit.MeshHit.Normals[rayHit.VertexIndex2] * rayHit.VertexWeight2
                   + rayHit.MeshHit.Normals[rayHit.VertexIndex3] * rayHit.VertexWeight3;
        }

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="parentTransform">
        /// The parent transform.
        /// </param>
        /// <returns>
        /// The transform.
        /// </returns>
        private static GeneralTransform3D GetTransform(Model3D current, Model3D model, Transform3D parentTransform)
        {
            var currentTransform = Transform3DHelper.CombineTransform(current.Transform, parentTransform);
            if (current == model)
            {
                return currentTransform;
            }

            var mg = current as Model3DGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    var result = GetTransform(m, model, currentTransform);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recursive search for an object of a given type
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="type">The type.</param>
        /// <param name="output">The output.</param>
        private static void SearchFor(IEnumerable<Visual3D> collection, Type type, IList<Model3D> output)
        {
            foreach (var visual in collection)
            {
                var modelVisual = visual as ModelVisual3D;
                if (modelVisual != null)
                {
                    var model = modelVisual.Content;
                    if (model != null)
                    {
                        if (type.IsInstanceOfType(model))
                        {
                            output.Add(model);
                        }

                        // recursive
                        SearchFor(modelVisual.Children, type, output);
                    }

                    var modelGroup = model as Model3DGroup;
                    if (modelGroup != null)
                    {
                        SearchFor(modelGroup.Children, type, output);
                    }
                }
            }
        }

        /// <summary>
        /// Searches for models of the specified type.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        private static void SearchFor(IEnumerable<Model3D> collection, Type type, IList<Model3D> output)
        {
            foreach (var model in collection)
            {
                if (type.IsInstanceOfType(model))
                {
                    output.Add(model);
                }

                var group = model as Model3DGroup;
                if (group != null)
                {
                    SearchFor(group.Children, type, output);
                }
            }
        }

        /// <summary>
        /// A hit result.
        /// </summary>
        public class HitResult
        {
            /// <summary>
            /// Gets or sets the distance.
            /// </summary>
            /// <value>The distance.</value>
            public double Distance { get; set; }

            /// <summary>
            /// Gets the mesh.
            /// </summary>
            /// <value>The mesh.</value>
            public MeshGeometry3D Mesh
            {
                get
                {
                    return this.RayHit.MeshHit;
                }
            }

            /// <summary>
            /// Gets the model.
            /// </summary>
            /// <value>The model.</value>
            public Model3D Model
            {
                get
                {
                    return this.RayHit.ModelHit;
                }
            }

            /// <summary>
            /// Gets or sets the normal.
            /// </summary>
            /// <value>The normal.</value>
            public Vector3D Normal { get; set; }

            /// <summary>
            /// Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Point3D Position { get; set; }

            /// <summary>
            /// Gets or sets the ray hit.
            /// </summary>
            /// <value>The ray hit.</value>
            public RayMeshGeometry3DHitTestResult RayHit { get; set; }

            /// <summary>
            /// Gets the visual.
            /// </summary>
            /// <value>The visual.</value>
            public Visual3D Visual
            {
                get
                {
                    return this.RayHit.VisualHit;
                }
            }

        }
    }
}