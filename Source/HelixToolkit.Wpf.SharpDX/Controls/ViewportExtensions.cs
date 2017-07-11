// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewportExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides extension methods for Viewport3DX.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    using Matrix = global::SharpDX.Matrix;
    using Plane = global::SharpDX.Plane;
    using Ray = global::SharpDX.Ray;
    using Vector2 = global::SharpDX.Vector2;
    using Vector3 = global::SharpDX.Vector3;

    /// <summary>
    /// Provides extension methods for <see cref="Viewport3DX" />.
    /// </summary>
    public static class ViewportExtensions
    {
        /// <summary>
        /// Gets the total number of triangles in the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total number of triangles</returns>        
        public static int GetTotalNumberOfTriangles(this Viewport3DX viewport)
        {
            int count = 0;
            foreach (var item in viewport.Renderables)
            {
                var model = item as MeshGeometryModel3D;
                if (model != null)
                {
                    if (model.Visibility == Visibility.Visible)
                        count += model.Geometry.Indices.Count / 3;
                }
            }
            return count;
        }

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
        /// Gets the camera transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>
        /// The camera transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D GetViewProjectionMatrix3D(this Viewport3DX viewport)
        {
            return GetViewProjectionMatrix(viewport).ToMatrix3D();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ViewMatrix * viewport.RenderContext.ProjectionMatrix
                : viewport.Camera.GetViewProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ProjectionMatrix
                : viewport.Camera.GetProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
        }
        /// <summary>
        /// Gets the total transform for a Viewport3DX. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D GetScreenViewProjectionMatrix3D(this Viewport3DX viewport)
        {
            return GetScreenViewProjectionMatrix(viewport).ToMatrix3D();
        }

        /// <summary>
        /// Gets the total transform for a Viewport3DX. 
        /// Old name of this function: GetTotalTransform
        /// New name of the function: GetScreenViewProjectionTransform
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The total transform.</returns>       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetScreenViewProjectionMatrix(this Viewport3DX viewport)
        {
            return GetViewProjectionMatrix(viewport) * GetViewportMatrix(viewport);
        }

        /// <summary>
        /// Gets the viewport transform aka the screen-space transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3D GetViewportMatrix3D(this Viewport3DX viewport)
        {
            return GetViewportMatrix(viewport).ToMatrix3D();
        }

        /// <summary>
        /// Gets the viewport transform aka the screen-space transform.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <returns>The transform.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetViewportMatrix(this Viewport3DX viewport)
        {
            return new Matrix(
                (float)(viewport.ActualWidth / 2),
                0,
                0,
                0,
                0,
                (float)(-viewport.ActualHeight / 2),
                0,
                0,
                0,
                0,
                1,
                0,
                (float)((viewport.ActualWidth - 1) / 2),
                (float)((viewport.ActualHeight - 1) / 2),
                0,
                1);
        }

        /// <summary>
        /// Finds the bounding box of the viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The bounding box.</returns>
        public static Rect3D FindBounds(this Viewport3DX viewport)
        {
            var bounds = new global::SharpDX.BoundingBox();
            foreach (var element in viewport.Renderables)
            {
                var model = element as IBoundable;
                if (model != null && !model.Bounds.Minimum.IsUndefined() && !model.Bounds.Maximum.IsUndefined())
                {
                    if (model.Visibility != Visibility.Collapsed)
                    {
                        bounds = global::SharpDX.BoundingBox.Merge(bounds, model.Bounds);
                    }
                }
            }
            return new Rect3D(bounds.Minimum.ToPoint3D(), (bounds.Maximum - bounds.Minimum).ToSize3D());
        }

        /// <summary>
        /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Viewport3DX viewport, Action<T, Transform3D> action) where T : Model3D
        {
            foreach (var element in viewport.Renderables)
            {
                var model = element as Model3D; // ITraversable;
                if (model != null)
                {
                    Traverse(model, action);
                }
            }
        }

        /// <summary>
        /// Traverses the Visual3D/Model3D tree and invokes the specified action on each Model3D of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type filter.
        /// </typeparam>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public static void Traverse<T>(this Model3D element, Action<T, Transform3D> action) where T : Model3D
        {
            Traverse(element, action);
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
        public static IList<HitTestResult> FindHits(this Viewport3DX viewport, Point position)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return null;
            }

            var ray = UnProject(viewport, new Vector2((float)position.X, (float)position.Y));
            var hits = new List<HitTestResult>();
                        
            foreach (var element in viewport.Renderables)
            {
                var model = element as IHitable;
                if (model != null)
                {
                    model.HitTest(viewport.RenderContext, ray, ref hits);
                }
            }

            return hits.OrderBy(k => k.Distance).ToList();
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
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The find nearest.
        /// </returns>
        public static bool FindNearest(this Viewport3DX viewport, Point position,
            out Point3D point, out Vector3D normal, out Model3D model)
        {
            point = new Point3D();
            normal = new Vector3D();
            model = null;

            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                return false;
            }

            var hits = FindHits(viewport, position);
            if (hits.Count > 0)
            {
                point = hits[0].PointHit;
                normal = hits[0].NormalAtHit;
                model = hits[0].ModelHit;
                return true;
            }
            else
            {
                // check for nearest points in the scene
                // TODO!!
                return false;
            }
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
            Model3D obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
            {
                return p;
            }
            return null;
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Vector2 point2d)//, out Vector3 pointNear, out Vector3 pointFar)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera != null)
            {
                var px = (float)point2d.X;
                var py = (float)point2d.Y;

                var viewMatrix = camera.GetViewMatrix();
                Vector3 v = new Vector3();
                
                var matrix = CameraExtensions.InverseViewMatrix(ref viewMatrix);
                float w = (float)viewport.ActualWidth;
                float h = (float)viewport.ActualHeight;
                var aspectRatio = w / h;

                var projMatrix = camera.GetProjectionMatrix(aspectRatio);
                Vector3 zn, zf;
                v.X = (2 * px / w - 1) / projMatrix.M11;
                v.Y = -(2 * py / h - 1) / projMatrix.M22;
                v.Z = 1 / projMatrix.M33;
                Vector3.TransformCoordinate(ref v, ref matrix, out zf);

                if (camera is PerspectiveCamera)
                {
                    zn = camera.Position.ToVector3();
                }
                else
                {
                    v.Z = 0;
                    Vector3.TransformCoordinate(ref v, ref matrix, out zn);
                }           
                Vector3 r = zf - zn;
                r.Normalize();               

                return new Ray(zn, r);
            }
            throw new HelixToolkitException("Unproject camera error.");
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray3D UnProject(this Viewport3DX viewport, Point point2d)
        {
            //Vector3 p0, p1;
            var r = UnProject(viewport, point2d.ToVector2());//, out p0, out p1);
            return new Ray3D(r.Position.ToPoint3D(), r.Direction.ToVector3D());
        }

        /// <summary>
        /// Un-projects the specified 2D screen point to a ray.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The point.</param>
        /// <returns>The ray.</returns>
        public static Ray3D UnProjectToRay(this Viewport3DX viewport, Point point2d)
        {
            var r = viewport.UnProject(point2d.ToVector2());            
            return new Ray3D(r.Position.ToPoint3D(), r.Direction.ToVector3D());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point2d"></param>
        /// <returns></returns>
        public static Ray UnProjectToRay(this Viewport3DX viewport, Vector2 point2d)
        {
            var r = viewport.UnProject(point2d);
            return new Ray(r.Position, r.Direction);
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
        public static Point3D? UnProjectOnPlane(this Viewport3DX viewport, Point p, Point3D position, Vector3D normal)
        {
            var ray = UnProjectToRay(viewport, p);
            if (ray == null)
            {
                return null;
            }
            return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="p"></param>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Vector2 p, Vector3 position, Vector3 normal)
        {            
            var plane = new Plane(position, normal);
            return UnProjectOnPlane(viewport, p, plane);
        }
          
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="p"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Vector3? UnProjectOnPlane(this Viewport3DX viewport, Vector2 p, Plane plane)
        {
            var ray = UnProjectToRay(viewport, p);
            Vector3 hitPoint;
            if (ray.Intersects(ref plane, out hitPoint))
            {
                return hitPoint;
            }
            return null;
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
        public static Point3D? UnProjectOnPlane(this Viewport3DX viewport, Point p)
        {
            var pc = viewport.Camera as ProjectionCamera;
            if (pc == null)
            {
                return null;
            }
            return UnProjectOnPlane(viewport, p, pc.Position + pc.LookDirection, pc.LookDirection);
        }

        /// <summary>
        /// Projects the specified 3D point to a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point">The 3D point.</param>
        /// <returns>The point.</returns>
        public static Point Project(this Viewport3DX viewport, Point3D point)
        {
            var matrix = GetScreenViewProjectionMatrix3D(viewport);
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
            var vm = originalCamera.GetViewMatrix3D();
            double ar = view.ActualWidth / view.ActualHeight;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // change the camera viewport and scaling
                    var pm = originalCamera.GetProjectionMatrix3D(ar);
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
        /// <param name="viewport">
        /// The viewport.
        /// </param>
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
    }
}