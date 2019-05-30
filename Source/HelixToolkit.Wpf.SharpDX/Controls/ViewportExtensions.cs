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
    using Cameras;
    using Model.Scene;
    using global::SharpDX.Direct3D11;

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
            var totalModel = viewport.Renderables.PreorderDFT((x) => 
            {
                if(x is GeometryNode g)
                {
                    if (g.Visible && g.Geometry != null && g.Geometry.Indices != null)
                    {
                        count += g.Geometry.Indices.Count / 3;
                    }
                }
                return true;
            }).Count();
            return count;
        }

        /// <summary>
        /// Copies the specified viewport to the clipboard.
        /// </summary>
        public static void Copy(this Viewport3DX view)
        {
            Clipboard.SetImage(RenderBitmap(view));
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
                : viewport.CameraCore.GetViewProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix GetProjectionMatrix(this Viewport3DX viewport)
        {
            return viewport.RenderContext != null ? viewport.RenderContext.ProjectionMatrix
                : viewport.CameraCore.GetProjectionMatrix(viewport.ActualWidth / viewport.ActualHeight);
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
            if(viewport.RenderHost != null && viewport.RenderHost.IsRendering)
            {
                viewport.RenderHost.UpdateAndRender();
            }
            return FindBoundsInternal(viewport);
        }

        internal static Rect3D FindBoundsInternal(this Viewport3DX viewport)
        {
            var maxVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var firstModel = viewport.Renderables.PreorderDFT((r) =>
            {
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }).Where(x =>
            {
                if (x is IBoundable b)
                {
                    return b.HasBound && b.BoundsWithTransform.Maximum != b.BoundsWithTransform.Minimum
                    && b.BoundsWithTransform.Maximum != Vector3.Zero && b.BoundsWithTransform.Maximum != maxVector;
                }
                else
                {
                    return false;
                }
            }).FirstOrDefault();
            if (firstModel == null)
            {
                return new Rect3D();
            }
            var bounds = firstModel.BoundsWithTransform;

            foreach (var renderable in viewport.Renderables.PreorderDFT((r) =>
            {
                if (r.Visible && !(r is ScreenSpacedNode))
                {
                    return true;
                }
                return false;
            }))
            {
                if (renderable is IBoundable r)
                {
                    if (r.HasBound && r.BoundsWithTransform.Maximum != maxVector)
                    {
                        bounds = global::SharpDX.BoundingBox.Merge(bounds, r.BoundsWithTransform);
                    }
                }
            }
            return new Rect3D(bounds.Minimum.ToPoint3D(), (bounds.Maximum - bounds.Minimum).ToSize3D());
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
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
        public static void Traverse<T>(this Viewport3DX viewport, Action<T, Transform3D> action) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) => 
            {
                if(node.WrapperSource is T element)
                {
                    action(element, element.Transform);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the specified action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewport">The viewport.</param>
        /// <param name="function">The function. Return true to continue traverse, otherwise stop at current node</param>
        public static void Traverse<T>(this Viewport3DX viewport, Func<T, bool> function) where T : Element3D
        {
            viewport.Renderables.PreorderDFT((node) =>
            {
                if (node.WrapperSource is T element)
                {
                    return function(element);
                }
                return true;
            });
        }

        /// <summary>
        /// Traverses the Visual3D/Element3D tree and invokes the specified action on each Element3D of the specified type.
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
        public static void Traverse<T>(this Element3D element, Action<T, Transform3D> action) where T : Element3D
        {
            var sceneNode = new SceneNode[] { element.SceneNode };
            Traverse(element, action);
        }

        public static readonly HitTestResult[] EmptyHits = new HitTestResult[0];
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
            if (!(viewport.Camera is ProjectionCamera camera))
            {
                return EmptyHits;
            }

            var ray = UnProject(viewport, new Vector2((float)position.X, (float)position.Y));
            var hits = new List<HitTestResult>();

            foreach (var element in viewport.Renderables)
            {
                element.HitTest(viewport.RenderContext, ray, ref hits);
            }
            hits.Sort();
            return hits;
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
            out Point3D point, out Vector3D normal, out Element3D model)
        {
            point = new Point3D();
            normal = new Vector3D();
            model = null;

            if (!(viewport.Camera is ProjectionCamera camera))
            {
                return false;
            }

            var hits = FindHits(viewport, position);
            if (hits.Count > 0)
            {
                point = hits[0].PointHit.ToPoint3D();
                normal = hits[0].NormalAtHit.ToVector3D();
                model = hits[0].ModelHit as Element3D;
                return true;
            }
            else
            {
                // check for nearest points in the scene
                // TODO!!
                return false;
            }
        }

        public static bool FindNearest(this Viewport3DX viewport, Point position,
            out Vector3 point, out Vector3 normal, out Element3D model)
        {
            point = new Vector3();
            normal = new Vector3();
            model = null;

            if (!(viewport.Camera is ProjectionCamera camera))
            {
                return false;
            }

            var hits = FindHits(viewport, position);
            if (hits.Count > 0)
            {
                point = hits[0].PointHit;
                normal = hits[0].NormalAtHit;
                model = hits[0].ModelHit as Element3D;
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
            if (FindNearest(viewport, position, out Point3D p, out Vector3D n, out Element3D obj))
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
            if (viewport.CameraCore is ProjectionCameraCore camera)
            {
                var px = (float)point2d.X;
                var py = (float)point2d.Y;

                var viewMatrix = camera.GetViewMatrix();
                Vector3 v = new Vector3();

                var matrix = MatrixExtensions.PsudoInvert(ref viewMatrix);
                float w = (float)viewport.ActualWidth;
                float h = (float)viewport.ActualHeight;
                var aspectRatio = w / h;

                var projMatrix = camera.GetProjectionMatrix(aspectRatio);
                Vector3 zn, zf;
                v.X = (2 * px / w - 1) / projMatrix.M11;
                v.Y = -(2 * py / h - 1) / projMatrix.M22;
                v.Z = 1 / projMatrix.M33;
                Vector3.TransformCoordinate(ref v, ref matrix, out zf);

                if (camera is PerspectiveCameraCore)
                {
                    zn = camera.Position;
                }
                else
                {
                    v.Z = 0;
                    Vector3.TransformCoordinate(ref v, ref matrix, out zn);
                }
                Vector3 r = zf - zn;
                r.Normalize();

                return new Ray(zn + r * camera.NearPlaneDistance, r);
            }
            throw new HelixToolkitException("Unproject camera error.");
        }

        /// <summary>
        /// Un-projects a 2D screen point.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The input point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProject(this Viewport3DX viewport, Point point2d)
        {
            //Vector3 p0, p1;
            var r = UnProject(viewport, point2d.ToVector2());//, out p0, out p1);
            return new Ray(r.Position, r.Direction);
        }

        /// <summary>
        /// Un-projects the specified 2D screen point to a ray.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="point2d">The point.</param>
        /// <returns>The ray.</returns>
        public static Ray UnProjectToRay(this Viewport3DX viewport, Point point2d)
        {
            var r = viewport.UnProject(point2d.ToVector2());            
            return new Ray(r.Position, r.Direction);
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
            return UnProjectOnPlane(viewport, p.ToVector2(), position.ToVector3(), normal.ToVector3())?.ToPoint3D();
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
            if (ray.Intersects(ref plane, out Vector3 hitPoint))
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
            if (!(viewport.Camera is ProjectionCamera pc))
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

        public static Point Project(this Viewport3DX viewport, Vector3 point)
        {
            var matrix = GetScreenViewProjectionMatrix(viewport);
            var pointTransformed = Vector3.TransformCoordinate(point, matrix);
            var pt = new Point(pointTransformed.X, pointTransformed.Y);
            return pt;
        }

        /// <summary>
        /// Finds the intersection with a plane.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="ray"></param>
        /// <returns>The intersection point.</returns>
        public static Vector3? PlaneIntersection(this Ray ray, Vector3 position, Vector3 normal)
        {
            // http://paulbourke.net/geometry/planeline/
            var dn = Vector3.Dot(normal, ray.Direction);
            if (dn == 0)
            {
                return null;
            }

            var u = Vector3.Dot(normal, position - ray.Position) / dn;
            return ray.Position + u * ray.Direction;
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
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(this Viewport3DX view)
        {
            if(view.RenderHost != null && view.RenderHost.IsRendering)
            {
                view.RenderHost.UpdateAndRender();
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    if (view.RenderHost != null && view.RenderHost.IsRendering)
                    {
                        Utilities.ScreenCapture.SaveWICTextureToBitmapStream(view.RenderHost.EffectsManager, view.RenderHost.RenderBuffer.BackBuffer.Resource as Texture2D, memoryStream);
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        memoryStream.Position = 0;
                        bitmap.StreamSource = memoryStream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Renders the viewport to a bitmap.
        /// </summary>
        /// <param name="view">The viewport.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>A bitmap.</returns>
        public static BitmapSource RenderBitmap(
            this Viewport3DX view, int width, int height)
        {
            var w = (int)view.Width;
            var h = (int)view.Height;
            ResizeAndArrange(view, width, height);
            var rtb = RenderBitmap(view);
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
        public static void ResizeAndArrange(this Viewport3DX view, int width, int height)
        {
            view.Width = width;
            view.Height = height;
            if (view.RenderHost == null || !view.RenderHost.IsRendering)
            {
                return;
            }            
            view.Measure(new Size(width, height));
            view.Arrange(new Rect(0, 0, width, height));
            view.RenderHost.Resize(width, height);
        }


        /// <summary>
        /// Saves the bitmap.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void SaveScreen(this Viewport3DX view, string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName);
            Direct2DImageFormat format = Direct2DImageFormat.Bmp;
            switch (ext)
            {
                case "bmp":
                    format = Direct2DImageFormat.Bmp;
                    break;
                case "jpeg":
                case "jpg":
                    format = Direct2DImageFormat.Jpeg;
                    break;
                case "png":
                    format = Direct2DImageFormat.Png;
                    break;
                default:
                    break;
            }
            SaveScreen(view, fileName, format);
        }

        /// <summary>
        /// Saves the bitmap.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="format">The format.</param>
        public static void SaveScreen(this Viewport3DX view, string fileName, Direct2DImageFormat format)
        {
            using (var file = System.IO.File.OpenWrite(fileName))
            {
                if (!file.CanWrite)
                {
                    throw new AccessViolationException($"File cannot be written. {fileName}");
                }
            }
            if (view.RenderHost != null && view.RenderHost.IsRendering)
            {
                view.RenderHost.UpdateAndRender();
                Utilities.ScreenCapture.SaveWICTextureToFile(view.RenderHost.EffectsManager, view.RenderHost.RenderBuffer.BackBuffer.Resource as Texture2D, fileName, format);
            }
        }

        /// <summary>
        /// Zooms to the extents of the specified viewport.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="animationTime">The animation time.</param>
        public static void ZoomExtents(this Viewport3DX viewport, double animationTime = 0)
        {
            var bounds = viewport.FindBoundsInternal();
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
            if (camera is PerspectiveCamera pcam)
            {
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / viewport.ActualWidth * viewport.ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = pcam.LookDirection;
                dir.Normalize();
                pcam.LookAt(center, dir * dist, animationTime);
            }
            else if (camera is OrthographicCamera ocam)
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
            if (viewport.Camera is ProjectionCamera pcam)
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
            if (!(viewport.Camera is PerspectiveCamera pcamera) || !viewport.IsChangeFieldOfViewEnabled)
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
    }
}